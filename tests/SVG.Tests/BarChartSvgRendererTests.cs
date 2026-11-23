using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class BarChartSvgRendererTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private readonly BarChartSvgRenderer _renderer = new();

    [Fact]
    public void Renders_accessible_single_series_chart_with_axes_categories_and_values()
    {
        var document = XDocument.Parse(_renderer.Render(CreateChart()));
        var root = document.Root!;

        Assert.Equal("img", (string?)root.Attribute("role"));
        Assert.Equal("Quarterly sales", (string?)root.Attribute("aria-label"));
        Assert.Equal("sales-title sales-description", (string?)root.Attribute("aria-labelledby"));
        Assert.StartsWith("Sales by quarter in thousands.", root.Element(Svg + "desc")!.Value);
        Assert.Contains("Q2: missing", root.Element(Svg + "desc")!.Value);
        Assert.Contains("Q3: 18 USD thousands", root.Element(Svg + "desc")!.Value);
        Assert.Equal(new[] { "Quarter 1", "Quarter 2", "Quarter 3" },
            root.Descendants(Svg + "text").Where(x => x.Value.StartsWith("Quarter", StringComparison.Ordinal)).Select(x => x.Value).ToArray());
        Assert.Equal(new[] { "12", "18" }, root.Descendants(Svg + "rect").Select(x => (string?)x.Attribute("data-value")).ToArray());
        Assert.Contains("Revenue (USD thousands)", root.Value);
    }

    [Fact]
    public void Missing_values_are_not_rendered_as_zero_and_remain_visible_as_missing()
    {
        var document = XDocument.Parse(_renderer.Render(CreateChart()));
        var bars = document.Descendants(Svg + "rect").ToArray();

        Assert.Equal(2, bars.Length);
        Assert.Contains(document.Descendants(Svg + "text"), node => node.Value == "—");
        Assert.DoesNotContain(bars, node => (string?)node.Attribute("data-category-id") == "Q2");
    }

    [Fact]
    public void Negative_values_render_below_zero_baseline()
    {
        var chart = new BarChartDefinition("profit", "Profit", "Profit by month.",
            new[] { new BarChartCategory("jan", "January"), new BarChartCategory("feb", "February") },
            new[] { new BarChartSeries("net", "Net", new double?[] { -4, 6 }) },
            "Month", "Net", "USD");

        var document = XDocument.Parse(_renderer.Render(chart));
        var negativeBar = document.Descendants(Svg + "rect").Single(x => (string?)x.Attribute("data-value") == "-4");
        var baseline = document.Descendants(Svg + "line")
            .Single(line => (string?)line.Attribute("stroke") == "#b8c9d8");
        var baselineY = double.Parse((string)baseline.Attribute("y1")!, System.Globalization.CultureInfo.InvariantCulture);
        var barY = double.Parse((string)negativeBar.Attribute("y")!, System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(barY > baselineY);
    }

    [Fact]
    public void Empty_chart_and_all_missing_chart_show_an_explicit_empty_state()
    {
        var empty = new BarChartDefinition("empty", "Empty chart", "No data was supplied.",
            Array.Empty<BarChartCategory>(), Array.Empty<BarChartSeries>());
        var allMissing = new BarChartDefinition("missing", "Missing chart", "All observations are missing.",
            new[] { new BarChartCategory("a", "A") },
            new[] { new BarChartSeries("s", "Series", new double?[] { null }) });

        Assert.Contains("No data available", _renderer.Render(empty));
        Assert.Contains("No data available", _renderer.Render(allMissing));
    }

    [Fact]
    public void Multiple_series_are_rejected_until_grouping_semantics_are_defined()
    {
        var chart = new BarChartDefinition("multi", "Multiple", "Two series.",
            new[] { new BarChartCategory("a", "A") },
            new[]
            {
                new BarChartSeries("one", "One", new double?[] { 1 }),
                new BarChartSeries("two", "Two", new double?[] { 2 })
            });

        Assert.Throws<NotSupportedException>(() => _renderer.Render(chart));
    }

    [Fact]
    public void Invalid_dimensions_are_rejected()
    {
        var chart = CreateChart();
        Assert.Throws<ArgumentOutOfRangeException>(() => _renderer.Render(chart, width: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => _renderer.Render(chart, height: -1));
    }

    [Fact]
    public void Text_content_is_xml_escaped()
    {
        var chart = new BarChartDefinition("escaped", "R&D <sales>", "A & B.",
            new[] { new BarChartCategory("a", "A & B") },
            new[] { new BarChartSeries("s", "Net <value>", new double?[] { 2 }) });

        var document = XDocument.Parse(_renderer.Render(chart));
        Assert.Equal("R&D <sales>", document.Root!.Element(Svg + "title")!.Value);
        Assert.Contains("A & B", document.Root.Value);
    }

    private static BarChartDefinition CreateChart() => new(
        "sales", "Quarterly sales", "Sales by quarter in thousands.",
        new[]
        {
            new BarChartCategory("Q1", "Quarter 1"),
            new BarChartCategory("Q2", "Quarter 2"),
            new BarChartCategory("Q3", "Quarter 3")
        },
        new[] { new BarChartSeries("revenue", "Revenue", new double?[] { 12, null, 18 }) },
        "Quarter", "Revenue", "USD thousands");
}
