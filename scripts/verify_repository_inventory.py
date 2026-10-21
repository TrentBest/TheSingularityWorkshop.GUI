#!/usr/bin/env python3
"""Fail CI when project/solution inventory or documentation links drift.

Every .csproj must be either included in the solution or named in the intentional
non-solution list below. New projects therefore require an explicit classification.
The script also validates solution references and explicit documentation-project items.
"""
from __future__ import annotations

import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SOLUTION = ROOT / "TheSingularityWorkshop.GUI.slnx"
DOCUMENTATION_PROJECT = ROOT / "TheSingularityWorkshop.GUI.Documentation.csproj"

# These executable samples are run directly by CI and deliberately stay out of
# the IDE solution's default build graph. Change this list when that decision changes.
INTENTIONALLY_OUTSIDE_SOLUTION = {
    "samples/FoxSheepdogPlane/FoxSheepdogPlane.csproj",
    "samples/SvgGallery/SvgGallery.csproj",
    "samples/demonstrations/CoordinateFunctions/CoordinateFunctions.csproj",
    "samples/demonstrations/PrimitiveGeometry/PrimitiveGeometry.csproj",
    "samples/demonstrations/ProjectileTrajectory/ProjectileTrajectory.csproj",
}

# Inventory ownership/status classifications. New workflow and visual files must be
# explicitly classified here rather than silently inheriting a guessed lifecycle.
MAINTAINED_WORKFLOWS = {
    ".github/workflows/package.yml",
    ".github/workflows/promote-source-uml-docs.yml",
    ".github/workflows/source-derived-diagrams.yml",
    ".github/workflows/solution-derived-uml.yml",
}
GENERATED_DRIFT_CHECKED_ASSETS = {
    "docs/assets/svg-adapter-gallery.svg",
    "docs/assets/uml/classes/Core/DataCard.svg",
}
AUTHORED_SCENE_ASSETS = {
    "docs/assets/concepts/gui-semantic-boundary.svg",  # authored conceptual illustration, not renderer output
    "docs/assets/concepts/living-energy-vortex.svg",  # authored visual experiment, not renderer output
    "docs/assets/fox-sheepdog-10x10.svg",  # richer authored reference; generator parity remains open
    "samples/demonstrations/01-primitive-geometry.svg",  # authored reference, not renderer output
}
AUTHORED_UML_ASSETS = {
    "docs/assets/uml/classes/Core/CoordinateSystem.svg",
    "docs/assets/uml/classes/Core/CoordinateSystemUtilities.svg",
    "docs/assets/uml/classes/Core/GuiNode.svg",
}
AUTHORED_REFERENCE_IMAGES = {
    "docs/assets/gui-engineering-boundary.jpg",
    "docs/assets/ideal-gui-separation-of-concerns.jpg",
    "docs/assets/repository-structure-overview.jpg",
    "docs/assets/workshop-map-reference.jpg",
}


def rel(path: Path) -> str:
    return path.relative_to(ROOT).as_posix()


def main() -> int:
    errors: list[str] = []
    all_projects = {rel(p) for p in ROOT.rglob("*.csproj")
                    if not any(part in {"bin", "obj", ".git"} for part in p.parts)}

    try:
        solution_xml = ET.parse(SOLUTION).getroot()
        solution_projects = {
            Path(node.attrib["Path"].replace("\\", "/")).as_posix()
            for node in solution_xml.iter("Project") if "Path" in node.attrib
        }
    except (ET.ParseError, OSError, KeyError) as exc:
        print(f"ERROR: cannot parse solution manifest: {exc}", file=sys.stderr)
        return 1

    missing_solution_files = sorted(p for p in solution_projects if not (ROOT / p).is_file())
    if missing_solution_files:
        errors.extend(f"Solution references missing project: {p}" for p in missing_solution_files)

    unknown_projects = sorted(all_projects - solution_projects - INTENTIONALLY_OUTSIDE_SOLUTION)
    stale_exclusions = sorted(INTENTIONALLY_OUTSIDE_SOLUTION - all_projects)
    overlap = sorted(solution_projects & INTENTIONALLY_OUTSIDE_SOLUTION)
    errors.extend(f"Project is not classified as solution member or intentional exclusion: {p}"
                  for p in unknown_projects)
    errors.extend(f"Intentional exclusion no longer exists; remove stale classification: {p}"
                  for p in stale_exclusions)
    errors.extend(f"Project cannot be both included and excluded: {p}" for p in overlap)

    try:
        docs_xml = ET.parse(DOCUMENTATION_PROJECT).getroot()
        doc_items: list[str] = []
        for node in docs_xml.iter():
            include = node.attrib.get("Include")
            if include and node.tag.split("}")[-1] in {"None", "Content", "AdditionalFiles"}:
                normalized = include.replace("\\", "/")
                # This project uses explicit paths; wildcards need separate handling.
                if "*" in normalized or "?" in normalized:
                    errors.append(f"Wildcard documentation item is not inventory-checked: {include}")
                    continue
                doc_items.append(normalized)
    except (ET.ParseError, OSError) as exc:
        print(f"ERROR: cannot parse documentation project: {exc}", file=sys.stderr)
        return 1

    duplicate_doc_items = sorted({p for p in doc_items if doc_items.count(p) > 1})
    errors.extend(f"Documentation item is included more than once: {p}" for p in duplicate_doc_items)
    missing_doc_items = sorted(p for p in doc_items if not (ROOT / p).is_file())
    errors.extend(f"Documentation project references missing file: {p}" for p in missing_doc_items)

    print("Repository project inventory")
    print("============================")
    print(f"Solution projects: {len(solution_projects)}")
    for path in sorted(solution_projects):
        print(f"  SOLUTION  {path}")
    print(f"Intentional non-solution projects: {len(INTENTIONALLY_OUTSIDE_SOLUTION)}")
    for path in sorted(INTENTIONALLY_OUTSIDE_SOLUTION):
        print(f"  EXCLUDED  {path} (sample run independently)")
    print(f"Documentation project explicit items: {len(doc_items)}")
    print(f"Discovered C# project files: {len(all_projects)}")

    # Extend the inventory beyond build projects. These categories are deliberately
    # descriptive: they expose the maintained docs/workflows/assets that still need
    # finer-grained authored/generated/historical ownership labels.
    markdown_docs = sorted(
        rel(p) for p in ROOT.rglob("*.md") if p.is_file()
        and not any(part in {"bin", "obj", ".git", "packages", "TestResults"}
                    for part in p.relative_to(ROOT).parts)
    )
    workflows = sorted(rel(p) for p in (ROOT / ".github" / "workflows").glob("*")
                       if p.is_file() and p.suffix.lower() in {".yml", ".yaml"})
    asset_suffixes = {".svg", ".png", ".jpg", ".jpeg", ".webp", ".gif"}
    assets = sorted(
        rel(p) for p in ROOT.rglob("*")
        if p.is_file() and p.suffix.lower() in asset_suffixes
        and not any(part in {"bin", "obj", ".git", "packages", "TestResults"}
                    for part in p.relative_to(ROOT).parts)
    )
    classified_assets = (
        GENERATED_DRIFT_CHECKED_ASSETS | AUTHORED_SCENE_ASSETS |
        AUTHORED_UML_ASSETS | AUTHORED_REFERENCE_IMAGES
    )
    unknown_workflows = sorted(set(workflows) - MAINTAINED_WORKFLOWS)
    stale_workflows = sorted(MAINTAINED_WORKFLOWS - set(workflows))
    unknown_assets = sorted(set(assets) - classified_assets)
    stale_assets = sorted(classified_assets - set(assets))
    errors.extend(f"Workflow needs an explicit lifecycle classification: {p}" for p in unknown_workflows)
    errors.extend(f"Classified workflow no longer exists; remove stale entry: {p}" for p in stale_workflows)
    errors.extend(f"Visual asset needs an explicit lifecycle classification: {p}" for p in unknown_assets)
    errors.extend(f"Classified visual asset no longer exists; remove stale entry: {p}" for p in stale_assets)

    print(f"Repository Markdown files (classified authored/maintained): {len(markdown_docs)}")
    for path in markdown_docs:
        print(f"  DOC       {path} [authored-maintained]")
    print(f"GitHub Actions workflow files: {len(workflows)}")
    for path in workflows:
        print(f"  WORKFLOW  {path} [maintained-automation]")
    print(f"Repository visual/reference assets (classified): {len(assets)}")
    for path in sorted(set(assets) & GENERATED_DRIFT_CHECKED_ASSETS):
        print(f"  GENERATED {path} [generated; byte-for-byte drift checked in CI]")
    for path in sorted(set(assets) & AUTHORED_SCENE_ASSETS):
        print(f"  SCENE     {path} [authored reference; source parity tracked separately]")
    for path in sorted(set(assets) & AUTHORED_UML_ASSETS):
        print(f"  UML       {path} [authored source-grounded reference; CI drift check not yet wired]")
    for path in sorted(set(assets) & AUTHORED_REFERENCE_IMAGES):
        print(f"  IMAGE     {path} [authored reference image]")

    if errors:
        print("\nInventory validation FAILED:", file=sys.stderr)
        for error in errors:
            print(f"  - {error}", file=sys.stderr)
        return 1

    print("\nInventory validation passed: project and explicit reference checks passed. "
          "Repository Markdown is classified as authored-maintained; every workflow and visual "
          "asset must match an explicit lifecycle classification. Broader generated-output "
          "drift and orphan checks remain open.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
