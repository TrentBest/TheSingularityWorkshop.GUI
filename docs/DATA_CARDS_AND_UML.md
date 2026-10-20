# UML-Inspired Data Cards

> If a picture is worth a thousand words… then what is a living picture worth?

## Purpose

Standardize how structured definitions are presented as inspectable, editable diagrams. The initial use case is UML-style source-code structure: namespace, types, fields, properties, methods, and their details. The same data-card vocabulary should remain useful for other structured domains rather than hard-coding C# or UML into the renderer.

A **data card** is a fully enclosed, bordered, colored panel with a header and body. Cards nest according to semantic ownership: a namespace card contains its complete represented contents (including class, struct, interface, enum, record, and nested namespace definitions where applicable); each type contains its members; a member may reveal its initializer, implementation, or other detail when expanded. The tree follows the source/domain hierarchy instead of flattening owned content into unrelated sibling cards.

## Ownership boundary

The diagram is not an SVG document and it is not a renderer-specific widget tree.

1. **Authoritative domain definition** — owns the real meaning of the represented content. For source code, that means a code model/AST with namespaces, types, signatures, access modifiers, initializers, and method bodies. It belongs with the code/creation domain, not in GUI Core. A diagram edit must update this model through an explicit command; the diagram must not become a second, conflicting source of truth.
2. **GUI Core diagram semantics** — owns platform-neutral diagram concepts: data-card structure, header/body regions, containment, stable identity, presentation metadata, expansion/visibility intent, and semantic edit actions such as “add type” or “add member.” These are reusable interaction/representation contracts, not C# syntax and not SVG elements.
3. **Renderer adapter** — manifests those semantics as SVG, WPF, Blazor, or another supported surface. SVG is especially useful for inspectable, portable living diagrams, but it is one view of the same diagram semantics.
4. **Editor/command handler** — decides whether a requested edit is permitted, validates it against the authoritative domain model, applies the change, and regenerates the view. A visible plus sign alone never grants mutation authority.

The existing `GuiNode` is a generic recursive GUI snapshot. It can carry a manifestation, but by itself it is not a typed code model or a sufficient long-term contract for structured diagram editing. Add explicit diagram semantics deliberately instead of encoding the whole contract in arbitrary string properties.

## Canonical data-card structure

- **Usings section** appears before namespace/type content; it contains one card per directive and is collapsed by default.
- **Namespace card** is a true content container, not just a heading: its body owns all namespace content represented by the tree. It appears after file-level directives where applicable; nested namespaces remain nested.
- **Type cards** for classes, structs, interfaces, enums, records, and other declarations are children of the namespace that declares them; nested types remain inside their containing type.
- **Member-group cards** group members by visibility in the fixed display order Public, Protected, Private. For example, `Public Properties` contains one child card per public property.
- **Member cards** represent fields, properties, methods, constructors, events, and other declared members; expanding a property reveals accessors and initializer details.
- **Header** identifies the represented item and can carry its kind/access metadata.
- **Body** contains child members or expandable detail.
- **Border and color** visually enclose the complete card. Color communicates a documented semantic category, not decoration alone; meaning must not depend on color only.
- **Stable identity** persists across redraws so selection, expansion, diagnostics, and edits do not attach to the wrong item.

The first implementation should support namespaces and classes without making the model class-only. Interfaces, structs, enums, records, nested types, and other member kinds should fit the same vocabulary when the domain model supplies them.

## Source-code tree: AST-backed cards

For C# source, the authoritative representation should be a real **Abstract Syntax Tree (AST)** or syntax model produced by a C#-aware parser. The AST is the source structure; the data-card tree is its renderer-neutral presentation. Do not make GUI Core parse C#, and do not let SVG/XML or a hand-maintained card hierarchy become a competing source of truth. A code-domain adapter translates AST nodes into data cards and maps validated edit commands back to the source model. Preserve source trivia and formatting where the chosen syntax technology supports it.

The source outline should place `using` directives in their own **Usings** section before namespace/type content. Each directive is an individual data card (for example, `using System.Collections.Generic;`); the section is collapsed by default. Keep directive order and aliases/static/global distinctions from the AST rather than flattening them into display-only strings.

Each type should have explicit member groups in this order: **Public**, **Protected**, then **Private**. Each group is itself a data card (for properties, a card titled `Public Properties`, `Protected Properties`, or `Private Properties`). A group card's body contains a data card for every member in that category. Public member cards are expanded by default to expose the declaration/signature; protected and private group cards are collapsed by default. Apply the same predictable policy to fields, methods, constructors, events, and nested types. Preserve the exact C# accessibility in member metadata, including `internal`, `protected internal`, and `private protected`; document how these less common categories are assigned to display groups rather than pretending they are one of the three exact language modifiers.

Clicking a property card expands its details, including accessors and initializer, such as `{ get; private set; } = ...;`. The declaration and accessor visibility are distinct facts: a public property can legitimately have a private setter. Do not confuse the property's group with its accessor modifiers.

In edit mode, changing a member's access modifier is a domain edit, not merely a card drag. After the authoritative AST/source model accepts the change, the member appears under the matching visibility group, with the same stable semantic identity where possible. Update the view from the changed model; do not maintain independent mutable copies in each group. The edit command must validate legal modifier combinations and report rejected changes without corrupting source.

## Default expansion and visibility

The initial presentation policy is:

| Content | Default state |
| --- | --- |
| Namespace and type hierarchy | Expanded enough to reveal the public structure |
| Usings section and individual using-directive cards | Collapsed by default; preserve source order |
| Public/Protected/Private member-group cards | Public expanded; Protected and Private collapsed |
| Public member cards | Expanded to show declaration/signature; details remain separately expandable |
| Public field/property/method/constructor declaration | Expanded to its declaration/name/signature |
| Public field/property initializer and accessor details | Collapsed until that member card is expanded |
| Public method/constructor implementation body | Collapsed |
| Non-public declarations and their details | Collapsed by default |
| Empty namespace in edit mode | Show an add-type (`+`) affordance |
| End of a type/member collection in edit mode | Show the applicable add-member (`+`) affordance |

“Collapsed” is a presentation state, not deletion, filtering from the semantic model, or an access-control boundary. Non-public content remains available to authorized inspection/editing workflows. Expansion state should be separate from the authoritative code definition so opening a body does not mutate source semantics.

### Collapsed regions and counts

When a section or group is collapsed, its header may show an aggregate such as **`Collapsed (4)`** to communicate how many direct items it contains without expanding them. The count must be derived from the current projected model, not maintained as a separate mutable number. The collapsed region remains a real target in the hierarchy: in edit mode, a user can invoke an explicit add-to-region action from its header without opening it first. That action is available only when the current domain command set permits insertion. In view/inspect mode, the count is informational and no add affordance is shown. A zero-item region should not expose a pointless disclosure control; if edit mode permits insertion, its explicit add action may still be available. The count must not imply that hidden items are deleted or inaccessible.

Public declarations should show enough signature to distinguish overloads and understand the contract; “name only” means implementation/initializer detail is not expanded by default, not that parameter or type information must be discarded. The policy should be configurable by mode and user preference without changing the represented definition.

## Modes and edit affordances

- **View mode:** render cards and expansion controls; do not show add/edit affordances that imply mutation.
- **Inspect mode:** permit navigation, selection, and expansion where supported, without mutating the source definition.
- **Edit mode:** show `+` actions at valid insertion points and expose the appropriate editing controls for the selected element. Text inputs and other editors are manifestations of a semantic edit session, not properties baked into the card itself.
- **Read-only or unsupported adapter:** retain the same diagram meaning while omitting unavailable operations. An adapter must not invent edit behavior it cannot implement.

The command contract should describe intent (for example, add class to namespace, add property to type, edit declaration, edit initializer, edit body, expand/collapse detail). The editor/domain layer validates and applies commands. Renderers only present available actions and return user intent through their interaction boundary.

## SVG and living documentation

SVG should render the diagram as a foundational proof surface: namespace and type cards, colored borders, headers/bodies, nested member rows, expansion states, and edit affordances when an interactive host supports them. A static SVG can faithfully depict structure but does not, by itself, provide a secure source-editing session. Editing requires a host/command bridge and a domain model that can validate the requested change.

Prefer generating the SVG from the same diagram model used by tests and editor views. That makes the diagram a living picture of real definitions rather than a decorative mock-up. Keep SVG element names, XML attributes, and event mechanics inside the SVG adapter.

## Incremental implementation contract

1. Define and test platform-neutral data-card/diagram types in GUI Core: identity, semantic kind, header/body structure, containment, access/visibility metadata, expansion intent, and available semantic actions.
2. Define a C#-aware AST/source-model adapter outside GUI Core. Project using directives, namespaces, types, and visibility-grouped member cards without making the diagram the source of truth.
3. Define a separate source-code definition model in the code/creation domain. Do not add C# AST concepts to GUI Core.
4. Define the initial UML presentation policy for namespaces, classes, public members, non-public members, initializers, and bodies.
5. Manifest a fixture through SVG and at least one interactive adapter; compare the same semantic fixture across adapters.
6. Test view/inspect/edit affordance differences, stable identities, empty namespace insertion, add-member insertion points, and that rendering/expansion never mutates the source definition.
7. Connect editor commands only after the semantic contracts and validation boundary are explicit.

## Acceptance criteria

- The same diagram data produces equivalent structure across supported adapters.
- Namespace is a real containment card for all represented namespace content, including class and struct definitions; nested namespaces/types retain their ownership hierarchy.
- A collapsed region can show a model-derived count such as `Collapsed (4)` and accept an explicit add-to-region command in edit mode without requiring expansion; an empty region has no pointless disclosure control.
- Cards have a complete border, distinct header and body, and documented color semantics.
- The Usings section is collapsed by default and contains one card per source directive.
- Public, Protected, and Private member groups appear in that order; Public is expanded by default and Protected/Private are collapsed.
- Public declarations are expanded to their signature; property accessor/initializer details and method implementation bodies are collapsed until the member is expanded.
- Non-public details are collapsed by default but remain represented and available to authorized workflows; changing visibility through an accepted edit moves the member to its new group without losing identity.
- Plus buttons appear only where the current mode and domain command set permit an add operation.
- No renderer or card directly edits source text without passing through the domain command/validation boundary.
- The SVG living example is generated from a fixture shared with automated tests.


## UML beyond source-code structure

The data-card hierarchy is a useful first manifestation, not the complete UML model. UML diagrams describe different projections of a system and must not be forced into one tree shape. A class/namespace outline answers “what is defined and contained here?”; a sequence diagram answers “which participant sends what message, and in what order?”; a use-case diagram answers “which actors pursue which goals across a system boundary?”

The GUI repository is a natural home for **platform-neutral diagram presentation contracts and their SVG/WPF/Blazor manifestations**. It should not become the owner of every application's domain model, C# parser, or code-generation rules. Keep the boundaries explicit:

- **Diagram semantics** describe participants, lifelines, messages, relationships, actors, goals, constraints, and diagram-specific layout/interaction intent.
- **Domain/source models** remain authoritative for their own facts: a C# syntax model owns source declarations; a workflow model owns workflow behavior; a product/domain model owns its use cases and rules.
- **Projections** turn authoritative domain models into diagrams. A diagram is editable only through commands that validate against the relevant domain model.
- **Generators** transform sufficiently precise, validated models into code, tests, documentation, or executable definitions. Generated output must be traceable to its source model and generator version.

### Diagram families to support

| Diagram family | What it communicates | Useful model facts |
| --- | --- | --- |
| Class / type structure | Types and their relationships | Type identity, members, inheritance, interfaces, associations, multiplicity |
| Sequence | Time-ordered interactions | Participants, lifelines, messages, call/return, guards, loops, alternatives, activation |
| Collaboration / communication | Which objects collaborate and through which links | Participants/objects, links, numbered messages, message ordering |
| Use case | User or external-system goals | Actors, use cases, system boundary, associations, include/extend relationships |
| Activity | Workflow and decisions | Actions, control/object flows, decisions, merges, forks, joins, guards |
| State machine | Lifecycle and reactive behavior | States, events, transitions, guards, effects, initial/final states |
| Component / deployment | Logical modules and runtime placement | Components, provided/required interfaces, dependencies, nodes, deployment links |

The first implementation should not attempt every UML feature at once. Prove the shared architecture with one narrow vertical slice, then add diagram families using the same principles and only family-specific semantics where necessary.

### Recommended vertical slice: model → diagram → validated artifact

1. Define a small platform-neutral diagram contract that can represent nodes/participants, typed relationships/messages, stable identity, constraints, and provenance without embedding SVG elements or C# syntax.
2. Define a sequence-diagram fixture with named participants and ordered messages, including a response and one guarded/conditional interaction.
3. Render that fixture to SVG as a generated, inspectable living picture; test structure and semantic labels from the same fixture.
4. Add view/inspect/edit affordance policy. A visible control expresses an available operation; it does not authorize that operation.
5. Add a validated command path for one safe edit (for example, rename a participant or reorder a message where constraints permit), then regenerate the diagram and assert the authoritative model changed exactly once.
6. Only then prove a code-generation path for a deliberately limited subset, such as generating an interface plus a deterministic test skeleton from a validated class/interaction contract.

### Code generation is a contract, not a promise

A picture alone is underspecified for reliable code generation. A sequence diagram may show message order without defining parameter types, error behavior, concurrency, persistence, cancellation, or transaction boundaries. A use-case diagram captures goals and actors, not the implementation algorithm. The tooling must therefore:

- identify which model fields are required for a particular generator;
- reject or flag incomplete and contradictory models rather than silently guessing;
- preserve stable identities and trace generated members/tests back to model elements;
- report unsupported constructs and generated-code regions clearly;
- keep generation deterministic for the same model and generator version;
- distinguish generated files from user-authored files, and define safe regeneration/merge behavior;
- test the generated artifact by compiling or executing it where practical.

This is how the tooling can make a non-coder more capable without hiding uncertainty behind an attractive diagram. The aim is to make expert reasoning visible, reusable, reviewable, and testable—not to pretend that every design sketch already contains enough information to produce a correct program.

### UML acceptance criteria

- At least class/type, sequence, collaboration/communication, and use-case diagrams have explicit semantic definitions before claiming support.
- The same fixture drives rendering tests and generated SVG output; the committed visual is regenerated and checked in CI rather than hand-maintained.
- Relationships and message ordering are represented as model data, not inferred from SVG element positions alone.
- View/inspect modes do not expose mutation affordances; edit commands validate against the authoritative model.
- A first code generator has a documented supported subset, deterministic output, diagnostics for missing information, and compile/test evidence.
- GUI Core remains platform-neutral and does not acquire C# AST, UML parsing, or application-specific command authority merely because it renders the diagrams.

## Per-type documentation contract

Every documented source type—class, struct, interface, record, enum, and other named type with a meaningful contract—must have a dedicated type-level documentation page. A file-level overview may link to these pages, but it does not replace them when a source file declares multiple types. The page must include an **embedded SVG UML class diagram generated from the authoritative source/model**, not a manually redrawn approximation.

At minimum, each type page should provide:

- **Purpose and responsibility:** what the type owns and what it deliberately does not own.
- **Source identity:** fully qualified name, source file, and relevant generic/type parameters.
- **UML SVG:** actual members and their meaningful visibility, signatures, inheritance/implemented interfaces, and important associations or dependencies where known. Do not fabricate relationships or omit uncertainty silently.
- **Behavior and constraints:** key invariants, lifecycle/threading expectations, failure behavior, and extension points where applicable.
- **Examples and links:** representative use, tests that demonstrate the contract, and related type pages.

Keep the diagram legible and scoped to the type's contract. The SVG should be embedded in the Markdown page and should be reproducible from source/model metadata. When a diagram is derived from a projection rather than directly from the language AST, document the projection boundary. If a relationship cannot be established from available evidence, omit it or mark it unknown instead of guessing.

### Repository-wide adoption and verification

This is a cross-repository documentation convention. GUI provides the reusable diagram semantics, renderers, and eventual generation/validation tooling; each consuming repository remains responsible for its own domain facts and documentation pages. A repository adopting the convention should maintain a source-type-to-document coverage manifest or equivalent deterministic inventory. CI should fail on missing required type pages, missing embedded diagrams, malformed SVG, or generated diagrams that differ from current source/model output. Exceptions (for generated/private/compiler-only types or intentionally grouped declarations) must be explicit and reviewable, not silent gaps.

Do not claim the convention is fully automated until an executable inventory and CI validation prove it. The immediate GUI milestone is a source-grounded, tested SVG class-diagram generator and a small fixture whose documentation image is generated and verified in CI; broad cross-repository enforcement follows after the contract is proven.

## Recursive declaration cards and direct AST editing

The source-code outline is not a conventional UML class box with a flat list of members. It is a recursively nested **data-card document** in which every owned declaration is itself a complete card.

### Containment at every level

- The **namespace** is the outer data card. Its body contains the types and nested namespaces declared within it.
- Each **class, struct, interface, record, enum, or nested type** is a complete card inside its owning namespace/type card.
- Each type card contains ordered member-group cards. Within those groups, every property, method, field, constructor, event, and other supported member is its own card.
- Each member card contains its declaration/signature and, when applicable, further cards for its constituent details: parameters, return type, accessors, initializer, constraints, attributes, or implementation body. The exact child vocabulary is supplied by the source model; do not invent children a declaration does not have.
- A declaration's signature remains visible as a meaningful data card even when its details are collapsed. The view should show the real declaration, not reduce a field to an unlabeled box.

### Required presentation order

Within each type, preserve this stable display order:

1. Public properties
2. Public methods
3. Public fields
4. Other public/static-public declaration groups, with static status visibly distinguished and their ordering defined consistently by the projection
5. Protected declarations
6. Private declarations

Within a group, retain a deterministic order grounded in the source/model (prefer source order unless the projection explicitly defines another order). Preserve the exact language modifiers and signature even when the presentation groups related members together. In particular, do not lose `static`, `const`, `readonly`, generic parameters, overload distinctions, accessor modifiers, or nullable annotations. If the projection separates static public members into a dedicated group, it must document the exact sub-order and apply it consistently.

### Atomic signature pills are still cards

A small signature element can use a **pill-shaped data card** when it contains one atomic value, but it remains an individually identifiable, selectable semantic element. For example, a field declaration such as `public float f1;` contains distinct elements:

- an access-modifier pill/card: `public`;
- a type pill/card: `float`;
- an editable name element/card: `f1`;
- the declaration/signature card that preserves their combined meaning: `public float f1;`.

Clicking the modifier pill opens the supported modifier choices. Choosing `private` requests a semantic edit against the authoritative AST/source model; only after the command validates and succeeds does the projection move the field into the private group. The refreshed view should preserve the field's stable identity and, if a group is too long to fit in the viewport, scroll it into view and center it as closely as the viewport permits. Rejected edits must leave the authoritative model and displayed grouping unchanged and provide a visible diagnostic.

Clicking the type pill opens a searchable type chooser. The chooser puts supported language/runtime built-ins first in a deliberate canonical order, followed by user-defined and referenced types in alphabetical order, with namespace qualification available to disambiguate duplicate names. A search input belongs at the top. The list must be derived from the current compilation/project context, not a hard-coded list that pretends every possible type is available. Unknown or unresolved types must remain representable and diagnosable.

The name element uses a text input or equivalent editing control during an edit session. It submits a rename command, observes language rules and scope collisions, and updates the authoritative AST/source model only after validation. The card body then presents the declaration's relevant detail: for a field this can include initializer and attributes; for a property, accessors and initializer; for a method, parameters, return type, constraints, attributes, and body. Keep the signature visible independently from expanded implementation detail.

### Edit transaction and projection rules

1. A click selects a semantic element by stable identity and opens the applicable editor/chooser.
2. The UI submits an explicit typed command such as change accessibility, change type, rename symbol, or edit initializer.
3. The code-domain handler validates the request against the current AST/project context, including syntax legality, name collisions, and supported type resolution.
4. On success, the authoritative model changes and the card projection is regenerated from that model. The GUI must not independently patch a second mutable tree.
5. The projection updates member ordering/grouping, retains identity where the language model allows it, and scrolls the edited item into view. On failure, retain the previous model/view and show the diagnostic.
6. Tests must assert both the model change and the resulting projection/viewport intent; a visually moved card alone is not proof that the source model changed.

GUI Core owns generic card hierarchy, identity, presentation, and interaction intent. It does **not** own C# parsing, symbol resolution, compilation context, or AST mutation authority. SVG can visualize this model and may express clickable affordances when hosted by an interactive application, but a standalone SVG image is not itself a trusted code editor. The host interaction bridge and validated code-domain command handler are required for edits.


## Interim per-file UML coverage contract

Until repository-wide source-derived coverage is fully enforced, retain this minimum coverage rule:

- Every implementation file declaring a class, interface, struct, record, or enum must be accounted for by a source-accurate UML diagram. If one page covers a file with multiple declarations, include every declared type, including nested, helper, and internal types; alternatively, maintain one page per type and link related pages.
- Where applicable, diagrams identify fully qualified name and type kind, inheritance and implemented interfaces, meaningful fields/properties/constructors/methods with parameter and return types, and important source-supported relationships. Never invent a relationship when source evidence is absent.
- Files that declare no types must still appear in the coverage inventory. Mark them explicitly as “class diagram not applicable” with a brief rationale, rather than silently omitting them.
- Maintain a deterministic source-file/type-to-document-and-diagram coverage manifest. CI must fail when required pages or embedded diagrams are missing, diagrams are malformed, or generated diagrams are stale relative to the source/model.
- Keep the current interim Mermaid/source-outline verification as a bridge only where the source-derived SVG generator does not yet cover a case. Do not describe that interim check as equivalent to generated, source-accurate UML.
- Exceptions for generated, compiler-only, or intentionally grouped declarations must be explicit and reviewable.

This is a coverage gate, not a claim that all of it is already automated. The source-derived generator and its CI must demonstrate each supported case before the repository claims full automation.


## File-level coverage and interim Mermaid verification

For source files that declare types, the repository-level coverage contract is file-complete as well as type-complete:

- Every implementation file declaring a class, interface, struct, record, or enum must map to a source-accurate UML class diagram. A file with multiple declarations must cover every type, including helper, nested, and internal types. A per-type page is acceptable when it links related declarations and every type page contains its own diagram.
- Where applicable, show fully qualified name and type kind; inheritance and implemented interfaces; meaningful fields, properties, constructors, and methods with parameter/return types; visibility and static/abstract/virtual distinctions when contract-relevant; and source-supported associations, containment, dependencies, multiplicities, and cross-file links.
- A simplified overview may omit private implementation detail for readability only when it labels itself as simplified and the detailed type page still makes the omitted contract inspectable.
- Until source-derived SVG generation covers a case, **Mermaid `classDiagram` is the portable interim source representation**. Review it against source and tests, and check that referenced type/member names exist. This is a bridge, not evidence that automated SVG generation is complete.
- Update a diagram in the same change as a relevant source-contract change. The long-term generated diagram must come from the same syntax/AST-backed model used by the data-card projection, not become a second manually maintained source of truth.
- Files without declared types must be included in the inventory and document their actual unit/dependencies using an appropriate diagram, or state “class diagram not applicable” with a short rationale. Never use a misleading empty UML diagram.
- Generated code and test fixtures follow their repository-specific documentation policy, but must not be silently counted as complete coverage.

The source-to-page/diagram manifest and CI checks must make these cases observable and fail on missing coverage or stale generated output. Exceptions must be explicit and reviewable.
