using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class BadgeDefinitionTests
{
    [Fact]
    public void Badge_set_preserves_order_and_keeps_classification_separate_from_artifact_identity()
    {
        var lod = new BadgeDefinition(
            "lod-1",
            BadgeRole.Classification,
            "LOD 1",
            "LOD 1 classification badge",
            GuiAssetReference.Repository("docs/assets/rulebook-lod-1.svg"));
        var cover = new BadgeDefinition(
            "dungeons-cover",
            BadgeRole.ArtifactIdentity,
            "Dungeons and Underground",
            "Dungeons and Underground sourcebook cover",
            GuiAssetReference.Repository("docs/books/covers/dungeons-and-underground.svg"));

        var badges = BadgeSet.Empty.WithBadge(lod).WithBadge(cover);

        Assert.Collection(badges.Badges,
            badge =>
            {
                Assert.Equal(BadgeRole.Classification, badge.Role);
                Assert.Equal("lod-1", badge.Id);
            },
            badge =>
            {
                Assert.Equal(BadgeRole.ArtifactIdentity, badge.Role);
                Assert.Equal("dungeons-cover", badge.Id);
            });
    }

    [Fact]
    public void Badge_set_rejects_duplicate_stable_ids()
    {
        var first = new BadgeDefinition(
            "same", BadgeRole.Other, "First", "First badge", GuiAssetReference.Repository("assets/first.svg"));
        var second = new BadgeDefinition(
            "same", BadgeRole.Other, "Second", "Second badge", GuiAssetReference.Repository("assets/second.svg"));

        Assert.Throws<ArgumentException>(() => new BadgeSet(new[] { first, second }));
    }

    [Theory]
    [InlineData("../outside.svg")]
    [InlineData("/rooted.svg")]
    [InlineData("assets\\outside.svg")]
    [InlineData("assets//empty-segment.svg")]
    [InlineData("assets/./same-folder.svg")]
    [InlineData("assets/../outside.svg")]
    public void Repository_asset_paths_reject_ambiguous_or_escaping_paths(string path)
    {
        Assert.Throws<ArgumentException>(() => GuiAssetReference.Repository(path));
    }

    [Fact]
    public void External_assets_and_destinations_require_https()
    {
        var asset = GuiAssetReference.External(new Uri("https://example.com/badge.svg"));
        var destination = GuiLinkReference.External(new Uri("https://example.com/book"));

        Assert.IsType<GuiAssetReference.ExternalAsset>(asset);
        Assert.IsType<GuiLinkReference.ExternalLink>(destination);
        Assert.Throws<ArgumentException>(() => GuiAssetReference.External(new Uri("http://example.com/badge.svg")));
        Assert.Throws<ArgumentException>(() => GuiLinkReference.External(new Uri("http://example.com/book")));
    }

    [Fact]
    public void Badge_preserves_optional_description_and_destination()
    {
        var badge = new BadgeDefinition(
            "sourcebook",
            BadgeRole.ArtifactIdentity,
            "Dungeons and Underground",
            "Dungeons and Underground cover",
            GuiAssetReference.Repository("docs/books/covers/dungeons-and-underground.svg"),
            "SURPS Domain sourcebook",
            GuiLinkReference.Repository("docs/books/dungeons-and-underground.md"));

        Assert.Equal("SURPS Domain sourcebook", badge.Description);
        Assert.Equal("docs/books/dungeons-and-underground.md",
            Assert.IsType<GuiLinkReference.RepositoryLink>(badge.Destination).Path);
    }
}
