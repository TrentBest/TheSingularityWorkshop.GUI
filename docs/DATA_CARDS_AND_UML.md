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
