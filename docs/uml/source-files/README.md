# Source-derived UML documentation

This directory separates generated Core type documentation from authored repository documentation.

- `Core.uml-inventory.md` is generated from the compiled Core project and links to the exact source revision used to produce it.
- `Core/` contains generated per-type Markdown pages, an index, and accessible SVG class diagrams.
- `.generated-source-uml-files.txt` is the promotion tool's ownership manifest. Only paths in this manifest may be pruned as stale generated output.
- This README is authored and is intentionally not managed by the generator.

## Refresh the living pages

Use the repository's **Promote source-derived UML documentation** workflow on the `development` branch and explicitly set `confirm_promotion` to true. The workflow rebuilds Core, regenerates the project-semantic model, runs the promotion safety tests, and commits only `docs/uml/source-files`.

The promotion tool refuses unsafe paths and non-generated file types. It never deletes an untracked/authored file; stale output can be removed only when the prior manifest names it. The normal source-derived CI generates and validates artifacts, while the promotion workflow is the deliberate step that updates checked-in living documentation.

Generated pages are source-structure documentation, not a substitute for authored use cases or behavior-oriented guides. Keep those human-owned artifacts separate.
