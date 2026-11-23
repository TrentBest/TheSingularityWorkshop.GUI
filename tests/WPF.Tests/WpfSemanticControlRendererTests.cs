using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.GUI.WPF;

namespace TheSingularityWorkshop.GUI.WPF.Tests;

public sealed class WpfSemanticControlRendererTests
{
    [Fact]
    public void Text_input_renders_initial_value_and_dispatches_changed_text()
    {
        RunOnSta(() =>
        {
            string? command = null;
            string? value = null;
            var node = GuiBuilders.TextInput(
                "pilot-name",
                new TextInputDefinition("Pilot", "bot.name.changed", "Enter a name", "Required")).Build();

            var control = Assert.IsType<TextBox>(WpfGuiRenderer.RenderWithValueDispatcher(node, (id, changed) =>
            {
                command = id;
                value = changed;
            }));

            Assert.Equal("Pilot", control.Text);
            Assert.Equal("Enter a name", control.ToolTip);
            Assert.Equal("Enter a name", control.Tag);
            Assert.Equal("Required", AutomationProperties.GetHelpText(control));
            control.Text = "Ranger";

            Assert.Equal("bot.name.changed", command);
            Assert.Equal("Ranger", value);
        });
    }

    [Fact]
    public void Select_renders_stable_option_ids_and_dispatches_selection()
    {
        RunOnSta(() =>
        {
            string? command = null;
            string? value = null;
            var definition = new SelectDefinition(
                new[] { new GuiSelectOption("random", "Random"), new GuiSelectOption("fixed", "Fixed") },
                "random",
                "bot.race.mode.changed");
            var node = GuiBuilders.Select("race-mode", definition).Build();

            var control = Assert.IsType<ComboBox>(WpfGuiRenderer.RenderWithValueDispatcher(node, (id, changed) =>
            {
                command = id;
                value = changed;
            }));

            Assert.Equal(2, control.Items.Count);
            Assert.Equal("random", Assert.IsType<ComboBoxItem>(control.SelectedItem).Tag);
            control.SelectedItem = control.Items.Cast<ComboBoxItem>().Single(item => Equals(item.Tag, "fixed"));

            Assert.Equal("bot.race.mode.changed", command);
            Assert.Equal("fixed", value);
        });
    }

    [Fact]
    public void Toggle_renders_initial_boolean_and_dispatches_changes()
    {
        RunOnSta(() =>
        {
            string? command = null;
            string? value = null;
            var node = GuiBuilders.Toggle(
                "enabled",
                new ToggleDefinition(false, "bot.enabled.changed")).Build();

            var control = Assert.IsType<CheckBox>(WpfGuiRenderer.RenderWithValueDispatcher(node, (id, changed) =>
            {
                command = id;
                value = changed;
            }));

            Assert.False(control.IsChecked);
            control.IsChecked = true;

            Assert.Equal("bot.enabled.changed", command);
            Assert.Equal(bool.TrueString, value);
        });
    }

    [Fact]
    public void Value_controls_are_non_mutating_when_no_host_dispatcher_is_supplied()
    {
        RunOnSta(() =>
        {
            var text = Assert.IsType<TextBox>(WpfGuiRenderer.Render(
                GuiBuilders.TextInput("name", new TextInputDefinition("Pilot", "name.changed")).Build()));
            var select = Assert.IsType<ComboBox>(WpfGuiRenderer.Render(
                GuiBuilders.Select("mode", new SelectDefinition(
                    new[] { new GuiSelectOption("a", "A") }, "a", "mode.changed")).Build()));
            var toggle = Assert.IsType<CheckBox>(WpfGuiRenderer.Render(
                GuiBuilders.Toggle("enabled", new ToggleDefinition(true, "enabled.changed")).Build()));

            Assert.True(text.IsReadOnly);
            Assert.False(select.IsEnabled);
            Assert.False(toggle.IsEnabled);
        });
    }

    private static void RunOnSta(Action action)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { action(); }
            catch (Exception exception) { failure = exception; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure is not null)
            throw new Xunit.Sdk.XunitException($"STA test action failed: {failure}");
    }
}
