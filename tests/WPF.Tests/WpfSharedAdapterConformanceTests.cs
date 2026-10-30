using System.Threading;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.GUI.WPF;
using Xunit;

namespace TheSingularityWorkshop.GUI.WPF.Tests;

/// <summary>
/// WPF-side executions of the named semantic regression cases also exercised
/// by the cross-platform Blazor/SVG conformance suite. These remain native
/// WPF tests so the shared project does not acquire a Windows dependency.
/// </summary>
public sealed class WpfSharedAdapterConformanceTests
{
    [Fact]
    public void Wpf_adapter_preserves_semantic_identity_when_properties_contain_id()
    {
        RunOnSta(() =>
        {
            var model = new GuiNode(
                "Panel",
                "semantic-id",
                properties: new Dictionary<string, string>
                {
                    ["id"] = "property-id",
                    ["ID"] = "case-variant-property-id"
                });

            var rendered = WpfGuiRenderer.Render(model);

            Assert.Equal("semantic-id", WpfGuiIdentity.GetNodeId(rendered));
        });
    }

    [Fact]
    public void Wpf_adapter_rejects_the_shared_duplicate_id_fixture()
    {
        RunOnSta(() =>
        {
            var duplicateIds = GuiBuilder.Create("Panel", "root")
                .Child("Text", "duplicate")
                .Child("Button", "duplicate")
                .Build();

            var exception = Assert.Throws<ArgumentException>(
                () => WpfGuiRenderer.Render(duplicateIds));

            Assert.Contains("duplicated", exception.Message);
            Assert.Contains("duplicate", exception.Message);
        });
    }

    [Fact]
    public void Wpf_adapter_rejects_the_shared_depth_limit_fixture()
    {
        RunOnSta(() =>
        {
            GuiNode tooDeep = new("Panel", "node-0");
            for (var depth = 1; depth <= 257; depth++)
                tooDeep = new GuiNode("Panel", $"node-{depth}", children: new[] { tooDeep });

            var exception = Assert.Throws<InvalidOperationException>(
                () => WpfGuiRenderer.Render(tooDeep));

            Assert.Contains("maximum depth (256)", exception.Message);
        });
    }


    [Fact]
    public void Wpf_adapter_preserves_unknown_kind_identity_and_descendants_using_documented_fallback()
    {
        RunOnSta(() =>
        {
            var model = GuiBuilder.Create("Panel", "root")
                .Child("FutureKind", "future", future => future
                    .Child("Text", "caption", text => text.Text("Still present")))
                .Build();

            var rendered = Assert.IsType<Border>(WpfGuiRenderer.Render(model));
            var rootHost = Assert.IsType<StackPanel>(rendered.Child);
            var fallback = Assert.IsType<Border>(rootHost.Children[0]);
            Assert.Equal("future", WpfGuiIdentity.GetNodeId(fallback));

            var fallbackHost = Assert.IsType<StackPanel>(fallback.Child);
            var caption = Assert.IsType<TextBlock>(fallbackHost.Children[0]);
            Assert.Equal("caption", WpfGuiIdentity.GetNodeId(caption));
            Assert.Equal("Still present", caption.Text);
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
