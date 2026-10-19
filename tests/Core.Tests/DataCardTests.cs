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

    [Fact]
    public void Collapsed_summary_reports_the_current_direct_child_count()
    {
        var card = new DataCard(
            "private-properties",
            "member-group",
            "Private Properties",
            isExpandedByDefault: false,
            children: new[]
            {
                new DataCard("a", "property", "A"),
                new DataCard("b", "property", "B"),
                new DataCard("c", "property", "C"),
                new DataCard("d", "property", "D")
            });

        Assert.Equal("Collapsed (4)", card.GetCollapsedSummary(isExpanded: false));
        Assert.Null(card.GetCollapsedSummary(isExpanded: true));
    }

    [Fact]
    public void Empty_collapsed_region_can_offer_insertion_without_a_disclosure_summary()
    {
        var card = new DataCard(
            "private-properties",
            "member-group",
            "Private Properties",
            isExpandedByDefault: false,
            canRequestChildInsertion: true);

        Assert.Empty(card.Children);
        Assert.Null(card.GetCollapsedSummary(isExpanded: false));
        Assert.True(card.CanRequestChildInsertion);
    }

    [Fact]
    public void Insertion_affordance_is_not_enabled_by_default()
    {
        var card = new DataCard("namespace", "namespace", "Example");

        Assert.False(card.CanRequestChildInsertion);
    }

}
