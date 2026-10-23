# GuiBuilder

**Type:** `TheSingularityWorkshop.Workshop.Gui.GuiBuilder` — sealed class  
**Package:** `TheSingularityWorkshop.GUI.Core`  
**Source:** [src/Core/GuiBuilder.cs](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/src/Core/GuiBuilder.cs)

![Compiler-derived UML neighborhood for GuiBuilder](../../uml/source-files/Core/TheSingularityWorkshop-Workshop-Gui-GuiBuilder-415898c6b3.svg)

## Purpose and boundary

`GuiBuilder` assembles a mutable, fluent description of a renderer-neutral GUI tree and produces an immutable `GuiNode` snapshot. It describes interface intent; it does not render the tree, own an event loop, or execute application commands.

## Example

```csharp
using TheSingularityWorkshop.Workshop.Gui;

var scene = GuiBuilder.Create("svg-scene", "scene")
    .Child("svg-circle", "marker", marker => marker
        .Property("cx", "50").Property("cy", "50").Property("r", "4"))
    .Child("svg-text", "label", label => label.Text("Status")
        .Property("x", "5").Property("y", "12"))
    .Build();
```

The `svg-*` kinds are selected by the consumer; Core does not itself define their rendering semantics.

## Behavior and invariants

- `Create(kind, id)` requires nonblank kind and ID values.
- `Text`, `Image`, `Property`, and `WithOnClick` configure the root. Properties are string-valued and use ordinal key comparison.
- `Child(kind, id, configure)` appends a configured child in declaration order. Other overloads compose an existing builder or node.
- `Build()` uses an explicit stack to create the snapshot without recursive traversal. Direct or indirect builder cycles cause `InvalidOperationException`.
- The built `GuiNode` snapshots properties and children; later builder edits do not mutate earlier snapshots.
- `WithOnClick(commandId)` stores a stable command identifier only. The host must map it to an allowed operation; Core does not dispatch or execute it.

## Failure modes and constraints

Blank kind/ID, blank property names, null property values, null child arguments, and blank command IDs are rejected by argument validation. Cycle detection occurs at build time because cycles can be introduced through composition. Core does not enforce tree-wide unique IDs; adapters may impose stricter requirements (GUI.SVG rejects duplicate IDs).

## Relationships and evidence

- Produces [`GuiNode`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/src/Core/GuiNode.cs).
- Composed by [`CoordinateSystemGuiBuilder`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/src/Core/CoordinateSystemGuiBuilder.cs).
- Convenience factories: [`GuiBuilders`](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/src/Core/GuiBuilders.cs).
- Verification: [Core tests](https://github.com/TrentBest/TheSingularityWorkshop.GUI/tree/development/tests/Core.Tests) and [SVG renderer tests](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/tests/SVG.Tests/SvgGuiRendererTests.cs).

## UML text fallback

`GuiBuilder` owns a private root builder, offers fluent configuration and child composition, and creates immutable `GuiNode` snapshots. Coordinate-aware construction composes it; renderer adapters consume its output. The embedded diagram is compiler-derived, while the purpose and behavioral contract above are human-authored.
