# GUI Gallery

The Singularity Workshop GUI explores a portable semantic description of interface intent, with platform-specific manifestations kept at explicit boundaries. This gallery is an index of demonstrable capabilities in this repository—not a claim that every planned rendering or interaction capability is complete.

## 01 — Semantic model to SVG

**What it demonstrates:** a semantic GUI model can be represented independently of a native widget tree, then manifested as SVG.

- Start with the [repository README](README.md) for the semantic-model boundary and a minimal builder example.
- Inspect the [SVG manifestation example](docs/images/gui-svg-manifestation.svg).
- SVG is an artifact/manifestation path; it should not be described as a full browser interaction host unless an implementation and test prove that behavior.

## 02 — Core builders

**What it demonstrates:** compose a GUI model from named semantic nodes such as columns, text, buttons, panels, and warnings.

```csharp
using TheSingularityWorkshop.Workshop.Gui;

var settings = GuiBuilders.Column("settings")
    .Child(GuiBuilders.Text("title", "Settings"))
    .Child(GuiBuilders.Button("save", "Save"))
    .Child(GuiBuilders.Warning("status", "Changes are not saved yet."))
    .Build();
```

This constructs semantic data. It does not by itself create platform widgets, run an interaction loop, or persist application state.

## 03 — Platform manifestations

The repository separates several concerns:

- **GUI.Core** — semantic GUI model and builders.
- **GUI.Blazor** — browser-hosted manifestation.
- **GUI.WPF** — WPF manifestation and deliberately WPF-specific native builder capabilities.
- **GUI.SVG** — SVG artifacts and visual documentation.

These packages should be evaluated independently for API coverage, tests, supported scenarios, and package readiness. A capability in one renderer must not be assumed to exist in another.

## 04 — Verification lanes

The package workflow defines dedicated restore, test/build, and pack lanes for Core, Blazor, SVG, and WPF. Core, Blazor, and SVG have test projects in that workflow; WPF currently has a Windows build/pack lane, with its test lane explicitly deferred until a WPF test project exists.

See [Package GUI workflow](.github/workflows/package.yml) for the executable quality gates. Workflow success is evidence for the commands it runs, not proof of untested behavior.

## 05 — Architectural boundaries

The current design separates:

- **Meaning** — what a node represents.
- **Composition** — how nodes form a tree.
- **Manifestation** — how a renderer expresses that tree.
- **Hosting and interaction** — runtime behavior that must be explicitly provided by a host.
- **Persistence and serialization** — separate contracts; not implied by the in-memory tree.

Typed properties, portable capability negotiation, and a common cross-renderer interaction contract should be treated as future work unless the corresponding API and tests are present.

## Deeper documentation

- [README — package overview and first example](README.md)
- [SVG manifestation illustration](docs/images/gui-svg-manifestation.svg)
- [Package workflow](.github/workflows/package.yml)

## Gallery maintenance rule

Each gallery entry must point to a real source file, runnable example, test, workflow, or generated artifact. Update this page when implementation changes; clearly label planned capabilities and do not promote architecture direction into a claim of shipped functionality.