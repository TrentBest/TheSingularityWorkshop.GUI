# GUI Model Contract

This document describes the current neutral GUI representation and the direction in which it is expected to evolve.

## Current node contract

GuiNode currently contains:

| Concept | Representation |
| --- | --- |
| Kind | string |
| Identity | string |
| Text | optional string |
| Source | optional string |
| Properties | read-only string-to-string map |
| Children | ordered recursive list |

GuiBuilder is a fluent construction API over that model.

## Invariants

1. A node must have a non-empty kind.
2. A node must have a non-empty identity.
3. Child order is significant.
4. A built node is a snapshot.
5. Core properties are copied when the node is constructed.
6. Core does not execute platform callbacks.
7. Core does not own a rendering lifecycle.

## What neutral means

Neutral does not mean the lowest common denominator of HTML and WPF.

It means the model represents intent rather than implementation.

For example:

- kind = Button is intent.
- onclick = JavaScript callback is implementation.
- role = primary-action can be intent if its meaning is defined by the contract.
- display:flex is implementation.
- position = 50,50 can be intent when expressed through a documented coordinate contract.

This distinction is critical. If a Core property is merely a disguised HTML attribute, the architecture has already leaked.

## Planned contract growth

The model should evolve deliberately toward explicit concepts rather than adding arbitrary string properties forever.

Candidate future concepts include:

- typed layout constraints;
- semantic interaction declarations;
- accessibility intent;
- resource references;
- data/state bindings;
- visibility and enabled state;
- measurement and sizing intent;
- capability negotiation;
- animation intent;
- localization;
- deterministic serialization.

Each candidate should first acquire a platform-neutral meaning and tests before entering Core.

## Serialization

Serialization is a future capability, not an implicit requirement of the current GuiNode API.

When introduced, serialized GUI trees should preserve semantic intent and stable identities without serializing platform objects or delegates.

## Structured diagrams and data cards

`DataCard` is the first explicit, platform-neutral structured-diagram primitive. It provides stable identity, semantic kind, primary and optional secondary header text, an initial expansion preference, and ordered child cards. Its child sequence is snapshotted at construction, like `GuiNode`.

`DataCard` deliberately does not own source-code ASTs, access-control policy, edit commands, current expansion state, or renderer elements. For C# source, a real AST/syntax model belongs to the code/creation domain. A code-domain projection should present ordered `using` directive cards, then a true namespace containment hierarchy whose contents include class, struct, and other type declarations. Member groups are ordered Public, Protected, Private; public groups and declarations are expanded by default, while the Usings section and protected/private groups are collapsed. Expanding a property reveals accessor and initializer details. Accepted visibility edits regenerate the card projection so the member moves groups without losing identity.

A collapsed section/group may show a derived count such as `Collapsed (4)`. In edit mode, an explicit add-to-region action may target that collapsed region without forcing expansion first; an empty region should not offer a pointless disclosure control. View/inspect modes must not expose mutation affordances. Counts are derived from the current projected children, not independently mutable state.

Exact C# modifiers such as `internal`, `protected internal`, and `private protected` must remain represented even where the display policy groups them. The code/creation domain owns the authoritative definition; the editor/command handler validates and applies edits; SVG/WPF/Blazor adapters manifest the model. Semantic add/edit actions and explicit header/body presentation regions remain follow-up work rather than being disguised as arbitrary string properties. See [UML-Inspired Data Cards](DATA_CARDS_AND_UML.md) for the intended boundary and acceptance criteria.
