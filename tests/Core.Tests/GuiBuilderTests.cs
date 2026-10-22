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
    public void WithOnClick_records_a_stable_command_identity()
    {
        var node = GuiBuilders.Button("create-scene", "Create Scene")
            .WithOnClick("storybuilder.scene.create")
            .Build();

        Assert.Equal("storybuilder.scene.create", node.Properties["onClickCommand"]);
        Assert.Equal("Create Scene", node.Text);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void WithOnClick_rejects_blank_command_identity(string commandId)
    {
        var builder = GuiBuilders.Button("create-scene", "Create Scene");

        Assert.Throws<ArgumentException>(() => builder.WithOnClick(commandId));
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
    public void TryFind_returns_found_node_without_exceptions()
    {
        var root = GuiBuilder.Create("Panel", "root")
            .Child("Text", "message", child => child.Text("Hello"))
            .Build();

        var found = root.TryFind("message", out var node);

        Assert.True(found);
        Assert.NotNull(node);
        Assert.Equal("Hello", node!.Text);
    }

    [Fact]
    public void TryFind_returns_false_for_missing_node()
    {
        var root = GuiBuilder.Create("Panel", "root").Build();

        var found = root.TryFind("missing", out var node);

        Assert.False(found);
        Assert.Null(node);
    }

    [Fact]
    public void TryFind_handles_deep_trees_without_recursive_traversal()
    {
        GuiNode? root = null;
        const int depth = 4096;

        for (var i = depth - 1; i >= 0; i--)
            root = new GuiNode("Panel", $"node-{i}", children: root is null ? null : new[] { root });

        Assert.NotNull(root);
        Assert.True(root!.TryFind($"node-{depth - 1}", out var found));
        Assert.NotNull(found);
        Assert.Equal($"node-{depth - 1}", found!.Id);
        Assert.False(root.TryFind("missing", out _));
    }

    [Fact]
    public void Find_reports_missing_node()
    {
        var root = GuiBuilder.Create("Panel", "root").Build();

        var exception = Assert.Throws<InvalidOperationException>(() => root.Find("missing"));

        Assert.Contains("missing", exception.Message);
    }

    [Fact]
    public void Build_preserves_order_for_deep_callback_composition()
    {
        const int depth = 256;
        var root = GuiBuilder.Create("Panel", "root");

        void AddDepth(GuiBuilder parent, int level)
        {
            if (level == depth)
                return;

            parent.Child("Panel", $"child-{level}", child => AddDepth(child, level + 1));
        }

        AddDepth(root, 0);
        var built = root.Build();

        Assert.Equal($"child-{depth - 1}", built.Find($"child-{depth - 1}").Id);
    }

    [Fact]
    public void Direct_node_constructor_rejects_blank_kind_and_id()
    {
        Assert.Throws<ArgumentException>(() => new GuiNode("", "valid-id"));
        Assert.Throws<ArgumentException>(() => new GuiNode(" ", "valid-id"));
        Assert.Throws<ArgumentException>(() => new GuiNode("Panel", ""));
        Assert.Throws<ArgumentException>(() => new GuiNode("Panel", " "));
    }

    [Fact]
    public void Direct_node_constructor_snapshots_child_sequence()
    {
        var suppliedChildren = new List<GuiNode> { new("Text", "first") };
        var node = new GuiNode("Panel", "root", children: suppliedChildren);
        suppliedChildren.Add(new GuiNode("Text", "later"));

        Assert.Single(node.Children);
        Assert.Equal("first", node.Children[0].Id);
    }

    [Fact]
    public void Lookup_returns_first_duplicate_id_in_depth_first_left_to_right_order()
    {
        var left = new GuiNode("Panel", "branch-left", children: new[]
        {
            new GuiNode("Text", "duplicate", text: "left descendant")
        });
        var right = new GuiNode("Panel", "branch-right", children: new[]
        {
            new GuiNode("Text", "duplicate", text: "right descendant")
        });
        var root = new GuiNode("Panel", "root", children: new[] { left, right });

        Assert.Equal("left descendant", root.Find("duplicate").Text);
    }

    [Fact]
    public void Lookup_rejects_blank_identifiers()
    {
        var node = new GuiNode("Panel", "root");

        Assert.Throws<ArgumentException>(() => node.Find(""));
        Assert.Throws<ArgumentException>(() => node.Find(" "));
        Assert.Throws<ArgumentException>(() => node.TryFind("", out _));
        Assert.Throws<ArgumentException>(() => node.TryFind(" ", out _));
    }

}
