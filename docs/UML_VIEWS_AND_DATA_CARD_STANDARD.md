# UML Views and Data-Card Standard

**Canonical semantics for Workshop UML views**  
Applies to authored diagrams, source-derived UML, GUI.SVG, and any future renderer.

The following are different view types: a **UML data card** is a visual primitive; a **UML class diagram** is the AST/source structure view; a **UML class neighborhood** is a relationship view; a **sequence diagram** is an ordered interaction view. They answer different questions and must not be blended into one ambiguous diagram.

## 1. UML data card

A UML data card is a box with a colored border and a header. When interaction is supported, activating the header expands or collapses nested content inside the box.

- Use a stable, shared palette with readable contrast; border color is semantic, not decoration.
- Keep the item identity visible in the header at every expansion level.
- Preserve parent/child nesting; nested content must not appear to be a peer.
- Interactive cards must be keyboard-operable, expose accessible names and expanded state, and show focus. Color alone never conveys meaning.
- Static SVGs are not interactive. They may show a state or link to deeper details, but documentation must not claim click-to-expand unless implemented.
- Stable identity belongs to the model element, not its screen position or SVG element ID.

## 2. UML class diagram — AST/source structure

**Question answered:** “What is declared in this namespace/class, and what is each member's contract?”

This is the declaration hierarchy, not a consumer/dependency graph.

### Bounding and nesting

1. The namespace is the primary outer bounding box.
2. Each type declared in that namespace has its own data card inside the namespace card.
3. Each field, property, event, constructor, and method is represented by its own member card nested under its declaring type. Nested types remain nested under their declaring type.
4. Grouping may aid navigation, but must not flatten namespace → type → member ownership or erase source declaration identity.

### Member order and default expansion

Present groups in this order: **Public → Static → Protected → Private**.

This is a grouping precedence, not permission to duplicate a member. A public static member belongs in Public and is marked static there. Members with other access levels (such as internal, protected internal, or private protected) must retain their exact modifier in a clearly labeled additional group; do not misclassify them.

- **Public members default expanded to their top-level contract:** fields/properties/events show name and type; methods/constructors show the callable signature, including parameters and parameter types, return type where applicable, and relevant generic information.
- Public implementation/body and deeper declaration details default collapsed.
- Static, protected, and private members default collapsed. Their visible header still identifies the member; expanding reveals its contract and deeper details.
- Expansion is progressive: identity/header → contract/signature → declaration details/source spans → implementation/body, only where the source model provides that information.
- Keep stable declaration order within groups unless a documented deterministic secondary order is required.
- Label the analysis method and gaps. Reflection-only output must not claim to be a full AST.

These are presentation defaults. Interactive AST editing is a separate feature: edits must be validated commands against the authoritative source/AST model, not direct mutations of SVG or renderer-owned copies.

## 3. UML class neighborhood — associations, not content

**Question answered:** “Who consumes this class, what does it use, and what external references are in the diagram's scope?”

The target class appears in the center as a **fully collapsed data card**. This view emphasizes associations and type identity, not member details.

### Spatial layout

- **Above:** verified consumers of the target, such as direct callers, builders, hosts, or tests. Label the real relationship; do not invent consumers or imply transitive usage is direct.
- **Center:** the focal target class, fully collapsed, with name and stable identity visible.
- **Below:** types/components the target itself uses. Separate direct dependencies from optional, inferred, transitive, or unresolved references.
- **Right:** external packages, assemblies, frameworks, or platform references relevant to the diagram. This is a **diagram-scope reference rail**, not an assertion that the target class uses every item shown there. Add a connector from a specific participant only when that relationship is evidenced; label direct versus transitive, optional, or unresolved scope.
- **Left:** built-in data-type conduit stub-outs and their legend, for types such as `byte`, `int`, `long`, `float`, `double`, `char`, and `string`.

### Conduit semantics

- Each built-in type identity has a stable palette token shared across diagrams and renderers. Keep `float` distinct from `double`, `int` distinct from `long`, and `char` distinct from `string`.
- The left-side stub-outs have readable type labels and the semantic color for that type.
- Where the target/member card consumes that type, show a matching **stub-in** on the card's left edge. Rotate the stub-in 180 degrees relative to the legend stub and **omit the repeated type name** from the stub-in; its color maps back to the labeled legend.
- Keep the association from conduit to the relevant member/type traceable. Shared color does not claim conversion, serialization, or runtime transfer.
- User-defined types, generic type parameters, unresolved types, and package references must not silently borrow a built-in type's color.
- If geometry is ambiguous, add an explicit label and accessible textual description. Conduits must not obscure UML relationships.

Show analysis scope and provenance. “No consumer found in this scan” is not proof that no consumer exists. Relationships must be source/compiler-backed or clearly labeled as authored assumptions.

## 4. Sequence diagrams and other families

A sequence diagram shows ordered messages between named participants in a particular scenario. Ground it in tests, traces, documented behavior, or an explicitly labeled intended use case. Do not infer a sequence from class proximity or member listings. Distinguish observed runtime traces from illustrative intended flows.

Keep other UML families distinct and evidence-backed:
- **Use case:** actors and goals.
- **Activity:** workflow steps and decisions.
- **State machine:** states, events, guards, and transitions.
- **Component/package:** module boundaries and dependencies.
- **Deployment:** actual runtime nodes and deployment topology.
- **Communication/collaboration:** participants and relationships for a specific interaction.

A class AST view, class neighborhood, and sequence diagram may link to each other, but must not be merged in a way that confuses declaration ownership, static relationships, and runtime message order.

## 5. Evidence and acceptance checklist

- [ ] Data card has a colored border, identifiable header, and nested expandable content when hosted interactively.
- [ ] AST class view uses a namespace outer card and nests types/members according to declaration ownership.
- [ ] Member ordering is Public → Static → Protected → Private, with overlapping modifiers handled once and other access levels preserved accurately.
- [ ] Public members show their top-level contract/signature by default; implementation detail stays collapsed.
- [ ] Neighborhood target is fully collapsed; consumers are above, target centered, its direct dependencies below.
- [ ] Built-in types have distinct stable colors; labeled left stubs correspond to rotated 180-degree, unlabeled matching card stub-ins.
- [ ] External package/framework references occupy the right rail and are not implicitly asserted to be dependencies of the target.
- [ ] Sequence diagrams represent ordered, named, evidence-backed scenarios.
- [ ] SVG has meaningful title/description, readable high-contrast text, unique IDs, and a textual fallback. Static and interactive behavior are described honestly.
- [ ] Fixtures test member order/default expansion, nested ownership, primitive conduit identity/color matching, rotated unlabeled stub-ins, and the separation of external references from target dependencies.

This document defines the intended standard; it does not claim every existing renderer or diagram already conforms. Track implementation and conformance tests separately from documentation of the desired behavior.
