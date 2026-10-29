# Per-Type Documentation and UML Standard

This standard applies to **every declared type in the GUI repository**, not only public API types. It is a documentation acceptance criterion for Core, Blazor, SVG, and WPF. XML API comments and generated package XML files remain valuable, but they do not replace a dedicated human-readable type page.


> **Normative view semantics:** [UML Views and Data-Card Standard](UML_VIEWS_AND_DATA_CARD_STANDARD.md) defines the distinct data-card, AST class-diagram, class-neighborhood, and sequence-diagram views. Where older wording here conflicts with that taxonomy, the new standard takes precedence. An AST structure view and a relationship neighborhood are different diagrams and must not be blended.

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

## Workshop target-centered UML layout

This is the canonical Workshop composition, aligned with the shared [FSM_COS Documentation Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/DOCUMENTATION_STANDARD.md) and [Living UML and Data-Card Visual Standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/blob/development/docs/UML_VISUAL_STYLE.md). A Workshop diagram is a recursive, target-centered neighborhood map—not a conventional UML sheet rearranged with different colors.

- **Above — verified users and consumers.** Put hosts, callers, builders, tests, or other direct consumers above the target. Label the actual relationship (constructs, calls, implements, inherits, supplies, or another verified relationship). Do not invent a product or transitive consumer; use “Consumer / host” when the caller is not known.
- **Center — focal target.** Center and visually emphasize the type being explained. The collapsed card begins with declaration kind and name. Preserve its stable identity and the relationship context as the reader expands it.
- **Below — dependencies used by the target.** Place the types/declarations the focal type directly uses below it, grouped by role. Preserve the distinction between declared API types and implementation-only details. Never place a dependency below the target merely because it is nearby in a namespace.
- **Right — package and platform references.** Place verified project, assembly, NuGet, framework, and runtime references to the right. Distinguish direct project/package references from transitive, inferred, optional, or unresolved references. Do not label a framework assembly as a NuGet package.
- **Card edges — type-conduit stubs.** Use short, visibly colored conduit stubs to identify type identities at card boundaries. Both ends for the same semantic type identity use the same stable palette token and visible type label; the receiving stub is rotated 180 degrees. Keep labels and accessible descriptions so color is never the sole carrier of meaning. Stubs express a type/contract relationship, not a promise of runtime conversion or data transfer.
- **Relationships — preserve meaning.** Distinguish inheritance, interface realization, association, composition, and dependency. Use explicit, labeled connectors when needed to disambiguate semantics; never add decorative or unsupported lines. A method calling another type does not by itself prove composition or ownership.
- **No false completeness.** Identify the analysis scope and provenance. A partial scan that does not find a consumer does not prove that no consumer exists. Mark unresolved relationships as unresolved or omit them.

### Recursive card levels

Apply the same card language recursively, while keeping static structure separate from runtime behavior:

1. **Collapsed:** declaration kind and name (for example, `interface IResolver` or `record BundleIdentity`).
2. **Contract:** relevant members, signatures, parameters, return values, property/field types, and meaningful modifiers.
3. **Declaration / AST:** nested declaration structure and source spans only when compiler-backed source analysis provides them.
4. **Implementation:** method bodies or deeper details only when the source model provides them.

When the host supports input, cards must be clickable and keyboard-operable to expand and collapse, with visible focus, accessible names, and a text equivalent. Static SVGs must link to deeper pages/source instead; **never describe a static SVG as interactive**. Reflection is not a complete AST, and source structure is not a runtime trace. Human-authored use cases, source-derived structure, and captured runtime behavior are separate evidence types.

### Stable palette and accessibility

Reuse the Workshop's approved high-contrast palette, but do not invent a primitive-type-to-color mapping. Maintain one versioned, shared mapping for built-in and compound types; the same semantic identity receives the same token across diagrams and renderers. Preserve distinctions such as `byte` versus `int`, `string` versus `char`, `int` versus `long`, and type parameters versus built-in types. Unknown or user-defined types must not silently inherit a primitive color. Keep readable contrast, a meaningful SVG `title` and `desc`, visible labels, and a textual explanation beside every important image.

### Evidence and regeneration

Record the target type, analysis scope, source revision, generation path, analysis method (syntax-only, compiler-backed, reflection-derived, or manual), source links/spans where available, and unresolved/external relationships. Build a normalized renderer-neutral model before rendering SVG. A manual diagram must be labeled manual and source-reviewed; a generated diagram must identify its generator and be reproducible. CI must inventory declarations, validate required pages/SVGs and links, regenerate generated assets, detect stale output, and test that missing pages/diagrams fail. Workflow artifacts alone do not prove that committed documentation is complete or current.

## Recursive card structure and source editing

The ecosystem class diagram is a spatial overview, while the source outline itself is a recursive card hierarchy. Do not flatten namespace/type/member ownership to satisfy the overview layout. The namespace is the outer data card; each contained type is a full card; each member group and member is a nested card; declaration signatures retain their own identifiable card, with atomic values such as access modifiers, types, and names represented as clickable pills where the host supports interaction.

Use the documented declaration ordering and edit contract in [UML-Inspired Data Cards](DATA_CARDS_AND_UML.md#recursive-declaration-cards-and-direct-ast-editing). A visibility/type/name edit is a validated domain command against the authoritative AST/source model, not a direct mutation of SVG or a renderer-owned copy. On success, regenerate the card projection, move the member to its correct group, preserve stable identity where possible, and bring the edited item into view. On failure, preserve the previous authoritative model and surface a diagnostic.

The type chooser must put supported built-in types first, followed by available user-defined and referenced types alphabetically, and include search at the top. Its choices come from the current project/compilation context. GUI Core remains language-neutral; C# parsing, symbol resolution, and AST mutation belong to a separate code-domain adapter/command handler.

These are intended semantics, not a claim that interactive AST editing already exists. Track implementation and fixture-based tests in the roadmap; a static SVG image cannot by itself provide the trusted edit/validation bridge.
