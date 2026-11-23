using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>A stable category identity and its human-readable label in a categorical chart.</summary>
public sealed class BarChartCategory
{
    /// <summary>Creates a category with a stable identifier and display label.</summary>
    /// <param name="id">A non-empty identifier unique within the chart.</param>
    /// <param name="label">The non-empty human-readable category label.</param>
    /// <exception cref="ArgumentException">Thrown when an identifier or label is null, empty, or whitespace.</exception>
    public BarChartCategory(string id, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Id = id;
        Label = label;
    }

    /// <summary>Gets the stable, chart-local category identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the human-readable category label.</summary>
    public string Label { get; }
}

/// <summary>
/// One named data series. Values align by index with the parent chart's category sequence;
/// null represents a missing observation, not zero.
/// </summary>
public sealed class BarChartSeries
{
    private readonly ReadOnlyCollection<double?> _values;

    /// <summary>Creates an immutable series snapshot.</summary>
    /// <param name="id">A non-empty identifier unique among the chart's series.</param>
    /// <param name="label">The non-empty human-readable series label.</param>
    /// <param name="values">Finite numeric observations or null entries for missing observations.</param>
    /// <exception cref="ArgumentException">Thrown when the identifier or label is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an observation is not finite.</exception>
    public BarChartSeries(string id, string label, IEnumerable<double?> values)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        ArgumentNullException.ThrowIfNull(values);

        var snapshot = values.ToList();
        for (var i = 0; i < snapshot.Count; i++)
        {
            if (snapshot[i] is double value && !double.IsFinite(value))
                throw new ArgumentOutOfRangeException(nameof(values), $"Value at index {i} must be finite or null.");
        }

        Id = id;
        Label = label;
        _values = new ReadOnlyCollection<double?>(snapshot);
    }

    /// <summary>Gets the stable, chart-local series identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the human-readable series label.</summary>
    public string Label { get; }

    /// <summary>
    /// Gets the immutable, positionally aligned observations. A null entry means missing data,
    /// not a numeric zero.
    /// </summary>
    public IReadOnlyList<double?> Values => _values;
}

/// <summary>
/// Renderer-neutral categorical bar-chart data. Categories and series are semantic data;
/// colors, typography, orientation, spacing, and other appearance belong to adapters.
/// </summary>
/// <remarks>
/// Every series value is aligned positionally with <see cref="Categories"/>. A null value
/// is a missing observation and must not be silently converted to zero. Empty category and
/// series collections are valid and describe an empty chart. This first contract describes
/// a simple bar chart only; grouping, stacking, and renderer capability negotiation are
/// deliberately not inferred.
/// </remarks>
public sealed class BarChartDefinition
{
    private readonly ReadOnlyCollection<BarChartCategory> _categories;
    private readonly ReadOnlyCollection<BarChartSeries> _series;

    /// <summary>Creates an immutable renderer-neutral categorical bar-chart definition.</summary>
    /// <param name="id">A non-empty chart identifier.</param>
    /// <param name="title">A non-empty human-readable chart title.</param>
    /// <param name="accessibleDescription">A non-empty description for assistive technologies and non-visual consumers.</param>
    /// <param name="categories">The ordered category sequence shared by every series.</param>
    /// <param name="series">The ordered data-series sequence; an empty sequence is valid.</param>
    /// <param name="categoryAxisLabel">An optional category-axis label.</param>
    /// <param name="valueAxisLabel">An optional numeric value-axis label.</param>
    /// <param name="valueUnit">An optional unit associated with numeric observations.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when required text is blank, a collection contains null entries, identifiers are duplicated,
    /// or a series does not contain exactly one value per category.
    /// </exception>
    /// <exception cref="ArgumentNullException">Thrown when either collection is null.</exception>
    public BarChartDefinition(
        string id,
        string title,
        string accessibleDescription,
        IEnumerable<BarChartCategory> categories,
        IEnumerable<BarChartSeries> series,
        string? categoryAxisLabel = null,
        string? valueAxisLabel = null,
        string? valueUnit = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(accessibleDescription);
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(series);

        var categorySnapshot = categories.ToList();
        var seriesSnapshot = series.ToList();

        if (categorySnapshot.Any(category => category is null))
            throw new ArgumentException("Chart categories cannot contain null entries.", nameof(categories));
        if (seriesSnapshot.Any(item => item is null))
            throw new ArgumentException("Chart series cannot contain null entries.", nameof(series));
        if (categorySnapshot.Select(category => category.Id).Distinct(StringComparer.Ordinal).Count() != categorySnapshot.Count)
            throw new ArgumentException("Chart category IDs must be unique.", nameof(categories));
        if (seriesSnapshot.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count() != seriesSnapshot.Count)
            throw new ArgumentException("Chart series IDs must be unique.", nameof(series));
        foreach (var item in seriesSnapshot)
        {
            if (item.Values.Count != categorySnapshot.Count)
                throw new ArgumentException(
                    $"Series '{item.Id}' has {item.Values.Count} values for {categorySnapshot.Count} categories.",
                    nameof(series));
        }

        Id = id;
        Title = title;
        AccessibleDescription = accessibleDescription;
        CategoryAxisLabel = categoryAxisLabel;
        ValueAxisLabel = valueAxisLabel;
        ValueUnit = valueUnit;
        _categories = new ReadOnlyCollection<BarChartCategory>(categorySnapshot);
        _series = new ReadOnlyCollection<BarChartSeries>(seriesSnapshot);
    }

    /// <summary>Gets the stable chart identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the human-readable chart title.</summary>
    public string Title { get; }

    /// <summary>Gets the chart's authored accessible description.</summary>
    public string AccessibleDescription { get; }

    /// <summary>Gets the optional category-axis label.</summary>
    public string? CategoryAxisLabel { get; }

    /// <summary>Gets the optional numeric value-axis label.</summary>
    public string? ValueAxisLabel { get; }

    /// <summary>Gets the optional unit associated with numeric values.</summary>
    public string? ValueUnit { get; }

    /// <summary>Gets the immutable ordered category sequence used by every series.</summary>
    public IReadOnlyList<BarChartCategory> Categories => _categories;

    /// <summary>Gets the immutable ordered data-series sequence.</summary>
    public IReadOnlyList<BarChartSeries> Series => _series;
}
