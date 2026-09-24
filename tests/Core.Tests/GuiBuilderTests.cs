using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class GuiBuilderTests
{
    [Fact]
    public void Builder_creates_recursive_tree()
    {
        var root = GuiBuilder.Create("Panel", "root")
            .Text("Root")
            .Property("role", "container")
            .Child("Panel", "child", child => child
                .Text("Child")
                .Child("Text", "grandchild", grandchild => grandchild.Text("Hello")))
            .Build();

        Assert.Equal("Panel", root.Kind);
        Assert.Equal("Root", root.Text);
        Assert.Equal("container", root.Properties["role"]);
        Assert.Single(root.Children);
        Assert.Equal("Hello", root.Find("grandchild").Text);
    }

    [Fact]
    public void Built_node_is_snapshot_of_builder_state()
    {
        var builder = GuiBuilder.Create("Panel", "root").Text("Before");
        var first = builder.Build();

        builder.Text("After").Child("Text", "later");

        Assert.Equal("Before", first.Text);
        Assert.Empty(first.Children);
    }

    [Fact]
    public void Node_properties_are_read_only_snapshots()
    {
        var source = new Dictionary<string, string> { ["role"] = "button" };
        var node = new GuiNode("Button", "save", properties: source);
        source["role"] = "changed";

        Assert.Equal("button", node.Properties["role"]);
    }

    [Fact]
    public void Find_reports_missing_node_without_using_null_as_not_found()
    {
        var root = GuiBuilder.Create("Panel", "root").Build();

        var exception = Assert.Throws<InvalidOperationException>(() => root.Find("missing"));

        Assert.Contains("missing", exception.Message);
    }
}
