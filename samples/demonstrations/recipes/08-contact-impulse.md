# Recipe 08 — Contact impulse from a recorded Physics result

This example manifests one actual Physics resolver result as a semantic two-body plate. The source record supplies the body masses, initial and final velocities, contact normal, restitution, impulse magnitude, and time boundary. GUI Core composes the presentation tree; GUI.SVG serializes it. The consumer does not solve the contact response.

## Source and provenance

- Physics record: [contact-impulse.run.json at pinned development commit `8b7b3f0e99bb90a4f17445fc438985925cb84048`](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/8b7b3f0e99bb90a4f17445fc438985925cb84048/docs/experiments/contact-impulse.run.json)
- Physics explanation: [Contact impulse: the instant that changes velocity](https://github.com/TrentBest/TheSingularityWorkshop.Physics/blob/development/docs/experiments/contact-impulse.md)
- [GUI.SVG consumer project](../ContactImpulse/ContactImpulse.csproj)
- [Consumer and record validation](../ContactImpulse/Program.cs)
- [Dedicated CI workflow](../../../.github/workflows/physics-contact-impulse.yml)

The record identifies `collision.single-contact-normal-impulse`, version 1, and declares kg, m/s, and N*s. In the checked-in case, two 1 kg bodies approach at +1 and −1 m/s along the contact normal. A 1 N*s normal impulse changes both final normal velocities to zero. The resolver advances no physical time.

## Ownership and interpretation

- Physics owns the resolver, contact semantics, numerical result, assumptions, and run record.
- GUI Core owns only the neutral scene tree and stable element identities.
- GUI.SVG owns SVG manifestation and accessible document structure.
- SURPS owns how this subject is organized and taught in its rulebook.
- The host owns any authoritative world-step ordering and state commit.

**Important:** impulse (N*s) is not force (N). The arrows show equal-and-opposite normal impulses, not sustained forces. The circles are schematic markers, not collision geometry. The resolver preserves pose, does not separate the bodies, does not advance time, and does not account for energy conversion. The SVG is a static observation of one recorded result—not a live simulation.

## Reproduce

From the GUI repository root, provide the Physics-owned JSON record:

```bash
dotnet run --project samples/demonstrations/ContactImpulse/ContactImpulse.csproj --configuration Release -- path/to/contact-impulse.run.json artifacts/contact-impulse.svg
```

The consumer validates model/schema identity, units, finite vector values, positive masses, restitution range, unit contact normal, non-negative impulse, and zero physical time advanced. It then verifies the SVG contains the two bodies, both impulse vectors, the recorded magnitude, before/after captions, and accessible root description.

The dedicated [CI workflow](../../../.github/workflows/physics-contact-impulse.yml) [passed on its first run](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions/runs/45103020144): it fetched the Physics record from the pinned commit, ran the .NET consumer, validated the generated SVG structure and accessibility/evidence labels, and uploaded both source record and SVG. It does not publish packages or deploy Pages.

## Why this is useful to a SURPS sourcebook

The same pattern lets a rulebook chapter present a concept with inspectable provenance:

1. link to the authoritative Physics chapter and run record;
2. identify the model and assumptions in the visual;
3. manifest the recorded values without reproducing the solver;
4. preserve an accessible textual explanation and explicit limitations.

This is a second cross-repository proof alongside [Recipe 03 — projectile trajectory](03-projectile-trajectory.md). Together they show that GUI.SVG can present both recorded motion samples and a discrete contact response while remaining independent of Physics and SURPS.
