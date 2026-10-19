using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class DataCardTests
{
    [Fact]
    public void Card_preserves_semantics_and_child_order()
    {
        var first = new DataCard("member-a", "member", "Open", "Open()", isExpandedByDefault: false);
        var second = new DataCard("member-b", "member", "Close", "Close()");
        var card = new DataCard("type-main", "type", "Door", children: new[] { first, second });

        Assert.Equal("type-main", card.Id);
        Assert.Equal("type", card.Kind);
        Assert.Equal("Door", card.Title);
        Assert.True(card.IsExpandedByDefault);
        Assert.Collection(card.Children,
            child => Assert.Equal("member-a", child.Id),
            child => Assert.Equal("member-b", child.Id));
        Assert.Equal("Open()", card.Children[0].Subtitle);
        Assert.False(card.Children[0].IsExpandedByDefault);
    }

    [Theory]
    [InlineData("", "type", "Door", "id")]
    [InlineData(" ", "type", "Door", "id")]
    [InlineData("door", "", "Door", "kind")]
    [InlineData("door", " ", "Door", "kind")]
    [InlineData("door", "type", "", "title")]
    [InlineData("door", "type", " ", "title")]
    public void Card_rejects_missing_required_identity_or_labels(
        string id, string kind, string title, string expectedParameter)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new DataCard(id, kind, title));

        Assert.Equal(expectedParameter, exception.ParamName);
    }

    [Fact]
    public void Card_snapshots_the_child_sequence()
    {
        var children = new List<DataCard> { new("member", "member", "Open") };
        var card = new DataCard("type", "type", "Door", children: children);

        children.Add(new DataCard("other", "member", "Close"));

        Assert.Single(card.Children);
    }

    [Fact]
    public void Card_allows_a_leaf_without_children()
    {
        var card = new DataCard("empty", "namespace", "Example");

        Assert.Empty(card.Children);
        Assert.Null(card.Subtitle);
    }
}
