using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Renders the supported single-series categorical bar-chart subset as accessible SVG.</summary>
/// <remarks>
/// This renderer intentionally rejects multiple series until the neutral model defines
/// grouped/stacked semantics. Missing observations are omitted rather than drawn as zero.
/// The SVG includes visible category/value labels and a non-interactive textual summary.
/// </remarks>
public sealed class BarChartSvgRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    /// <summary>Creates a standalone SVG manifestation of a categorical bar chart.</summary>
    public string Render(BarChartDefinition chart, int width = 960, int height = 540)
    {
        ArgumentNullException.ThrowIfNull(chart);
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
        if (chart.Series.Count > 1)
            throw new NotSupportedException("SVG bar charts currently support one series; grouped and stacked semantics are not defined.");

        var titleId = $"{chart.Id}-title";
        var descriptionId = $"{chart.Id}-description";
        var dataSummary = chart.Series.Count == 0 || chart.Categories.Count == 0
            ? " No data is available."
            : $" {chart.Series[0].Label}: " + string.Join("; ", chart.Categories.Select((category, index) =>
                $"{category.Label}: {(chart.Series[0].Values[index] is double value ? F(value) + UnitSuffix(chart.ValueUnit) : "missing")}."));
        var svg = new XElement(Svg + "svg",
            new XAttribute("id", chart.Id),
            new XAttribute("width", width),
            new XAttribute("height", height),
            new XAttribute("viewBox", "0 0 100 60"),
            new XAttribute("role", "img"),
            new XAttribute("aria-label", chart.Title),
            new XAttribute("aria-labelledby", $"{titleId} {descriptionId}"),
            new XElement(Svg + "title", new XAttribute("id", titleId), chart.Title),
            new XElement(Svg + "desc", new XAttribute("id", descriptionId), chart.AccessibleDescription + dataSummary));

        svg.Add(new XElement(Svg + "text",
            new XAttribute("x", "50"), new XAttribute("y", "5"),
            new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "3"),
            new XAttribute("font-weight", "700"), new XAttribute("fill", "#eef6ff"),
            chart.Title));

        if (chart.Categories.Count == 0 || chart.Series.Count == 0 ||
            chart.Series[0].Values.All(value => value is null))
        {
            svg.Add(new XElement(Svg + "text",
                new XAttribute("x", "50"), new XAttribute("y", "31"),
                new XAttribute("text-anchor", "middle"), new XAttribute("fill", "#b8c9d8"),
                "No data available"));
            return Document(svg);
        }

        var series = chart.Series[0];
        var values = series.Values.Where(value => value.HasValue).Select(value => value!.Value).ToArray();
        var minimum = Math.Min(0, values.Min());
        var maximum = Math.Max(0, values.Max());
        if (minimum == maximum)
        {
            minimum = -1;
            maximum = 1;
        }

        const double plotLeft = 12;
        const double plotRight = 96;
        const double plotTop = 10;
        const double plotBottom = 46;
        double MapY(double value) => plotBottom - (value - minimum) / (maximum - minimum) * (plotBottom - plotTop);
        var baseline = MapY(0);

        svg.Add(new XElement(Svg + "text",
            new XAttribute("x", "54"), new XAttribute("y", "8"),
            new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "2"),
            new XAttribute("fill", "#b8c9d8"), series.Label));

        for (var tick = 0; tick <= 4; tick++)
        {
            var y = plotTop + (plotBottom - plotTop) * tick / 4;
            svg.Add(new XElement(Svg + "line",
                new XAttribute("x1", F(plotLeft)), new XAttribute("x2", F(plotRight)),
                new XAttribute("y1", F(y)), new XAttribute("y2", F(y)),
                new XAttribute("stroke", "#294055"), new XAttribute("stroke-width", "0.15")));
        }

        svg.Add(new XElement(Svg + "line",
            new XAttribute("x1", F(plotLeft)), new XAttribute("x2", F(plotRight)),
            new XAttribute("y1", F(baseline)), new XAttribute("y2", F(baseline)),
            new XAttribute("stroke", "#b8c9d8"), new XAttribute("stroke-width", "0.3")));

        var band = (plotRight - plotLeft) / chart.Categories.Count;
        var barWidth = Math.Min(6, band * 0.58);
        for (var index = 0; index < chart.Categories.Count; index++)
        {
            var center = plotLeft + band * (index + 0.5);
            var category = chart.Categories[index];
            var value = series.Values[index];
            if (value.HasValue)
            {
                var valueY = MapY(value.Value);
                var barY = Math.Min(valueY, baseline);
                var barHeight = Math.Abs(baseline - valueY);
                svg.Add(new XElement(Svg + "rect",
                    new XAttribute("id", $"bar-{index}"),
                    new XAttribute("data-category-id", category.Id),
                    new XAttribute("data-value", F(value.Value)),
                    new XAttribute("x", F(center - barWidth / 2)),
                    new XAttribute("y", F(barY)),
                    new XAttribute("width", F(barWidth)),
                    new XAttribute("height", F(barHeight)),
                    new XAttribute("fill", "#7ee0bd"),
                    new XAttribute("aria-label", $"{category.Label}: {F(value.Value)}{UnitSuffix(chart.ValueUnit)}")));

                var labelY = value.Value >= 0 ? Math.Max(9, valueY - 1.2) : Math.Min(49, valueY + 2.8);
                svg.Add(new XElement(Svg + "text",
                    new XAttribute("x", F(center)), new XAttribute("y", F(labelY)),
                    new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "1.8"),
                    new XAttribute("fill", "#eef6ff"), F(value.Value)));
            }
            else
            {
                svg.Add(new XElement(Svg + "text",
                    new XAttribute("x", F(center)), new XAttribute("y", "49"),
                    new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "1.8"),
                    new XAttribute("fill", "#b8c9d8"), "—"));
            }

            svg.Add(new XElement(Svg + "text",
                new XAttribute("x", F(center)), new XAttribute("y", "53"),
                new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "2"),
                new XAttribute("fill", "#d6e7f4"), category.Label));
        }

        if (!string.IsNullOrWhiteSpace(chart.CategoryAxisLabel))
            svg.Add(new XElement(Svg + "text",
                new XAttribute("x", "54"), new XAttribute("y", "58"),
                new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "2"),
                new XAttribute("fill", "#b8c9d8"), chart.CategoryAxisLabel));

        if (!string.IsNullOrWhiteSpace(chart.ValueAxisLabel))
            svg.Add(new XElement(Svg + "text",
                new XAttribute("transform", "translate(4 29) rotate(-90)"),
                new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "2"),
                new XAttribute("fill", "#b8c9d8"),
                string.IsNullOrWhiteSpace(chart.ValueUnit) ? chart.ValueAxisLabel : $"{chart.ValueAxisLabel} ({chart.ValueUnit})"));

        return Document(svg);
    }

    private static string Document(XElement svg) =>
        new XDocument(new XDeclaration("1.0", "utf-8", null), svg).ToString(SaveOptions.DisableFormatting);

    private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private static string UnitSuffix(string? unit) => string.IsNullOrWhiteSpace(unit) ? string.Empty : $" {unit}";
}
