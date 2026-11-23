using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Blazor.Tests;

public sealed class BlazorSemanticControlRendererTests
{
    [Fact]
    public async Task Text_input_renders_value_and_dispatches_changed_text()
    {
        string? command = null;
        string? value = null;
        var node = GuiBuilders.TextInput(
            "pilot-name",
            new TextInputDefinition("Pilot", "bot.name.changed", "Enter a name")).Build();
        var builder = new RenderTreeBuilder();

        BlazorGuiRenderer.Render(node, (id, changed) =>
        {
            command = id;
            value = changed;
        })(builder);

        var frames = builder.GetFrames().Array;
        Assert.Equal("input", frames[0].ElementName);
        Assert.Equal("Pilot", FindAttribute(frames, "value"));
        Assert.Equal("Enter a name", FindAttribute(frames, "placeholder"));

        var callback = Assert.IsType<EventCallback<ChangeEventArgs>>(FindAttributeValue(frames, "onchange"));
        await callback.InvokeAsync(new ChangeEventArgs { Value = "Ranger" });

        Assert.Equal("bot.name.changed", command);
        Assert.Equal("Ranger", value);
    }

    [Fact]
    public async Task Select_renders_stable_options_and_dispatches_selected_option_id()
    {
        string? command = null;
        string? value = null;
        var node = GuiBuilders.Select("race-mode", new SelectDefinition(
            new[] { new GuiSelectOption("random", "Random"), new GuiSelectOption("fixed", "Fixed") },
            "fixed",
            "bot.race.mode.changed")).Build();
        var builder = new RenderTreeBuilder();

        BlazorGuiRenderer.Render(node, (id, changed) =>
        {
            command = id;
            value = changed;
        })(builder);

        var frames = builder.GetFrames().Array;
        Assert.Equal("select", frames[0].ElementName);
        Assert.Equal("fixed", FindAttribute(frames, "value"));
        Assert.Contains(frames, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == "selected" &&
            Equals(frame.AttributeValue, true));

        var callback = Assert.IsType<EventCallback<ChangeEventArgs>>(FindAttributeValue(frames, "onchange"));
        await callback.InvokeAsync(new ChangeEventArgs { Value = "random" });

        Assert.Equal("bot.race.mode.changed", command);
        Assert.Equal("random", value);
    }

    [Fact]
    public async Task Toggle_renders_boolean_state_and_dispatches_changed_value()
    {
        string? command = null;
        string? value = null;
        var node = GuiBuilders.Toggle("enabled", new ToggleDefinition(false, "bot.enabled.changed")).Build();
        var builder = new RenderTreeBuilder();

        BlazorGuiRenderer.Render(node, (id, changed) =>
        {
            command = id;
            value = changed;
        })(builder);

        var frames = builder.GetFrames().Array;
        Assert.Equal("input", frames[0].ElementName);
        Assert.Equal("checkbox", FindAttribute(frames, "type"));
        Assert.Equal(false, FindAttributeValue(frames, "checked"));

        var callback = Assert.IsType<EventCallback<ChangeEventArgs>>(FindAttributeValue(frames, "onchange"));
        await callback.InvokeAsync(new ChangeEventArgs { Value = true });

        Assert.Equal("bot.enabled.changed", command);
        Assert.Equal(bool.TrueString, value);
    }

    [Fact]
    public void Value_controls_are_read_only_when_no_host_dispatcher_is_supplied()
    {
        var node = GuiBuilders.TextInput("name", new TextInputDefinition("Pilot", "name.changed")).Build();
        var builder = new RenderTreeBuilder();

        BlazorGuiRenderer.Render(node)(builder);

        Assert.Equal(true, FindAttributeValue(builder.GetFrames().Array, "readonly"));
        Assert.DoesNotContain(builder.GetFrames().Array, frame =>
            frame.FrameType == RenderTreeFrameType.Attribute && frame.AttributeName == "onchange");
    }

    private static string? FindAttribute(RenderTreeFrame[] frames, string name)
    {
        var value = FindAttributeValue(frames, name);
        return value?.ToString();
    }

    private static object? FindAttributeValue(RenderTreeFrame[] frames, string name) =>
        frames.FirstOrDefault(frame =>
            frame.FrameType == RenderTreeFrameType.Attribute &&
            frame.AttributeName == name).AttributeValue;
}
