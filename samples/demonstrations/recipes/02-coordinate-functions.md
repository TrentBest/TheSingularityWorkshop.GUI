# Recipe 02 — Cartesian coordinates and a sampled function

This recipe starts with GUI Core + GUI.SVG only: a semantic coordinate-system node, function samples calculated in C#, and SVG produced by the repository's actual adapter. It intentionally begins with f(x)=x²; translations and scaling belong in the next sequence once this base plot is verified.

## What the scene represents

The generator samples the function over [-2,2] at 32 equal intervals, producing 33 points:

```
f(x) = x²
xᵢ = -2 + 4i/32
yᵢ = f(xᵢ)
```

The coordinate-system root defines the Cartesian range and asks GUI.SVG to draw grid, axes, origin, and bounds. Each coordinate-point stores its mathematical x and y; SvgGuiRenderer maps those values into SVG coordinates. The plotted marks therefore come from the function's computed samples, rather than from a hand-authored SVG path.

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

The program parses the adapter output as XML and checks the SVG root, the Cartesian axes, and all 33 function samples before writing the artifact.

**Evidence status:** source and project files have been added; the command has not yet been executed in this work session. Do not treat the generated artifact or passing assertion as verified until the command succeeds and its output is reviewed.

## Next sequence: transformations

The next step should reuse the same coordinate and sampling model to compare:

- f(x)=x²
- f(x)+C=x²+C: vertical translation
- f(x+C)=(x+C)²: horizontal translation
- af(x)=ax²: vertical scaling, with negative a reflecting across the x-axis

A sequence can show one transformation at a time or place the stages left-to-right with the current stage highlighted. In either presentation, the formula, parameter values, and sampled points must be generated from the same function definition. This recipe does not yet claim those transformations are implemented.
