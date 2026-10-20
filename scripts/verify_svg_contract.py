#!/usr/bin/env python3
"""Fail-closed structural checks for generated, source-derived SVG artifacts.

This checks machine-verifiable SVG contracts; it does not replace rendered visual
inspection or prove that glyphs fit their painted bounds.
"""
from __future__ import annotations

import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

SVG_NS = "{http://www.w3.org/2000/svg}"
XLINK_HREF = "{http://www.w3.org/1999/xlink}href"
URL_REFERENCE = re.compile(r"url\(#([^)]+)\)")


def verify(path: Path) -> list[str]:
    errors: list[str] = []
    try:
        root = ET.parse(path).getroot()
    except (ET.ParseError, OSError) as exc:
        return [f"{path}: cannot parse SVG: {exc}"]

    if root.tag != SVG_NS + "svg":
        errors.append(f"{path}: root element is not SVG")
        return errors

    for element_name in ("title", "desc"):
        element = root.find(SVG_NS + element_name)
        if element is None or not "".join(element.itertext()).strip():
            errors.append(f"{path}: accessible {element_name} is missing or empty")

    raw_view_box = root.attrib.get("viewBox", "").replace(",", " ").split()
    try:
        if len(raw_view_box) != 4:
            raise ValueError("expected four numbers")
        x, y, width, height = (float(value) for value in raw_view_box)
        if not all(map(lambda value: value == value and abs(value) != float("inf"),
                       (x, y, width, height))):
            raise ValueError("coordinates must be finite")
        if width <= 0 or height <= 0:
            raise ValueError("width and height must be positive")
    except ValueError as exc:
        errors.append(f"{path}: invalid viewBox ({exc})")

    ids: list[str] = []
    for element in root.iter():
        element_id = element.attrib.get("id")
        if element_id:
            ids.append(element_id)
        for attribute_name in ("href", XLINK_HREF):
            target = element.attrib.get(attribute_name, "")
            if target.startswith("#") and len(target) > 1:
                pass  # Resolved after collecting all IDs.
            elif target and attribute_name in element.attrib and not target.startswith("data:"):
                errors.append(f"{path}: non-local {attribute_name} reference is not allowed: {target}")
        for attribute_value in element.attrib.values():
            for target in URL_REFERENCE.findall(attribute_value):
                if not target:
                    errors.append(f"{path}: empty local URL reference")

    duplicates = sorted({value for value in ids if ids.count(value) > 1})
    if duplicates:
        errors.append(f"{path}: duplicate SVG IDs: {', '.join(duplicates)}")

    known_ids = set(ids)
    for element in root.iter():
        for attribute_name in ("href", XLINK_HREF):
            target = element.attrib.get(attribute_name, "")
            if target.startswith("#") and target[1:] not in known_ids:
                errors.append(f"{path}: unresolved local {attribute_name} reference: {target}")
        for attribute_value in element.attrib.values():
            for target in URL_REFERENCE.findall(attribute_value):
                if target not in known_ids:
                    errors.append(f"{path}: unresolved url(#{target}) reference")

    return errors


def main() -> int:
    if len(sys.argv) < 2:
        print("Usage: verify_svg_contract.py <svg> [<svg> ...]", file=sys.stderr)
        return 2
    errors = [error for argument in sys.argv[1:] for error in verify(Path(argument))]
    if errors:
        print("\n".join(errors), file=sys.stderr)
        return 1
    for argument in sys.argv[1:]:
        print(f"SVG contract verified: {argument}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
