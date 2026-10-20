# Observable Diagram Models and Console-Driven Views

> A view observes a model. A driver changes the model. The renderer manifests the current state.

## Purpose

GUI.SVG should be useful for both published, source-derived documentation and live experiments. These are related workflows, but they have different freshness and ownership rules.

- **Generated documentation:** Roslyn source analysis and optional compiled reflection produce a canonical model; UML projections produce Markdown and SVG artifacts; CI detects drift.
- **Observable model:** an application constructs or loads a model, applies changes through a defined mutation boundary, and notifies subscribers; a view renders the latest consistent snapshot.

The second workflow means a console application can drive a visualization without requiring a source file or compiled domain assembly as its data source. The source-analysis generator is one producer of model data, not the only producer.

## Direction of authority

```text
C# source ──Roslyn──┐
                    ├──> Canonical model ──> Diagram projection ──> GUI.SVG renderer
Compiled assembly ─reflection─┘                       ▲
                                                      │ observes snapshots
Console / test driver ──validated commands──> Model ──┘
```

The arrows describe data and responsibility, not a requirement that every run use every producer. Reflection is a build-time evidence source; it is not a live observation bus. The console driver is an optional producer/mutator. GUI.SVG is a view/renderer, not the authoritative store.

## Minimum observable-model contract

The initial contract should stay small and renderer-neutral:

1. **Stable identity:** each model element has a durable ID independent of screen coordinates and display order.
2. **Consistent snapshot:** a renderer receives one complete model version, not a mixture of fields from different mutations.
3. **Version/revision:** every accepted change advances a monotonically increasing revision or otherwise provides a reliable change token.
4. **Change notification:** subscribers are told that a revision is available. Notifications should identify changed elements when practical, but a full-snapshot refresh remains a correct fallback.
5. **Explicit mutation boundary:** changes pass through model/domain operations or validated commands. A renderer cannot mutate authoritative state by editing its own SVG nodes.
6. **Deterministic projection:** the same model snapshot and projection/rendering versions produce equivalent diagram data and reproducible SVG output.
7. **Lifecycle:** subscriptions can be disposed; renderer errors and observer failures cannot silently corrupt model state.
8. **Provenance:** source-derived facts retain their source/build evidence; driver-authored test data is labeled as such.

Prefer immutable snapshots or an equivalent synchronization boundary. A notification means “read revision N” rather than “the renderer may inspect partially updated mutable collections.” If high-frequency changes arrive faster than rendering, the view may coalesce obsolete intermediate revisions and render the newest complete snapshot. It must not lose the final state.

## How a live SVG view updates

An SVG string/file is a snapshot, not a reactive object by itself. A live host can subscribe to model revisions, project the current snapshot to diagram data, render new SVG, and replace or update the displayed SVG. A web host may use a DOM update strategy; a desktop host may replace the image/document; a console-only test can render to files and compare output. These are host choices, not responsibilities of GUI Core.

Keep three layers separate:

- **Model:** meaning, identity, values, constraints, revisions, provenance.
- **Projection/layout:** UML semantics, grouping, coordinates, connectors, bounds.
- **Manifestation/host:** SVG XML, browser DOM, desktop presentation, refresh lifecycle.

This separation lets the same driven model feed an SVG view, a textual diagnostic, and tests without giving any renderer ownership of the data.

## Console driver: make the view prove itself

A small console driver should eventually exercise the same public model contract as a real consuming application. It should not have private shortcuts that bypass validation just to make a demo move.

A useful demonstration sequence:

1. Create a small model containing several types and relationships.
2. Render revision 0.
3. Add, rename, and remove a type through model commands.
4. Add/remove an inheritance or dependency relationship and change member signatures.
5. Render each selected revision and verify that only expected semantic data changed.
6. Apply a deterministic seeded series of mutations to stress the model and renderer.
7. Test boundary cases: empty model, large model, long names/signatures, duplicate IDs, invalid relationships, rapid repeated changes, failed commands, and observer disposal.
8. Assert invariants and report revision, operation, duration, and failure diagnostics; do not report success merely because SVG was emitted.

“Drive the SVG to extremes” should mean exercising the full supported data contract and its bounds—not inserting arbitrary untrusted XML or bypassing semantic constraints. The model can deliberately generate very large valid diagrams to test layout, overflow, rendering cost, and degradation behavior. Unsupported or invalid states should produce explicit diagnostics.

## Testable claims

- A model mutation changes the expected model revision and emits a notification.
- A rejected command leaves the authoritative model unchanged.
- A view eventually renders the newest committed revision, even when intermediate revisions are coalesced.
- Disposed observers stop receiving updates.
- Repeated rendering of the same snapshot is deterministic.
- Large valid models remain bounded or report a documented limit rather than silently clipping content.
- A console driver, a unit test, and a host view use the same public mutation/observation contract.

Do not claim “live” behavior until a host-level test proves that a model mutation results in a displayed or inspectable refreshed view. Do not claim thread safety until concurrency tests exercise the actual synchronization contract.

## Implementation boundary and sequence

1. Define the versioned model/snapshot and mutation/notification contract without adding renderer-specific types to GUI Core.
2. Add a deterministic console driver and tests against that contract.
3. Connect a renderer adapter that turns snapshots into diagram projections and SVG.
4. Add one host refresh path and test a model change end-to-end.
5. Reuse the same source-derived documentation model where appropriate, without conflating source AST, reflection metadata, mutable runtime state, and diagram layout.
6. Add semantic UML relationships and larger model scenarios only after identity, consistency, and refresh behavior are proven.

This is a complementary milestone to project-wide Roslyn/reflection UML generation. It is not a claim that the current source generator or SVG renderer already supports live subscriptions.
