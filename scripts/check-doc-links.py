#!/usr/bin/env python3
"""Check repository-local Markdown links and image targets.

External URLs are intentionally not fetched: this check is deterministic and does not
confuse network outages or third-party changes with repository documentation errors.
"""
from __future__ import annotations

import re
import sys
from pathlib import Path
from urllib.parse import unquote, urlsplit

ROOT = Path(__file__).resolve().parents[1]
MARKDOWN_LINK = re.compile(r"!?(?:\[[^\]]*\])\((<[^>]+>|[^)]+)\)")
HTML_TARGET = re.compile(r"""\b(?:href|src)\s*=\s*["']([^"']+)["']""", re.IGNORECASE)


def target_path(raw: str) -> str | None:
    raw = raw.strip()
    if raw.startswith("<") and raw.endswith(">"):
        raw = raw[1:-1].strip()
    # Markdown permits an optional title after the destination.
    if raw.startswith(("http://", "https://", "mailto:", "tel:", "data:", "javascript:")):
        return None
    parsed = urlsplit(raw)
    if parsed.scheme or parsed.netloc:
        return None
    path = unquote(parsed.path)
    if not path:  # fragment-only link
        return None
    return path


def main() -> int:
    failures: list[str] = []
    files = sorted(p for p in ROOT.rglob("*.md") if ".git" not in p.parts)
    for source in files:
        text = source.read_text(encoding="utf-8")
        destinations = [m.group(1) for m in MARKDOWN_LINK.finditer(text)]
        destinations += [m.group(1) for m in HTML_TARGET.finditer(text)]
        for raw in destinations:
            path = target_path(raw)
            if path is None:
                continue
            destination = (source.parent / path).resolve()
            try:
                destination.relative_to(ROOT.resolve())
            except ValueError:
                failures.append(f"{source.relative_to(ROOT)}: link escapes repository: {raw}")
                continue
            if not destination.exists():
                failures.append(f"{source.relative_to(ROOT)}: missing local target: {raw}")
    if failures:
        print("Documentation link check failed:")
        print("\n".join(f"- {failure}" for failure in failures))
        return 1
    print(f"Documentation link check passed ({len(files)} Markdown files scanned).")
    return 0


if __name__ == "__main__":
    sys.exit(main())
