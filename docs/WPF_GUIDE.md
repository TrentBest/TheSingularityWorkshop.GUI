# GUI.WPF Guide

GUI.WPF is the Windows-specific manifestation and native-builder package for the platform-neutral GUI model.

> **Current status:** initial, actively tested adapter; not a claim of complete parity with Core, Blazor, or SVG. This guide describes the current development tree, not the historical `GuiHub` implementation found on older branches.

## Responsibility boundary

The supported semantic path is:

```text
GuiNode / Core builders
        |
        v
   WpfGuiRenderer
        |
        v
 native WPF controls
```

- **GUI.Core** owns the neutral semantic model and platform-independent builder contracts.
- **GUI.WPF** owns WPF controls, WPF-specific layout, native window behavior, and the mapping from supported semantic nodes to WPF.
- **The consuming host** owns application/domain meaning, authoritative runtime lifecycle, and composition of its experience.

WPF-specific capability does not automatically become a cross-platform Core primitive. Core must remain independent of WPF assemblies and types.

## Current package surface

The current `src/WPF` project includes:

- `WpfGuiRenderer` for rendering supported Core nodes to native WPF controls.
- Native layout/building surfaces including Canvas, Grid, Dock, Stack, UniformGrid, Wrap, and panel builders.
- Native window and identity/configuration support through `WpfWindowBuilder`, `WpfGuiIdentity`, and `GuiConfiguration` / `GuiMode`.
- WPF-native numeric and enum value-control builders.

The exact behavior of each type is defined by its source and tests; this overview is not a promise of complete semantic-kind coverage or feature parity. Consult the source and the per-type documentation as that inventory is completed.

## Use the package intentionally

Use Core builders for interface meaning that should remain platform-neutral. Use WPF-native builders when the requirement is specifically Windows desktop behavior, such as native layout, window configuration, or native value controls.

Keep WPF types and dependencies inside the WPF adapter. Do not put WPF controls, application-specific services, or runtime orchestration into GUI.Core.

## Verification

The current Windows test project is `tests/WPF.Tests`. It includes renderer conformance coverage, builder tests, and value-control builder tests. The package workflow builds/tests the WPF adapter on Windows; current workflow results must be checked against the exact commit under review.

Passing WPF tests demonstrates only the contracts those tests exercise. It does not establish complete adapter parity, a universal accessibility abstraction, portable native-control behavior, or release authorization.

## Known boundaries

- WPF is Windows-only and targets the configured Windows framework.
- The renderer is an initial implementation; unsupported or unknown semantic kinds must not be described as having a correct native interpretation without test evidence.
- Host lifecycle and recursive experience composition are not owned by a GUI Hub API in the current implementation. See [Nested Experiences](GUI_NESTED_EXPERIENCES.md), which marks that model as a proposal rather than a callable contract.
- Release readiness, package versions, and publication authorization are tracked separately in [Release Readiness](RELEASE_READINESS.md). Passing CI does not authorize publication.

## Related documentation

- [WPF project README](../src/WPF/README.md)
- [WPF Builder Pattern Audit](WPF_BUILDER_PATTERN_AUDIT.md)
- [Platform Adapters](PLATFORM_ADAPTERS.md)
- [GUI Model](GUI_MODEL.md)
- [Per-Type Documentation and UML Standard](CLASS_DOCUMENTATION_STANDARD.md)
- [Release Readiness](RELEASE_READINESS.md)
