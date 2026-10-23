# DataCard — UML and Use Cases

**Author:** Trent Best  
**Target type:** `TheSingularityWorkshop.Workshop.Gui.DataCard`  
**Source:** [`src/Core/DataCard.cs`](../../src/Core/DataCard.cs)  
**API contract:** [`DataCard.md`](../api/Core/DataCard.md)  
**Diagram:** compiler-derived structural view, linked below

## Target-centered class diagram

![Compiler-derived class neighborhood for DataCard](source-files/Core/TheSingularityWorkshop-Workshop-Gui-DataCard-aa6db0751e.svg)

The diagram records source structure, not runtime behavior. `DataCard` is a sealed class with immutable public values and an ordered read-only child collection.

## Use cases

### 1. Present a structured hierarchy

**Actor:** a renderer or application view  
**Goal:** describe a namespace/type/member hierarchy without binding the data model to a UI framework.

1. Create each card with a stable ID, semantic kind, and nonblank title.
2. Supply optional subtitles and ordered child cards.
3. Pass the root card to a renderer that understands the semantic kinds.
4. Let the presentation use `IsExpandedByDefault` as an initial preference and `GetCollapsedSummary` for collapsed child counts.

**Outcome:** a stable, framework-neutral hierarchy description. Rendering and ongoing expansion state remain the presentation's responsibility.

### 2. Offer child insertion without granting authority

**Actor:** a presentation and its domain command handler  
**Goal:** expose an add-child affordance while keeping mutation policy outside the display model.

1. Set `CanRequestChildInsertion` only when the presentation should offer the affordance.
2. On user activation, send a request to the owning application.
3. Validate authorization, parent identity, allowed child type, and domain invariants in the command handler.
4. Apply the mutation in the owning model and create a refreshed card snapshot.

**Outcome:** UI affordance and domain authorization remain separate. The boolean alone never authorizes insertion.

## Invariants and boundaries

- Blank ID, kind, or title values are rejected with `ArgumentException`.
- Child order is preserved in a read-only snapshot; the original input enumerable is not retained.
- `GetCollapsedSummary(true)` and calls on childless cards return `null`; a collapsed card with children returns `Collapsed (N)`.
- This type does not render, persist, mutate children, own live expansion state, or enforce domain authorization.
- The diagram is a source-derived structural artifact. It does not prove that a renderer maps every card kind correctly or that a command is authorized.

## Verification

The public contract is grounded in [`DataCard.cs`](../../src/Core/DataCard.cs). Automated source inventory and diagram generation are separate from behavioral tests; release claims require the exact candidate's CI and consumer checks.
