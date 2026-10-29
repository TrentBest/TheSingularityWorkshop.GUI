<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# CoordinateSystemGuiBuilder UML

**Author:** Trent Best  
**Target:** `TheSingularityWorkshop.Workshop.Gui.CoordinateSystemGuiBuilder`

## Class diagram

![Source-derived structural view of CoordinateSystemGuiBuilder](source-files/Core/TheSingularityWorkshop-Workshop-Gui-CoordinateSystemGuiBuilder-2858320f16.svg)

The target wraps `GuiBuilder` and retains `CoordinateSystem`; it emits a semantic `GuiNode`. The diagram is compiler-derived structural evidence, not a claim that the SVG renderer or runtime interaction is implemented by this class.

## Use case

An application describes a coordinate space and selected visual aids once, then builds a semantic node for a renderer:

```csharp
var plot = CoordinateSystemGuiBuilder.Create("plot")
    .Axes()
    .Grid()
    .Point("sample", 25, 75)
    .Build();
```

The default coordinate system accepts finite values in its configured inclusive range. Rendering, pixel mapping, and interaction are downstream responsibilities.

## Verified relationships

- Uses `GuiBuilder` to compose the semantic node.
- Retains `CoordinateSystem` for point mapping.
- Returns `GuiNode` from `Build()`.
- Does not reference a concrete rendering framework.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
