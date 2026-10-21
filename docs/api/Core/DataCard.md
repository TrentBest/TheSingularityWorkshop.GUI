# DataCard

**Namespace:** `TheSingularityWorkshop.Workshop.Gui`  
**Kind:** sealed class  
**Source:** [`DataCard.cs`](../../../src/Core/DataCard.cs)  
**Focused tests:** [`DataCardTests.cs`](../../../tests/Core.Tests/DataCardTests.cs)

## Responsibility

`DataCard` is an immutable, renderer-neutral card node used to describe structured diagrams. It carries stable identity, semantic kind, header text, an optional subtitle, a default expansion preference, an insertion-capability hint, and an ordered child snapshot. Domain-specific meaning remains in the model that projects into cards.

## UML class diagram

![UML ecosystem class diagram for DataCard](../../assets/uml/classes/Core/DataCard.svg)

The ecosystem view places the verified direct consumer `SvgDataCardRenderer` above the focal type, constructor input types at left, implementation dependencies below, and .NET base-library provenance at right. Spatial position and labels communicate the roles; the diagram intentionally avoids decorative relationship lines. The focus card lists the source-declared field, constructor, properties, and method. It declares no base class or implemented interface.

## Constructor and invariants

The constructor requires nonblank `id`, `kind`, and `title`; violations throw `ArgumentException`. It snapshots the supplied children into a read-only collection, so later mutation of the original enumerable does not change the card's child list.

## Public contract

- `Id`, `Kind`, and `Title` are required, immutable values.
- `Subtitle` is optional secondary header text.
- `IsExpandedByDefault` is an initial presentation preference; live expansion state belongs to the presentation layer.
- `CanRequestChildInsertion` is a capability hint, not authorization. The domain command handler must validate an insertion request.
- `Children` exposes the ordered, read-only child snapshot.
- `GetCollapsedSummary(isExpanded)` returns `null` when expanded or childless; otherwise it derives `Collapsed (N)` from the current number of direct children.

## Relationships and boundaries

- A card can contain zero or more child `DataCard` instances.
- `SvgDataCardRenderer` directly consumes the card hierarchy and produces a static SVG manifestation; its dedicated type page is still pending.
- The type does not parse source code, execute edits, own live UI state, or depend on an SVG/WPF renderer.
- The insertion flag is a presentation hint only; it does not grant mutation authority.

## Verification

Use [`DataCardTests.cs`](../../../tests/Core.Tests/DataCardTests.cs) as the focused contract suite when changing constructor validation, child snapshots, or collapsed summaries. SVG manifestation behavior is covered separately by [`SvgDataCardRendererTests.cs`](../../../tests/SVG.Tests/SvgDataCardRendererTests.cs). The SVG is manually maintained today; no generator or CI drift check is claimed by this page.

## Related documentation

- [Data Cards and UML](../../DATA_CARDS_AND_UML.md)
- [Per-Type Documentation and UML Standard](../../CLASS_DOCUMENTATION_STANDARD.md)
