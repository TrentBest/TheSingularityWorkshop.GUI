# Solution-level source-derived UML inventory

- Repository: `TrentBest/TheSingularityWorkshop.GUI`
- Source revision: `f5c9a2817cdcb9c9ab1ec235e7b67502bcd3b732`
- Scope: maintained product projects under `src/`; tests, samples, tooling, and documentation projects are deliberately excluded.
- Evidence: each project is restored and built in Release, loaded with MSBuild/Roslyn, checked for compiler errors, and reconciled against the exact build output by assembly identity and type metadata inventory.
- Framework policy: each included project must currently declare exactly one literal target framework. Multi-targeted projects fail closed until per-framework output partitioning is implemented.
- Limitation: this verifies type inventory against the exact build, not member-level reflection parity or full-solution graph rendering.

| Project | Types | Compiler-symbol relationships | Inventory | Type pages |
|---|---:|---:|---|---|
| `src/Core/TheSingularityWorkshop.GUI.Core.csproj` | 31 | 119 | [Core inventory](Core/inventory.md) | [Core type pages](Core/index.md) |
| `src/Blazor/TheSingularityWorkshop.GUI.Blazor.csproj` | 6 | 69 | [Blazor inventory](Blazor/inventory.md) | [Blazor type pages](Blazor/index.md) |
| `src/WPF/TheSingularityWorkshop.GUI.WPF.csproj` | 23 | 67 | [WPF inventory](WPF/inventory.md) | [WPF type pages](WPF/index.md) |
| `src/SVG/TheSingularityWorkshop.GUI.SVG.csproj` | 9 | 39 | [SVG inventory](SVG/inventory.md) | [SVG type pages](SVG/index.md) |

**Coverage:** 4/4 maintained product projects analyzed, built, and reconciled successfully.
