using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class GuiControlDefinitionTests
{
    [Fact]
    public void Text_input_preserves_value_placeholder_validation_and_command_identity()
    {
        var definition = new TextInputDefinition(
            "Pilot", "bot.name.changed", "Enter a name", "Name is required");

        Assert.Equal("Pilot", definition.Value);
        Assert.Equal("Enter a name", definition.Placeholder);
        Assert.Equal("Name is required", definition.ValidationMessage);
        Assert.Equal("bot.name.changed", definition.ChangeCommandId);
        Assert.False(definition.IsReadOnly);
    }

    [Fact]
    public void Select_snapshots_options_and_preserves_stable_selected_identity()
    {
        var options = new List<GuiSelectOption>
        {
            new("random", "Random"),
            new("fixed", "Fixed")
        };
        var definition = new SelectDefinition(options, "fixed", "bot.race.mode.changed");
        options.Add(new GuiSelectOption("custom", "Custom"));

        Assert.Equal("fixed", definition.SelectedOptionId);
        Assert.Collection(definition.Options,
            option => Assert.Equal("random", option.Id),
            option => Assert.Equal("fixed", option.Id));
    }

    [Fact]
    public void Select_rejects_duplicate_or_unknown_option_ids()
    {
        Assert.Throws<ArgumentException>(() => new SelectDefinition(
            new[] { new GuiSelectOption("same", "One"), new GuiSelectOption("same", "Two") },
            "same", "change"));

        Assert.Throws<ArgumentException>(() => new SelectDefinition(
            new[] { new GuiSelectOption("one", "One") }, "missing", "change"));
    }

    [Fact]
    public void Toggle_preserves_boolean_value_and_host_command_identity()
    {
        var definition = new ToggleDefinition(true, "bot.enabled.changed");

        Assert.True(definition.IsChecked);
        Assert.Equal("bot.enabled.changed", definition.ChangeCommandId);
    }

    [Fact]
    public void Builder_and_node_preserve_typed_control_definition()
    {
        var definition = new SelectDefinition(
            new[] { new GuiSelectOption("observe", "Observe"), new GuiSelectOption("execute", "Execute") },
            "observe",
            "bot.mode.changed");

        var node = GuiBuilders.Select("mode", definition).Build();

        Assert.Equal("Select", node.Kind);
        Assert.Same(definition, node.Control);
    }

    [Fact]
    public void Node_rejects_a_control_definition_that_does_not_match_its_kind()
    {
        Assert.Throws<ArgumentException>(() => new GuiNode(
            "Button", "action", control: new ToggleDefinition(false, "toggle.changed")));
    }

    [Fact]
    public void Builder_clone_preserves_typed_control_definition()
    {
        var definition = new TextInputDefinition("Initial", "text.changed");
        var original = GuiBuilders.TextInput("name", definition).Build();
        var parent = GuiBuilders.Panel("root").Child(original).Build();

        Assert.Same(definition, parent.Children[0].Control);
    }
}
