# The Singularity Workshop GUI

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

[![Build Status](https://img.shields.io/github/actions/workflow/status/TrentBest/TheSingularityWorkshop.GUI/package.yml?branch=master&style=flat-square&logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/actions)
[![Last commit](https://img.shields.io/github/last-commit/TrentBest/TheSingularityWorkshop.GUI/master)](https://github.com/TrentBest/TheSingularityWorkshop.GUI/commits/master)
[![Code Coverage](https://img.shields.io/codecov/c/github/TrentBest/TheSingularityWorkshop.GUI?style=flat-square)](https://app.codecov.io/gh/TrentBest/TheSingularityWorkshop.GUI)

A platform-neutral GUI engineering layer for C# applications.

The Singularity Workshop GUI separates **semantic interface intent** from **platform-specific manifestation**.

```text
application / experience
        |
semantic GUI model
        |
platform adapter
        |
native GUI
```

## Packages

### TheSingularityWorkshop.GUI.Core

The platform-neutral recursive GUI model and builder primitives.

### TheSingularityWorkshop.GUI.Blazor

Blazor builders and the Blazor manifestation layer built on GUI Core.

### TheSingularityWorkshop.GUI.WPF

Reusable WPF builders and the native desktop manifestation layer built on GUI Core.

The WPF layer contains the reusable builder lineage recovered from earlier application work, but it has no dependency on Revit, FTI, or any application-specific host.

## Relationship with FSM

- **FSM_API** provides state-transition and runtime behavior.
- **FSM_COS** assembles runtime composition.
- **GUI** provides visualization and interaction representation.
- **Platform adapters** turn semantic GUI intent into platform-native manifestation.
- **MicroBundleDomain** provides the microbundle contracts used by composition infrastructure.

## Current status

This package family is under active development. NuGet publication remains a deliberate release action; repository documentation may be updated independently of package publication.

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
