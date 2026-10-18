using System.Xml.Linq;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.Adapter.Conformance.Tests;

/// <summary>
/// Runs one semantic fixture through both current adapters and checks only
/// invariants that can be observed in both output formats.
/// </summary>
public sealed class SharedAdapterConformanceTests
{
    [Fact]
    public void Both_adapters_preserve_node_identity_child_order_and_text()
    {
        var model = CreateFixture();

        var blazorBuilder = new RenderTreeBuilder();
        BlazorGuiRenderer.Render(model)(blazorBuilder);
        var blazorFrames = blazorBuilder.GetFrames().Array;
        var blazorIds = blazorFrames
            .Where(frame => frame.FrameType == RenderTreeFrameType.Attribute &&
                            frame.AttributeName == "id")
            .Select(frame => (string)frame.AttributeValue!)
            .ToArray();
        var blazorText = blazorFrames
            .Where(frame => frame.FrameType == RenderTreeFrameType.Text)
            .Select(frame => frame.TextContent)
            .ToArray();

        var svgDocument = XDocument.Parse(new SvgGuiRenderer().Render(model));
        XNamespace svg = "http://www.w3.org/2000/svg";
        var svgIds = svgDocument.Descendants()
            .Select(element => (string?)element.Attribute("id"))
            .Where(id => id is not null)
            .Cast<string>()
            .ToArray();
        var svgRoot = svgDocument.Descendants(svg + "g")
            .Single(element => (string?)element.Attribute("id") == "root");
        var svgChildIds = svgRoot.Elements()
            .Select(element => (string?)element.Attribute("id"))
            .Where(id => id is not null)
            .Cast<string>()
            .ToArray();
        var svgText = svgDocument.Descendants(svg + "title")
            .Select(element => element.Value)
            .Where(value => value is "Save" or "Ready")
            .ToArray();

        Assert.Equal(new[] { "root", "save", "status" }, blazorIds);
        Assert.Equal(new[] { "root", "save", "status" }, svgIds);
        Assert.Equal(new[] { "First", "Save", "Ready" }.Skip(1), blazorText);
        Assert.Equal(new[] { "save", "status" }, svgChildIds);
        Assert.Equal(new[] { "Save", "Ready" }, svgText);
    }

    private static GuiNode CreateFixture() =>
        GuiBuilder.Create("Panel", "root")
            .Child("Button", "save", button => button.Text("Save"))
            .Child("Text", "status", text => text.Text("Ready"))
            .Build();
}
