<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# BlazorGuiRenderer — source companion

**Purpose:** map the implementation file to its human-authored contract and UML/use-case documentation.

- **Implementation:** [BlazorGuiRenderer.cs](BlazorGuiRenderer.cs)
- **API contract:** [BlazorGuiRenderer contract](../../docs/api/Blazor/BlazorGuiRenderer.md)
- **UML and use cases:** [BlazorGuiRendererUML.md](../../docs/uml/BlazorGuiRendererUML.md)
- **Tests:** [BlazorGuiRendererTests.cs](../../tests/Blazor.Tests/BlazorGuiRendererTests.cs) and [BlazorGuiRendererConformanceTests.cs](../../tests/Blazor.Tests/BlazorGuiRendererConformanceTests.cs)

This adapter converts a Core `GuiNode` tree into a Blazor `RenderFragment`. Its implemented guarantees include null-root validation, unique IDs, a 256-level depth ceiling, ordered child rendering, and a deliberately small node-kind-to-HTML-tag mapping. It does not define a complete host lifecycle or sanitize arbitrary model input.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
