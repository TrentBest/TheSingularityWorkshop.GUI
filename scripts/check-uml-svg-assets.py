#!/usr/bin/env python3
"""Validate committed Workshop UML SVGs for accessibility and safe local links."""
from __future__ import annotations

import sys
from pathlib import Path
from urllib.parse import urlsplit
from xml.etree import ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
SVG_ROOT = ROOT / "docs" / "assets" / "uml"
SVG_NS = "http://www.w3.org/2000/svg"
XLINK_NS = "http://www.w3.org/1999/xlink"
FORBIDDEN_ELEMENTS = {"script", "foreignObject"}


def validate(path: Path) -> list[str]:
    errors: list[str] = []
    relative = path.relative_to(ROOT).as_posix()
    try:
        root = ET.parse(path).getroot()
    except (ET.ParseError, OSError) as exc:
        return [f"{relative}: invalid XML: {exc}"]
    if root.tag != f"{{{SVG_NS}}}svg":
        errors.append(f"{relative}: root must be an SVG element")
    if not root.attrib.get("viewBox"):
        errors.append(f"{relative}: viewBox is required")
    if root.find(f"{{{SVG_NS}}}title") is None:
        errors.append(f"{relative}: accessible title is required")
    if root.find(f"{{{SVG_NS}}}desc") is None:
        errors.append(f"{relative}: accessible description is required")

    ids = [element.attrib["id"] for element in root.iter() if "id" in element.attrib]
    if len(ids) != len(set(ids)):
        errors.append(f"{relative}: duplicate SVG IDs")

    for element in root.iter():
        local_name = element.tag.rsplit("}", 1)[-1]
        if local_name in FORBIDDEN_ELEMENTS:
            errors.append(f"{relative}: forbidden element <{local_name}>")
        for attribute, value in element.attrib.items():
            attribute_name = attribute.rsplit("}", 1)[-1]
            if attribute_name.lower().startswith("on"):
                errors.append(f"{relative}: event-handler attribute {attribute_name} is forbidden")
            if attribute_name != "href" or not value:
                continue
            if value.startswith("#"):
                continue
            parsed = urlsplit(value)
            if parsed.scheme or parsed.netloc or value.startswith("//"):
                errors.append(f"{relative}: external SVG link is forbidden: {value}")
                continue
            target = (path.parent / parsed.path).resolve()
            if ROOT.resolve() not in target.parents:
                errors.append(f"{relative}: SVG link escapes repository: {value}")
            elif not target.is_file():
                errors.append(f"{relative}: SVG link target does not exist: {value}")
    return errors


def main() -> int:
    files = sorted(SVG_ROOT.rglob("*.svg")) if SVG_ROOT.exists() else []
    if not files:
        print("No committed UML SVG assets found; expected at least one file under docs/assets/uml.", file=sys.stderr)
        return 1
    errors = [error for path in files for error in validate(path)]
    if errors:
        print("UML SVG asset validation failed:")
        print("\n".join(f"- {error}" for error in errors))
        return 1
    print(f"UML SVG asset validation passed ({len(files)} SVG files).")
    return 0


if __name__ == "__main__":
    sys.exit(main())
