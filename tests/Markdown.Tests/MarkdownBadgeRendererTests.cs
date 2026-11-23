using System.Text.Json;
using Xunit;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Markdown;

namespace TheSingularityWorkshop.GUI.Markdown.Tests;

public sealed class MarkdownBadgeRendererTests
{
    private readonly MarkdownBadgeRenderer _renderer = new();

    [Fact]
    public void Surps_dungeons_book_descriptor_renders_lod_and_cover_roles_from_the_book_target()
    {
        var descriptorPath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "dungeons-and-underground.book.json");
        var descriptor = JsonSerializer.Deserialize<SurpsBookDescriptor>(File.ReadAllText(descriptorPath))!;

        var badges = BadgeSet.Empty
            .WithBadge(new BadgeDefinition(
                $"lod-{descriptor.Lod}",
                BadgeRole.Classification,
                $"LOD {descriptor.Lod}",
                $"LOD {descriptor.Lod} classification badge",
                GuiAssetReference.Repository(descriptor.LodBadge)))
            .WithBadge(new BadgeDefinition(
                descriptor.Id,
                BadgeRole.ArtifactIdentity,
                descriptor.Title,
                $"{descriptor.Title} sourcebook cover",
                GuiAssetReference.Repository(descriptor.Cover)));

        var result = _renderer.Render(
            badges,
            new MarkdownDocumentTarget("docs/books/dungeons-and-underground.md"),
            new[] { descriptor.LodBadge, descriptor.Cover });

        Assert.Equal(
            "![LOD 1 classification badge](../assets/rulebook-lod-1.svg) ![Dungeons and Underground sourcebook cover](covers/dungeons-and-underground.svg)",
            result.Markdown);
        Assert.Empty(result.Diagnostics);
        Assert.Equal(BadgeRole.Classification, badges.Badges[0].Role);
        Assert.Equal(BadgeRole.ArtifactIdentity, badges.Badges[1].Role);
    }

    [Fact]
    public void Nested_markdown_targets_resolve_each_asset_from_their_own_directory()
    {
        var badges = BadgeSet.Empty
            .WithBadge(new BadgeDefinition(
                "lod",
                BadgeRole.Classification,
                "LOD 1",
                "LOD 1",
                GuiAssetReference.Repository("docs/assets/lod-1.svg")))
            .WithBadge(new BadgeDefinition(
                "cover",
                BadgeRole.ArtifactIdentity,
                "Book cover",
                "Book cover",
                GuiAssetReference.Repository("docs/books/covers/book.svg")));

        var result = _renderer.Render(
            badges,
            new MarkdownDocumentTarget("docs/books/chapters/underground/strata.md"),
            new[] { "docs/assets/lod-1.svg", "docs/books/covers/book.svg" });

        Assert.Equal(
            "![LOD 1](../../../assets/lod-1.svg) ![Book cover](../../covers/book.svg)",
            result.Markdown);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void Root_readme_target_keeps_repository_root_relative_asset_paths()
    {
        var badge = new BadgeDefinition(
            "status",
            BadgeRole.Status,
            "Preview",
            "Development preview badge",
            GuiAssetReference.Repository("docs/assets/preview.svg"));

        var result = _renderer.Render(
            new BadgeSet(new[] { badge }),
            new MarkdownDocumentTarget("README.md"),
            new[] { "docs/assets/preview.svg" });

        Assert.Equal("![Development preview badge](docs/assets/preview.svg)", result.Markdown);
    }

    [Fact]
    public void Missing_repository_assets_produce_diagnostics_and_a_text_fallback()
    {
        var badge = new BadgeDefinition(
            "lod",
            BadgeRole.Classification,
            "LOD 1",
            "LOD 1 classification badge",
            GuiAssetReference.Repository("docs/assets/missing.svg"));

        var result = _renderer.Render(
            new BadgeSet(new[] { badge }),
            new MarkdownDocumentTarget("docs/books/book.md"),
            Array.Empty<string>());

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("MISSING_ASSET", diagnostic.Code);
        Assert.Equal("docs/assets/missing.svg", diagnostic.ResourcePath);
        Assert.Equal("LOD 1", result.Markdown);
    }

    [Fact]
    public void Optional_repository_destination_is_resolved_from_the_markdown_target()
    {
        var badge = new BadgeDefinition(
            "cover",
            BadgeRole.ArtifactIdentity,
            "Book",
            "Book cover",
            GuiAssetReference.Repository("docs/books/covers/book.svg"),
            destination: GuiLinkReference.Repository("docs/books/dungeons-and-underground.md"));

        var result = _renderer.Render(
            new BadgeSet(new[] { badge }),
            new MarkdownDocumentTarget("docs/books/README.md"),
            new[] { "docs/books/covers/book.svg" });

        Assert.Equal(
            "[![Book cover](covers/book.svg)](dungeons-and-underground.md)",
            result.Markdown);
    }

    [Fact]
    public void External_https_asset_and_destination_remain_absolute()
    {
        var badge = new BadgeDefinition(
            "external",
            BadgeRole.Provenance,
            "Publisher",
            "Publisher identity badge",
            GuiAssetReference.External(new Uri("https://example.com/publisher.svg")),
            destination: GuiLinkReference.External(new Uri("https://example.com/about")));

        var result = _renderer.Render(
            new BadgeSet(new[] { badge }),
            new MarkdownDocumentTarget("docs/books/book.md"));

        Assert.Equal(
            "[![Publisher identity badge](https://example.com/publisher.svg)](https://example.com/about)",
            result.Markdown);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void Markdown_document_target_rejects_non_markdown_and_parent_paths()
    {
        Assert.Throws<ArgumentException>(() => new MarkdownDocumentTarget("docs/books/book.txt"));
        Assert.Throws<ArgumentException>(() => new MarkdownDocumentTarget("docs/books/../secret.md"));
    }

    private sealed record SurpsBookDescriptor(
        int SchemaVersion,
        string Id,
        string Category,
        int Lod,
        string Title,
        string Version,
        string Cover,
        string LodBadge,
        string Publisher,
        string PublisherMoniker);
}
