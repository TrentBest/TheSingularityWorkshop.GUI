using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Represents an immutable, renderer-neutral card in a structured diagram.
/// Domain-specific meaning remains in the model that the card represents.
/// </summary>
public sealed class DataCard
{
    private readonly ReadOnlyCollection<DataCard> _children;

    /// <summary>Creates a card and snapshots its ordered child cards.</summary>
    /// <param name="id">Stable identity of this card within its diagram.</param>
    /// <param name="kind">Platform-neutral semantic kind, such as namespace or type.</param>
    /// <param name="title">Primary text displayed in the card header.</param>
    /// <param name="subtitle">Optional secondary header text, such as a signature.</param>
    /// <param name="isExpandedByDefault">Whether presentation should initially expand this card.</param>
    /// <param name="children">Ordered child cards contained by this card.</param>
    /// <param name="canRequestChildInsertion">Whether the presentation may offer an add-child affordance; a command handler must still validate every request.</param>
    public DataCard(
        string id,
        string kind,
        string title,
        string? subtitle = null,
        bool isExpandedByDefault = true,
        IEnumerable<DataCard>? children = null,
        bool canRequestChildInsertion = false)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Data card identity is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(kind))
            throw new ArgumentException("Data card kind is required.", nameof(kind));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Data card title is required.", nameof(title));

        Id = id;
        Kind = kind;
        Title = title;
        Subtitle = subtitle;
        IsExpandedByDefault = isExpandedByDefault;
        CanRequestChildInsertion = canRequestChildInsertion;
        _children = new ReadOnlyCollection<DataCard>(
            (children ?? Enumerable.Empty<DataCard>()).ToList());
    }

    /// <summary>Gets the stable identity used to preserve selection and presentation state.</summary>
    public string Id { get; }

    /// <summary>Gets the platform-neutral semantic kind of the represented item.</summary>
    public string Kind { get; }

    /// <summary>Gets the primary header text.</summary>
    public string Title { get; }

    /// <summary>Gets optional secondary header text, such as a declaration signature.</summary>
    public string? Subtitle { get; }

    /// <summary>Gets the initial expansion preference; current UI state belongs to the presentation.</summary>
    public bool IsExpandedByDefault { get; }

    /// <summary>Gets whether the presentation may offer an add-child affordance.</summary>
    /// <remarks>This is a UI capability hint, not authorization; the domain command handler must validate the request.</remarks>
    public bool CanRequestChildInsertion { get; }

    /// <summary>Gets the ordered, read-only snapshot of child cards.</summary>
    public IReadOnlyList<DataCard> Children => _children;

    /// <summary>Gets a summary for a currently collapsed card with children, or <see langword="null"/> when no summary is needed.</summary>
    /// <param name="isExpanded">Whether the card is currently expanded in the presentation.</param>
    /// <returns>A count derived from the current child snapshot, such as <c>Collapsed (4)</c>, or null.</returns>
    public string? GetCollapsedSummary(bool isExpanded)
    {
        if (isExpanded || Children.Count == 0)
            return null;

        return $"Collapsed ({Children.Count})";
    }
}
