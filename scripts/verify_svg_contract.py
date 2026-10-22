#!/usr/bin/env python3
"""Fail-closed structural checks for generated, source-derived SVG artifacts.

This checks machine-verifiable SVG contracts; it does not replace rendered visual
inspection or prove that glyphs fit their painted bounds.
"""
from __future__ import annotations

import re
import sys
from collections import Counter
import xml.etree.ElementTree as ET
from pathlib import Path
from urllib.parse import unquote, urlsplit

SVG_NS = "{http://www.w3.org/2000/svg}"
XLINK_HREF = "{http://www.w3.org/1999/xlink}href"
URL_REFERENCE = re.compile(r"url\(#([^)]*)\)")
SOURCE_LINK = re.compile(
    r"https://github\.com/TrentBest/TheSingularityWorkshop\.GUI/blob/[0-9a-f]{40}/"
    r"[A-Za-z0-9._~%/-]+#L[1-9][0-9]*"
)


def is_allowed_source_link(target: str) -> bool:
    """Allow only revision-pinned links to source in this repository."""
    return SOURCE_LINK.fullmatch(target) is not None


def is_pinned_source_href(target: str) -> bool:
    parsed = urlsplit(target)
    if parsed.scheme != "https" or parsed.netloc != "github.com":
        return False
    parts = parsed.path.strip("/").split("/")
    if (
        len(parts) < 5
        or parts[0] != "TrentBest"
        or parts[1] != "TheSingularityWorkshop.GUI"
        or parts[2] != "blob"
        or re.fullmatch(r"[0-9a-fA-F]{40}", parts[3]) is None
    ):
        return False
    if any(segment in {"", ".", ".."} for segment in map(unquote, parts[4:])):
        return False
    return not parsed.fragment or re.fullmatch(r"L[1-9][0-9]*", parsed.fragment) is not None


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
            elif (target and attribute_name in element.attrib and
                  not target.startswith("data:") and not is_allowed_source_link(target)):
                errors.append(f"{path}: non-local {attribute_name} reference is not allowed: {target}")
        for attribute_value in element.attrib.values():
            for target in URL_REFERENCE.findall(attribute_value):
                if not target:
                    errors.append(f"{path}: empty local URL reference")

    duplicates = sorted(value for value, count in Counter(ids).items() if count > 1)
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
