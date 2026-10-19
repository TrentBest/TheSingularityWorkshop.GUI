# GUI Type Documentation

This is the entry point for source-grounded per-type documentation and UML class diagrams.

- [Production type inventory and coverage status](TYPE_INVENTORY.md)
- [Per-Type Documentation and UML Standard](../CLASS_DOCUMENTATION_STANDARD.md)
- [Data Cards and UML architecture](../DATA_CARDS_AND_UML.md)

## Current first vertical slice

- [DataCard](Core/DataCard.md) — immutable card model, child snapshot, collapsed-summary contract, and its source-grounded UML SVG.

## Coverage policy

The inventory tracks every declared production type across Core, Blazor, SVG, and WPF, including nested and internal types. A type is complete only when its page and embedded UML SVG both exist, the relationships have been checked against source, and the documentation links pass CI. XML API comments remain complementary and do not replace these pages.
