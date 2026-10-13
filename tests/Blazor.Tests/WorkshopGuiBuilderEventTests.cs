using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Gui;

namespace TheSingularityWorkshop.GUI.Blazor.Tests;

public sealed class WorkshopGuiBuilderEventTests
{
    [Fact]
    public void ElementBuilder_ProvidesWheelCallbackAndPropagationDirective()
    {
        var receiver = new object();
        var builder = WorkshopGui.Element(receiver, "div")
            .OnWheel(_ => { })
            .StopPropagation("onclick");

        var renderTree = new RenderTreeBuilder();
        builder.Build()(renderTree);
        var frames = renderTree.GetFrames().Array;

        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "onwheel" &&
            frame.AttributeValue is EventCallback<WheelEventArgs>);

        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "onclick:stopPropagation" &&
            Equals(frame.AttributeValue, true));
    }
}
