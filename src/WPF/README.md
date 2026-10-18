# GUI WPF

The WPF project provides WPF-specific GUI construction and rendering support for the shared GUI model.

## What it is

- A Windows Presentation Foundation adapter project targeting `net8.0-windows`.
- A consumer of `TheSingularityWorkshop.GUI.Core` and its platform-neutral `GuiNode` model.
- An initial renderer with Windows-targeted tests for the behavior currently implemented.

## Current scope

The WPF renderer is an initial implementation, not a claim of complete parity with Blazor or SVG. The tested surface includes hierarchy and child ordering, text, dimensions and spacing, colors, typography, alignment, null-root rejection, unknown-kind fallback, duplicate-ID rejection, depth-limit handling, and semantic ID preservation.

Features outside that tested surface—including broader property mapping, accessibility, interaction/event behavior, and complete cross-adapter parity—must be treated as unverified until implementation and tests demonstrate them.

## Build and test

Build on Windows with the .NET 8 SDK:

```powershell
dotnet build src/WPF/TheSingularityWorkshop.GUI.WPF.csproj --configuration Release
dotnet test --configuration Release
```

The solution-level test command is illustrative; run the repository's documented Windows WPF test workflow to verify the actual WPF test project and record the exact commit and workflow result.

## Package status

The project file currently declares package version `0.1.0`. Check [release readiness](../../docs/RELEASE_READINESS.md) before relying on that value: project-file versions and public NuGet versions have diverged, and whether WPF belongs in a particular release remains an explicit release-scope decision.

This README documents the project; it does not by itself mean the README is included in the NuGet package. The project must explicitly configure package README metadata and the resulting `.nupkg` must be inspected before making that claim.
