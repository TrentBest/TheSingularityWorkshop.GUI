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
        var labels = document.Descendants(Svg + "text").Select(element => element.Value).ToArray();
        var propertiesIndex = Array.IndexOf(labels, "Properties");
        var operationsIndex = Array.IndexOf(labels, "Operations");
        var fieldsIndex = Array.IndexOf(labels, "Fields");
        Assert.True(propertiesIndex >= 0 && propertiesIndex < operationsIndex,
            "properties must be presented before operations");
        Assert.True(operationsIndex < fieldsIndex,
            "implementation fields must follow the exposed API surface");
        Assert.Contains("GetCollapsedSummary", text);
        Assert.Contains("Children", text);
    }

    [Fact]
    public void Groups_members_into_ordered_visibility_tiers_within_each_tier()
    {
        var model = new UmlClassDiagram(
            "VisibilityExample",
            fields:
            [
                new UmlMember("_secret", "_secret : int", "private"),
                new UmlMember("_state", "_state : int", "protected")
            ],
            properties:
            [
                new UmlMember("Name", "Name : string", "public"),
                new UmlMember("ProtectedName", "ProtectedName : string", "protected")
            ],
            operations:
            [
                new UmlMember("Run", "Run() : void", "public"),
                new UmlMember("Reset", "Reset() : void", "private")
            ]);

        var document = XDocument.Parse(new SvgUmlClassDiagramRenderer().Render(model));
        var labels = document.Descendants(Svg + "text").Select(element => element.Value).ToArray();
        var publicIndex = Array.IndexOf(labels, "Public visibility");
        var protectedIndex = Array.IndexOf(labels, "Protected visibility");
        var privateIndex = Array.IndexOf(labels, "Private visibility");

        Assert.True(publicIndex >= 0 && publicIndex < protectedIndex && protectedIndex < privateIndex,
            "visibility tiers must be ordered from most exposed to least exposed");
        Assert.True(Array.IndexOf(labels, "Properties") < Array.IndexOf(labels, "Operations"));
        Assert.True(Array.IndexOf(labels, "Operations") < Array.IndexOf(labels, "Fields"));
        Assert.Contains("Name : string", labels);
        Assert.Contains("Reset() : void", labels);
        Assert.Contains("_secret : int", labels);
    }

    [Fact]
    public void Rejects_too_narrow_diagram_width()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SvgUmlClassDiagramRenderer().Render(new UmlClassDiagram("Example"), width: 200));
    }
}
