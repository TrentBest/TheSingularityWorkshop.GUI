# Solution-level source-derived UML inventory

- Repository: `TrentBest/TheSingularityWorkshop.GUI`
- Source revision: `c314067d1411b65a797609459058e454c46bb143`
- Scope: maintained product projects under `src/`; tests, samples, tooling, and documentation projects are deliberately excluded.
- Evidence: each project is restored and built in Release, loaded with MSBuild/Roslyn, checked for compiler errors, and reconciled against the exact build output by assembly identity and type metadata inventory.
- Framework policy: each included project must currently declare exactly one literal target framework. Multi-targeted projects fail closed until per-framework output partitioning is implemented.
- Limitation: this verifies type inventory against the exact build, not member-level reflection parity or full-solution graph rendering.

| Project | Types | Compiler-symbol relationships | Inventory | Type pages |
|---|---:|---:|---|---|
| `src/Core/TheSingularityWorkshop.GUI.Core.csproj` | 49 | 168 | [Core inventory](Core/inventory.md) | [Core type pages](Core/index.md) |
| `src/Blazor/TheSingularityWorkshop.GUI.Blazor.csproj` | 6 | 78 | [Blazor inventory](Blazor/inventory.md) | [Blazor type pages](Blazor/index.md) |
| `src/WPF/TheSingularityWorkshop.GUI.WPF.csproj` | 23 | 71 | [WPF inventory](WPF/inventory.md) | [WPF type pages](WPF/index.md) |
| `src/SVG/TheSingularityWorkshop.GUI.SVG.csproj` | 11 | 43 | [SVG inventory](SVG/inventory.md) | [SVG type pages](SVG/index.md) |
| `src/Markdown/TheSingularityWorkshop.GUI.Markdown.csproj` | 4 | 11 | [Markdown inventory](Markdown/inventory.md) | [Markdown type pages](Markdown/index.md) |

**Coverage:** 5/5 maintained product projects analyzed, built, and reconciled successfully.
