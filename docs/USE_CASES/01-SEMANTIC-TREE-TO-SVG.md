# Use case: Build a semantic tree and manifest it as SVG

## Goal

Construct a small interface description without depending on a native widget tree, then produce inspectable SVG output.

## Participants

- **Application** — supplies interface meaning and data.
- **GUI Core** — stores the recursive semantic tree.
- **GUI.SVG** — converts the supported tree vocabulary into an SVG document.
- **SVG viewer or host** — displays the resulting artifact.

## Preconditions

- The consumer references `TheSingularityWorkshop.GUI.Core` and `TheSingularityWorkshop.GUI.SVG`.
- The input uses node/property combinations supported by the SVG adapter. This use case does not promise that every Core node has an SVG mapping.

## Main path

```csharp
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

var scene = GuiBuilder.Create("svg-scene", "scene")
    .Child("svg-circle", "marker", marker => marker
        .Property("cx", "50")
        .Property("cy", "50")
        .Property("r", "4"))
    .Build();

var svg = new SvgGuiRenderer().Render(scene);

if (!svg.Contains("<svg", StringComparison.Ordinal))
    throw new InvalidOperationException("SVG output was not produced.");
```

This example follows the package-consumer smoke path recorded in the repository's package workflow. It constructs a semantic tree, renders it, and checks that the result is an SVG document. It is a minimal smoke example, not a complete visual-correctness assertion.

## Expected result

The consumer receives a string containing an SVG document that can be saved, inspected, diffed, or opened by an SVG-capable viewer. The renderer is responsible for translating supported semantic nodes and properties into SVG syntax.

## Failure and boundary cases

- **Unsupported node/property:** behavior is adapter-specific; consult [SVG Adapter](../SVG_ADAPTER.md) and its tests rather than assuming all Core properties map to SVG.
- **External `href` values:** serialization is not URL authorization. The consumer must apply its own destination/resource policy before displaying untrusted output.
- **Interaction:** an SVG artifact is not, by itself, an application event loop or a GUI-owned interaction host.
- **Animation:** declarative SVG animation in an output artifact does not establish a GUI-owned animation clock or GPU rendering pipeline.
- **Package availability:** this example must be verified against the selected packed package versions before release; current development workflow evidence is not candidate-version sign-off.

## Evidence

- [Package workflow](../../.github/workflows/package.yml) — clean consumer smoke test and package artifact production.
- [SVG Adapter guide](../SVG_ADAPTER.md) — supported output contract and external-reference policy.
- [GUI Surface Coverage Audit](../GUI_SURFACE_COVERAGE_AUDIT.md) — explicit verification gaps.
- [Release Readiness](../RELEASE_READINESS.md) — authoritative release gate.

## What this proves—and does not prove

It proves the intended, minimal path from a Core semantic tree to SVG output when run against the package versions in the workflow. It does not prove full adapter parity, safe navigation to external resources, interactive hosting, complete SVG vocabulary coverage, or successful consumption of a yet-to-be-selected release candidate.
