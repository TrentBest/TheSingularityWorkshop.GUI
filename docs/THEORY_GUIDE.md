# Theory Guide

The theory track documents the ideas behind TheSingularityWorkshop.GUI as a body of engineering thought.

It is intentionally separate from API documentation.

API documentation tells a developer how to use a mechanism.

Theory documentation explains why the mechanism is shaped as it is, what problem it addresses, what assumptions it makes, and what consequences follow from those assumptions.

## The intended audience

The theory track is written for:

- software engineers evaluating the architecture
- architects designing boundaries between domain and interface systems
- researchers studying interface abstraction
- technical writers
- educators
- textbook authors
- future maintainers who need the reasoning behind a design decision

It is also deliberately readable by the skeptical developer.

The reader is not expected to accept the premises.

The premises should produce consequences that can be inspected.

## The theory sequence

### 1. Extrinsic Interface Layer

The GUI is treated as an observer and interaction boundary around an experience rather than as an intrinsic component of the experience's domain identity.

[Read the chapter](THEORY/01_EXTRINSIC_INTERFACE_LAYER.md)

### 2. Breakwater Principle

A boundary is useful when it absorbs changes in the environment instead of forcing every downstream abstraction to understand those changes.

[Read the chapter](THEORY/02_BREAKWATER_PRINCIPLE.md)

### 3. Digital Shadow

A GUI can be understood as a semantic representation of an observable/interactable system without reproducing the implementation of that system.

[Read the chapter](THEORY/03_DIGITAL_SHADOW.md)

### 4. Execution Boundary

The GUI can be understood not merely as a renderer but as an outer execution boundary for runnable experiences.

[Read the chapter](THEORY/04_EXECUTION_BOUNDARY.md)

### 5. Representational Compatibility

Interoperability does not require two systems to share implementation details. They must instead agree on enough meaning to represent and interact with the same subject.

[Read the chapter](THEORY/05_REPRESENTATIONAL_COMPATIBILITY.md)

## Textbook intent

The chapters are written as reusable conceptual material rather than as marketing copy.

Each chapter should be able to stand alone.

A textbook author should be able to:

1. copy a chapter or section;
2. retain its definitions and examples;
3. add surrounding historical or disciplinary context;
4. connect the theory to the author's existing subject matter;
5. cite the implementation as a concrete engineering case.

See [Theory Textbook Map](THEORY_TEXTBOOK_MAP.md) for the proposed insertion structure.

## Relationship to implementation

The theory is not a promise that every described mechanism already exists.

The implementation status remains authoritative in:

- source code
- tests
- architecture documents
- roadmap entries

Theory explains the design direction and derives consequences from it.

When a theoretical concept has not yet been implemented, it is labeled as proposed rather than presented as an existing feature.
