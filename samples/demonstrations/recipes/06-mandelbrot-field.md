# Recipe 06 — Mandelbrot field from Physics

This demonstration checks out a pinned Physics commit, runs the actual `MandelbrotEscapeTime2d.Generate` implementation, serializes its samples into a versioned run record, and feeds that record to GUI.SVG. The GUI consumer validates the schema, dimensions, bounds, row-major indices, iteration values, and escape classifications before rendering one SVG cell for every recorded sample.

## Run locally

From the GUI repository root, first generate the record from a Physics checkout:

```bash
dotnet run --project ../TheSingularityWorkshop.Physics/samples/MandelbrotEscapeTime2dExport/MandelbrotEscapeTime2dExport.csproj --configuration Release -- ./artifacts/mandelbrot-escape-time-2d.run.json
dotnet run --project samples/demonstrations/MandelbrotEscapeTime2d/MandelbrotEscapeTime2d.csproj --configuration Release -- ./artifacts/mandelbrot-escape-time-2d.run.json ./artifacts/06-mandelbrot.generated.svg
```

The sample renders the stored iteration/escape values. It never recomputes the Mandelbrot algorithm in the GUI layer.

## Evidence boundary

The SVG is a visual manifestation of a finite mathematical classification field. It is not a physical simulation, a GPU renderer, or evidence of a material/terrain generator. The package workflow uploads both the run record and generated SVG for inspection.
