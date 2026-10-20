# Per-Type Documentation and UML Standard

This standard applies to **every declared type in the GUI repository**, not only public API types. It is a documentation acceptance criterion for Core, Blazor, SVG, and WPF. XML API comments and generated package XML files remain valuable, but they do not replace a dedicated human-readable type page.

## Required coverage

- Every class, interface, struct, record, and enum declared in production source has its own Markdown page. Include nested and internal types when they are part of the maintained implementation; explicitly justify any generated, trivial, or intentionally undocumented type in the audit inventory rather than silently omitting it.
- A source file containing multiple types maps to multiple type pages. A type page may link to related types, but it must not stand in for several distinct types.
- Each page embeds that type's UML class diagram as an SVG. The SVG must describe the real type and verified relationships; it is not a decorative placeholder.
- A central index maps source file and fully qualified type name to the documentation page and diagram asset. The inventory is the source of truth for measuring coverage.

## Location and naming

Use this layout unless an existing repository convention provides a stronger, consistent alternative:

- Pages: `docs/api/<Area>/<FullyQualifiedTypeName>.md`
- Diagrams: `docs/assets/uml/classes/<FullyQualifiedTypeName>.svg`
- Index and coverage inventory: `docs/api/README.md` and `docs/api/TYPE_INVENTORY.md`

Use a filesystem-safe encoding for namespace separators and nested-type separators, and document it in the index. Never create duplicate pages merely because two types have the same short name.

## Required contents of every type page

1. **Identity and source** — fully qualified name, kind, assembly/package, and a link to the defining source file.
2. **Purpose and responsibility** — what the type owns, what it deliberately does not own, and its architectural boundary.
3. **Embedded UML SVG** — relative Markdown image reference plus a text description or equivalent relationship summary for readers who cannot view the image.
4. **Relationships** — base type, implemented interfaces, important associations/composition, collaborators, and dependencies that are confirmed by source. Distinguish direct code relationships from conceptual/optional relationships.
5. **Public and relevant non-public surface** — constructors, properties, fields, events, methods, and significant state/invariants. Explain behavior and constraints, not just repeat signatures.
6. **Behavior and failure modes** — lifecycle, validation, exceptions/failure results, threading or mutability constraints where relevant.
7. **Usage and verification** — a concise source-grounded example when useful, and links to representative tests. Do not invent APIs or claim tests cover behavior they do not assert.
8. **Related types and diagrams** — links to referenced type pages and larger interaction/sequence diagrams when these materially explain behavior.

A page should remain useful without reading the source, but source and tests remain authoritative. Keep the page proportional to the type's complexity; do not pad simple types with boilerplate.

## UML diagram integrity

- Use UML class-diagram semantics for type structure: name, kind, inheritance, interface realization, association/dependency, and key members where useful.
- Draw only relationships supported by source or an explicitly identified contract. Mark optional/runtime-discovered relationships as such; do not imply composition merely because one type calls another.
- Generate diagrams from a repeatable model/source-analysis path where practical. If a diagram is initially authored manually, record the source facts and verification step, and make it a candidate for generation rather than claiming automatic synchronization.
- SVG must be well-formed, have a meaningful `title` and `desc`, readable text, and no dependence on color alone. Include a textual fallback on the Markdown page.
- Do not hand-edit generated SVG output as the source of truth. Update the model/template and regenerate the asset.

## CI acceptance criteria

CI should fail when any production type in the inventory lacks its page or SVG, a page points to a missing diagram/source path, or the diagram lacks its required accessible title/description. The checker must compare the inventory with actual declarations so newly added types cannot quietly escape documentation. If complete source parsing is not yet available, begin with a generated, reviewable inventory and make parser-backed drift detection an explicit follow-up; do not represent a manually maintained list as automated proof.

Before declaring the requirement complete, retain evidence for:
- inventory coverage across Core, Blazor, SVG, and WPF;
- one page and one embedded UML SVG per declared type;
- link and SVG accessibility validation;
- CI enforcement against a deliberately missing page/diagram;
- a source-to-diagram review of inheritance, interfaces, and dependencies;
- the exact commit and workflow run that passed.

## Scope boundary

This is a documentation and traceability contract. It does not mean that all UML families, editable diagrams, or model-to-code generation already exist. Sequence, collaboration/communication, use-case, activity, state-machine, component, and deployment diagrams need their own semantics and fixtures as tracked in [Data Cards and UML](DATA_CARDS_AND_UML.md). Class pages must not fabricate these capabilities or imply that an illustrative diagram proves runtime behavior.

## Ecosystem class-diagram layout

A class diagram is an **ecosystem map centered on the type of focus**, not merely a class box with lines to every neighbor. Use the same spatial grammar consistently in every type page and in the larger repository diagrams:

- **Center — focus type.** The focal UML card is the visual anchor and receives the strongest border weight and brilliant semantic accent. Keep the actual class/interface/struct/enum identity, key members, and kind readable inside the card.
- **Left edge — primitive input conduits.** Show a small, open-ended conduit/port for each primitive data type the focal type consumes. Ports are not connected by drawn wires to the class: their position and matching color communicate the contract at a glance. The primitive-type color is a fixed, repository-wide token, not a per-diagram choice. Include only types actually consumed by the focal type's declared API or verified implementation; distinguish public API inputs from private implementation details if both are shown.
- **Right edge of the focal card — dependency ports.** Show named dependencies that the type directly uses, with a right-side port on the class card aligned to each dependency category/target. Do not draw a tangle of crossing lines through the center. Distinguish type dependencies from package/assembly provenance.
- **Right outer edge — package/repository conduits.** Show NuGet packages, framework assemblies, project/repository dependencies, or other external packages as named continuation endpoints. Connect a focal type's right-side dependency port to the corresponding external endpoint only when that relationship is verified. Do not label a framework type as a NuGet package unless it actually comes from one; identify framework/runtime dependencies accurately.
- **Above — direct users.** Place types that directly reference, construct, call, or otherwise consume the focal type above it. The diagram's direction is spatial: users above depend on the focus below them. Include only verified direct users; do not infer every transitive consumer.
- **Below — dependencies used by the focus.** Place the types and components the focal type itself uses below it, grouped by role where useful. This makes direction clear: focus uses items below; items above use focus. External package identity remains on the right outer edge.
- **No arbitrary relationship lines.** The edge ports and consistent placement carry the visual grammar. Use a line only when a relationship cannot be unambiguously understood from alignment/port pairing or when standard UML semantics specifically require it. Never add decorative lines that suggest unverified relationships.

### Primitive color tokens

Reuse the established brilliant accent colors already recorded for the Workshop moniker: **green** `#52e05a`, **blue** `#00a8ff`, **magenta** `#ff2cff`, **red** `#ff3030`, **orange** `#ff7a00`, and **yellow** `#ffd34d`. These are the known shared accent palette, not yet a complete primitive-type mapping. Do not guess which primitive maps to which color; define and review that mapping centrally before claiming primitive color semantics are standardized. Keep the futuristic high-contrast treatment and brilliant thick borders; avoid defaulting to gray panels or muted outlines as the dominant visual language.

Maintain one authoritative, versioned primitive-color legend shared by every diagram renderer. A primitive has the same color everywhere; a diagram must not locally recolor it to suit its composition. The legend must cover at least the primitive types actually supported by the source/API, including `bool`, `byte`, `char`, `decimal`, `double`, `float`, `int`, `long`, `object`, `short`, `string`, `uint`, `ulong`, and `ushort`, plus any additional language/runtime primitives the repository chooses to support. Unknown or user-defined types must never be silently assigned a primitive color.

Use brilliant, high-contrast borders with a consistent thick stroke for focal cards and distinct, repeatable border treatments for conduit ports. Preserve readability in light and dark contexts; include a text/type label or accessible description so meaning never depends on color alone. Keep fill and border roles distinct. The exact color values must come from the repository's recovered/approved GUI.SVG palette and must be centralized as named tokens; do not make up a new palette independently inside individual SVG files.

### Relationship truth and direction

For each diagram, document the evidence used to populate each region: source declarations/usages, project references, package metadata, or an explicit architecture contract. Direct users above and direct dependencies below are different sets; do not swap them. Use a separate label or treatment for inherited members, interface realization, package provenance, and optional/runtime relationships where relevant. If a relationship cannot be proven, omit it or label it explicitly as conceptual/unverified.

This ecosystem layout supplements rather than replaces the class card's UML content. Every per-type page still needs a textual explanation, accessible SVG title/description, and a link to source and relevant tests. A diagram is not considered automatically generated or drift-free until a repeatable source/model path and CI checks establish that claim.

## Recursive card structure and source editing

The ecosystem class diagram is a spatial overview, while the source outline itself is a recursive card hierarchy. Do not flatten namespace/type/member ownership to satisfy the overview layout. The namespace is the outer data card; each contained type is a full card; each member group and member is a nested card; declaration signatures retain their own identifiable card, with atomic values such as access modifiers, types, and names represented as clickable pills where the host supports interaction.

Use the documented declaration ordering and edit contract in [UML-Inspired Data Cards](DATA_CARDS_AND_UML.md#recursive-declaration-cards-and-direct-ast-editing). A visibility/type/name edit is a validated domain command against the authoritative AST/source model, not a direct mutation of SVG or a renderer-owned copy. On success, regenerate the card projection, move the member to its correct group, preserve stable identity where possible, and bring the edited item into view. On failure, preserve the previous authoritative model and surface a diagnostic.

The type chooser must put supported built-in types first, followed by available user-defined and referenced types alphabetically, and include search at the top. Its choices come from the current project/compilation context. GUI Core remains language-neutral; C# parsing, symbol resolution, and AST mutation belong to a separate code-domain adapter/command handler.

These are intended semantics, not a claim that interactive AST editing already exists. Track implementation and fixture-based tests in the roadmap; a static SVG image cannot by itself provide the trusted edit/validation bridge.
