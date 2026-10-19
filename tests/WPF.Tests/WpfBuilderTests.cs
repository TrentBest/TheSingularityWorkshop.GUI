using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheSingularityWorkshop.GUI.WPF.Abstractions;
using TheSingularityWorkshop.GUI.WPF.Layouts;

namespace TheSingularityWorkshop.GUI.WPF.Tests;

public sealed class WpfBuilderTests
{
    [Fact]
    public void Native_builder_applies_system_theme_defaults_to_controls()
    {
        RunOnSta(() =>
        {
            var button = Assert.IsType<Button>(new NativeGuiBuilder<Button>().Build());

            Assert.Same(SystemColors.ControlBrush, button.Background);
            Assert.Same(SystemColors.ControlTextBrush, button.Foreground);
            Assert.Same(SystemColors.ActiveBorderBrush, button.BorderBrush);
            Assert.Equal(new Thickness(1), button.BorderThickness);
            Assert.Equal(SystemFonts.MessageFontFamily.Source, button.FontFamily.Source);
        });
    }

    [Fact]
    public void Native_builder_fluent_configuration_returns_configured_element()
    {
        RunOnSta(() =>
        {
            var background = new SolidColorBrush(Color.FromRgb(12, 34, 56));
            var foreground = new SolidColorBrush(Color.FromRgb(210, 220, 230));
            var border = new SolidColorBrush(Color.FromRgb(90, 80, 70));
            var builder = new NativeGuiBuilder<Button>()
                .WithBackground(background)
                .WithForeground(foreground)
                .WithBorder(border, new Thickness(2, 3, 4, 5))
                .WithPadding(new Thickness(6))
                .WithMargin(new Thickness(7, 8, 9, 10))
                .WithSize(120, 32)
                .WithAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom)
                .Configure(button => button.Content = "Apply");

            var button = Assert.IsType<Button>(builder.Build());

            Assert.Equal("Apply", button.Content);
            Assert.Same(background, button.Background);
            Assert.Same(foreground, button.Foreground);
            Assert.Same(border, button.BorderBrush);
            Assert.Equal(new Thickness(2, 3, 4, 5), button.BorderThickness);
            Assert.Equal(new Thickness(6), button.Padding);
            Assert.Equal(new Thickness(7, 8, 9, 10), button.Margin);
            Assert.Equal(120, button.Width);
            Assert.Equal(32, button.Height);
            Assert.Equal(HorizontalAlignment.Center, button.HorizontalAlignment);
            Assert.Equal(VerticalAlignment.Bottom, button.VerticalAlignment);
        });
    }

    [Fact]
    public void Native_builder_build_is_stable_and_builder_ids_are_distinct()
    {
        RunOnSta(() =>
        {
            var first = new NativeGuiBuilder<TextBlock>();
            var second = new NativeGuiBuilder<TextBlock>();

            Assert.Same(first.Build(), first.Build());
            Assert.NotEqual(first.GetBuilderId(), second.GetBuilderId());
            Assert.False(string.IsNullOrWhiteSpace(first.GetBuilderId()));
        });
    }

    [Fact]
    public void Stack_builder_rebuilds_the_declared_roster_without_duplicate_children()
    {
        RunOnSta(() =>
        {
            var builder = new StackGuiPanelBuilderWPF(new GuiConfiguration())
                .WithOrientation(Orientation.Horizontal)
                .Add<Button>(button => button.Content = "First")
                .Add<TextBlock>(text => text.Text = "Second");

            var firstBuild = Assert.IsType<StackPanel>(builder.Build());
            Assert.Equal(Orientation.Horizontal, firstBuild.Orientation);
            Assert.Equal(2, firstBuild.Children.Count);
            Assert.Equal("First", Assert.IsType<Button>(firstBuild.Children[0]).Content);
            Assert.Equal("Second", Assert.IsType<TextBlock>(firstBuild.Children[1]).Text);

            var secondBuild = Assert.IsType<StackPanel>(builder.Build());
            Assert.Same(firstBuild, secondBuild);
            Assert.Equal(2, secondBuild.Children.Count);
            Assert.Equal("First", Assert.IsType<Button>(secondBuild.Children[0]).Content);
            Assert.Equal("Second", Assert.IsType<TextBlock>(secondBuild.Children[1]).Text);
        });
    }

    [Fact]
    public void Grid_builder_maps_logical_tracks_and_propagates_parent_to_placed_components()
    {
        RunOnSta(() =>
        {
            var child = new TestComponent();
            var builder = new GridGuiBuilderWPF("layout")
                .AddColumnTrack(1, GridUnitType.Star, isResizable: true)
                .AddColumnTrack(2, GridUnitType.Star)
                .AddRowTrack(1, GridUnitType.Star, isResizable: true)
                .AddRowTrack(3, GridUnitType.Star)
                .PlaceComponent(1, 1, child);

            var grid = Assert.IsType<Grid>(builder.Build());

            Assert.Equal(3, grid.ColumnDefinitions.Count);
            Assert.Equal(3, grid.RowDefinitions.Count);
            Assert.Equal(2, grid.Children.OfType<GridSplitter>().Count());
            Assert.Same(builder, child.GetParent());
            Assert.Equal(2, Grid.GetColumn(child.BuiltElement!));
            Assert.Equal(2, Grid.GetRow(child.BuiltElement!));
        });
    }

    [Fact]
    public void Canvas_builder_places_elements_at_requested_coordinates()
    {
        RunOnSta(() =>
        {
            var element = new TextBlock { Text = "At position" };
            var canvas = new CanvasGuiBuilderWPF(new GuiConfiguration())
                .PlaceAt(element, 42, 84)
                .Build();

            Assert.Single(canvas.Children);
            Assert.Same(element, canvas.Children[0]);
            Assert.Equal(42, Canvas.GetLeft(element));
            Assert.Equal(84, Canvas.GetTop(element));
        });
    }

    private sealed class TestComponent : IComponentWPF
    {
        private IComponentWPF? _parent;

        public string Id { get; } = Guid.NewGuid().ToString();
        public FrameworkElement? BuiltElement { get; private set; }

        public void SetParent(IComponentWPF? parent) => _parent = parent;
        public IComponentWPF? GetParent() => _parent;
        public void Publish(string topic, object payload) => _parent?.Publish(topic, payload);
        public void Receive(string topic, object payload) { }
        public FrameworkElement Build()
        {
            BuiltElement = new Border();
            return BuiltElement;
        }
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
