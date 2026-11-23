using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Identifies the semantic purpose of a document badge.</summary>
public enum BadgeRole
{
    /// <summary>A classification marker such as a level-of-detail badge.</summary>
    Classification,
    /// <summary>The identity of the represented artifact, such as a sourcebook cover.</summary>
    ArtifactIdentity,
    /// <summary>A lifecycle or readiness marker.</summary>
    Status,
    /// <summary>A creator, publisher, or source attribution marker.</summary>
    Provenance,
    /// <summary>A badge with another explicitly declared semantic purpose.</summary>
    Other
}

/// <summary>Identifies an image asset without assuming a filesystem or hosting platform.</summary>
public abstract record GuiAssetReference
{
    private GuiAssetReference() { }

    /// <summary>References an asset checked into the repository.</summary>
    public sealed record RepositoryAsset : GuiAssetReference
    {
        public RepositoryAsset(string path) => Path = GuiRepositoryPath.Validate(path);
        public string Path { get; }
    }

    /// <summary>References an explicitly supplied HTTPS asset URI; callers own version pinning and immutability.</summary>
    public sealed record ExternalAsset : GuiAssetReference
    {
        public ExternalAsset(Uri uri) => Uri = GuiRepositoryPath.ValidateHttps(uri, nameof(uri));
        public Uri Uri { get; }
    }

    /// <summary>Creates a repository-relative asset reference.</summary>
    public static GuiAssetReference Repository(string path) => new RepositoryAsset(path);
    /// <summary>Creates an explicit HTTPS asset reference.</summary>
    public static GuiAssetReference External(Uri uri) => new ExternalAsset(uri);
}

/// <summary>Identifies an optional destination associated with a badge.</summary>
public abstract record GuiLinkReference
{
    private GuiLinkReference() { }

    /// <summary>Links to another repository-relative document or resource.</summary>
    public sealed record RepositoryLink : GuiLinkReference
    {
        public RepositoryLink(string path) => Path = GuiRepositoryPath.Validate(path);
        public string Path { get; }
    }

    /// <summary>Links to an absolute HTTPS destination.</summary>
    public sealed record ExternalLink : GuiLinkReference
    {
        public ExternalLink(Uri uri) => Uri = GuiRepositoryPath.ValidateHttps(uri, nameof(uri));
        public Uri Uri { get; }
    }

    /// <summary>Creates a repository-relative destination reference.</summary>
    public static GuiLinkReference Repository(string path) => new RepositoryLink(path);
    /// <summary>Creates an absolute HTTPS destination reference.</summary>
    public static GuiLinkReference External(Uri uri) => new ExternalLink(uri);
}

/// <summary>
/// Describes one semantic badge. Its role, stable identity, accessible text, asset, and optional
/// destination remain distinct from any Markdown, HTML, SVG, or platform-specific rendering.
/// </summary>
public sealed class BadgeDefinition
{
    /// <summary>Creates a semantic badge with an explicit role, accessible text, and asset reference.</summary>
    public BadgeDefinition(
        string id,
        BadgeRole role,
        string label,
        string altText,
        GuiAssetReference asset,
        string? description = null,
        GuiLinkReference? destination = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentException.ThrowIfNullOrWhiteSpace(altText);
        ArgumentNullException.ThrowIfNull(asset);

        Id = id;
        Role = role;
        Label = label;
        AltText = altText;
        Asset = asset;
        Description = description;
        Destination = destination;
    }

    /// <summary>Gets the stable identity of this badge within its composition.</summary>
    public string Id { get; }
    /// <summary>Gets the semantic role of this badge.</summary>
    public BadgeRole Role { get; }
    /// <summary>Gets the concise text fallback when the visual asset is unavailable.</summary>
    public string Label { get; }
    /// <summary>Gets the accessible alternative text for the visual asset.</summary>
    public string AltText { get; }
    /// <summary>Gets the image asset reference.</summary>
    public GuiAssetReference Asset { get; }
    /// <summary>Gets the optional human-readable description or image title.</summary>
    public string? Description { get; }
    /// <summary>Gets the optional destination associated with this badge.</summary>
    public GuiLinkReference? Destination { get; }
}

/// <summary>An immutable, ordered composition of independent semantic badges.</summary>
public sealed class BadgeSet
{
    private readonly ReadOnlyCollection<BadgeDefinition> _badges;

    /// <summary>Creates an immutable badge composition, rejecting null entries and duplicate IDs.</summary>
    public BadgeSet(IEnumerable<BadgeDefinition> badges)
    {
        ArgumentNullException.ThrowIfNull(badges);
        var snapshot = badges.ToList();
        if (snapshot.Any(badge => badge is null))
            throw new ArgumentException("Badge sets cannot contain null entries.", nameof(badges));
        if (snapshot.Select(badge => badge.Id).Distinct(StringComparer.Ordinal).Count() != snapshot.Count)
            throw new ArgumentException("Badge IDs must be unique within a composition.", nameof(badges));

        _badges = new ReadOnlyCollection<BadgeDefinition>(snapshot);
    }

    /// <summary>Gets an empty badge composition.</summary>
    public static BadgeSet Empty { get; } = new(Array.Empty<BadgeDefinition>());

    /// <summary>Gets the badges in their declared presentation order.</summary>
    public IReadOnlyList<BadgeDefinition> Badges => _badges;

    /// <summary>Returns a new composition with the badge appended after existing badges.</summary>
    public BadgeSet WithBadge(BadgeDefinition badge)
    {
        ArgumentNullException.ThrowIfNull(badge);
        return new BadgeSet(_badges.Append(badge));
    }
}

internal static class GuiRepositoryPath
{
    public static string Validate(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (path.StartsWith("/", StringComparison.Ordinal)
            || path.Contains('\\')
            || path.Contains('?')
            || path.Contains('#'))
            throw new ArgumentException("Repository paths must be relative, slash-separated paths without query or fragment components.", nameof(path));

        var segments = path.Split('/');
        if (segments.Any(segment => string.IsNullOrWhiteSpace(segment) || segment is "." or ".."))
            throw new ArgumentException("Repository paths cannot contain empty, current-directory, or parent-directory segments.", nameof(path));

        if (Uri.TryCreate(path, UriKind.Absolute, out _))
            throw new ArgumentException("Use an external HTTPS reference for absolute URIs.", nameof(path));

        return string.Join("/", segments);
    }

    public static Uri ValidateHttps(Uri uri, string parameterName)
    {
        ArgumentNullException.ThrowIfNull(uri, parameterName);
        if (!uri.IsAbsoluteUri || !string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("External GUI resources must use an absolute HTTPS URI.", parameterName);
        return uri;
    }
}
