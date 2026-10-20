#!/usr/bin/env python3
"""Generate and verify source-derived UML inventories for maintained product projects."""
from __future__ import annotations

import json
import os
import subprocess
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SOLUTION = ROOT / "TheSingularityWorkshop.GUI.slnx"
GENERATOR = ROOT / "samples/SourceUmlGenerator/SourceUmlGenerator.csproj"
OUTPUT = ROOT / "artifacts/solution-uml"
REPOSITORY = "TrentBest/TheSingularityWorkshop.GUI"

# Product libraries only. Tests, samples, docs and tooling are deliberately excluded.
MAINTAINED_PROJECTS = (
    "src/Core/TheSingularityWorkshop.GUI.Core.csproj",
    "src/Blazor/TheSingularityWorkshop.GUI.Blazor.csproj",
    "src/WPF/TheSingularityWorkshop.GUI.WPF.csproj",
    "src/SVG/TheSingularityWorkshop.GUI.SVG.csproj",
)


def project_build_properties(project: Path) -> tuple[str, str]:
    """Return the sole target framework and output assembly name, failing closed on multi-targeting."""
    try:
        root = ET.parse(project).getroot()
    except (ET.ParseError, OSError) as exc:
        raise ValueError(f"Cannot read project properties from {project}: {exc}") from exc

    properties: dict[str, str] = {}
    for group in root:
        if group.tag.split("}")[-1] != "PropertyGroup":
            continue
        for element in group:
            name = element.tag.split("}")[-1]
            if name in {"TargetFramework", "TargetFrameworks", "AssemblyName"} and element.text:
                properties[name] = element.text.strip()

    frameworks = properties.get("TargetFrameworks", properties.get("TargetFramework", ""))
    target_frameworks = [value.strip() for value in frameworks.split(";") if value.strip()]
    if len(target_frameworks) != 1 or "$(" in target_frameworks[0]:
        raise ValueError(
            f"{project.relative_to(ROOT)} must resolve to exactly one literal target framework; "
            f"found {frameworks!r}. Add explicit per-framework inventory support before multi-targeting."
        )

    assembly_name = properties.get("AssemblyName") or project.stem
    if "$(" in assembly_name or "/" in assembly_name or "\\" in assembly_name:
        raise ValueError(f"{project.relative_to(ROOT)} has an unsupported AssemblyName: {assembly_name!r}")
    return target_frameworks[0], assembly_name


def run(command: list[str], *, label: str) -> None:
    print(f"{label}: {' '.join(command)}", flush=True)
    result = subprocess.run(command, cwd=ROOT, check=False)
    if result.returncode:
        raise RuntimeError(f"{label} failed with exit code {result.returncode}.")


def main() -> int:
    revision = os.environ.get("SOURCE_REVISION", "").strip()
    if not revision:
        print("SOURCE_REVISION must contain the exact source commit SHA.", file=sys.stderr)
        return 2

    try:
        solution_root = ET.parse(SOLUTION).getroot()
    except (ET.ParseError, OSError) as exc:
        print(f"Cannot read solution manifest: {exc}", file=sys.stderr)
        return 2

    solution = {
        Path(node.attrib["Path"].replace("\\", "/")).as_posix()
        for node in solution_root.iter("Project")
        if "Path" in node.attrib
    }
    discovered = {
        project.relative_to(ROOT).as_posix()
        for project in (ROOT / "src").rglob("*.csproj")
        if not any(part in {"bin", "obj"} for part in project.parts)
    }
    expected = set(MAINTAINED_PROJECTS)
    errors = (
        [f"Maintained product project is absent from solution: {path}" for path in sorted(expected - solution)]
        + [f"Maintained product project is missing from checkout: {path}" for path in sorted(expected - discovered)]
        + [f"New src project needs explicit UML scope classification: {path}" for path in sorted(discovered - expected)]
    )
    if errors:
        print("Solution UML scope validation failed:\n" + "\n".join("  - " + error for error in errors), file=sys.stderr)
        return 1

    OUTPUT.mkdir(parents=True, exist_ok=True)
    rows: list[tuple[str, str, int, int]] = []

    try:
        for project_relative in MAINTAINED_PROJECTS:
            project = ROOT / project_relative
            project_framework, assembly_name = project_build_properties(project)
            slug = project.parent.name.lower()
            destination = OUTPUT / slug
            destination.mkdir(parents=True, exist_ok=True)
            model_path = destination / "model.json"
            inventory_path = destination / "inventory.md"
            pages_path = destination / "types"

            run(
                ["dotnet", "restore", project_relative, "-p:EnableWindowsTargeting=true"],
                label=f"Restore {project_relative}",
            )
            run(
                ["dotnet", "build", project_relative, "--configuration", "Release", "--no-restore",
                 "-p:EnableWindowsTargeting=true"],
                label=f"Build exact reflection artifact for {project_relative}",
            )

            reflection_assembly = project.parent / "bin" / "Release" / project_framework / f"{assembly_name}.dll"
            if not reflection_assembly.is_file():
                raise FileNotFoundError(
                    f"Expected exact-build assembly was not produced for {project_relative}: "
                    f"{reflection_assembly.relative_to(ROOT)}"
                )

            command = [
                "dotnet", "run", "--project", str(GENERATOR), "--configuration", "Release", "--",
                "--project-model-json", str(model_path),
                "--project", project_relative,
                "--repository", REPOSITORY,
                "--revision", revision,
                "--configuration", "Release",
                "--reflection-assembly", str(reflection_assembly),
                "--inventory-markdown", str(inventory_path),
                "--type-pages-dir", str(pages_path),
            ]
            run(command, label=f"Analyze and reflect {project_relative}")

            try:
                model = json.loads(model_path.read_text(encoding="utf-8"))
            except (OSError, json.JSONDecodeError) as exc:
                raise ValueError(f"Invalid generated model for {project_relative}: {exc}") from exc

            types = model.get("types")
            pages = sorted(page for page in pages_path.glob("*.md") if page.name != "index.md")
            if not isinstance(types, list) or not types or len(pages) != len(types) or not (pages_path / "index.md").is_file():
                raise ValueError(
                    f"Type-page coverage mismatch for {project_relative}: {len(types or [])} types, {len(pages)} pages"
                )

            provenance = model.get("provenance", {})
            if provenance.get("sourceRevision") != revision or provenance.get("projectPath") != project_relative:
                raise ValueError(f"Model provenance mismatch for {project_relative}: {provenance}")

            rows.append((project_relative, slug, len(types), len(model.get("relationships", []))))

    except (RuntimeError, ValueError, FileNotFoundError) as exc:
        print(str(exc), file=sys.stderr)
        return 1

    lines = [
        "# Solution-level source-derived UML inventory",
        "",
        f"- Repository: `{REPOSITORY}`",
        f"- Source revision: `{revision}`",
        "- Scope: maintained product projects under `src/`; tests, samples, tooling, and documentation projects are deliberately excluded.",
        "- Evidence: each project is restored and built in Release, loaded with MSBuild/Roslyn, checked for compiler errors, and reconciled against the exact build output by assembly identity and type metadata inventory.",
        "- Framework policy: each included project must currently declare exactly one literal target framework. Multi-targeted projects fail closed until per-framework output partitioning is implemented.",
        "- Limitation: this verifies type inventory against the exact build, not member-level reflection parity or full-solution graph rendering.",
        "",
        "| Project | Types | Compiler-symbol relationships | Inventory | Type pages |",
        "|---|---:|---:|---|---|",
    ]
    for project, slug, count, edges in rows:
        lines.append(
            f"| `{project}` | {count} | {edges} | [{slug}/inventory.md]({slug}/inventory.md) | "
            f"[{slug}/types/index.md]({slug}/types/index.md) |"
        )
    lines.extend([
        "",
        f"**Coverage:** {len(rows)}/{len(MAINTAINED_PROJECTS)} maintained product projects analyzed, built, and reconciled successfully.",
        "",
    ])
    (OUTPUT / "index.md").write_text("\n".join(lines), encoding="utf-8")
    print(f"Verified exact-build UML coverage for {len(rows)} maintained product projects -> {OUTPUT / 'index.md'}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
