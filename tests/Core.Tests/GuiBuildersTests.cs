using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class GuiBuildersTests
{
    [Fact]
    public void Factories_compose_a_nested_platform_neutral_tree()
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
    public void Child_accepts_existing_node_as_an_independent_snapshot()
    {
        var child = GuiBuilders.Text("child", "Hello").Build();
        var root = GuiBuilders.Panel("root").Child(child).Build();

        Assert.Equal("Hello", root.Find("child").Text);
        Assert.Single(root.Children);
    }

    [Fact]
    public void Build_handles_a_deep_builder_tree_without_recursive_materialization()
    {
        var root = GuiBuilders.Panel("node-0");
        var current = root;

        for (var i = 1; i <= 4096; i++)
        {
            var child = GuiBuilders.Panel($"node-{i}");
            current.Child(child);
            current = child;
        }

        var built = root.Build();

        Assert.Equal("node-4096", built.Find("node-4096").Id);
    }

    [Fact]
    public void Child_existing_node_clones_a_deep_tree_without_recursive_cloning()
    {
        GuiNode child = new("Text", "node-4096", "bottom");
        for (var i = 4095; i >= 0; i--)
            child = new GuiNode("Panel", $"node-{i}", children: new[] { child });

        var root = GuiBuilders.Panel("outer").Child(child).Build();

        Assert.Equal("bottom", root.Find("node-4096").Text);
    }

    [Fact]
    public void Build_rejects_a_cyclic_builder_graph_with_a_controlled_exception()
    {
        var root = GuiBuilders.Panel("root");
        var child = GuiBuilders.Panel("child");
        root.Child(child);
        child.Child(root);

        var error = Assert.Throws<InvalidOperationException>(() => root.Build());

        Assert.Contains("cannot contain itself", error.Message);
    }

    [Fact]
    public void Factory_and_direct_builder_share_the_same_fluent_type()
    {
        GuiBuilder fromFactory = GuiBuilders.Panel("factory");
        GuiBuilder direct = GuiBuilder.Create("Panel", "direct");

        Assert.Equal("factory", fromFactory.Build().Id);
        Assert.Equal("direct", direct.Build().Id);
    }
}
