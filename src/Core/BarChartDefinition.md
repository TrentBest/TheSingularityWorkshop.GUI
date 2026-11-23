# BarChartDefinition

**Author:** Trent Best

## Purpose

`BarChartDefinition` is the first renderer-neutral quantitative-data contract in GUI.Core. It describes a categorical bar chart without prescribing SVG geometry, DOM markup, WPF controls, colors, fonts, or layout.

## Data semantics

- A chart has a stable ID, title, required accessible description, ordered categories, and ordered series.
- Each category has a stable ID and a human-readable label.
- Each series has a stable ID, label, and one value per category, aligned by index.
- Values are finite numbers or `null`. A null is a missing observation; it is never an implicit zero.
- Empty category and series collections are valid and represent an empty chart.
- Duplicate category or series IDs, null entries, and mismatched series lengths are rejected.
- Axis labels and units are optional semantic descriptions, not styling instructions.

## Appearance and adapter boundary

Colors, typography, gridlines, bar orientation, grouping, stacking, spacing, animation, and interaction are not part of this first data contract. An adapter must not silently claim support for semantics it cannot preserve. This initial addition does not claim that SVG, Blazor, or WPF can render this chart yet; adapter capability evidence remains a separate step.

## Example

```csharp
using TheSingularityWorkshop.Workshop.Gui;

var chart = new BarChartDefinition(
    "quarterly-sales",
    "Quarterly sales",
    "Sales by quarter in thousands of dollars.",
    new[]
    {
        new BarChartCategory("q1", "Quarter 1"),
        new BarChartCategory("q2", "Quarter 2")
    },
    new[]
    {
        new BarChartSeries("revenue", "Revenue", new double?[] { 120, null })
    },
    categoryAxisLabel: "Quarter",
    valueAxisLabel: "Revenue",
    valueUnit: "USD thousands");
```

## Scope and next evidence

This is a simple categorical bar-chart data contract, not a general chart grammar. Grouped/stacked bars, line/time-series, pie/donut, scatter, ranges, gauges, aggregation, sorting, and normalization require explicit follow-up semantics. The Core contract has no renderer dependency and does not introduce a new package boundary.

Tests: `tests/Core.Tests/BarChartDefinitionTests.cs`.
