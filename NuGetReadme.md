# The Singularity Workshop GUI

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

[![GUI Core NuGet version](https://img.shields.io/nuget/v/TheSingularityWorkshop.GUI.Core?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Core)
[![GUI Core NuGet downloads](https://img.shields.io/nuget/dt/TheSingularityWorkshop.GUI.Core?logo=nuget&style=flat-square)](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Core)
[![GUI Blazor NuGet version](https://img.shields.io/nuget/v/TheSingularityWorkshop.GUI.Blazor?style=flat-square&logo=nuget&logoColor=white)](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Blazor)
[![GUI Blazor NuGet downloads](https://img.shields.io/nuget/dt/TheSingularityWorkshop.GUI.Blazor?logo=nuget&style=flat-square)](https://www.nuget.org/packages/TheSingularityWorkshop.GUI.Blazor)

[![Build Status](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.GUI/package.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions)
[![Last commit](https://img.shields.io/github/last-commit/TrentBest/TheSingularityWorkshop.GUI/master)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commits/master)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.GUI?style=flat-square)](https://app.codecov.io/gh/TrentBest/TheSingularityWorkshop.GUI)
[![Known Vulnerabilities](https://snyk.io/test/github/TrentBest/TheSingularityWorkshop.GUI/badge.svg)](https://snyk.io/test/github/TrentBest/TheSingularityWorkshop.GUI)

[![GitHub stars](https://img.shields.io/github/stars/TrentBest/TheSingularityWorkshop.GUI?style=social)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/TrentBest/TheSingularityWorkshop.GUI)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/graphs/contributors)
[![Open Issues](https://img.shields.io/github/issues/TrentBest/TheSingularityWorkshop.GUI)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues)

[![CoderLegion](https://coderlegion.com/cl_badge_logo1.png) Join the CoderLegion Community](https://coderlegion.com/user/The+Singularity+Workshop)

[**💖 Support Us**](https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J)

A platform-neutral GUI engineering layer for C# applications.

The Singularity Workshop GUI separates **semantic interface intent** from **platform-specific manifestation**.

```
application / experience
        ↓
semantic GUI model
        ↓
platform adapter
        ↓
native GUI
```

The same semantic GUI can therefore be represented through Blazor, WPF, Unity UI Toolkit, or future adapters without making any one presentation technology intrinsic to the application's domain.

![The Land of Idealism](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.GUI/master/docs/assets/ideal-gui-separation-of-concerns.jpg)

## Packages

### TheSingularityWorkshop.GUI.Core

The platform-neutral recursive GUI model and builder primitives.

For the current prerelease:

```bash
dotnet add package TheSingularityWorkshop.GUI.Core --version 0.1.0-alpha
```

### TheSingularityWorkshop.GUI.Blazor

Blazor builders and the Blazor manifestation layer built on GUI Core.

For the current prerelease:

```bash
dotnet add package TheSingularityWorkshop.GUI.Blazor --version 0.1.0-alpha.1
```

The Blazor package depends on GUI Core.

## Quick start

Build a semantic GUI tree without introducing HTML, DOM, CSS, or Blazor types into Core:

```csharp
using TheSingularityWorkshop.Workshop.Gui;

var gui = GuiBuilder
    .Create("Panel", "workshop")
    .Text("The Singularity Workshop")
    .Child("Panel", "content", child =>
    {
        child
            .Child("Text", "title", text => text.Text("Welcome"))
            .Child("Button", "enter", button => button.Text("Enter Workshop"));
    })
    .Build();
```

The resulting object is a recursive semantic model. Platform adapters decide how that model is manifested.

## Why this exists

GUI code tends to become coupled to the technology used to render it.

This project deliberately puts a boundary between:

- **experience semantics** — what the application wants to communicate or allow;
- **GUI semantics** — the representation and interaction model;
- **platform manifestation** — how that representation becomes HTML, WPF controls, Unity UI, or another native surface.

Core therefore does **not** depend on HTML, DOM, CSS, JavaScript, Blazor `RenderFragment`, WPF `DependencyObject`, XAML, or Unity UI Toolkit types.

## Recursive composition

The model is intentionally recursive:

```
Experience
  └── Panel
      ├── Panel
      │   ├── Text
      │   └── Image
      └── Button
          └── Text
```

Small builders can therefore compose larger interfaces while retaining semantic identity and structure.

## Relationship with FSM_API

The GUI layer and the state layer have different responsibilities:

- **FSM_API** provides state-transition and runtime behavior.
- **GUI** provides visualization and interaction representation.
- **Platform adapters** turn semantic GUI intent into platform-native manifestation.

This separation allows an experience to change state without making the state machine itself responsible for rendering technology.

## Current status

This package is under active development.

The first production proving ground is the **WebPage** repository, where the semantic GUI model is being integrated into a live Blazor experience.

The architecture is intentionally being developed in the open while the contracts stabilize.

The first public NuGet packages are **0.1.0-alpha** and **0.1.0-alpha.1** prereleases. The repository's CI validates tests, uploads code coverage, and publishes the versioned packages to NuGet.org through trusted publishing.

## Documentation

The full engineering and theory documentation lives in the repository:

- [Start Here](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/START_HERE.md)
- [Architecture](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/ARCHITECTURE.md)
- [GUI Model](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/GUI_MODEL.md)
- [Platform Adapters](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/PLATFORM_ADAPTERS.md)
- [GUI Execution Model](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/GUI_EXECUTION_MODEL.md)
- [Testing](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/TESTING.md)
- [Roadmap](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/ROADMAP.md)

## Feedback and contributions

Issues, experiments, architectural criticism, and pull requests are welcome.

[Open an issue](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues) or explore the repository on GitHub.

## License

MIT License.

Copyright © 2026 The Singularity Workshop.

