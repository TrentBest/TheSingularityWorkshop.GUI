# WPF Guide

GUI.WPF is the Windows manifestation package for The Singularity Workshop GUI architecture.

It has an important dual role:

1. manifest the platform-neutral Core semantic model as native WPF controls;
2. provide richer WPF-native builders where a desktop application intentionally wants capabilities beyond the neutral model.

## Package

**Package:** `TheSingularityWorkshop.GUI.WPF`  
**Current staged version:** `0.1.0-alpha.1`  
**Target:** .NET 8 / Windows  
**License:** MIT

The package depends on `TheSingularityWorkshop.GUI.Core`.

It does not depend on FSM_COS, WebPage, Revit, FTI, or another application-specific host.

## 1. Semantic WPF manifestation

The semantic path is:

~~~text
GuiBuilder / GuiNode
        |
        v
   WpfGuiRenderer
        |
        v
   native WPF tree
~~~

The Hub path is:

~~~text
IGuiHub
   |
   v
WpfGuiHubRenderer
   |
   v
native WPF root
~~~

The renderer currently understands the repository's canonical semantic kinds and translates selected common properties.

### Canonical kinds

| Core kind | Current WPF manifestation |
|---|---|
| `Panel` | `Grid` |
| `Stack` | `StackPanel` |
| `Row` | horizontal `StackPanel` |
| `Column` | vertical `StackPanel` |
| `Text` | `TextBlock` |
| `Button` | `Button` |
| `TextBox` | `TextBox` |
| `Image` | `Image` |
| `Warning` | `Border` / text presentation |
| `Separator` | separator presentation |
| unknown kind | neutral `Grid` fallback |

The fallback is a compatibility mechanism, not a claim that every unknown semantic kind has a correct native interpretation.

## 2. Native WPF builders

The WPF builder family is intentionally richer than Core.

Current builder areas include:

### Layout

- panel builder infrastructure;
- grid composition;
- stack panels;
- uniform grids;
- wrapping layouts;
- tabbed panels;
- multi-panel composition.

### Controls

- numeric controls;
- enum controls;
- tree views;
- pivot-grid composition.

### Dialogs

- color picker;
- CRUD-oriented dialog construction.

### Reflection

The reflective builder can inspect supported object properties and construct editors for the current supported primitive/enum/string/enumerable surface.

The reflection layer owns the WPF/editor behavior. It does not add reflection semantics to Core.

### Diagnostics

The WPF layer includes a diagnostic/log viewer builder for desktop-facing diagnostic presentation.

### Window and root composition

The root WPF builder lineage provides fluent configuration for native window composition, sizing, ownership, activation, chrome, and related desktop behavior.

These APIs are intentionally WPF-specific.

## 3. Builder philosophy

The WPF package should not become a second semantic Core.

Use Core when the meaning is platform-neutral:

~~~csharp
var page = GuiBuilders.Column("page")
    .Child(GuiBuilders.Text("title", "Workshop"))
    .Child(GuiBuilders.Button("enter", "Enter"))
    .Build();
~~~

Use WPF-native builders when the application explicitly needs WPF behavior that is not part of the Core contract.

This distinction is intentional:

~~~text
semantic meaning
      |
      +--> GUI.Core
      |
      +--> WPF-native capability when required
~~~

A WPF-specific capability does not automatically become a cross-platform semantic primitive.

## 4. What WPF does not promise yet

The first WPF package is an alpha release.

It does not currently promise:

- complete Core semantic coverage;
- complete WPF builder test coverage;
- shared adapter conformance;
- capability negotiation;
- semantic input portability;
- accessibility abstraction;
- deterministic GUI serialization;
- cross-platform lifecycle parity.

Those are future contracts, not hidden features.

## 5. Testing

The Windows CI lane currently:

1. restores GUI.WPF;
2. builds it in Release;
3. packs the NuGet artifact.

A dedicated `tests/WPF.Tests` project is being established separately.

The intended test suite will cover both:

- semantic manifestation behavior;
- native WPF builder behavior.

The package should not be considered fully behavior-verified until those tests exist and run in the Windows lane.

## 6. Recommended usage

For a shared experience:

~~~text
experience
   |
   v
GUI.Core
   |
   +---- browser ----> GUI.Blazor
   |
   +---- Windows ----> GUI.WPF
~~~

For a WPF-only desktop tool:

~~~text
desktop application
       |
       +---- GUI.Core for shared semantic surfaces
       |
       +---- GUI.WPF for native desktop capabilities
~~~

This lets desktop developers use the full power of WPF without forcing WPF concepts into the platform-neutral layer.

## Related documentation

- [Start Here](START_HERE.md)
- [Implementation Status](IMPLEMENTATION_STATUS.md)
- [Architecture](ARCHITECTURE.md)
- [Platform Adapters](PLATFORM_ADAPTERS.md)
- [GUI Model](GUI_MODEL.md)
- [Testing](TESTING.md)
- [Roadmap](../ROADMAP.md)
