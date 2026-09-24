# Start Here: Explore the GUI

If you arrived here thinking:

> "I already know how to build a GUI. What would another abstraction add?"

That is a useful question.

This repository is designed to answer it gradually. Rather than asking you to adopt a new vocabulary all at once, it starts with familiar GUI problems and works outward toward the abstraction.

The fastest way to explore it is **not** to read every document. Follow the two documentation prongs below, move between the implementation and the ideas behind it, and let the examples do most of the persuasion.

---

## The two-prong documentation model

This project documents two different things because they answer two different questions.

### Prong 1 — The technology

**Question:** *How does it work?*

This is the engineering documentation:

- the semantic GUI model
- recursive builders
- platform adapters
- lifecycle
- testing
- execution-boundary design
- repository structure
- implementation constraints
- roadmap and known unfinished contracts

Start with:

- [GUI Model](GUI_MODEL.md)
- [Architecture](ARCHITECTURE.md)
- [Platform Adapters](PLATFORM_ADAPTERS.md)
- [Testing](TESTING.md)
- [GUI Execution Model](GUI_EXECUTION_MODEL.md)
- [Roadmap](../ROADMAP.md)

This prong should let another developer inspect the implementation and answer:

> "What does this thing actually do?"

---

### Prong 2 — The theory

**Question:** *Why is it designed this way, and what larger engineering problem is it trying to solve?*

The theory track treats the project as an engineering thesis rather than a widget library.

It develops concepts such as:

- the GUI as an **extrinsic interface layer**
- the **breakwater principle**
- the **digital shadow**
- **execution-boundary inversion**
- **representational compatibility**
- the separation of semantic intent from platform manifestation

Start with:

- [Theory Guide](THEORY_GUIDE.md)
- [Textbook Map](THEORY_TEXTBOOK_MAP.md)
- [Extrinsic Interface Layer](THEORY/01_EXTRINSIC_INTERFACE_LAYER.md)
- [Breakwater Principle](THEORY/02_BREAKWATER_PRINCIPLE.md)
- [Digital Shadow](THEORY/03_DIGITAL_SHADOW.md)
- [Execution Boundary](THEORY/04_EXECUTION_BOUNDARY.md)
- [Representational Compatibility](THEORY/05_REPRESENTATIONAL_COMPATIBILITY.md)

The theory track is deliberately written so that a textbook author can lift a chapter, section, definition, or example with minimal rewriting.

---

# A practical evaluation path

You do not need to agree with the architecture.

Instead, ask five questions.

## 1. Can the semantic model remain platform-neutral?

Open the Core project.

Look for references to:

- HTML
- DOM
- CSS
- JavaScript
- Blazor rendering types
- WPF controls
- XAML
- Unity UI Toolkit

The claim is not that platforms are irrelevant.

The claim is that **platform mechanisms belong at the boundary rather than in the semantic model**.

If Core cannot maintain that boundary, the architecture has failed its own test.

---

## 2. Can the same intent have different manifestations?

The model describes GUI intent before a platform turns it into a concrete interface.

Conceptually:

```
intent
  -> semantic GUI model
      -> platform adapter
          -> manifestation
```

A button is therefore not fundamentally a WPF Button, an HTML element, or a Unity control.

Those are manifestations.

The semantic object is the thing that means:

> "This is an actionable interaction."

The adapter decides how that meaning appears on a particular platform.

---

## 3. Does the abstraction earn its place?

This is the important skeptical test.

If a supposedly neutral abstraction forces developers to write the lowest common denominator of every platform, it has failed.

Neutrality therefore means:

> A concept belongs in Core only when its meaning can be defined independently of a particular platform.

Platform-specific power is not erased. It is moved to an appropriate boundary.

---

## 4. Can the model describe more than conventional widgets?

Try to leave the world of buttons.

Consider:

- a floor plan
- a map
- a state inspector
- a CAD view
- a 3D scene
- a diagnostic visualization
- an image
- a mesh
- animation
- spatial interaction
- accessibility interfaces
- deliberately unstructured visual output

If the architecture only knows how to draw panels and buttons, it is a widget abstraction.

The intended model is larger:

> GUI is a semantic representation and interaction space.

That is a much stronger claim—and therefore a much more useful thing to test.

See [GUI Expressiveness](GUI_EXPRESSIVENESS.md) and [GUI Coverage Survey](GUI_COVERAGE_SURVEY.md).

---

## 5. Does the theory clarify useful engineering boundaries?

The theory is not decoration.

It makes architectural predictions.

For example:

- If GUI is extrinsic, experience identity should not depend on a particular GUI.
- If GUI is a boundary, input translation should occur at that boundary.
- If manifestation is separate from semantics, multiple manifestations can observe the same experience.
- If representation is fundamental, interoperability should not require implementation equivalence.
- If execution belongs at the boundary, `Gui.Execute(...)` can become a host contract rather than a rendering convenience.

Those predictions are useful precisely because they can be challenged.

---

# A ten-minute evaluation

If you have ten minutes, do this:

1. Read [GUI Model](GUI_MODEL.md).
2. Read [Platform Adapters](PLATFORM_ADAPTERS.md).
3. Inspect `src/Core`.
4. Inspect `src/Blazor` and `src/WPF`.
5. Read the Core tests.
6. Read [Extrinsic Interface Layer](THEORY/01_EXTRINSIC_INTERFACE_LAYER.md).
7. Find one place where the architecture appears unnecessarily complicated.
8. Decide whether that complexity protects a real boundary or merely moves complexity around.

That last step matters.

The goal is not to turn a skeptic into a believer. It is to make the tradeoffs clear enough that a developer can decide whether the abstraction removes friction from problems they already have.

---

# What this repository is not claiming yet

The architecture contains ideas that are ahead of the current implementation.

In particular, the execution-host model, capability negotiation, and some of the richer semantic contracts are still being designed.

The roadmap is therefore part of the documentation, not an afterthought.

See [ROADMAP.md](../ROADMAP.md).

The distinction is intentional:

- **Implemented:** inspect the code and tests.
- **Specified:** read the architecture documents.
- **Proposed:** read the theory and roadmap.
- **Unresolved:** find the open boundary in the design and challenge it.

That separation keeps the documentation honest.

---

# The ultimate test

Do not ask:

> "Do I believe in this GUI abstraction?"

Ask:

> "Does this abstraction give me a cleaner place to put a problem that already exists?"

If the answer is no, do not use it.

If the answer becomes yes only after seeing the architecture, implementation, and theory together, that is exactly why the two-prong documentation model exists.
