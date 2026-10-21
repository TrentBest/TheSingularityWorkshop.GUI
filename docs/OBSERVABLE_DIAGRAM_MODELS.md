![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Renderer/development/docs/assets/moniker.svg)

# Observable Diagram Models and Console-Driven Views

> A view observes a model. A driver changes the model. The renderer manifests a consistent snapshot.

## Purpose

GUI.SVG is useful for both source-derived documentation and live experiments, but these are different workflows with different freshness and ownership rules.

- **Generated documentation:** source analysis and optional compiled metadata produce a model; diagram projections produce Markdown/SVG artifacts; CI verifies generation and, eventually, committed-artifact drift.
- **Observable model:** an application constructs or loads a model, applies validated changes through an explicit mutation boundary, and lets a view render a consistent revision.

A console application, test driver, or consuming host could drive a visualization without making source analysis the only producer of model data. This is a target architecture, not a claim that a general reactive model/subscription API exists in GUI Core today.

## Authority and flow

~~~text
C# source / compiled metadata
              |
              v
      canonical documentation model
              |
              v
       diagram projection -------> GUI.SVG renderer
                                      ^
                                      |
console / host -> validated changes -> model snapshot
~~~

The arrows describe responsibility, not a requirement that every run uses every producer. The authoritative model owns data and identity; the driver owns changes; the view observes; the renderer manifests the snapshot. A displayed image must not become the source of truth.

## Required contract for a future observable view

Before calling a visualization live or reactive, define and test:

- stable model identity and monotonic revision/version;
- the mutation boundary and validation/authorization rules;
- snapshot consistency while updates occur;
- observer/subscriber lifecycle, cancellation, and error behavior;
- how a view learns that a revision changed and refreshes the rendered output;
- deterministic behavior for repeated inputs and out-of-order updates;
- backpressure/coalescing rules if updates arrive faster than rendering;
- a clear boundary between UI animation and actual model/runtime time.

For console-driven experiments, the console is a driver, not an authority bypass. Commands must be validated against the owning model. A rendered transition is not evidence that a real runtime transition occurred unless the source is an actual trace or observed state.

## How a live SVG view would refresh

An SVG string or file is a snapshot, not a reactive object by itself. A future host could subscribe to committed model revisions, project a consistent snapshot into diagram data, render new SVG, and replace or update the displayed document. A web host may update a DOM subtree; a desktop host may replace an image/document; a console test may render snapshots to files and compare them. These are host choices, not responsibilities of GUI Core.

Keep three layers separate:

- **Model:** meaning, identity, values, constraints, revisions, and provenance.
- **Projection/layout:** UML semantics, grouping, coordinates, connectors, and bounds.
- **Manifestation/host:** SVG XML, browser DOM, desktop presentation, and refresh lifecycle.

The same driven model should eventually be usable by a visual view, textual diagnostics, and tests without making any renderer the owner of the data.

## Console driver: proposed proof sequence

A small console driver could exercise the same public model contract as a consuming application:

1. Create a small model with several types and relationships; render revision 0.
2. Add, rename, and remove a type through validated model commands.
3. Add/remove a relationship and change a member signature.
4. Render selected revisions and verify that only expected semantic data changed.
5. Apply a deterministic seeded sequence of mutations to stress identity, layout, and rendering.
6. Test empty/large models, long names, duplicate IDs, invalid relationships, rejected commands, rapid updates, and observer disposal.

The proof must assert model invariants and report failures; emitting an SVG alone is not success. A model mutation must change the expected revision, rejected commands must leave the authoritative model unchanged, and a host-level test must eventually prove that the displayed/inspectable view refreshes to the newest committed revision. These are proposed acceptance tests, not claims that a general reactive API already exists.

## Future boundary: `GUI.SVG.Hosted` (TBD)

A future `GUI.SVG.Hosted` package is only a package hypothesis. The architectural idea is that **AnyApp or WebApp drives the experience** while the SVG view observes model state; the SVG itself is not the application loop or source of truth. A host may supply ticks, events, or state revisions, while a host adapter translates them into validated model operations.

**Decision for now: defer implementation.** Do not add a `GUI.SVG.Hosted` project, public API, dependency, or promised launch behavior until a concrete host prototype proves the required boundaries. First prove the renderer-neutral model/snapshot contract and a small console-driven render/refresh loop. Then use the working host evidence to decide package boundaries, lifecycle/cancellation ownership, update/coalescing behavior, interaction commands, and the smallest honest end-to-end proof.

## Current implementation versus proposal

Current GUI Core provides a recursive semantic GuiNode tree and builders. GUI.SVG serializes a tree snapshot into SVG. The source-derived UML tool can produce documentation models and focused diagram output at build time. The general-purpose observable-model/subscription mechanism described above is not yet a GUI Core contract.

Do not add runtime reflection, Roslyn, MSBuild, FSM_COS lifecycle, or domain-specific execution to GUI Core merely to make a diagram look live. A consuming host or a separate model/observation layer should own mutation, runtime state, and refresh policy.

## Related documents

- [Living Documentation](LIVING_DOCUMENTATION.md)
- [Source-derived UML tooling](SOURCE_DERIVED_UML.md)
- [Living UML architecture](LIVING_UML_ARCHITECTURE.md)
- [GUI Gallery](GALLERY.md)
