using System;
using System.IO;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Markdown;

/// <summary>Identifies the Markdown file that will receive a rendered badge fragment.</summary>
public sealed class MarkdownDocumentTarget
{
    public MarkdownDocumentTarget(string repositoryRelativePath)
    {
        var assetReference = GuiAssetReference.Repository(repositoryRelativePath);
        RepositoryRelativePath = ((GuiAssetReference.RepositoryAsset)assetReference).Path;

        var extension = Path.GetExtension(RepositoryRelativePath);
        if (!string.Equals(extension, ".md", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, ".markdown", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("The document target must have a .md or .markdown extension.", nameof(repositoryRelativePath));
    }

    /// <summary>Gets the normalized repository-relative path of the Markdown document.</summary>
    public string RepositoryRelativePath { get; }
}
