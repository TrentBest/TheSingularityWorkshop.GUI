#!/usr/bin/env python3
"""Fail when a workflow references an external GitHub Action by a mutable tag.

Local composite actions and Docker image references are intentionally handled
separately; this check targets owner/repository@<40-character commit SHA>.
"""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
USES = re.compile(r"^\s*uses:\s*([^\s#]+)", re.MULTILINE)
PIN = re.compile(r"^[A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+@[0-9a-fA-F]{40}$")


def main() -> int:
    failures: list[str] = []
    workflows = sorted((ROOT / ".github" / "workflows").glob("*.yml"))
    workflows += sorted((ROOT / ".github" / "workflows").glob("*.yaml"))
    for path in workflows:
        content = path.read_text(encoding="utf-8")
        for reference in USES.findall(content):
            if reference.startswith("./") or reference.startswith("docker://"):
                continue
            if not PIN.fullmatch(reference):
                failures.append(f"{path.relative_to(ROOT)}: mutable or unsupported action reference: {reference}")
    if failures:
        print("Workflow action pin check failed:")
        print("\n".join(f"- {failure}" for failure in failures))
        return 1
    print(f"Workflow action pin check passed ({len(workflows)} workflow files scanned).")
    return 0


if __name__ == "__main__":
    sys.exit(main())
