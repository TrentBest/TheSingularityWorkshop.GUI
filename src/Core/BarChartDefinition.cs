using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>A stable category identity and its human-readable label in a categorical chart.</summary>
public sealed class BarChartCategory
{
    public BarChartCategory(string id, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Id = id;
        Label = label;
    }

    public string Id { get; }
    public string Label { get; }
}

/// <summary>
/// One named data series. Values align by index with the parent chart's category sequence;
/// null represents a missing observation, not zero.
/// </summary>
public sealed class BarChartSeries
{
    private readonly ReadOnlyCollection<double?> _values;

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

    public string Id { get; }
    public string Label { get; }
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

    public string Id { get; }
    public string Title { get; }
    public string AccessibleDescription { get; }
    public string? CategoryAxisLabel { get; }
    public string? ValueAxisLabel { get; }
    public string? ValueUnit { get; }
    public IReadOnlyList<BarChartCategory> Categories => _categories;
    public IReadOnlyList<BarChartSeries> Series => _series;
}
