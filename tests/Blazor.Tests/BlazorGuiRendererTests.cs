using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
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
    public void Renderer_dispatches_button_command_only_through_supplied_host_dispatcher()
    {
        string? dispatched = null;
        var node = GuiBuilders.Button("create-scene", "Create Scene")
            .WithOnClick("storybuilder.scene.create")
            .Build();
        var builder = new RenderTreeBuilder();

        BlazorGuiRenderer.Render(node, commandId => dispatched = commandId)(builder);

        var clickFrame = builder.GetFrames().Array.Single(frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "onclick");
        var callback = Assert.IsType<Action<MouseEventArgs>>(clickFrame.AttributeValue);
        callback(new MouseEventArgs());

        Assert.Equal("storybuilder.scene.create", dispatched);
    }

    [Fact]
    public void Renderer_does_not_expose_host_command_as_dom_attribute_without_dispatcher()
    {
        var node = GuiBuilders.Button("create-scene", "Create Scene")
            .WithOnClick("storybuilder.scene.create")
            .Build();
        var builder = new RenderTreeBuilder();

        BlazorGuiRenderer.Render(node)(builder);

        Assert.DoesNotContain(builder.GetFrames().Array, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            (frame.AttributeName == "onClickCommand" || frame.AttributeName == "onclick"));
    }

    [Fact]
    public void Renderer_rejects_null_command_dispatcher()
    {
        var node = new GuiNode("Button", "action");
        Assert.Throws<ArgumentNullException>(() => BlazorGuiRenderer.Render(node, null!));
    }


    [Fact]
    public void Renderer_delegates_nested_viewport_to_host_owned_surface()
    {
        var viewport = new GuiNode(
            "Viewport",
            "scene",
            text: "Live scene",
            properties: new Dictionary<string, string> { ["class"] = "visualize-host" });
        var node = GuiBuilder.Create("Panel", "root")
            .Child(viewport)
            .Build();
        GuiNode? delegated = null;
        var builder = new RenderTreeBuilder();

        BlazorGuiRenderer.RenderWithHostedSurface(node, target =>
        {
            delegated = target;
            return surfaceBuilder => surfaceBuilder.AddContent(0, "host-owned Visualize surface");
        })(builder);

        var frames = builder.GetFrames().Array;
        Assert.Equal("scene", delegated?.Id);
        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Element && frame.ElementName == "section");
        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "aria-label" &&
            Equals(frame.AttributeValue, "Live scene"));
        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "data-gui-viewport" &&
            Equals(frame.AttributeValue, "true"));
        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "class" &&
            Equals(frame.AttributeValue, "visualize-host"));
        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Text &&
            frame.TextContent == "host-owned Visualize surface");
    }

    [Fact]
    public void Renderer_requires_host_surface_for_each_delegated_viewport()
    {
        var node = new GuiNode("Viewport", "scene");
        var fragment = BlazorGuiRenderer.RenderWithHostedSurface(node, _ => null);
        var builder = new RenderTreeBuilder();

        Assert.Throws<InvalidOperationException>(() => fragment(builder));
    }

    [Fact]
    public void Renderer_rejects_null_hosted_surface_renderer()
    {
        var node = new GuiNode("Viewport", "scene");

        Assert.Throws<ArgumentNullException>(() =>
            BlazorGuiRenderer.RenderWithHostedSurface(node, null!));
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
}
