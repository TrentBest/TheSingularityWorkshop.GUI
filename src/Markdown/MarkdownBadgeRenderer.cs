using System;
using System.Collections.Generic;
using System.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Markdown;

/// <summary>A diagnostic produced while rendering a semantic badge composition.</summary>
public sealed record MarkdownBadgeDiagnostic
{
    /// <summary>Creates a diagnostic for a missing or invalid badge resource.</summary>
    public MarkdownBadgeDiagnostic(string code, string message, string resourcePath)
    {
        Code = code;
        Message = message;
        ResourcePath = resourcePath;
    }

    /// <summary>Gets the stable diagnostic code.</summary>
    public string Code { get; }
    /// <summary>Gets the human-readable diagnostic message.</summary>
    public string Message { get; }
    /// <summary>Gets the repository-relative resource path associated with the diagnostic.</summary>
    public string ResourcePath { get; }
}

/// <summary>The Markdown fragment and any asset diagnostics produced by a render operation.</summary>
public sealed class MarkdownBadgeRenderResult
{
    /// <summary>Creates a render result with its Markdown fragment and diagnostics.</summary>
    public MarkdownBadgeRenderResult(string markdown, IEnumerable<MarkdownBadgeDiagnostic> diagnostics)
    {
        Markdown = markdown ?? throw new ArgumentNullException(nameof(markdown));
        Diagnostics = Array.AsReadOnly((diagnostics ?? throw new ArgumentNullException(nameof(diagnostics))).ToArray());
    }

    public string Markdown { get; }
    public IReadOnlyList<MarkdownBadgeDiagnostic> Diagnostics { get; }
}

/// <summary>
/// Serializes semantic badges to Markdown and resolves repository paths relative to the target
/// Markdown document. It performs no filesystem access; callers may supply a repository path
/// inventory to receive missing-asset diagnostics.
/// </summary>
public sealed class MarkdownBadgeRenderer
{
    /// <summary>Renders the ordered badge set relative to a target Markdown document.</summary>
    /// <param name="badges">The semantic badges to render.</param>
    /// <param name="target">The repository-relative Markdown file receiving the fragment.</param>
    /// <param name="existingRepositoryPaths">Optional repository path inventory used to report missing image assets.</param>
    /// <returns>The Markdown fragment and any asset diagnostics.</returns>
    public MarkdownBadgeRenderResult Render(
        BadgeSet badges,
        MarkdownDocumentTarget target,
        IEnumerable<string>? existingRepositoryPaths = null)
    {
        ArgumentNullException.ThrowIfNull(badges);
        ArgumentNullException.ThrowIfNull(target);

        HashSet<string>? existingPaths = null;
        if (existingRepositoryPaths is not null)
        {
            existingPaths = new HashSet<string>(
                existingRepositoryPaths.Select(path =>
                    ((GuiAssetReference.RepositoryAsset)GuiAssetReference.Repository(path)).Path),
                StringComparer.Ordinal);
        }

        var diagnostics = new List<MarkdownBadgeDiagnostic>();
        var fragments = new List<string>(badges.Badges.Count);

        foreach (var badge in badges.Badges)
        {
            var assetPath = ResolveAsset(badge.Asset, target);
            if (assetPath.RepositoryPath is not null
                && existingPaths is not null
                && !existingPaths.Contains(assetPath.RepositoryPath))
            {
                diagnostics.Add(new MarkdownBadgeDiagnostic(
                    "MISSING_ASSET",
                    $"Badge '{badge.Id}' references a repository asset that is not present in the supplied path inventory.",
                    assetPath.RepositoryPath));
                fragments.Add(EscapeMarkdownText(badge.Label));
                continue;
            }

            var image = RenderImage(badge, assetPath.RenderedPath);
            if (badge.Destination is not null)
            {
                var destination = ResolveLink(badge.Destination, target);
                image = $"[{image}]({destination})";
            }

            fragments.Add(image);
        }

        return new MarkdownBadgeRenderResult(string.Join(" ", fragments), diagnostics);
    }

    private static (string RenderedPath, string? RepositoryPath) ResolveAsset(
        GuiAssetReference asset,
        MarkdownDocumentTarget target) =>
        asset switch
        {
            GuiAssetReference.RepositoryAsset repository =>
                (ResolveRelativePath(target.RepositoryRelativePath, repository.Path), repository.Path),
            GuiAssetReference.ExternalAsset external =>
                (external.Uri.AbsoluteUri, null),
            _ => throw new NotSupportedException($"Unsupported GUI asset reference type '{asset.GetType().FullName}'.")
        };

    private static string ResolveLink(GuiLinkReference link, MarkdownDocumentTarget target) =>
        link switch
        {
            GuiLinkReference.RepositoryLink repository =>
                ResolveRelativePath(target.RepositoryRelativePath, repository.Path),
            GuiLinkReference.ExternalLink external =>
                external.Uri.AbsoluteUri,
            _ => throw new NotSupportedException($"Unsupported GUI link reference type '{link.GetType().FullName}'.")
        };

    private static string RenderImage(BadgeDefinition badge, string assetPath)
    {
        var title = string.IsNullOrWhiteSpace(badge.Description)
            ? string.Empty
            : $" \"{EscapeMarkdownTitle(badge.Description)}\"";
        return $"![{EscapeMarkdownAlt(badge.AltText)}]({assetPath}{title})";
    }

    private static string ResolveRelativePath(string markdownPath, string repositoryPath)
    {
        var fromDirectory = markdownPath.Split('/').SkipLast(1).ToArray();
        var toSegments = repositoryPath.Split('/');
        var common = 0;

        while (common < fromDirectory.Length
            && common < toSegments.Length
            && string.Equals(fromDirectory[common], toSegments[common], StringComparison.Ordinal))
            common++;

        var relativeSegments = Enumerable.Repeat("..", fromDirectory.Length - common)
            .Concat(toSegments.Skip(common))
            .Select(segment => segment == ".." ? segment : Uri.EscapeDataString(segment));

        return string.Join("/", relativeSegments);
    }

    private static string EscapeMarkdownAlt(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("[", "\\[", StringComparison.Ordinal)
            .Replace("]", "\\]", StringComparison.Ordinal);

    private static string EscapeMarkdownTitle(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal);

    private static string EscapeMarkdownText(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("*", "\\*", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal)
            .Replace("[", "\\[", StringComparison.Ordinal)
            .Replace("]", "\\]", StringComparison.Ordinal)
            .Replace("`", "\\`", StringComparison.Ordinal);
}
