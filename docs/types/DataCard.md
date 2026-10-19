# DataCard

**Source:** `src/Core/DataCard.cs`  
**Namespace:** `TheSingularityWorkshop.Workshop.Gui`  
**Kind:** sealed class  
**Tests:** [SVG renderer tests](../../tests/SVG.Tests/SvgDataCardRendererTests.cs) and Core tests  
**Related:** [UML-inspired data-card contract](../DATA_CARDS_AND_UML.md), [SVG adapter](../SVG_ADAPTER.md)

## Responsibility

`DataCard` is an immutable, renderer-neutral snapshot of one card in a structured diagram. It records stable identity, semantic kind, display text, default expansion intent, a child-insertion capability hint, and an ordered snapshot of child cards. It deliberately does not own renderer elements, current interactive expansion state, source-code ASTs, or edit authorization.

## UML class diagram

![UML class diagram for DataCard](assets/uml-data-card.svg)

The diagram records the members declared by `DataCard.cs`. Visibility uses UML notation: `+` public and `−` private. Nullable annotations and optional constructor defaults are shown where useful. No inheritance or implemented interface is shown because the class declares none.

## Contract and behavior

- The constructor rejects blank `id`, `kind`, or `title` values with `ArgumentException`.
- Child cards are enumerated into a list at construction time and exposed through a read-only collection as `IReadOnlyList<DataCard>`. The order supplied by the caller is preserved.
- `Id`, `Kind`, `Title`, `Subtitle`, `IsExpandedByDefault`, and `CanRequestChildInsertion` are get-only properties.
- `GetCollapsedSummary(isExpanded)` returns `null` if the card is expanded or has no children. Otherwise, it returns `Collapsed (n)`, where `n` is the current direct-child count.
- `CanRequestChildInsertion` is a presentation hint, not authorization. The domain command handler must validate any mutation request.
- Expansion state supplied by an interactive presentation is external to this model; `IsExpandedByDefault` only defines the initial preference.

## Usage

```csharp
var card = new DataCard(
    id: "namespace:Example",
    kind: "namespace",
    title: "Example",
    children: new[]
    {
        new DataCard("type:Example.Widget", "class", "Widget")
    });

var summary = card.GetCollapsedSummary(isExpanded: false);
// "Collapsed (1)"
```

## Boundaries and limitations

This type is a generic presentation model, not a C# parser or UML metamodel. Source-specific meaning belongs in the authoritative code model and its adapter. The SVG is a source-grounded documentation artifact; automated regeneration and stale-output enforcement are tracked separately in the [roadmap](../../ROADMAP.md) and are not yet claimed as complete.
