# Implementation Status

This document is the practical boundary between **what the repository contains today** and the larger GUI architecture described by the theory and roadmap.

Status vocabulary:

- **Implemented** — code exists and is exercised by the current verification surface.
- **Implemented / partial** — working code exists, but the contract is intentionally incomplete or not yet covered by dedicated tests.
- **Specified** — documented design exists, but the runtime contract is not established.
- **Planned** — identified future work without a current implementation.
- **External** — owned by another repository or platform layer.

## Package status

| Package | Branch version | Status | Current verification |
|---|---:|---|---|
| `TheSingularityWorkshop.GUI.Core` | `0.1.0-alpha.2` | Implemented | Build, xUnit, coverage |
| `TheSingularityWorkshop.GUI.Blazor` | `0.1.0-alpha.7` | Implemented / partial | Build, Blazor tests |
| `TheSingularityWorkshop.GUI.WPF` | `0.1.0-alpha.1` | Implemented / partial | Windows build and pack; dedicated WPF tests pending |

## GUI.Core

### Implemented

- Recursive `GuiNode` semantic tree.
- Fluent `GuiBuilder`.
- Direct builder-to-builder composition.
- Stable node identity validation.
- Snapshot construction.
- Canonical `GuiKinds`.
- Convenience `GuiBuilders`.
- Semantic `IGuiHub` / `GuiHub`.
- Core unit tests.

### Current canonical builders

- `Panel`
- `Stack`
- `Row`
- `Column`
- `Text`
- `Button`
- `Image`
- `Warning`
- `Separator`
- `TextBox`

### Not yet a stable contract

- Typed semantic properties.
- Semantic input events.
- Accessibility semantics.
- Localization semantics.
- Capability negotiation.
- Deterministic GUI serialization.
- Data/state binding.
- Animation/transition intent.
- A complete media/geometry contract.

## GUI.Blazor

### Implemented

- Core project dependency boundary.
- Blazor renderer for the current canonical semantic vocabulary.
- Hub renderer.
- Basic recursive child manifestation.
- Selected common presentation properties.
- Blazor test project.

### Partial

The renderer is a compatibility implementation for the current semantic vocabulary. It is not yet a complete manifestation of the full expressiveness described by the theory documents.

Not yet established:

- complete semantic input routing;
- capability negotiation;
- accessibility contract;
- full property vocabulary;
- shared adapter conformance suite;
- deterministic artifact reconstruction.

## GUI.WPF

### Implemented

- `net8.0-windows` WPF package project.
- Dependency on GUI.Core only.
- Core semantic renderer.
- Hub renderer.
- Native fluent WPF builder infrastructure.
- Panel/layout builder family.
- Tabbed and multi-panel composition.
- Numeric and enum controls.
- Tree-view control.
- Pivot-grid control.
- Color-picker dialog.
- CRUD dialog builder.
- Reflection-driven property editor.
- Diagnostic/log viewer builder.
- WPF factory and visual helper infrastructure.
- Windows CI build and package artifact lane.

### Partial

The WPF layer deliberately has two APIs:

1. **semantic manifestation** from Core `GuiNode` / `IGuiHub`;
2. **native WPF builders** for applications that need capabilities beyond the neutral semantic vocabulary.

The native builder surface is intentionally platform-specific. Those capabilities are not claims about Core.

### Not yet complete

- Dedicated `tests/WPF.Tests` project.
- Shared Core-to-platform adapter conformance suite.
- Complete automated coverage of native WPF builder behavior.
- Capability negotiation.
- Stable semantic input contract.
- Accessibility contract.
- Deterministic GUI serialization.
- Full desktop lifecycle contract.

## Hub and FSM_COS

### Implemented

The repository contains the GUI-side Hub contract and renderers.

~~~text
RuntimeManifest
      |
      v
   FSM_COS
      |
      v
RuntimeAssembly
      |
      +-- Hub MicroBundle
              |
              v
          GUI.Core
              |
        +-----+-----+
        |           |
      Blazor       WPF
~~~

FSM_COS remains the owner of runtime composition. GUI does not become a composition kernel.

### Not yet a stable GUI contract

- Published GUI MicroBundle contract.
- GUI execution host.
- `Gui.Execute(...)`.
- Runtime GUI configuration contract.
- Cross-host execution lifecycle.
- Capability negotiation between a semantic GUI and a platform adapter.

## Platform status

| Platform | Status |
|---|---|
| Blazor | Implemented / partial |
| WPF | Implemented / partial |
| Unity UI Toolkit | Planned |
| WinUI | Planned |
| Avalonia | Planned |
| .NET MAUI | Planned |

## Testing status

The current quality boundary is intentionally asymmetric:

~~~text
Core
  build + tests + coverage
       |
Blazor
  build + tests
       |
WPF
  Windows build + pack
  dedicated tests pending
~~~

A green WPF build proves that the package compiles and packs on the Windows runner. It does **not** prove complete WPF behavioral conformance.

## Publication status

Documentation and source changes may be committed without publishing packages.

The package workflow is manually gated:

~~~text
build-core   -> publish_nuget_core
build-blazor -> publish_nuget_blazor
build-wpf    -> publish_nuget_wpf
~~~

The WPF package is staged as `0.1.0-alpha.1` for its first NuGet publication.

## How to interpret the roadmap

The repository deliberately documents ideas ahead of implementation.

- Read **this document** for what exists.
- Read **the tests** for executable behavior.
- Read **architecture documents** for current ownership rules.
- Read **the roadmap** for intended next contracts.
- Read **the theory** for the larger architectural argument.

No proposed capability should be treated as a shipped API until code, tests, and package documentation establish it.
