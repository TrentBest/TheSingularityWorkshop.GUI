using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Blazor.Tests;

/// <summary>
/// Pins the narrow tree invariants the current Blazor renderer already promises.
/// This is not yet the shared cross-adapter conformance suite.
/// </summary>
public sealed class BlazorGuiRendererConformanceTests
{
    [Fact]
    public void Renderer_preserves_nested_identity_text_and_sibling_order()
    {
        var root = GuiBuilder.Create("Panel", "root")
            .Child("Button", "first", button => button.Text("First"))
            .Child("Text", "second", text => text.Text("Second"))
            .Build();

        var builder = new RenderTreeBuilder();
        BlazorGuiRenderer.Render(root)(builder);
        var frames = builder.GetFrames().Array;

        var elements = frames
            .Where(frame => frame.FrameType == RenderTreeFrameType.Element)
            .ToArray();

        Assert.Equal(new[] { "div", "button", "span" }, elements.Select(frame => frame.ElementName));
        Assert.Equal(new[] { 0, 0, 0 }, elements.Select(frame => frame.Sequence));
        Assert.Equal("root", FindAttribute(frames, "id", "root"));
        Assert.Equal("first", FindAttribute(frames, "id", "first"));
        Assert.Equal("second", FindAttribute(frames, "id", "second"));

        var text = frames
            .Where(frame => frame.FrameType == RenderTreeFrameType.Text)
            .Select(frame => frame.TextContent)
            .ToArray();
        Assert.Equal(new[] { "First", "Second" }, text);
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
                return expectedValue;
        }

        return null;
    }
}
