# Recipe 06 — Parameterized Mandelbrot field from Physics

This demonstration runs Physics' public `MandelbrotEscapeTime2d.Generate` implementation with configurable grid dimensions, iteration cap, and complex-plane bounds. It serializes actual samples into a versioned run record, then GUI.SVG validates the record and renders one cell per recorded sample without recomputing the fractal.

## Generate a default field and a detail window

From the GUI repository root, with a sibling checkout of `TheSingularityWorkshop.Physics`:

```bash
# Default overview: 160 × 100, cap 80, real [-2.5, 1), imaginary [-1.25, 1.25)
dotnet run --project ../TheSingularityWorkshop.Physics/samples/MandelbrotEscapeTime2dExport/MandelbrotEscapeTime2dExport.csproj --configuration Release -- ./artifacts/mandelbrot-default.run.json

# Detail window: 120 × 80, cap 160, real [-0.9, -0.2), imaginary [-0.35, 0.35)
dotnet run --project ../TheSingularityWorkshop.Physics/samples/MandelbrotEscapeTime2dExport/MandelbrotEscapeTime2dExport.csproj --configuration Release -- ./artifacts/mandelbrot-detail.run.json 120 80 160 -0.9 -0.35 -0.2 0.35

# Render each saved Physics record with GUI.SVG
dotnet run --project samples/demonstrations/MandelbrotEscapeTime2d/MandelbrotEscapeTime2d.csproj --configuration Release -- ./artifacts/mandelbrot-default.run.json ./artifacts/06-mandelbrot-default.generated.svg
dotnet run --project samples/demonstrations/MandelbrotEscapeTime2d/MandelbrotEscapeTime2d.csproj --configuration Release -- ./artifacts/mandelbrot-detail.run.json ./artifacts/06-mandelbrot-detail.generated.svg
```

The exporter parameters after the output path are `width height maxIterations minReal minImaginary maxReal maxImaginary`. Use invariant-culture decimal values. The Physics sampler and exporter enforce the finite bounds, sample-count, iteration, and total work budgets.

## What the explorer proves

CI generates both parameter sets, validates the serialized sample contract, renders each record, and verifies every SVG cell against the stored source classification. This provides a reproducible side-by-side overview/detail workflow; it is currently command-line driven, not an interactive browser control panel.

## Evidence boundary

The SVG is a visual manifestation of a finite mathematical classification field. It is not a physical simulation, GPU renderer, or evidence of a material/terrain generator. The workflow uploads both run records and both generated SVGs for inspection.
