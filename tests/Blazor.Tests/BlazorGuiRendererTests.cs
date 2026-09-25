using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Blazor.Tests;

public sealed class BlazorGuiRendererTests
{
    [Fact]
    public void Renderer_materializes_recursive_core_tree()
    {
        var node = GuiBuilder.Create("Panel", "root")
            .Child("Button", "save", button => button
                .Text("Save"))
            .Build();

        var fragment = BlazorGuiRenderer.Render(node);
        var builder = new RenderTreeBuilder();

        fragment(builder);

        var frames = builder.GetFrames().Array;

        Assert.Equal("div", frames[0].ElementName);
        Assert.Equal("root", FindAttribute(frames, "id", "root"));
        Assert.Equal("button", frames[2].ElementName);
        Assert.Equal("save", FindAttribute(frames, "id", "save"));
    }

    [Fact]
    public void Renderer_maps_known_semantic_kinds_to_native_elements()
    {
        var node = GuiBuilder.Create("Button", "action")
            .Text("Run")
            .Build();

        var builder = new RenderTreeBuilder();
        BlazorGuiRenderer.Render(node)(builder);

        var frames = builder.GetFrames().Array;

        Assert.Equal("button", frames[0].ElementName);
        Assert.Equal("action", FindAttribute(frames, "id", "action"));
        Assert.Equal("Run", frames[2].TextContent);
    }

    [Fact]
    public void ElementBuilder_materializes_wheel_and_stop_propagation_attributes()
    {
        var builder = new RenderTreeBuilder();

        WorkshopGui.Element(this, "div")
            .OnWheel(_ => { })
            .StopPropagation("onclick")
            .Build()(builder);

        var frames = builder.GetFrames().Array;

        Assert.Contains(
            frames,
            frame => frame.FrameType == RenderTreeFrameType.Attribute &&
                     frame.AttributeName == "onwheel");

        Assert.Contains(
            frames,
            frame => frame.FrameType == RenderTreeFrameType.Attribute &&
                     frame.AttributeName == "onclick:stopPropagation" &&
                     Equals(frame.AttributeValue, true));
    }

    private static string? FindAttribute(
        RenderTreeFrame[] frames,
        string name,
        string expectedValue)
    {
        foreach (var frame in frames)
        {
            if (frame.FrameType == RenderTreeFrameType.Attribute &&
                frame.AttributeName == name &&
                Equals(frame.AttributeValue, expectedValue))
            {
                return expectedValue;
            }
        }

        return null;
    }
    [Fact]
    public void RendererManifestsSemanticStyleProperties()
    {
        var node = GuiBuilder
            .Create("Panel", "root")
            .Property("style:position", "fixed")
            .Property("style:inset", "0")
            .Build();

        var renderTree = new RenderTreeBuilder();
        BlazorGuiRenderer.Render(node)(renderTree);

        var frames = renderTree.GetFrames().Array;

        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "style" &&
            (string?)frame.AttributeValue == "position:fixed;inset:0");
    }

}
