<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# SvgGuiRenderer — source companion

**Purpose:** map the implementation file to its human-authored contract and UML/use-case documentation.

- **Implementation:** [SvgGuiRenderer.cs](SvgGuiRenderer.cs)
- **API contract:** [SvgGuiRenderer contract](../../docs/api/SVG/SvgGuiRenderer.md)
- **UML and use cases:** [SvgGuiRendererUML.md](../../docs/uml/SvgGuiRendererUML.md)
- **Adapter specification:** [SVG adapter contract](../../docs/SVG_ADAPTER.md)
- **Tests:** [SvgGuiRendererTests.cs](../../tests/SVG.Tests/SvgGuiRendererTests.cs)

The renderer serializes supported Core GUI node kinds to SVG XML. It validates dimensions, ID uniqueness, and tree depth and emits accessible root metadata. It is not a sanitizer, interactive host, animation scheduler, or GPU renderer.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
