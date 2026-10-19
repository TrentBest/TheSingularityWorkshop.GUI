using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Controls;

namespace TheSingularityWorkshop.GUI.WPF.Tests;

public sealed class WpfValueControlBuilderTests
{
    private enum DeploymentMode
    {
        Observe,
        Simulate,
        Execute
    }

    [Fact]
    public void Numeric_builder_renders_initial_value_and_reports_user_value_changes()
    {
        RunOnSta(() =>
        {
            double? changedValue = null;
            var builder = new NumericControlBuilderWPF("Load", 25, 0, 100)
            {
                ValueChanged = value => changedValue = value
            };

            var row = Assert.IsType<Grid>(builder.Build());
            var label = Assert.IsType<TextBlock>(row.Children[0]);
            var slider = Assert.IsType<Slider>(row.Children[1]);
            var readout = Assert.IsType<TextBlock>(row.Children[2]);

            Assert.Equal("Load", label.Text);
            Assert.Equal(25, slider.Value);
            Assert.Equal("25", readout.Text);
            Assert.Null(changedValue);

            slider.Value = 75;

            Assert.Equal(75, builder.Value);
            Assert.Equal(75, changedValue);
            Assert.Equal("75", readout.Text);
        });
    }

    [Fact]
    public void Numeric_builder_rejects_invalid_ranges_and_initial_values()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NumericControlBuilderWPF("Load", 1, 10, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => new NumericControlBuilderWPF("Load", 101, 0, 100));
        Assert.Throws<ArgumentOutOfRangeException>(() => new NumericControlBuilderWPF("Load", double.NaN, 0, 100));
    }

    [Fact]
    public void Enum_builder_renders_declared_values_and_reports_selection_changes()
    {
        RunOnSta(() =>
        {
            object? changedValue = null;
            var builder = new EnumControlBuilderWPF(
                "Mode",
                typeof(DeploymentMode),
                DeploymentMode.Observe,
                value => changedValue = value);

            var row = Assert.IsType<DockPanel>(builder.Build());
            var label = Assert.IsType<TextBlock>(row.Children[0]);
            var selector = Assert.IsType<ComboBox>(row.Children[1]);

            Assert.Equal("Mode", label.Text);
            Assert.Equal(3, selector.Items.Count);
            Assert.Equal(DeploymentMode.Observe, selector.SelectedItem);
            Assert.Null(changedValue);

            selector.SelectedItem = DeploymentMode.Execute;

            Assert.Equal(DeploymentMode.Execute, builder.SelectedValue);
            Assert.Equal(DeploymentMode.Execute, changedValue);
        });
    }

    [Fact]
    public void Enum_builder_rejects_non_enum_types_and_values_of_another_type()
    {
        Assert.Throws<ArgumentException>(() =>
            new EnumControlBuilderWPF("Mode", typeof(string), "Observe"));
        Assert.Throws<ArgumentException>(() =>
            new EnumControlBuilderWPF("Mode", typeof(DeploymentMode), "Observe"));
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
