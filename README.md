# TheSingularityWorkshop.GUI

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.GUI/package.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions)
[![Last commit](https://img.shields.io/github/last-commit/TrentBest/TheSingularityWorkshop.GUI/master)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commits/master)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.GUI?style=flat-square)](https://app.codecov.io/gh/TrentBest/TheSingularityWorkshop.GUI)

A platform-neutral GUI engineering layer with recursive semantic builders and native platform manifestations.

The repository separates **what an interface means** from **how a particular platform displays and operates it**.

~~~text
experience / application
          |
          v
     semantic GUI
          |
     +----+----+
     |         |
   Blazor     WPF
     |         |
 browser     desktop
 manifestation / platform behavior
~~~

## Start here

If you are new to the repository, read **[docs/START_HERE.md](docs/START_HERE.md)** first.

The documentation intentionally has two tracks:

- **Technology:** what is implemented, how it works, how it is tested, and where the boundaries are.
- **Theory:** why the boundaries exist and where the architecture is intended to go.

For a concise implementation snapshot, see **[docs/IMPLEMENTATION_STATUS.md](docs/IMPLEMENTATION_STATUS.md)**.

## Repository structure

~~~text
TheSingularityWorkshop.GUI/
|
+-- src/
|   +-- Core/       platform-neutral semantic model and builders
|   +-- Blazor/     browser manifestation
|   +-- WPF/        native desktop manifestation and WPF builders
|
+-- tests/
|   +-- Core.Tests/    Core contract tests
|   +-- Blazor.Tests/  Blazor manifestation tests
|   +-- WPF.Tests/     planned/being established
|
+-- docs/
|   +-- START_HERE.md
|   +-- IMPLEMENTATION_STATUS.md
|   +-- ARCHITECTURE.md
|   +-- PLATFORM_ADAPTERS.md
|   +-- WPF_GUIDE.md
|   +-- GUI_MODEL.md
|   +-- TESTING.md
|   +-- theory and design documents
|
+-- .github/workflows/
    +-- package.yml
~~~

## Core: author semantic intent once

GUI.Core is the platform-neutral package. Its current default vocabulary includes:

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

A simple composition looks like this:

```csharp
var view = GuiBuilders.Column("settings")
    .Child(GuiBuilders.Text("title", "Settings"))
    .Child(GuiBuilders.Button("save", "Save"))
    .Child(GuiBuilders.Warning("warning", "Changes are not saved yet."))
    .Build();
```

The result is a `GuiNode` tree. Core does **not** create WPF controls, Blazor components, HTML, XAML, JavaScript, or browser state.

That makes Core the place to author reusable GUI meaning.

## The Hub boundary

GUI.Core also defines the semantic Hub surface used by an assembled runtime.

~~~text
RuntimeManifest
      |
      v
   FSM_COS
      |
      v
RuntimeAssembly
      |
      +---- Hub MicroBundle
                |
                v
          GUI.Core / IGuiHub
                |
          +-----+-----+
          |           |
       Blazor        WPF
          |           |
       browser      desktop
~~~

FSM_COS owns composition. GUI owns the semantic presentation boundary. Platform projects own manifestation.

The GUI repository does **not** make FSM_COS depend on a rendering technology.

## WPF

GUI.WPF is a real implementation, not a placeholder for a future adapter.

It currently provides:

- a Core-to-WPF semantic renderer;
- an `IGuiHub` WPF renderer;
- reusable WPF panel/layout builders;
- a fluent native WPF window/builder surface;
- tabbed and multi-panel composition;
- numeric, enum, tree-view, and pivot-grid controls;
- color-picker and CRUD dialog builders;
- reflection-driven editors;
- diagnostic/log viewer infrastructure;
- a WPF utility/factory layer;
- WPF visual helpers.

The WPF package targets **.NET 8 for Windows** and depends on GUI.Core.

The current WPF release does **not** claim adapter parity with every Core semantic capability, and its dedicated `tests/WPF.Tests` project is not yet part of the repository. See [Implementation Status](docs/IMPLEMENTATION_STATUS.md) and [WPF Guide](docs/WPF_GUIDE.md).

## Blazor

GUI.Blazor provides the browser manifestation of the same semantic Core model.

The current renderer supports the repository's canonical node kinds and translates supported semantic properties into Blazor/HTML presentation.

Blazor is an adapter, not the semantic model.

## Package family

| Package | Current branch version | Role | Status |
|---|---:|---|---|
| **TheSingularityWorkshop.GUI.Core** | `0.1.0-alpha.2` | Semantic model and builders | Implemented |
| **TheSingularityWorkshop.GUI.Blazor** | `0.1.0-alpha.7` | Blazor manifestation | Implemented |
| **TheSingularityWorkshop.GUI.WPF** | `0.1.0-alpha.1` | WPF manifestation and native builders | Implemented; first release pending |

These packages are intentionally independently versioned.

The GUI family sits beside:

| Package | Role |
|---|---|
| **TheSingularityWorkshop.FSM_API** | State and transition runtime |
| **TheSingularityWorkshop.FSM_COS** | Runtime composition and assembly |
| **TheSingularityWorkshop.FSM_Serialization** | Binary representation boundary |
| **TheSingularityWorkshop.MicroBundleDomain** | MicroBundle domain descriptors |

## What is deliberately not implemented yet

The architecture contains capabilities that are documented but not yet stable runtime contracts.

Examples include:

- typed semantic property contracts;
- semantic input/interaction contracts;
- capability negotiation;
- accessibility contracts;
- localization contracts;
- deterministic GUI serialization;
- shared cross-platform adapter conformance tests;
- GUI execution-host contracts;
- a stable GUI MicroBundle contract;
- `Gui.Execute(...)`;
- Unity UI Toolkit manifestation;
- a dedicated WPF test project.

These are not being represented as completed features. The roadmap and status documents distinguish implemented code from specified or proposed architecture.

## Documentation map

| Document | Purpose |
|---|---|
| [Start Here](docs/START_HERE.md) | Guided entry point |
| [Implementation Status](docs/IMPLEMENTATION_STATUS.md) | What exists, what is partial, what is not implemented |
| [Architecture](docs/ARCHITECTURE.md) | Ownership and dependency boundaries |
| [Platform Adapters](docs/PLATFORM_ADAPTERS.md) | Adapter responsibilities and conformance |
| [WPF Guide](docs/WPF_GUIDE.md) | Current WPF builder and renderer surface |
| [GUI Model](docs/GUI_MODEL.md) | Current semantic tree contract |
| [Testing](docs/TESTING.md) | Verification strategy |
| [Roadmap](ROADMAP.md) | Future contract and integration work |

The remaining documents explore spatial GUI, expressiveness, lifecycle, execution, nested experiences, visual references, and the broader theory.

## Resources

### Repositories

- [TheSingularityWorkshop.GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI)
- [WebPage / WebForge](https://github.com/TrentBest/WebPage)
- [FSM_API](https://github.com/TrentBest/FSM_API)
- [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)
- [FSM_Serialization](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Serialization)
- [MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain)
- [FSM_API Unity package](https://github.com/TrentBest/FSM_API_Unity)

### NuGet packages

- [TheSingularityWorkshop.GUI.Core](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Core)
- [TheSingularityWorkshop.GUI.Blazor](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Blazor)
- [TheSingularityWorkshop.GUI.WPF](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.WPF)
- [TheSingularityWorkshop.FSM_API](https://www.nuget.org/packages/TheSingularityWorkshop.FSM_API)
- [TheSingularityWorkshop.FSM_COS](https://www.nuget.org/packages/TheSingularityWorkshop.FSM_COS)
- [TheSingularityWorkshop.FSM_Serialization](https://www.nuget.org/packages/TheSingularityWorkshop.FSM_Serialization)
- [TheSingularityWorkshop.MicroBundleDomain](https://www.nuget.org/packages/TheSingularityWorkshop.MicroBundleDomain)

---

<p align="center">
  <a href="https://github.com/TrentBest/FSM_API">
    <img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/FSM_API/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" height="200">
  </a>
</p>

<p align="center">
  <em>The Singularity Workshop — Tools for the curious, the bold, and the systemically inclined.</em><br>
  <strong>Because state shouldn't be a mess.</strong>
</p>
