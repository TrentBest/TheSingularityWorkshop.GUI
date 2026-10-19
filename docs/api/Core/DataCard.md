# DataCard

**Fully qualified name:** `TheSingularityWorkshop.Workshop.Gui.DataCard`  
**Kind:** sealed class  
**Source:** [`src/Core/DataCard.cs`](../../../src/Core/DataCard.cs)  
**Assembly/package:** `TheSingularityWorkshop.GUI.Core`  
**Verification:** [SVG renderer tests](../../../tests/SVG.Tests/SvgDataCardRendererTests.cs); Core tests are the authority for the model's own invariants.  
**Related:** [Data cards and UML](../../DATA_CARDS_AND_UML.md), [SVG adapter](../../SVG_ADAPTER.md)

## Responsibility

`DataCard` is an immutable, renderer-neutral snapshot of one card in a structured diagram. It records stable identity, semantic kind, display text, default expansion intent, a child-insertion capability hint, and an ordered snapshot of child cards. It deliberately does not own renderer elements, current interactive expansion state, source-code ASTs, or edit authorization.

## UML class diagram

![UML class diagram for DataCard](../../assets/uml/classes/Core/DataCard.svg)

The diagram reflects members declared by `DataCard.cs`. UML visibility notation: `+` public and `−` private. Nullable annotations and optional constructor defaults are shown where useful. The class declares no base class or implemented interface. The child collection is a private field; child cards are exposed through the public read-only `Children` property.

## Declared contract

- The constructor rejects blank `id`, `kind`, or `title` values with `ArgumentException`.
- Child cards are enumerated into a list at construction time and exposed through a read-only collection as `IReadOnlyList<DataCard>`. Caller order is preserved.
- `Id`, `Kind`, `Title`, `Subtitle`, `IsExpandedByDefault`, and `CanRequestChildInsertion` are get-only properties.
- `GetCollapsedSummary(isExpanded)` returns `null` if the card is expanded or has no children. Otherwise it returns `Collapsed (n)`, where `n` is the direct-child count.
- `CanRequestChildInsertion` is a presentation hint, not authorization. A domain command handler must validate any mutation request.
- Interactive expansion state remains external to the model; `IsExpandedByDefault` defines only the initial preference.

## Example

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

## Boundaries and verification status

This is a generic presentation model, not a C# parser or complete UML metamodel. Source-specific meaning belongs in the authoritative code model and its adapter. The SVG currently records the source-verified member surface; the automated generator and stale-diagram enforcement remain unfinished. This page is one worked example, not proof that the full per-type inventory is complete.
