# SvgUmlDocumentationGraphRenderer.cs.cs

- **Project:** SVG
- **Source:** [`src/SVG/SvgUmlDocumentationGraphRenderer.cs`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0/src/SVG/SvgUmlDocumentationGraphRenderer.cs)
- **Source revision:** [`1471b9a`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commit/1471b9aa90db1ad0de9ec2653f8422dbe5fa16d0)

## Responsibility

Renders a focused neighborhood from the canonical UML documentation model.

## Source-grounded facts

- Type identities and relationship endpoints come from model IDs, not display-name matching.
- Nested graph/layout records represent graph nodes and node positions.

## Declared types and generated UML evidence

- `SvgUmlDocumentationGraphRenderer`
  - Generated type page: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgUmlDocumentationGraphRenderer-ef7118f7d2.svg`
  - Generated SVG: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgUmlDocumentationGraphRenderer-ef7118f7d2.svg`
- `SvgUmlDocumentationGraphRenderer.GraphNode`
  - Generated type page: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgUmlDocumentationGraphRenderer-GraphNode-746c9a0037.svg`
  - Generated SVG: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgUmlDocumentationGraphRenderer-GraphNode-746c9a0037.svg`
- `SvgUmlDocumentationGraphRenderer.NodePosition`
  - Generated type page: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgUmlDocumentationGraphRenderer-NodePosition-76addb5852.svg`
  - Generated SVG: `svg/types/TheSingularityWorkshop-Workshop-Gui-Svg-SvgUmlDocumentationGraphRenderer-NodePosition-76addb5852.svg`

The type page and SVG filenames above are from the downloadable `solution-derived-uml-inventory` artifact in [run #39509559623](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39509559623). The SVG is generated from the compiler-backed documentation model and shows the focused type neighborhood; it is not a substitute for reading the source. The generated artifact is not yet committed beside this authored guide.

## Verification boundaries

- The solution workflow built this project's exact Release assembly and reconciled its assembly identity/type metadata against the Roslyn model.
- The generated model verifies type inventory, not member-level compiled metadata parity.
- SVG accessibility title/description and local structure are validated by CI; painted text bounds and visual layout still require a separate visual audit.
