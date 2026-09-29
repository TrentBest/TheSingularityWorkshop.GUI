# TheSingularityWorkshop.GUI

![The Land of Idealism](docs/assets/ideal-gui-separation-of-concerns.jpg)

A platform-neutral GUI engineering layer with recursive builders and platform adapters.

The repository exists to separate GUI intent from GUI manifestation.

## Repository structure

- src/Core — the neutral recursive GUI model, the intrinsic GUI Hub, and platform-independent primitives.
- src/Blazor — Blazor builders and the first Core-to-web manifestation.
- src/WPF — reusable WPF builders and desktop manifestation infrastructure.
- tests/Core.Tests — platform-independent contract tests.
- docs — architecture, model, adapter, and testing contracts.

## The integrated Hub

The GUI Hub is the default semantic presentation surface for an assembled runtime.

```text
FSM_COS
   |
   +-- assembles Hub MicroBundle
           |
           v
GUI.Core
   |
   +-- defines IGuiHub / GuiHub
           |
           v
GUI.Blazor / GUI.WPF / GUI.Unity...
   |
   +-- manifests the same semantic surface
```

GUI.Core contains no platform rendering dependency. GUI.Blazor contains the browser manifestation. The host supplies the Hub MicroBundle to FSM_COS; FSM_COS assembles it alongside the rest of the runtime.

This is the distinction between owning the Hub structure and owning runtime composition.

## Current integration

The WebPage repository is the first migration target for the Blazor layer and the proving ground for the integrated Hub boundary.

The intended flow is:

RuntimeManifest -> FSM_COS -> RuntimeAssembly -> Hub MicroBundle -> GUI.Core -> GUI.Blazor

The browser remains a manifestation target, not a dependency of FSM_COS.
