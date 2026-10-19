using System.Collections.Generic;
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
            .Where(element => element.Name.LocalName != "title" && element.Name.LocalName != "desc")
            .Select(element => (string?)element.Attribute("id"))
            .Where(id => id is not null)
            .Cast<string>()
            .ToArray();
        var svgRoot = svgDocument.Descendants(svg + "g")
            .Single(element => (string?)element.Attribute("id") == "root");
        var svgRootChildIds = svgRoot.Elements()
            .Select(element => (string?)element.Attribute("id"))
            .Where(id => id is not null)
            .Cast<string>()
            .ToArray();
        var svgNestedGroup = svgDocument.Descendants(svg + "g")
            .Single(element => (string?)element.Attribute("id") == "actions");
        var svgNestedChildIds = svgNestedGroup.Elements()
            .Select(element => (string?)element.Attribute("id"))
            .Where(id => id is not null)
            .Cast<string>()
            .ToArray();
        var svgText = svgDocument.Descendants(svg + "title")
            .Select(element => element.Value)
            .Where(value => value is "Save" or "Ready")
            .ToArray();

        Assert.Equal(new[] { "root", "actions", "save", "status" }, blazorIds);
        Assert.Equal(new[] { "root", "actions", "save", "status" }, svgIds);
        Assert.Equal(new[] { "Save", "Ready" }, blazorText);
        Assert.Equal(new[] { "actions" }, svgRootChildIds);
        Assert.Equal(new[] { "save", "status" }, svgNestedChildIds);
        Assert.Equal(new[] { "Save", "Ready" }, svgText);
    }

    [Fact]
    public void Property_forwarding_is_currently_adapter_specific()
    {
        var model = new GuiNode(
            "Panel",
            "property-surface",
            properties: new Dictionary<string, string>
            {
                ["class"] = "sample-class",
                ["data-note"] = "prototype-only",
                ["fill"] = "none"
            });

        var blazorBuilder = new RenderTreeBuilder();
        BlazorGuiRenderer.Render(model)(blazorBuilder);
        var blazorAttributes = blazorBuilder.GetFrames().Array
            .Where(frame => frame.FrameType == RenderTreeFrameType.Attribute)
            .ToDictionary(frame => frame.AttributeName, frame => frame.AttributeValue);

        var svgDocument = XDocument.Parse(new SvgGuiRenderer().Render(model));
        XNamespace svg = "http://www.w3.org/2000/svg";
        var svgRoot = svgDocument.Root!;
        var svgNode = svgDocument.Descendants(svg + "g")
            .Single(element => (string?)element.Attribute("id") == "property-surface");

        Assert.Equal("sample-class", blazorAttributes["class"]);
        Assert.Equal("prototype-only", blazorAttributes["data-note"]);
        Assert.Equal("none", blazorAttributes["fill"]);

        Assert.Equal("sample-class", (string?)svgNode.Attribute("class"));
        Assert.Equal("none", (string?)svgNode.Attribute("fill"));
        Assert.Null(svgNode.Attribute("data-note"));
        Assert.Null(svgRoot.Attribute("data-note"));
    }

    [Fact]
    public void Both_adapters_preserve_node_identity_when_properties_contain_id()
    {
        var model = new GuiNode(
            "Panel",
            "semantic-id",
            properties: new Dictionary<string, string>
            {
                ["id"] = "property-id",
                ["ID"] = "case-variant-property-id"
            });

        var blazorBuilder = new RenderTreeBuilder();
        BlazorGuiRenderer.Render(model)(blazorBuilder);
        var blazorIds = blazorBuilder.GetFrames().Array
            .Where(frame => frame.FrameType == RenderTreeFrameType.Attribute &&
                            frame.AttributeName == "id")
            .Select(frame => (string)frame.AttributeValue!)
            .ToArray();

        var svgDocument = XDocument.Parse(new SvgGuiRenderer().Render(model));
        var svgIds = svgDocument.Descendants()
            .Where(element => element.Name.LocalName != "title" && element.Name.LocalName != "desc")
            .Select(element => (string?)element.Attribute("id"))
            .Where(id => id is not null)
            .Cast<string>()
            .ToArray();

        Assert.Equal(new[] { "semantic-id" }, blazorIds);
        Assert.Equal(new[] { "semantic-id" }, svgIds);
    }

    [Fact]
    public void Both_adapters_reject_duplicate_ids_in_a_tree()
    {
        var duplicateIds = GuiBuilder.Create("Panel", "root")
            .Child("Text", "duplicate")
            .Child("Button", "duplicate")
            .Build();

        var blazorException = Assert.Throws<System.ArgumentException>(
            () => BlazorGuiRenderer.Render(duplicateIds));
        var svgException = Assert.Throws<System.ArgumentException>(
            () => new SvgGuiRenderer().Render(duplicateIds));

        Assert.Contains("duplicate", blazorException.Message);
        Assert.Contains("duplicate", svgException.Message);
    }

    [Fact]
    public void Both_adapters_reject_trees_deeper_than_the_shared_limit()
    {
        var tooDeep = CreateDeepTree(257);

        Assert.Throws<System.InvalidOperationException>(
            () => BlazorGuiRenderer.Render(tooDeep));
        Assert.Throws<System.InvalidOperationException>(
            () => new SvgGuiRenderer().Render(tooDeep));
    }

    private static GuiNode CreateDeepTree(int deepestNodeDepth)
    {
        GuiNode node = new("Panel", "node-0");
        for (var depth = 1; depth <= deepestNodeDepth; depth++)
            node = new GuiNode("Panel", $"node-{depth}", children: new[] { node });
        return node;
    }

    private static GuiNode CreateFixture() =>
        GuiBuilder.Create("Panel", "root")
            .Child("Panel", "actions", panel => panel
                .Child("Button", "save", button => button.Text("Save"))
                .Child("Text", "status", text => text.Text("Ready")))
            .Build();
}
