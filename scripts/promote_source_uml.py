#!/usr/bin/env python3
"""Safely promote generated source-derived UML artifacts without touching authored docs."""
from __future__ import annotations
import argparse
import os
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MANIFEST_NAME = ".generated-source-uml-files.txt"
SAFE_SUFFIXES = {".md", ".svg"}
# Keep this allowlist aligned with scripts/generate_solution_uml.py. Never discover
# arbitrary directories as publishable documentation.
PROJECTS = {"core": "Core", "blazor": "Blazor", "wpf": "WPF", "svg": "SVG"}
LEGACY_INVENTORY = "Core.uml-inventory.md"
LEGACY_TYPES = "Core.uml-types"


def _collect_types(types_dir: Path, prefix: str, outputs: dict[str, bytes]) -> None:
    if not types_dir.is_dir() or types_dir.is_symlink():
        raise ValueError(f"missing or unsafe generated type directory: {types_dir}")
    for path in sorted(types_dir.iterdir(), key=lambda item: item.name):
        if path.is_symlink() or not path.is_file():
            raise ValueError(f"non-regular generated entry is not allowed: {path.name}")
        if path.suffix.lower() not in SAFE_SUFFIXES or path.name in {"", ".", ".."}:
            raise ValueError(f"unsafe path or non-generated filename: {path.name}")
        if Path(path.name).name != path.name or "/" in path.name or "\\" in path.name:
            raise ValueError(f"unsafe generated filename: {path.name}")
        outputs[f"{prefix}/{path.name}"] = path.read_bytes()
    if f"{prefix}/index.md" not in outputs:
        raise ValueError(f"generated type set is missing {prefix}/index.md")
    if not any(name.startswith(f"{prefix}/") and name.endswith(".md") for name in outputs):
        raise ValueError(f"generated type set contains no Markdown pages for {prefix}")


def collect_generated(input_dir: Path) -> dict[str, bytes]:
    outputs: dict[str, bytes] = {}
    # Preferred layout is the four-project solution inventory produced by
    # scripts/generate_solution_uml.py: <slug>/inventory.md and <slug>/types/*.
    if (input_dir / "index.md").is_file() and any((input_dir / slug / "types").is_dir() for slug in PROJECTS):
        outputs["index.md"] = (input_dir / "index.md").read_bytes()
        for slug, project in PROJECTS.items():
            project_dir = input_dir / slug
            inventory = project_dir / "inventory.md"
            if not inventory.is_file() or inventory.is_symlink():
                raise ValueError(f"missing or unsafe generated inventory: {inventory}")
            outputs[f"{project}/inventory.md"] = inventory.read_bytes()
            _collect_types(project_dir / "types", project, outputs)
        return outputs

    # Backwards-compatible Core-only input used by the older manual workflow.
    inventory = input_dir / LEGACY_INVENTORY
    types_dir = input_dir / LEGACY_TYPES
    if not inventory.is_file() or inventory.is_symlink():
        raise ValueError(f"missing or unsafe generated inventory: {inventory}")
    outputs[LEGACY_INVENTORY] = inventory.read_bytes()
    _collect_types(types_dir, "Core", outputs)
    return outputs


def read_manifest(output_dir: Path) -> set[str]:
    path = output_dir / MANIFEST_NAME
    if not path.exists():
        return set()
    if path.is_symlink() or not path.is_file():
        raise ValueError("generated-file manifest is not a regular file")
    entries: set[str] = set()
    for line in path.read_text(encoding="utf-8").splitlines():
        entry = line.strip()
        if not entry or entry.startswith("#"):
            continue
        candidate = Path(entry)
        if candidate.is_absolute() or ".." in candidate.parts or candidate.as_posix() != entry:
            raise ValueError(f"unsafe path in generated-file manifest: {entry}")
        allowed = entry == LEGACY_INVENTORY or entry == "index.md"
        allowed = allowed or entry in {f"{name}/inventory.md" for name in PROJECTS.values()}
        for project in PROJECTS.values():
            if entry.startswith(f"{project}/") and candidate.suffix.lower() in SAFE_SUFFIXES:
                allowed = allowed or candidate.name == "index.md" or candidate.name.startswith("TheSingularityWorkshop-")
        if not allowed or candidate.suffix.lower() not in SAFE_SUFFIXES:
            raise ValueError(f"manifest entry is outside recognized generated UML scope: {entry}")
        entries.add(entry)
    return entries


def write_atomic(path: Path, content: bytes) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with tempfile.NamedTemporaryFile(dir=path.parent, delete=False) as stream:
        temporary = Path(stream.name)
        stream.write(content)
    os.replace(temporary, path)


def promote(input_dir: Path, output_dir: Path, check: bool = False) -> list[str]:
    generated = collect_generated(input_dir)
    absolute_output = output_dir.absolute()
    if any(parent.is_symlink() for parent in absolute_output.parents):
        raise ValueError("generated documentation output path must not traverse a symlinked parent")
    if output_dir.is_symlink():
        raise ValueError("generated documentation output directory must not be a symlink")
    for project in PROJECTS.values():
        target = output_dir / project
        if target.is_symlink():
            raise ValueError(f"generated {project} output directory must not be a symlink")
    previous = read_manifest(output_dir)
    current = set(generated)
    errors: list[str] = []
    for relative, content in sorted(generated.items()):
        target = output_dir / relative
        if target.is_symlink():
            errors.append(f"refusing to overwrite symlink: {relative}")
        elif check:
            if not target.is_file() or target.read_bytes() != content:
                errors.append(f"generated documentation is stale or missing: {relative}")
        else:
            write_atomic(target, content)
    for stale in sorted(previous - current):
        target = output_dir / stale
        if target.is_symlink():
            errors.append(f"refusing to remove symlink: {stale}")
        elif target.is_file():
            if check:
                errors.append(f"stale generated file remains: {stale}")
            else:
                target.unlink()
    manifest_content = ("# Generated by scripts/promote_source_uml.py; do not edit.\n"
                        + "".join(f"{path}\n" for path in sorted(current))).encode("utf-8")
    manifest_path = output_dir / MANIFEST_NAME
    if manifest_path.is_symlink():
        errors.append(f"refusing to overwrite symlink: {MANIFEST_NAME}")
    elif check:
        if not manifest_path.is_file() or manifest_path.read_bytes() != manifest_content:
            errors.append(f"generated-file manifest is stale or missing: {MANIFEST_NAME}")
    else:
        write_atomic(manifest_path, manifest_content)
    return errors


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--input-dir", type=Path, default=ROOT / "artifacts")
    parser.add_argument("--output-dir", type=Path, default=ROOT / "docs" / "uml" / "source-files")
    parser.add_argument("--check", action="store_true", help="fail if promoted files differ; do not write or prune")
    args = parser.parse_args()
    try:
        errors = promote(args.input_dir.resolve(), args.output_dir, args.check)
    except (OSError, ValueError) as exc:
        print(f"ERROR: source UML promotion failed closed: {exc}", file=sys.stderr)
        return 1
    if errors:
        for error in errors:
            print(f"ERROR: {error}", file=sys.stderr)
        return 1
    print("Source UML promotion check passed." if args.check else
          "Promoted generated UML files; authored/untracked files were preserved.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
