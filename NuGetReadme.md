# The Singularity Workshop GUI

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

[![Build Status](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.GUI/package.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions)
[![Last commit](https://img.shields.io/github/last-commit/TrentBest/TheSingularityWorkshop.GUI/master)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commits/master)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.GUI?style=flat-square)](https://app.codecov.io/gh/TrentBest/TheSingularityWorkshop.GUI)

A platform-neutral GUI engineering layer for C# applications.

The Singularity Workshop GUI separates **semantic interface intent** from **platform-specific manifestation**.

## Packages

### TheSingularityWorkshop.GUI.Core

The platform-neutral recursive GUI model and builder primitives.

**Current branch version:** `0.1.0-alpha.2`

### TheSingularityWorkshop.GUI.Blazor

Blazor builders and the Blazor manifestation layer built on GUI Core.

**Current branch version:** `0.1.0-alpha.7`

### TheSingularityWorkshop.GUI.WPF

Reusable WPF builders and the native desktop manifestation layer built on GUI Core.

**Current branch version:** `0.1.0-alpha.1`

The WPF layer contains reusable builder lineage recovered from earlier application work and adapted into this repository. It has no dependency on Revit, FTI, or an application-specific host.

The first WPF package release is intentionally separate from the Core and Blazor version lines.

## Core builder vocabulary

The Core package is intended to be useful on its own.

The default builders create a platform-neutral semantic tree:

- `GuiBuilders.Panel(...)` — generic container.
- `GuiBuilders.Stack(...)` — stack-oriented container.
- `GuiBuilders.Row(...)` — horizontal composition.
- `GuiBuilders.Column(...)` — vertical composition.
- `GuiBuilders.Text(...)` — textual content.
- `GuiBuilders.Button(...)` — semantic action surface.
- `GuiBuilders.Image(...)` — image/media surface.
- `GuiBuilders.Warning(...)` — warning/informational surface.
- `GuiBuilders.Separator(...)` — visual separation.
- `GuiBuilders.TextBox(...)` — editable text surface.

Example:

```csharp
var page = GuiBuilders.Column("page")
    .Child(GuiBuilders.Text("heading", "Hello"))
    .Child(GuiBuilders.Button("continue", "Continue"))
    .Build();
```

The result is a `GuiNode` snapshot. Core does not know how that node will be displayed.

## WPF native builders

GUI.WPF is deliberately more expressive than the neutral Core vocabulary because native desktop applications can require platform-specific capabilities.

The current WPF surface includes:

- root/window builder infrastructure;
- panel, stack, grid, uniform-grid, wrap, tabbed, and multi-panel builders;
- numeric and enum controls;
- tree-view and pivot-grid controls;
- color-picker and CRUD dialogs;
- reflection-driven property editors;
- diagnostic/log viewing;
- semantic Core-to-WPF rendering;
- Hub rendering;
- WPF visual helpers and factory utilities.

These builders are **WPF-native APIs**. They are not promoted into Core merely because WPF supports them.

See [WPF Guide](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/WPF_GUIDE.md) and [Implementation Status](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/master/docs/IMPLEMENTATION_STATUS.md).

## Choosing a package

Use **GUI.Core** wherever GUI intent is authored or shared.

Add **GUI.Blazor** at a browser manifestation boundary.

Add **GUI.WPF** at a Windows desktop manifestation boundary, or when a WPF application intentionally wants its richer native builder surface.

The semantic model and the native builder APIs therefore coexist without forcing native concerns downward into Core.

## Relationship with FSM

- **FSM_API** provides state-transition and runtime behavior.
- **FSM_COS** assembles runtime composition.
- **GUI** provides semantic presentation and interaction representation.
- **Platform adapters** turn semantic GUI intent into platform-native manifestation.
- **MicroBundleDomain** provides the MicroBundle contracts used by composition infrastructure.

GUI does not own runtime composition.

## Current release posture

This repository is actively developing an alpha platform family.

Documentation may advance independently of package publication.

A package is considered released only when it has successfully passed its build/test/pack lane and has been explicitly published through the repository's trusted-publishing workflow.

The WPF package is staged as `0.1.0-alpha.1` for its first publication.

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

## License

MIT License.

Copyright © 2026 The Singularity Workshop.
