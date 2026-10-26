using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.GUI.WPF;

namespace TheSingularityWorkshop.GUI.WPF.Tests;

public sealed class WpfGuiRendererConformanceTests
{
    [Fact]
    public void Renderer_preserves_supported_nested_tree_order_identity_and_text()
    {
        RunOnSta(() =>
        {
            var root = GuiBuilder.Create("Panel", "root")
                .Child("Button", "first", button => button.Text("First"))
                .Child("Text", "second", text => text.Text("Second"))
                .Build();

            var rendered = Assert.IsType<Border>(WpfGuiRenderer.Render(root));
            Assert.Equal("root", rendered.Name);
            Assert.Equal("root", WpfGuiIdentity.GetNodeId(rendered));
            var host = Assert.IsType<StackPanel>(rendered.Child);
            Assert.Equal(2, host.Children.Count);

            var button = Assert.IsType<Button>(host.Children[0]);
            Assert.Equal("first", button.Name);
            Assert.Equal("first", WpfGuiIdentity.GetNodeId(button));
            Assert.Equal("First", button.Content);

            var text = Assert.IsType<TextBlock>(host.Children[1]);
            Assert.Equal("second", text.Name);
            Assert.Equal("Second", text.Text);
        });
    }

    [Fact]
    public void Renderer_dispatches_declared_command_only_through_supplied_host_dispatcher()
    {
        RunOnSta(() =>
        {
            string? dispatched = null;
            var node = GuiBuilders.Button("create-scene", "Create Scene")
                .WithOnClick("storybuilder.scene.create")
                .Build();

            var button = Assert.IsType<Button>(WpfGuiRenderer.Render(node, commandId => dispatched = commandId));
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            Assert.Equal("storybuilder.scene.create", dispatched);
        });
    }

    [Fact]
    public void Renderer_rejects_null_command_dispatcher()
    {
        RunOnSta(() => Assert.Throws<ArgumentNullException>(() =>
            WpfGuiRenderer.Render(new GuiNode("Button", "action"), null!)));
    }

    [Fact]
    public void Renderer_preserves_arbitrary_semantic_ids_when_wpf_name_cannot_represent_them()
    {
        RunOnSta(() =>
        {
            const string id = "widget:save/primary";
            var rendered = WpfGuiRenderer.Render(new GuiNode("Button", id, text: "Save"));

            Assert.Equal(string.Empty, rendered.Name);
            Assert.Equal(id, WpfGuiIdentity.GetNodeId(rendered));
            Assert.Equal(id, rendered.GetValue(WpfGuiIdentity.NodeIdProperty));
        });
    }

    [Fact]
    public void Renderer_maps_supported_dimensions_spacing_and_colors()
    {
        RunOnSta(() =>
        {
            var node = new GuiNode(
                "Button",
                "save",
                text: "Save",
                properties: new Dictionary<string, string>
                {
                    ["width"] = "120",
                    ["height"] = "32",
                    ["margin"] = "1,2,3,4",
                    ["padding"] = "5",
                    ["background"] = "#FF0000",
                    ["foreground"] = "#00FF00",
                    ["fontSize"] = "18",
                    ["fontWeight"] = "700",
                    ["fontFamily"] = "Arial",
                    ["horizontalAlignment"] = "Center",
                    ["verticalAlignment"] = "Bottom",
                    ["tooltip"] = "Save changes"
                });

            var button = Assert.IsType<Button>(WpfGuiRenderer.Render(node));
            Assert.Equal(120, button.Width);
            Assert.Equal(32, button.Height);
            Assert.Equal(new Thickness(1, 2, 3, 4), button.Margin);
            Assert.Equal(new Thickness(5), button.Padding);
            Assert.Equal("Save changes", button.ToolTip);
            Assert.Equal(Color.FromRgb(255, 0, 0), Assert.IsType<SolidColorBrush>(button.Background).Color);
            Assert.Equal(Color.FromRgb(0, 255, 0), Assert.IsType<SolidColorBrush>(button.Foreground).Color);
            Assert.Equal(18, button.FontSize);
            Assert.Equal(FontWeights.Bold, button.FontWeight);
            Assert.Equal("Arial", button.FontFamily.Source);
            Assert.Equal(HorizontalAlignment.Center, button.HorizontalAlignment);
            Assert.Equal(VerticalAlignment.Bottom, button.VerticalAlignment);
        });
    }

    [Fact]
    public void Renderer_resolves_live_host_resources_for_background_and_foreground()
    {
        RunOnSta(() =>
        {
            var node = new GuiNode(
                "Button",
                "themed-action",
                text: "Run",
                properties: new Dictionary<string, string>
                {
                    ["background"] = "resource:ArchitectPanelBrush",
                    ["foreground"] = "resource:ArchitectAccentBrush"
                });
            var button = Assert.IsType<Button>(WpfGuiRenderer.Render(node));
            button.Resources["ArchitectPanelBrush"] = new SolidColorBrush(Color.FromRgb(0x12, 0x34, 0x56));
            button.Resources["ArchitectAccentBrush"] = new SolidColorBrush(Color.FromRgb(0x65, 0x43, 0x21));

            Assert.Equal(Color.FromRgb(0x12, 0x34, 0x56), Assert.IsType<SolidColorBrush>(button.Background).Color);
            Assert.Equal(Color.FromRgb(0x65, 0x43, 0x21), Assert.IsType<SolidColorBrush>(button.Foreground).Color);

            // A later host resource update must flow through the DynamicResource reference.
            button.Resources["ArchitectAccentBrush"] = new SolidColorBrush(Color.FromRgb(0xAA, 0xBB, 0xCC));
            Assert.Equal(Color.FromRgb(0xAA, 0xBB, 0xCC), Assert.IsType<SolidColorBrush>(button.Foreground).Color);
        });
    }

    [Fact]
    public void Renderer_allows_negative_margin_but_rejects_negative_padding()
    {
        RunOnSta(() =>
        {
            var button = Assert.IsType<Button>(WpfGuiRenderer.Render(new GuiNode(
                "Button",
                "spacing-contract",
                text: "Save",
                properties: new Dictionary<string, string>
                {
                    ["margin"] = "-1,2,-3,4",
                    ["padding"] = "-2"
                })));

            Assert.Equal(new Thickness(-1, 2, -3, 4), button.Margin);
            // WPF permits negative margins but rejects negative padding; invalid padding preserves the control default.
            Assert.Equal(new Thickness(1), button.Padding);
        });
    }

    [Fact]
    public void Renderer_uses_placeholder_for_unreadable_image_source()
    {
        RunOnSta(() =>
        {
            var rendered = Assert.IsType<TextBlock>(WpfGuiRenderer.Render(
                new GuiNode("Image", "missing-image", source: "file:///C:/__fsm_gui_missing_image_test__.png")));

            Assert.Contains("invalid image source", rendered.Text, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public void Renderer_ignores_invalid_font_family_property()
    {
        RunOnSta(() =>
        {
            var button = Assert.IsType<Button>(WpfGuiRenderer.Render(new GuiNode(
                "Button",
                "font-test",
                text: "Save",
                properties: new Dictionary<string, string>
                {
                    ["fontFamily"] = null!
                })));

            Assert.Equal(SystemFonts.MessageFontFamily.Source, button.FontFamily.Source);
        });
    }

    [Fact]
    public void Renderer_ignores_null_property_values_without_throwing()
    {
        RunOnSta(() =>
        {
            var button = Assert.IsType<Button>(WpfGuiRenderer.Render(new GuiNode(
                "Button",
                "null-properties",
                text: "Save",
                properties: new Dictionary<string, string>
                {
                    ["margin"] = null!,
                    ["padding"] = null!,
                    ["background"] = null!,
                    ["fontFamily"] = null!
                })));

            Assert.Equal(new Thickness(0), button.Margin);
            // WPF Button has a framework default padding of one unit on each side; null input must preserve it.
            Assert.Equal(new Thickness(1), button.Padding);
            Assert.Equal("Save", button.Content);
            Assert.Equal(SystemFonts.MessageFontFamily.Source, button.FontFamily.Source);
        });
    }

    [Fact]
    public void Renderer_preserves_button_label_and_renders_nested_button_children_in_order()
    {
        RunOnSta(() =>
        {
            var root = GuiBuilder.Create("Button", "action")
                .Text("Run")
                .Child("Text", "caption", text => text.Text("Details"))
                .Child("Button", "secondary", button => button.Text("More"))
                .Build();

            var rendered = Assert.IsType<Button>(WpfGuiRenderer.Render(root));
            var content = Assert.IsType<StackPanel>(rendered.Content);
            Assert.Equal(3, content.Children.Count);

            var label = Assert.IsType<TextBlock>(content.Children[0]);
            Assert.Equal("Run", label.Text);
            Assert.Equal("caption", WpfGuiIdentity.GetNodeId(content.Children[1]));
            Assert.Equal("Details", Assert.IsType<TextBlock>(content.Children[1]).Text);
            Assert.Equal("secondary", WpfGuiIdentity.GetNodeId(content.Children[2]));
            Assert.Equal("More", Assert.IsType<Button>(content.Children[2]).Content);
        });
    }

    [Fact]
    public void Renderer_rejects_null_root()
    {
        RunOnSta(() => Assert.Throws<ArgumentNullException>(() => WpfGuiRenderer.Render(null!)));
    }

    [Fact]
    public void Renderer_uses_fallback_panel_for_unknown_kind()
    {
        RunOnSta(() =>
        {
            var rendered = Assert.IsType<Border>(WpfGuiRenderer.Render(new GuiNode("FutureKind", "future")));
            Assert.IsType<StackPanel>(rendered.Child);
        });
    }

    [Fact]
    public void Renderer_rejects_duplicate_ids_before_materializing_controls()
    {
        RunOnSta(() =>
        {
            var root = new GuiNode("Panel", "root", children: new[]
            {
                new GuiNode("Text", "duplicate"),
                new GuiNode("Button", "duplicate")
            });

            var exception = Assert.Throws<ArgumentException>(() => WpfGuiRenderer.Render(root));
            Assert.Contains("duplicated", exception.Message);
        });
    }

    [Fact]
    public void Renderer_rejects_trees_deeper_than_256_edges()
    {
        RunOnSta(() =>
        {
            GuiNode node = new GuiNode("Text", "depth-257");
            for (var depth = 256; depth >= 0; depth--)
                node = new GuiNode("Panel", $"depth-{depth}", children: new[] { node });

            var exception = Assert.Throws<InvalidOperationException>(() => WpfGuiRenderer.Render(node));
            Assert.Contains("maximum depth (256)", exception.Message);
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
