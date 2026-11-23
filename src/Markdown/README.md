# TheSingularityWorkshop.GUI.Markdown

Markdown serialization for the renderer-neutral semantic GUI badge contracts in GUI.Core.

## Purpose

- Render an ordered `BadgeSet` as a Markdown-compatible inline fragment.
- Preserve semantic badge roles, stable identities, accessible alt text, optional descriptions, and optional destinations.
- Resolve repository asset/link paths relative to the target Markdown document, wherever that document lives in the repository.
- Report missing repository assets when the host supplies a repository path inventory.

## Minimal usage

```csharp
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Markdown;

var badges = BadgeSet.Empty
    .WithBadge(new BadgeDefinition(
        "lod-1",
        BadgeRole.Classification,
        "LOD 1",
        "LOD 1 classification badge",
        GuiAssetReference.Repository("docs/assets/rulebook-lod-1.svg")))
    .WithBadge(new BadgeDefinition(
        "dungeons-cover",
        BadgeRole.ArtifactIdentity,
        "Dungeons and Underground",
        "Dungeons and Underground sourcebook cover",
        GuiAssetReference.Repository("docs/books/covers/dungeons-and-underground.svg")));

var renderer = new MarkdownBadgeRenderer();
var result = renderer.Render(
    badges,
    new MarkdownDocumentTarget("docs/books/dungeons-and-underground.md"),
    existingRepositoryPaths);
```

The renderer resolves the LOD asset as `../assets/rulebook-lod-1.svg` and the cover as `covers/dungeons-and-underground.svg` for that target document.

## Ownership

GUI.Core owns the semantic badge contract. GUI.Markdown owns Markdown serialization and target-relative path resolution. The consuming domain owns descriptor schemas, policy, and content validation. The consuming repository's workflow owns publication and deployment.

This package does not read the filesystem, fetch external resources, interpret SURPS descriptors, or publish documents.
