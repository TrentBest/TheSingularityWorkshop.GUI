using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheSingularityWorkshop.GUI.WPF;
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
    public void Native_builder_crtp_preserves_derived_fluent_type()
    {
        RunOnSta(() =>
        {
            var builder = new SpecializedButtonBuilder()
                .WithBackground(SystemColors.ControlBrush)
                .WithMargin(new Thickness(3))
                .WithCaption("Specialized");

            Assert.IsType<SpecializedButtonBuilder>(builder);
            Assert.Equal("Specialized", Assert.IsType<Button>(builder.Build()).Content);
        });
    }

    private sealed class SpecializedButtonBuilder : NativeGuiBuilder<Button, SpecializedButtonBuilder>
    {
        public SpecializedButtonBuilder WithCaption(string caption)
        {
            Configure(button => button.Content = caption);
            return this;
        }
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
            ICoreBuilder<FrameworkElement> commonContract = builder;
            Assert.Equal(builder.Id, commonContract.GetBuilderId());
        });
    }

    [Fact]
    public void Panel_builder_rejects_duplicate_component_registration()
    {
        RunOnSta(() =>
        {
            var child = new TestComponent();
            var parent = new StackGuiPanelBuilderWPF(new GuiConfiguration()).AddComponent(child);

            var exception = Assert.Throws<InvalidOperationException>(() => parent.AddComponent(child));

            Assert.Contains("already registered", exception.Message);
            Assert.Same(parent, child.GetParent());
            Assert.Single(Assert.IsType<StackPanel>(parent.Build()).Children);
        });
    }

    [Fact]
    public void Panel_builder_rejects_reparenting_a_component_to_a_different_visual_parent()
    {
        RunOnSta(() =>
        {
            var child = new TestComponent();
            var firstParent = new StackGuiPanelBuilderWPF(new GuiConfiguration()).AddComponent(child);
            var secondParent = new StackGuiPanelBuilderWPF(new GuiConfiguration());

            var exception = Assert.Throws<InvalidOperationException>(() => secondParent.AddComponent(child));

            Assert.Contains("cannot be reparented", exception.Message);
            Assert.Same(firstParent, child.GetParent());
            Assert.Single(Assert.IsType<StackPanel>(firstParent.Build()).Children);
            Assert.Empty(Assert.IsType<StackPanel>(secondParent.Build()).Children);
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
            var canvas = Assert.IsType<Canvas>(new CanvasGuiBuilderWPF(new GuiConfiguration())
                .PlaceAt(element, 42, 84)
                .Build());

            Assert.Single(canvas.Children);
            Assert.Same(element, canvas.Children[0]);
            Assert.Equal(42, Canvas.GetLeft(element));
            Assert.Equal(84, Canvas.GetTop(element));
        });
    }

    [Fact]
    public void Canvas_builder_rejects_duplicate_raw_element_placements()
    {
        RunOnSta(() =>
        {
            var element = new TextBlock { Text = "Only once" };
            var builder = new CanvasGuiBuilderWPF(new GuiConfiguration()).PlaceAt(element, 1, 2);

            var exception = Assert.Throws<InvalidOperationException>(() => builder.PlaceAt(element, 3, 4));

            Assert.Contains("already registered", exception.Message);
            Assert.Single(Assert.IsType<Canvas>(builder.Build()).Children);
        });
    }

    [Fact]
    public void Canvas_builder_rejects_elements_already_attached_to_another_parent()
    {
        RunOnSta(() =>
        {
            var element = new TextBlock { Text = "Owned" };
            var existingParent = new StackPanel();
            existingParent.Children.Add(element);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                new CanvasGuiBuilderWPF(new GuiConfiguration()).PlaceAt(element, 10, 20));

            Assert.Contains("already belongs", exception.Message);
            Assert.Same(existingParent, VisualTreeHelper.GetParent(element));
            Assert.Same(element, existingParent.Children[0]);
        });
    }

    [Fact]
    public void Canvas_builder_rejects_placing_itself_as_a_child()
    {
        RunOnSta(() =>
        {
            var builder = new CanvasGuiBuilderWPF(new GuiConfiguration());

            var exception = Assert.Throws<InvalidOperationException>(() => builder.PlaceAt(builder.Root, 0, 0));

            Assert.Contains("cannot place itself", exception.Message);
        });
    }

    [Fact]
    public void Dock_builder_rejects_duplicate_raw_element_registration()
    {
        RunOnSta(() =>
        {
            var element = new TextBlock { Text = "Docked once" };
            var builder = new DockGuiPanelBuilderWPF(new GuiConfiguration()).AddDocked(element, Dock.Left);

            var exception = Assert.Throws<InvalidOperationException>(() => builder.AddDocked(element, Dock.Right));

            Assert.Contains("already registered", exception.Message);
            Assert.Single(Assert.IsType<DockPanel>(builder.Build()).Children);
        });
    }

    [Fact]
    public void Dock_builder_rejects_elements_already_attached_to_another_parent()
    {
        RunOnSta(() =>
        {
            var element = new TextBlock { Text = "Owned" };
            var existingParent = new StackPanel();
            existingParent.Children.Add(element);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                new DockGuiPanelBuilderWPF(new GuiConfiguration()).AddDocked(element, Dock.Top));

            Assert.Contains("already belongs", exception.Message);
            Assert.Same(existingParent, VisualTreeHelper.GetParent(element));
        });
    }

    [Fact]
    public void Canvas_builder_rebuilds_raw_element_roster_without_duplicate_children()
    {
        RunOnSta(() =>
        {
            var element = new TextBlock { Text = "Stable canvas child" };
            var builder = new CanvasGuiBuilderWPF(new GuiConfiguration()).PlaceAt(element, 12, 24);

            var first = Assert.IsType<Canvas>(builder.Build());
            var second = Assert.IsType<Canvas>(builder.Build());

            Assert.Same(first, second);
            Assert.Single(second.Children);
            Assert.Same(element, second.Children[0]);
            Assert.Equal(12, Canvas.GetLeft(element));
            Assert.Equal(24, Canvas.GetTop(element));
        });
    }

    [Fact]
    public void Dock_builder_rebuilds_raw_element_roster_without_duplicate_children()
    {
        RunOnSta(() =>
        {
            var element = new TextBlock { Text = "Stable dock child" };
            var builder = new DockGuiPanelBuilderWPF(new GuiConfiguration()).AddDocked(element, Dock.Right);

            var first = Assert.IsType<DockPanel>(builder.Build());
            var second = Assert.IsType<DockPanel>(builder.Build());

            Assert.Same(first, second);
            Assert.Single(second.Children);
            Assert.Same(element, second.Children[0]);
            Assert.Equal(Dock.Right, DockPanel.GetDock(element));
        });
    }

    [Fact]
    public void Grid_builder_rebuilds_splitters_and_component_placements_without_duplicates()
    {
        RunOnSta(() =>
        {
            var child = new TestComponent();
            var builder = new GridGuiBuilderWPF("repeatable-layout")
                .AddColumnTrack(1, GridUnitType.Star, isResizable: true)
                .AddColumnTrack(2, GridUnitType.Star)
                .AddRowTrack(1, GridUnitType.Star, isResizable: true)
                .AddRowTrack(3, GridUnitType.Star)
                .PlaceComponent(1, 1, child);

            var first = Assert.IsType<Grid>(builder.Build());
            var firstSplitterCount = first.Children.OfType<GridSplitter>().Count();
            var firstChild = child.BuiltElement;
            var second = Assert.IsType<Grid>(builder.Build());

            Assert.Same(first, second);
            Assert.Equal(3, second.ColumnDefinitions.Count);
            Assert.Equal(3, second.RowDefinitions.Count);
            Assert.Equal(2, firstSplitterCount);
            Assert.Equal(2, second.Children.OfType<GridSplitter>().Count());
            Assert.Equal(3, second.Children.Count);
            Assert.Same(firstChild, child.BuiltElement);
            Assert.Contains(firstChild, second.Children.Cast<UIElement>());
            Assert.Equal(2, Grid.GetColumn(firstChild!));
            Assert.Equal(2, Grid.GetRow(firstChild!));
        });
    }

    [Fact]
    public void Grid_compatibility_proxy_does_not_expose_a_child_mutation_path()
    {
        var proxyType = typeof(GridGuiBuilderWPF).GetNestedType("CompatibilityElementProxy", BindingFlags.NonPublic);
        Assert.NotNull(proxyType);

        var childProperty = proxyType!.GetProperty("Child", BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(childProperty);
        Assert.False(childProperty!.CanWrite);
    }

    [Fact]
    public void Window_builder_composes_content_and_host_options_without_showing_the_window()
    {
        RunOnSta(() =>
        {
            var owner = new Window { Title = "Owner" };
            try
            {
                owner.Show();
                owner.Hide();
                var contentElement = new StackPanel();
                var background = new SolidColorBrush(Color.FromRgb(22, 33, 44));

                var window = new WpfWindowBuilder()
                    .WithTitle("Workshop")
                    .WithSize(900, 600)
                    .WithContent(contentElement)
                    .OwnedBy(owner)
                    .WithStartupLocation(WindowStartupLocation.CenterOwner)
                    .WithResizeMode(ResizeMode.CanMinimize)
                    .WithWindowStyle(WindowStyle.SingleBorderWindow)
                    .WithBackground(background)
                    .ShowInTaskbar(false)
                    .Build();

                try
                {
                    Assert.Equal("Workshop", window.Title);
                    Assert.Equal(900, window.Width);
                    Assert.Equal(600, window.Height);
                    Assert.Same(contentElement, window.Content);
                    Assert.Same(owner, window.Owner);
                    Assert.Equal(WindowStartupLocation.CenterOwner, window.WindowStartupLocation);
                    Assert.Equal(ResizeMode.CanMinimize, window.ResizeMode);
                    Assert.Equal(WindowStyle.SingleBorderWindow, window.WindowStyle);
                    Assert.Same(background, window.Background);
                    Assert.False(window.ShowInTaskbar);
                    Assert.False(window.IsVisible);
                }
                finally
                {
                    window.Close();
                }
            }
            finally
            {
                // Showing a WPF window creates a native HWND/subclass. Close it before
                // this test's STA thread exits so WPF cannot dispatch to a dead thread.
                owner.Close();
            }
        });
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(-1, 100)]
    [InlineData(double.NaN, 100)]
    [InlineData(double.PositiveInfinity, 100)]
    [InlineData(100, 0)]
    [InlineData(100, double.NegativeInfinity)]
    public void Window_builder_rejects_non_positive_or_non_finite_sizes(double width, double height)
    {
        RunOnSta(() => Assert.Throws<ArgumentOutOfRangeException>(
            () => new WpfWindowBuilder().WithSize(width, height)));
    }

    private sealed class TestComponent : IComponentWPF
    {
        private IComponentWPF? _parent;

        public string Id { get; } = Guid.NewGuid().ToString();
        public FrameworkElement? BuiltElement { get; private set; }
        public string GetBuilderId() => Id;

        public void SetParent(IComponentWPF? parent) => _parent = parent;
        public IComponentWPF? GetParent() => _parent;
        public void Publish(string topic, object payload) => _parent?.Publish(topic, payload);
        public void Receive(string topic, object payload) { }
        public FrameworkElement Build()
        {
            BuiltElement ??= new Border();
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
