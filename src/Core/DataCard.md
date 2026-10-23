# DataCard

**Author:** Trent Best  
**Source:** [`DataCard.cs`](DataCard.cs)  
**Public API page:** [`docs/api/Core/DataCard.md`](../../../docs/api/Core/DataCard.md)  
**UML and use cases:** [`DataCardUML.md`](../../docs/uml/DataCardUML.md)

## Purpose

`DataCard` is an immutable, renderer-neutral presentation model for one card in a structured diagram. It stores identity, semantic kind, primary and optional secondary labels, an initial expansion preference, an ordered child-card snapshot, and a capability hint for requesting child insertion.

## Contract at a glance

- `id`, `kind`, and `title` must be nonblank; invalid values throw `ArgumentException`.
- The supplied child sequence is copied into an ordered read-only snapshot. Later changes to the source sequence do not alter the card.
- `IsExpandedByDefault` is an initial presentation preference, not live expansion state.
- `CanRequestChildInsertion` is a UI affordance hint, **not authorization**. The domain command handler must validate every requested insertion.
- `GetCollapsedSummary(isExpanded)` returns `null` when expanded or childless; otherwise it returns `Collapsed (N)`, where N is the snapshotted child count.

## Minimal usage

```csharp
var card = new DataCard(
    id: "namespace-workshop",
    kind: "namespace",
    title: "TheSingularityWorkshop.Workshop.Gui",
    children: typeCards,
    canRequestChildInsertion: false);

var summary = card.GetCollapsedSummary(isExpanded: false);
```

The model describes content and presentation hints. It does not draw the card, persist expansion state, insert children, or authorize domain mutations.

## Responsibility boundary

Keep domain meaning and command handling in the owning application. Renderer adapters may map `Kind`, labels, children, and hints into platform-specific presentation, but must not interpret `CanRequestChildInsertion` as permission to mutate the model or domain.
