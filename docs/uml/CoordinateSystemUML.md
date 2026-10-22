# CoordinateSystem — UML and Use Cases

**Author:** Trent Best  
**Target type:** `TheSingularityWorkshop.Workshop.Gui.CoordinateSystem`  
**Kind:** readonly record struct  
**Source:** [`src/Core/CoordinateSystem.cs`](../../src/Core/CoordinateSystem.cs)  
**Type reference:** [CoordinateSystem API documentation](../api/Core/CoordinateSystem.md)  
**Tests:** [CoordinateSystemTests.cs](../../tests/Core.Tests/CoordinateSystemTests.cs)  
**Diagram status:** manually maintained; reviewed against the current source declaration

## Target-centered class diagram

![Target-centered UML class diagram for CoordinateSystem](../assets/uml/classes/Core/CoordinateSystem.svg)

`CoordinateSystem` is a renderer-neutral value type. Its primary-constructor values define the inclusive numeric bounds and declared origin. The focal type exposes range inspection, containment, pass-through validation helpers, signed deltas, and a conventional normalized instance.

## Use cases

### 1. Declare a normalized coordinate space

**Actor:** an application author or semantic GUI builder  
**Goal:** use a consistent coordinate range when describing points and layout metadata.

1. Use `CoordinateSystem.Normalized` for the conventional 0–100 range and origin (50, 50), or construct a value with explicit bounds and origin.
2. Check points with `Contains(x, y)` before passing them to code that requires an in-range point.
3. Keep the coordinate-space description with the semantic model so adapters can interpret it according to their own supported conventions.

**Outcome:** the application has a small, renderer-independent value describing the declared space.

**Boundary:** construction does not validate that `Minimum < Maximum` or that the origin lies within the range. Callers choosing custom values must ensure the configuration is meaningful.

### 2. Validate coordinates before adding them to a semantic model

**Actor:** a coordinate-space builder or application service  
**Goal:** reject non-finite or out-of-range coordinates consistently.

1. Call `ToPanelX`, `ToPanelY`, `ToPanel`, `FromPanelX`, `FromPanelY`, or `FromPanel`.
2. Handle `ArgumentOutOfRangeException` for NaN, infinity, or values outside the configured inclusive range.
3. Use the returned values as validated coordinates.

**Outcome:** the supplied values are validated and returned unchanged.

**Boundary:** these methods do not scale, translate, invert, or convert pixel/device coordinates. Despite their names, the current implementation is pass-through validation.

### 3. Measure signed movement in coordinate space

**Actor:** a semantic visualization or application service  
**Goal:** calculate horizontal and vertical change between two values.

1. Call `DeltaX(fromX, toX)` or `DeltaY(fromY, toY)`.
2. Interpret the result as `to - from`; negative values indicate a decrease.
3. Apply any range validation separately if the caller requires it.

**Outcome:** the caller receives the signed difference.

**Boundary:** delta methods do not validate either endpoint against the configured range.

## Responsibility boundaries

- **Owns:** numeric bounds, declared origin, containment checks, finite/in-range validation helpers, and signed axis deltas.
- **Does not own:** pixel transforms, device scaling, rendering, layout, mutable UI state, or framework-specific objects.
- **Related projection:** [`CoordinateSystemGuiBuilder`](../../src/Core/CoordinateSystemGuiBuilder.cs) builds a semantic `GuiNode` representation; it does not change the coordinate type into a renderer.
- **Related utilities:** [`CoordinateSystemUtilities`](../../src/Core/CoordinateSystemUtilities.cs) supplies clamping, normalization, denormalization, and Euclidean distance helpers.

## Evidence and verification

The [source declaration](../../src/Core/CoordinateSystem.cs) establishes the value members and validation behavior. The [Core test suite](../../tests/Core.Tests/CoordinateSystemTests.cs) is the place to verify behavior claims. This page and its SVG are manually maintained; the current generator does not yet enforce freshness for every committed authored diagram.

A relationship or use case described here is not evidence that a particular adapter supports every coordinate convention or that a runtime rendering has been observed.
