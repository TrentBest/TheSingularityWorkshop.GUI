# Solution-level source-derived UML inventory

- Repository: `TrentBest/TheSingularityWorkshop.GUI`
- Source revision: `a25acf387b1e24b6dba0a927e763c0b55c8b9b3e`
- Scope: maintained product projects under `src/`; tests, samples, tooling, and documentation projects are deliberately excluded.
- Evidence: each project is restored and built in Release, loaded with MSBuild/Roslyn, checked for compiler errors, and reconciled against the exact build output by assembly identity and type metadata inventory.
- Framework policy: each included project must currently declare exactly one literal target framework. Multi-targeted projects fail closed until per-framework output partitioning is implemented.
- Limitation: this verifies type inventory against the exact build, not member-level reflection parity or full-solution graph rendering.

| Project | Types | Compiler-symbol relationships | Inventory | Type pages |
|---|---:|---:|---|---|
| `src/Core/TheSingularityWorkshop.GUI.Core.csproj` | 31 | 119 | [core/inventory.md](core/inventory.md) | [core/types/index.md](core/types/index.md) |
| `src/Blazor/TheSingularityWorkshop.GUI.Blazor.csproj` | 6 | 69 | [blazor/inventory.md](blazor/inventory.md) | [blazor/types/index.md](blazor/types/index.md) |
| `src/WPF/TheSingularityWorkshop.GUI.WPF.csproj` | 23 | 67 | [wpf/inventory.md](wpf/inventory.md) | [wpf/types/index.md](wpf/types/index.md) |
| `src/SVG/TheSingularityWorkshop.GUI.SVG.csproj` | 9 | 39 | [svg/inventory.md](svg/inventory.md) | [svg/types/index.md](svg/types/index.md) |

**Coverage:** 4/4 maintained product projects analyzed, built, and reconciled successfully.
