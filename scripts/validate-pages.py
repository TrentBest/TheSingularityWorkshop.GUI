#!/usr/bin/env python3
"""Validate that every local HTML resource reference resolves inside a Pages artifact."""
from __future__ import annotations

import argparse
from html.parser import HTMLParser
from pathlib import Path
from urllib.parse import unquote, urlsplit


class ResourceParser(HTMLParser):
    def __init__(self) -> None:
        super().__init__(convert_charrefs=True)
        self.references: list[tuple[str, str]] = []

    def handle_starttag(self, tag: str, attrs: list[tuple[str, str | None]]) -> None:
        for name, value in attrs:
            if name.lower() in {"href", "src", "poster", "data"} and value:
                self.references.append((name.lower(), value.strip()))


def resolve_reference(root: Path, page: Path, attribute: str, value: str) -> Path | None:
    parsed = urlsplit(value)
    if parsed.scheme or parsed.netloc or not parsed.path:
        return None
    raw_path = unquote(parsed.path)
    target = (root / raw_path.lstrip("/")) if raw_path.startswith("/") else (page.parent / raw_path)
    target = target.resolve()
    root = root.resolve()
    if target != root and root not in target.parents:
        raise ValueError(f"resource escapes Pages root: {value}")
    if target.is_dir():
        target = target / "index.html"
    return target


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--root", type=Path, default=Path("_site"))
    args = parser.parse_args()
    root = args.root.resolve()
    if not root.is_dir():
        raise SystemExit(f"Pages root does not exist: {root}")

    failures: list[str] = []
    pages = sorted(root.rglob("*.html"))
    if not pages:
        raise SystemExit(f"No HTML pages found under {root}")

    for page in pages:
        document = ResourceParser()
        document.feed(page.read_text(encoding="utf-8"))
        for attribute, value in document.references:
            try:
                target = resolve_reference(root, page, attribute, value)
            except ValueError as error:
                failures.append(f"{page.relative_to(root)}: {error}")
                continue
            if target is not None and not target.is_file():
                failures.append(
                    f"{page.relative_to(root)}: {attribute}={value!r} "
                    f"does not resolve to {target.relative_to(root) if target.is_relative_to(root) else target}"
                )

    if failures:
        print("Pages resource validation failed:")
        for failure in failures:
            print(f" - {failure}")
        return 1

    print(f"Pages resource validation passed: {len(pages)} HTML pages, all local resource references resolve.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
