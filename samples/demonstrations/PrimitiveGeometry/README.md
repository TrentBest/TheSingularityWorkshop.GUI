# Runnable proof: primitive geometry

This project uses project references to the current GUI Core and GUI.SVG source, so it exercises the repository implementation rather than a package version or hand-authored artifact.

From the repository root:

```bash
dotnet run --project samples/demonstrations/PrimitiveGeometry/PrimitiveGeometry.csproj -- samples/demonstrations/01-primitive-geometry.generated.svg
```

A successful run parses the renderer output as XML, verifies the expected rectangle and circle by their stable IDs, writes the generated SVG, and prints `PASS`. A failed assertion exits with an exception rather than silently producing a claimed proof.

The generated file is deliberately named `01-primitive-geometry.generated.svg` to distinguish it from `../01-primitive-geometry.svg`, the authored visual reference. Once this project has been run and the resulting artifact reviewed, the gallery should link to the generated artifact and record the tested commit and command. Do not label the reference SVG as adapter-generated.
