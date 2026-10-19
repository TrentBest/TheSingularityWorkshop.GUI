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

This is a documentation and traceability contract. It does not mean that all UML families, editable diagrams, or model-to-code generation already exist. Sequence, collaboration/communication, use-case, activity, state-machine, component, and deployment diagrams need their own semantics and fixtures as tracked in [Data Cards and UML](../DATA_CARDS_AND_UML.md). Class pages must not fabricate these capabilities or imply that an illustrative diagram proves runtime behavior.
