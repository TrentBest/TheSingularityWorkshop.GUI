using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class GuiBuildersTests
{
    [Fact]
    public void Factories_build_a_nested_platform_neutral_tree()
    {
        var root = GuiBuilders.Panel("root")
            .Property("background", "#020711")
            .Child(GuiBuilders.Column("content")
                .Property("padding", "28")
                .Child(GuiBuilders.Text("title", "THE FORGE")
                    .Property("fontSize", "64")))
            .Build();

        Assert.Equal("Panel", root.Kind);
        Assert.Equal("#020711", root.Properties["background"]);
        Assert.Equal("Column", root.Find("content").Kind);
        Assert.Equal("THE FORGE", root.Find("title").Text);
        Assert.Equal("64", root.Find("title").Properties["fontSize"]);
    }

    [Fact]
    public void Button_and_warning_factories_preserve_semantic_kind_and_text()
    {
        var button = GuiBuilders.Button("enter", "Enter").Build();
        var warning = GuiBuilders.Warning("problem", "Unavailable").Build();

        Assert.Equal("Button", button.Kind);
        Assert.Equal("Enter", button.Text);
        Assert.Equal("Warning", warning.Kind);
        Assert.Equal("Unavailable", warning.Text);
    }

    [Fact]
    public void Child_accepts_existing_node_without_sharing_mutable_builder_state()
    {
        var child = GuiBuilders.Text("child", "Hello").Build();
        var root = GuiBuilders.Panel("root").Child(child).Build();

        Assert.Equal("Hello", root.Find("child").Text);
        Assert.Single(root.Children);
    }
}
