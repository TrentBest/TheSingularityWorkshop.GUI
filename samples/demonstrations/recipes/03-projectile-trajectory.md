# Recipe 03 — Projectile trajectory from a recorded Physics run

This example visualizes a run record owned by [TheSingularityWorkshop.Physics](https://github.com/TrentBest/TheSingularityWorkshop.Physics), rather than inventing trajectory samples inside GUI.SVG. It reads the recorded X and Z positions from the JSON samples and uses GUI Core + GUI.SVG to render the trajectory and one marker for every recorded state.

## Source and provenance

- Physics run record: [projectile-trajectory.run.json at pinned commit `fc537e527a1590fbc4c18c39a4e2ad936b7e8ed3`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/fc537e527a1590fbc4c18c39a4e2ad936b7e8ed3/docs/experiments/projectile-trajectory.run.json)
- [GUI.SVG consumer project](../ProjectileTrajectory/ProjectileTrajectory.csproj)
- [Consumer and verification code](../ProjectileTrajectory/Program.cs)
- [CI workflow](../../../.github/workflows/package.yml)

The source record identifies model `kinematics.constant-acceleration-projectile`, model version 1, schema version 1, metres and seconds, and the `world, Z-up` coordinate frame. It contains nine states from 0 through 2 seconds. The GUI.SVG consumer does not re-run the physics model; the recorded samples are the input data.

## Generate and verify

From the GUI repository root, provide a copy of the Physics-owned run record:

```bash
dotnet run --project samples/demonstrations/ProjectileTrajectory/ProjectileTrajectory.csproj --configuration Release -- path/to/projectile-trajectory.run.json samples/demonstrations/03-projectile-trajectory.generated.svg
```

The consumer checks the model/schema identity, required units and coordinate frame, finite values, increasing sample times, and non-degenerate X/Z ranges. It then checks that the emitted SVG polyline matches the mapped input samples and that every input state has a matching marker.

The package workflow downloads the source JSON from the pinned Physics commit, runs the consumer, verifies the SVG against the input samples, and uploads the generated SVG. The complete workflow passed in [run 39453243758](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/39453243758); the uploaded `gui-svg-physics-projectile-trajectory` artifact was visually inspected. The curve and all nine recorded-state markers match the source record. This verifies the Physics JSON → GUI Core scene → GUI.SVG artifact path; it does not claim that GUI.SVG executes the Physics simulation.

## Interpretation

- Horizontal position is the recorded world X coordinate, in metres.
- Vertical position is the recorded world Z coordinate, in metres; Physics declares the frame Z-up.
- Each marker represents a saved state from the run record, not an interpolated or freshly simulated point.
- Elapsed time is retained as source metadata and shown in the plot summary; the curve itself plots X against Z.
