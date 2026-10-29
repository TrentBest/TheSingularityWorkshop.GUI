<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# CoordinateSystemGuiBuilder

**Author:** Trent Best  
**Fully qualified name:** `TheSingularityWorkshop.Workshop.Gui.CoordinateSystemGuiBuilder`  
**Kind:** sealed class  
**Assembly:** `TheSingularityWorkshop.GUI.Core`  
**Source:** [`CoordinateSystemGuiBuilder.cs`](../../../src/Core/CoordinateSystemGuiBuilder.cs)  
**Tests:** [Core tests](../../../tests/Core.Tests)

## Purpose and boundary

`CoordinateSystemGuiBuilder` projects a `CoordinateSystem` into the renderer-neutral GUI semantic tree. It is a fluent adapter over `GuiBuilder`, not a renderer, pixel-layout engine, or interactive control. The resulting `GuiNode` can be consumed by platform-specific renderers without making this type depend on them.

## UML class diagram

![Compiler-derived class neighborhood for CoordinateSystemGuiBuilder](../../uml/source-files/Core/TheSingularityWorkshop-Workshop-Gui-CoordinateSystemGuiBuilder-2858320f16.svg)

The source-derived diagram is a structural reference. The runtime relationship is explicit in the implementation: this builder owns a `GuiBuilder` and retains the `CoordinateSystem` used to map points.

## Contract and behavior

- `Create(id)` uses `CoordinateSystem.Normalized`; `Create(id, coordinates)` uses the supplied coordinate system.
- Construction initializes a semantic `coordinate-system` node and records coordinate-space, bounds, and origin values using invariant-culture formatting.
- `Text`, `Axes`, `Grid`, `Origin`, and `Bounds` configure semantic properties and return this builder for chaining. Visibility values are encoded as `visible` or `hidden`.
- `Point(id, x, y)` maps the coordinates through `CoordinateSystem.ToPanel` and adds a `coordinate-point` child with invariant-culture `x` and `y` properties.
- `Build()` returns the configured `GuiNode`; it does not serialize or render it.

The builder does not independently validate IDs or coordinate bounds. Those guarantees, if required, must come from the underlying builder, coordinate system, or downstream consumer; do not treat this convenience API as a security boundary.

## Use case

An application can describe axes, grid, origin, bounds, and points once in a semantic tree, then pass that tree to an SVG or another renderer:

```csharp
var node = CoordinateSystemGuiBuilder.Create("plot")
    .Axes()
    .Grid()
    .Origin()
    .Bounds()
    .Point("sample", 25, 75)
    .Build();
```

This example assumes the supplied coordinates are accepted by the configured `CoordinateSystem`; the normalized default uses an inclusive 0–100 range.

## Related types

- [`CoordinateSystem`](CoordinateSystem.md) — coordinate semantics and validation.
- [`CoordinateSystemUtilities`](CoordinateSystemUtilities.md) — range normalization, clamping, and distance calculations.
- [`GuiBuilder`](GuiBuilder.md) — underlying fluent semantic-tree builder.
- [Class documentation standard](../../CLASS_DOCUMENTATION_STANDARD.md)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
