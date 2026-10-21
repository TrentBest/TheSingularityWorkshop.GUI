using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgUmlClassDiagramRendererTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Fact]
    public void Renders_accessible_class_compartments_and_declared_references()
    {
        var model = new UmlClassDiagram(
            "DataCard",
            kind: "sealed class",
            @namespace: "TheSingularityWorkshop.Workshop.Gui",
            fields: [new UmlMember("_children", "_children : ReadOnlyCollection<DataCard>", "private")],
            properties: [new UmlMember("Id", "Id : string {get}", "public")],
            operations: [new UmlMember("GetCollapsedSummary", "GetCollapsedSummary(isExpanded : bool) : string?", "public")],
            declaredRelationships: ["DataCard → DataCard : Children (source-declared association candidate)"]);

        var document = XDocument.Parse(new SvgUmlClassDiagramRenderer().Render(model));
        Assert.Equal("UML class diagram: DataCard", document.Root!.Element(Svg + "title")!.Value);
        Assert.Contains("source-declared association candidate", document.Root.Element(Svg + "desc")!.Value);
        var text = string.Join(" ", document.Descendants(Svg + "text").Select(element => element.Value));
        Assert.Contains("Fields", text);
        Assert.Contains("Properties", text);
        Assert.Contains("Operations", text);
        Assert.Contains("GetCollapsedSummary", text);
        Assert.Contains("Children", text);
    }

    [Fact]
    public void Rejects_too_narrow_diagram_width()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SvgUmlClassDiagramRenderer().Render(new UmlClassDiagram("Example"), width: 200));
    }
}
