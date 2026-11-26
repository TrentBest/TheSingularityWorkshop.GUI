<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>
# UML Views and Data-Card Standard

**Status: normative presentation proposal for owner review.** This document reconciles the useful taxonomy from PR #39 and the visibility-card composition from PR #57. It specifies intended presentation semantics; it does not claim that every renderer or interactive editing capability implements them yet. The owner-acceptance and evidence gates in [Full GUI Domain Coverage](FULL_GUI_DOMAIN_COVERAGE.md) remain open.

## 1. Keep the view families distinct

A **data card** is a visual primitive: a bordered, colored container with an identifiable header and nested body. It is not itself a UML diagram.

- **Class / AST outline:** answers what is declared in a namespace/type and how declarations are owned and nested.
- **Class neighborhood:** answers which verified consumers use a focal type and which dependencies the focal type directly uses.
- **Sequence view:** answers which participants exchange which messages, and in what order, for a named scenario.
- **Use-case, activity, state-machine, component/package, deployment, and communication views** each answer different questions and require their own semantics and evidence.

These views may link to one another and reuse card primitives, stable identity, accessibility conventions, and palette tokens. They must not be collapsed into one layout that confuses declaration ownership, static relationships, and runtime message order.

## 2. Data-card contract

- A card has a complete visible border, distinct header and body, stable semantic identity, and readable text contrast.
- The header identifies the represented item at every nesting level. The body owns the nested content; children must not look like peers of their parent.
- Color is a semantic aid, never the only carrier of meaning. Use labels and/or icons, accessible names, focus indication, and textual equivalents.
- Interactive cards are keyboard-operable and expose expanded/collapsed state. Static SVG is not interactive and must not claim click-to-expand behavior.
- Expansion state and presentation metadata do not mutate the authoritative source/domain model.
- An edit affordance is available only when a host exposes an authorized, validated domain command. A plus sign or clickable card is not mutation authority.

## 3. Class / AST outline: ownership and ordering

The namespace is the outer source-outline card. Types are nested under their declaring namespace or containing type; members are nested under their declaring type. Each supported field, property, method, constructor, event, operator, and nested type retains its actual declaration kind and identity. Usings/imports form a separate namespace/file-context section and preserve aliases, static/global distinctions, and source order.

### Member presentation order

The intended source-outline presentation uses this deterministic precedence:

1. **Public instance declarations**
2. **Static declarations**
3. **Protected instance declarations**
4. **Internal / assembly-visible instance declarations**
5. **Private / implementation-only instance declarations**
6. **Other or unrecognized accessibility**, retained visibly with its exact source modifier

The Static section is a **presentation bucket**, not a C# accessibility level. It contains static declarations of every accessibility, including public, protected, internal, and private static members. A declaration appears exactly once: static members belong in the Static bucket, and their exact accessibility remains visible in the signature/metadata. Non-static declarations remain in their corresponding access group. This resolves the requested Public → Static → Protected → Private reading order without pretending that `static` is an access modifier or duplicating public static members.

Within every bucket, member-kind subgroups appear in this order:

1. **Properties**
2. **Methods**
3. **Fields**
4. Other supported kinds (constructors, events, operators, nested types, and additional declarations) in documented deterministic subgroups

Within a subgroup, retain source order unless the source projection documents a stable deterministic alternative. Preserve exact language accessibility and modifiers, including `internal`, `protected internal`, `private protected`, `static`, `const`, `readonly`, `abstract`, `virtual`, generic constraints, overload identity, nullable annotations, and property accessor modifiers. Never infer or silently normalize away a source modifier.

This is a projection rule, not a claim that the current SVG renderer already implements these buckets. The current renderer's tested visibility tiers and static markers are described separately in [Data Cards and UML](DATA_CARDS_AND_UML.md).

### Progressive disclosure

- Namespace/type ownership and public contract are initially discoverable.
- Public instance declarations show their signature/contract by default.
- Static declarations remain in their own ordered section and show exact accessibility plus the static marker.
- Less-exposed declarations may be collapsed by default in an interactive host, but their headers and direct-item counts remain visible.
- Method bodies, initializers, property accessor details, attributes, source spans, and deeper AST details are progressively expandable where the source model supplies them.
- Empty groups may be omitted in compact read-only output. An empty group exposes an add affordance only when the current edit mode and domain command explicitly permit insertion.
- A static SVG either renders all information it claims to represent or labels itself as a limited summary and links to a complete representation.

## 4. Class neighborhood: relationships, not content

The focal target is centered and fully collapsed to its identity. Place verified direct consumers above it and the target's verified direct dependencies below it. Put built-in type conduit labels/stubs on the left and relevant external package/framework references on a separate right-side scope rail.

- Label the actual relationship: constructs, calls, implements, inherits, supplies, or another source-grounded relation.
- Do not imply transitive use is direct, or composition merely because one type calls another.
- Distinguish direct dependencies from optional, inferred, transitive, external, and unresolved references.
- The right rail is diagram scope, not an assertion that the focal type uses every listed package/framework.
- Type-conduit colors are stable semantic tokens shared across diagrams/renderers. Keep labels and accessible descriptions; color alone is insufficient.
- Identify analysis scope and provenance. “No consumer found in this scan” is not proof that no consumer exists.

## 5. Sequence and other diagram families

A sequence diagram represents ordered messages between named participants in a particular scenario. Ground it in tests, traces, documented behavior, or an explicitly labeled intended use case. Distinguish observed runtime traces from illustrative intent. Do not infer message order from class proximity or member listings.

Use-case diagrams represent actors and goals; activity diagrams represent workflows and decisions; state machines represent states/events/guards/transitions; component/package diagrams represent module boundaries/dependencies; deployment diagrams represent actual runtime nodes/topology. Each family needs its own fixtures, acceptance criteria, and evidence boundary.

## 6. Source-of-truth and implementation boundary

For source code, a language-aware AST/compiler model is authoritative. The data-card tree is a renderer-neutral projection; GUI Core must not become a C# parser or source editor. A separate code-domain adapter/command handler maps validated edit commands to the authoritative source model, checks legal modifier combinations, applies or rejects the change, and regenerates the view. SVG/XML and renderer-owned copies are never the source of truth.

## 7. Evidence and acceptance

Before calling this standard implemented, retain evidence for:

- one fixture covering public, static across multiple access levels, protected, internal, private, and unrecognized visibility;
- no duplicated declarations and exact source modifiers retained;
- deterministic member-kind order: Properties → Methods → Fields → other supported kinds;
- the same semantic fixture rendered consistently by each adapter that claims support;
- tests for ordering, empty groups, progressive disclosure, accessibility, and stable identity;
- explicit supported/unsupported/unverified adapter behavior;
- a real interactive host for keyboard/focus/expand behavior; static SVG tests do not prove interaction;
- validated edit-command tests against an authoritative source model before claiming AST editing.

Until these checks and owner acceptance are complete, this is a documented presentation proposal, not a release-accepted GUI domain contract.


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
