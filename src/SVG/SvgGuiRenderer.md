# SvgGuiRenderer — source companion

**Purpose:** map the implementation file to its human-authored contract and UML/use-case documentation.

- **Implementation:** [SvgGuiRenderer.cs](SvgGuiRenderer.cs)
- **API contract:** [SvgGuiRenderer contract](../../docs/api/SVG/SvgGuiRenderer.md)
- **UML and use cases:** [SvgGuiRendererUML.md](../../docs/uml/SvgGuiRendererUML.md)
- **Adapter specification:** [SVG adapter contract](../../docs/SVG_ADAPTER.md)
- **Tests:** [SvgGuiRendererTests.cs](../../tests/SVG.Tests/SvgGuiRendererTests.cs)

The renderer serializes supported Core GUI node kinds to SVG XML. It validates dimensions, ID uniqueness, and tree depth and emits accessible root metadata. It is not a sanitizer, interactive host, animation scheduler, or GPU renderer.
