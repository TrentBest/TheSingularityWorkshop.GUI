# Recipe 02 — Cartesian coordinates, functions, and transformations

This recipe uses GUI Core + GUI.SVG only: a semantic coordinate-system node, mathematical samples calculated in C#, and SVG produced by the repository's actual adapter. The same 33 x-values drive the base function, vertical translation, horizontal translation, and vertical scaling/reflection.

## What the scene represents

The generator samples x over [-1.5,1.5] at 32 equal intervals, producing 33 points:

```
f(x) = x²
vertical:   f(x) + C = x² + C, with C = 0.5
horizontal: f(x + C) = (x + C)², with C = 0.5
scale:      a f(x) = a x², with a = -0.5
```

The base curve is shown with its sampled points. Three additional curves use the same x-values and derive their y-values directly from the parameters above. The formulas, parameter values, and curve coordinates are all generated from the same constants and sample records; the curves are not hand-drawn approximations.

The coordinate-system root defines the range [-2,4] and asks GUI.SVG to draw grid, axes, origin, and bounds. It explicitly sets both origin coordinates to zero: otherwise the renderer defaults the axes to the midpoint of the configured range, which would not represent the mathematical x=0 and y=0 axes for this asymmetric range.

## Source path

- [Generator source](../CoordinateFunctions/Program.cs)
- [Project references](../CoordinateFunctions/CoordinateFunctions.csproj)
- [Coordinate mapping and adapter](../../../src/SVG/SvgGuiRenderer.cs)
- [Core builder](../../../src/Core/GuiBuilder.cs)

The project references only GUI Core and GUI.SVG. It does not claim to consume Physics, FSM_API, Renderer, or Visualize.

## Generate and verify

From the repository root:

```bash
dotnet run --project samples/demonstrations/CoordinateFunctions/CoordinateFunctions.csproj -- samples/demonstrations/02-coordinate-functions.generated.svg
```

The program parses the adapter output as XML and checks the SVG root, mathematical origin and axes, all 33 base-function samples, and the four expected transformation curves before writing the artifact.

**Evidence status:** GitHub Actions run [39449342991](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39449342991) successfully executed the generator and its SVG/XML assertions, then uploaded the generated artifact as `gui-svg-coordinate-transformations`. This verifies generation and structural assertions in CI. A human visual review of the artifact is still outstanding; do not describe visual fidelity as reviewed until that step is completed.

## Interpretation

- **Vertical translation, f(x)+C:** adding C changes y-values but not x-values.
- **Horizontal translation, f(x+C):** adding C inside the function changes the input before squaring; the parabola moves left when C is positive.
- **Scaling/reflection, af(x):** multiplying by a scales the output; a=-0.5 reflects the base curve across the x-axis.

The current constants intentionally make the differences visible in one shared coordinate system. A later interactive sequence can expose one transformation at a time, but it should preserve this single-source relationship between parameters, formulas, and plotted data.
