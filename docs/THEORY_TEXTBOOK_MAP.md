# Theory Textbook Map

This document defines how the theory track can be inserted into a textbook with minimal editorial rework.

The goal is not to publish a product manual disguised as a textbook.

The goal is to provide a coherent set of engineering concepts that can stand independently of the repository.

## Proposed chapter sequence

### Chapter 1 — The Interface Is Not the Experience

**Core question:** What happens when an interface is treated as part of the identity of the system it represents?

Introduces:

- experience identity
- extrinsic interface layer
- observation
- interaction boundary
- ephemeral manifestation

Primary source:

[Extrinsic Interface Layer](THEORY/01_EXTRINSIC_INTERFACE_LAYER.md)

---

### Chapter 2 — Building the Breakwater

**Core question:** Where should platform volatility be absorbed?

Introduces:

- execution boundaries
- adapter boundaries
- environmental volatility
- semantic contracts
- the breakwater principle

Primary source:

[Breakwater Principle](THEORY/02_BREAKWATER_PRINCIPLE.md)

---

### Chapter 3 — The Digital Shadow

**Core question:** Can a system be represented without reproducing its implementation?

Introduces:

- semantic representation
- observable surface
- representational equivalence
- multiple manifestations
- implementation independence

Primary source:

[Digital Shadow](THEORY/03_DIGITAL_SHADOW.md)

---

### Chapter 4 — From Renderer to Execution Boundary

**Core question:** What changes when the interface layer owns the execution context rather than merely drawing the result?

Introduces:

- execution host
- runnable experience
- execution context
- capability resolution
- manifestation lifecycle
- `Gui.Execute(...)`

Primary source:

[Execution Boundary](THEORY/04_EXECUTION_BOUNDARY.md)

---

### Chapter 5 — Compatibility Without Implementation Equivalence

**Core question:** What must two systems actually share to interoperate?

Introduces:

- semantic compatibility
- representational compatibility
- adapters
- platform-specific mechanisms
- interoperability micro-bundles

Primary source:

[Representational Compatibility](THEORY/05_REPRESENTATIONAL_COMPATIBILITY.md)

---

### Chapter 6 — Spatial Representation and Controlled Observation

**Core question:** How can one interface describe many views of changing digital reality while keeping observation separate from presentation?

Introduces:

- the 2D presentation domain
- subjects, views, viewports, and presentation surfaces
- coordinate spaces and transforms
- camera as controlled observation
- projection from 3D into 2D
- multi-view composition
- spatial interaction and hit testing
- selection and synchronization across views

Primary source:

[Spatial Representation](THEORY/06_SPATIAL_REPRESENTATION.md)

Engineering companion:

[GUI Spatial Contracts](GUI_SPATIAL_CONTRACTS.md)

---

## Suggested textbook treatment

Each chapter should follow the same editorial pattern:

1. **Problem** — establish a concrete engineering problem.
2. **Observation** — identify the recurring failure mode.
3. **Principle** — state the architectural idea.
4. **Model** — introduce the semantic model.
5. **Consequence** — derive what the model requires.
6. **Counterargument** — present the strongest skeptical objection.
7. **Test** — identify an implementation or thought experiment that could falsify the claim.
8. **Case study** — connect the concept to TheSingularityWorkshop.GUI.
9. **Boundary** — state what the principle does not solve.
10. **Further reading** — point to implementation documentation.

This structure is deliberate.

It allows the theory to remain useful even if the reader never adopts this repository.

## Authoring rule

The repository should never require a textbook author to reconstruct the argument from source code.

The code is the case study.

The theory documents the argument.

The engineering documentation documents the implementation.

Those are three related but distinct artifacts.
