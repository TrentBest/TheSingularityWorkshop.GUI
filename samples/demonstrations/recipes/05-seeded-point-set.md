# Recipe 05 — Physics seeded point set rendered by GUI.SVG

This sample uses the exact point coordinates exported by Physics' public SeededPointSet2d API. GUI.SVG validates and renders those recorded points; it does not rerun the sampler or invent substitute data.

## Source and provenance

- [Physics point sampler](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/Physics/Spatial/SeededPointSet2d.cs)
- [Physics run-record exporter](https://github.com/TrentBest/TheSingularityWorkshop.Physics/tree/development/samples/SeededPointSet2dExport)
- [GUI.SVG consumer project](../SeededPointSet2d/SeededPointSet2d.csproj)
- [Consumer and marker-verification code](../SeededPointSet2d/Program.cs)

The record identifies the model and schema versions, seed, half-open bounds, count, and each point's stable index and X/Y coordinates. The consumer rejects unsupported schemas, non-finite or out-of-bounds points, unstable indices, and count mismatches. After rendering, it checks every SVG marker ID and mapped coordinate against the record.

## Generate and verify

First, from a checked-out Physics repository, generate the record:

```bash
dotnet run --project samples/SeededPointSet2dExport/SeededPointSet2dExport.csproj --configuration Release -- ./artifacts/seeded-point-set-2d.run.json
```

Then from the GUI repository root, pass that same file to the consumer:

```bash
dotnet run --project samples/demonstrations/SeededPointSet2d/SeededPointSet2d.csproj --configuration Release -- path/to/seeded-point-set-2d.run.json ./artifacts/05-seeded-point-set.generated.svg
```

The consumer's success message means all emitted point markers were checked against the actual input record. CI additionally runs the exporter from Physics and checks the JSON schema and bounds before generating the SVG artifact.

## Interpretation

The plot is a coordinate mapping of a deterministic sample set. The points are not asserted to be a physical probability distribution, collision-free packing, fractal, or simulation of mud or debris. This is the renderer-independent data hand-off proof needed before those richer algorithms are considered.
