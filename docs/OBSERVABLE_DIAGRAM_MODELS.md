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

## Current implementation versus proposal

Current GUI Core provides a recursive semantic GuiNode tree and builders. GUI.SVG serializes a tree snapshot into SVG. The source-derived UML tool can produce documentation models and focused diagram output at build time. The general-purpose observable-model/subscription mechanism described above is not yet a GUI Core contract.

Do not add runtime reflection, Roslyn, MSBuild, FSM_COS lifecycle, or domain-specific execution to GUI Core merely to make a diagram look live. A consuming host or a separate model/observation layer should own mutation, runtime state, and refresh policy.

## Related documents

- [Living Documentation](LIVING_DOCUMENTATION.md)
- [Source-derived UML tooling](SOURCE_DERIVED_UML.md)
- [Living UML architecture](LIVING_UML_ARCHITECTURE.md)
- [GUI Gallery](GALLERY.md)
