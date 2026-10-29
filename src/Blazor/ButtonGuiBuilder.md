<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# ButtonGuiBuilder — source companion

**Purpose:** map the implementation file to its human-authored contract and UML/use-case documentation.

- **Implementation:** [ButtonGuiBuilder.cs](ButtonGuiBuilder.cs)
- **API contract:** [ButtonGuiBuilder contract](../../docs/api/Blazor/ButtonGuiBuilder.md)
- **UML and use cases:** [ButtonGuiBuilderUML.md](../../docs/uml/ButtonGuiBuilderUML.md)
- **Tests:** [WorkshopGuiBuilderEventTests.cs](../../tests/Blazor.Tests/WorkshopGuiBuilderEventTests.cs)

The builder specializes `ElementBuilder` for a button, sets `type="button"` by default, and offers fluent label, positioning, attribute, accessibility, style, title, and click-handler methods. Business authorization and persistence remain the host application's responsibility.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop avatar" width="200" /></p>
