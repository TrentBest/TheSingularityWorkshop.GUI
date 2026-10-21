# Seeded point set from Physics

This sample consumes the versioned JSON run record emitted by the Physics repository's SeededPointSet2d exporter. It validates the record identity, bounds, point count, finite coordinates, and stable point order, then maps the recorded points into a GUI Core scene and manifests that scene with GUI.SVG.

The consumer never regenerates the points. After rendering, it parses the SVG and verifies that every marker ID and mapped coordinate matches the corresponding input record entry.

## Run

```bash
dotnet run --project samples/demonstrations/SeededPointSet2d/SeededPointSet2d.csproj --configuration Release -- path/to/seeded-point-set-2d.run.json samples/demonstrations/05-seeded-point-set.generated.svg
```

The source JSON schema is documented in the [Physics exporter guide](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/samples/SeededPointSet2dExport/README.md). The point coordinates are unit-agnostic; this visualization maps them to an SVG plot and does not assign physical meaning to the point distribution.

## Evidence boundary

This proves Physics-generated data can be consumed and displayed without moving mathematical authority into GUI.SVG. It does not establish a fractal generator, physical particle distribution, collision-free packing, or material simulation.
