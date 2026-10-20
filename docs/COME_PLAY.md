# Come Play: Explore the Singularity Workshop

![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

## Software should be something you can explore

The Singularity Workshop is an attempt to make software systems easier to compose, inspect, and experience—not just easier to describe in diagrams.

The work starts with a boundary: an experience has its own behavior and state; an interface observes that experience and gives people a way to understand and interact with it. The interface should not have to become the experience itself. The same intent should be able to take different forms on different platforms, without pretending those platforms have identical capabilities.

This repository explores that idea through a platform-neutral GUI model and several manifestations: Blazor, SVG, and WPF. The implementation is real, but its coverage is still growing. The roadmap and tests distinguish what exists today from what remains a design or implementation task.

## Pick a door

### I want to see something

Start with the [generated SVG gallery](assets/svg-adapter-gallery.svg). Open the SVG directly and watch the marker pulse and travel along its path. It is a generated, inspectable artifact—not a screenshot of a mockup.

Then open [Start Here](START_HERE.md) for the ten-minute evaluation path and links to the source and tests behind the examples.

### I want to understand the engineering

Follow the path:

```text
experience behavior
    -> semantic GUI model
    -> platform adapter
    -> browser, SVG, or native manifestation
```

Read the [GUI Model](GUI_MODEL.md), inspect [Platform Adapters](PLATFORM_ADAPTERS.md), and challenge the boundary in the source and tests. Core is intended to remain platform-neutral; adapters own platform-specific presentation. The current adapters do not yet have complete feature parity.

### I want to explore the bigger idea

The [Theory Guide](THEORY_GUIDE.md) explains the architectural ideas behind the implementation: the GUI as an extrinsic interface layer, the breakwater principle, digital shadows, execution boundaries, representational compatibility, and spatial representation.

Treat the theory as an argument to test—not a request to take the architecture on faith.

## How to help

You do not need to adopt the whole vision to contribute something valuable.

- **Try to break a boundary.** Find a platform-shaped dependency in Core or a semantic concept that only makes sense on one platform.
- **Reproduce a claim.** Build, run a test, inspect an SVG artifact, or report exactly where the documented behavior differs from the code.
- **Bring a real problem.** A state inspector, a floor plan, an operational dashboard, a spatial interface, or a diagnostic view can expose what the model is missing.
- **Improve one small piece.** A focused issue, test, documentation correction, or adapter improvement is more useful than agreeing with every long-term ambition.

When reporting a gap, include the repository path, steps to reproduce, and expected versus actual behavior. That helps turn an idea into a testable contract.

## Honesty is part of the invitation

The project is under active development. Some model and adapter paths are implemented and tested; broader interaction semantics, lifecycle, capability negotiation, cross-adapter parity, and runtime composition remain incomplete. A live Workshop page is useful to explore, but it is not by itself proof that every current development integration path is active.

Use the [roadmap](../ROADMAP.md) and [release-readiness checklist](RELEASE_READINESS.md) to see what is implemented, tested, verified in CI, specified, or still proposed.

**You do not have to believe the vision. Come inspect one working piece, ask a difficult question, and help make the next piece real.**
