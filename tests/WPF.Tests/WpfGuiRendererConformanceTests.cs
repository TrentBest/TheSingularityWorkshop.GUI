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
                    ["tooltip"] = "Save changes"
                });

            var button = Assert.IsType<Button>(WpfGuiRenderer.Render(node));
            Assert.Equal(120, button.Width);
            Assert.Equal(32, button.Height);
            Assert.Equal(new Thickness(1, 2, 3, 4), button.Margin);
            Assert.Equal(new Thickness(5), button.Padding);
            Assert.Equal("Save changes", button.ToolTip);
            Assert.Equal(Color.FromRgb(255, 0, 0), Assert.IsType<SolidColorBrush>(button.Background).Color);
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
