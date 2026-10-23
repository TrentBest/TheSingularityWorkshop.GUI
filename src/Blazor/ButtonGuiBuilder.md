# ButtonGuiBuilder — source companion

**Purpose:** map the implementation file to its human-authored contract and UML/use-case documentation.

- **Implementation:** [ButtonGuiBuilder.cs](ButtonGuiBuilder.cs)
- **API contract:** [ButtonGuiBuilder contract](../../docs/api/Blazor/ButtonGuiBuilder.md)
- **UML and use cases:** [ButtonGuiBuilderUML.md](../../docs/uml/ButtonGuiBuilderUML.md)
- **Tests:** [WorkshopGuiBuilderEventTests.cs](../../tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs)

The builder specializes `ElementBuilder` for a button, sets `type="button"` by default, and offers fluent label, positioning, attribute, accessibility, style, title, and click-handler methods. Business authorization and persistence remain the host application's responsibility.
