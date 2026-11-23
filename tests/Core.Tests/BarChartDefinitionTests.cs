using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class BarChartDefinitionTests
{
    [Fact]
    public void Bar_chart_preserves_category_and_series_semantics()
    {
        var chart = CreateChart();

        Assert.Equal("sales", chart.Id);
        Assert.Equal("Quarterly sales", chart.Title);
        Assert.Equal("Sales by quarter in thousands.", chart.AccessibleDescription);
        Assert.Equal("Quarter", chart.CategoryAxisLabel);
        Assert.Equal("Revenue", chart.ValueAxisLabel);
        Assert.Equal("USD thousands", chart.ValueUnit);
        Assert.Equal(new[] { "Q1", "Q2", "Q3" }, chart.Categories.Select(x => x.Id));
        Assert.Equal(new double?[] { 12, null, 18 }, chart.Series[0].Values);
    }

    [Fact]
    public void Empty_chart_is_valid_and_remains_descriptive()
    {
        var chart = new BarChartDefinition("empty", "No results", "No values are available.",
            Array.Empty<BarChartCategory>(), Array.Empty<BarChartSeries>());

        Assert.Empty(chart.Categories);
        Assert.Empty(chart.Series);
        Assert.Equal("No values are available.", chart.AccessibleDescription);
    }

    [Fact]
    public void Category_and_series_inputs_are_snapshotted()
    {
        var categories = new List<BarChartCategory> { new("q1", "Q1") };
        var series = new List<BarChartSeries> { new("revenue", "Revenue", new double?[] { 10 }) };
        var chart = new BarChartDefinition("sales", "Sales", "Sales chart.", categories, series);

        categories.Add(new("q2", "Q2"));
        series.Clear();

        Assert.Single(chart.Categories);
        Assert.Single(chart.Series);
    }

    [Fact]
    public void Series_values_are_snapshotted()
    {
        var values = new List<double?> { 1, 2 };
        var series = new BarChartSeries("s", "Series", values);
        values[0] = 99;

        Assert.Equal(new double?[] { 1, 2 }, series.Values);
    }

    [Fact]
    public void Missing_values_are_preserved_and_are_not_zero()
    {
        var chart = CreateChart();

        Assert.Null(chart.Series[0].Values[1]);
    }

    [Fact]
    public void Series_must_align_with_category_count()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new BarChartDefinition("id", "Title", "Description",
                new[] { new BarChartCategory("q1", "Q1"), new BarChartCategory("q2", "Q2") },
                new[] { new BarChartSeries("s", "Series", new double?[] { 1 }) }));

        Assert.Contains("values for 2 categories", exception.Message);
    }

    [Fact]
    public void Duplicate_category_and_series_ids_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => new BarChartDefinition("id", "Title", "Description",
            new[] { new BarChartCategory("same", "One"), new BarChartCategory("same", "Two") },
            Array.Empty<BarChartSeries>()));

        Assert.Throws<ArgumentException>(() => new BarChartDefinition("id", "Title", "Description",
            Array.Empty<BarChartCategory>(),
            new[] { new BarChartSeries("same", "One", Array.Empty<double?>()),
                    new BarChartSeries("same", "Two", Array.Empty<double?>()) }));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Non_finite_values_are_rejected(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new BarChartSeries("s", "Series", new double?[] { value }));
    }

    [Fact]
    public void Required_identity_and_accessibility_text_are_validated()
    {
        Assert.Throws<ArgumentException>(() => new BarChartDefinition("", "Title", "Description",
            Array.Empty<BarChartCategory>(), Array.Empty<BarChartSeries>()));
        Assert.Throws<ArgumentException>(() => new BarChartDefinition("id", " ", "Description",
            Array.Empty<BarChartCategory>(), Array.Empty<BarChartSeries>()));
        Assert.Throws<ArgumentException>(() => new BarChartDefinition("id", "Title", " ",
            Array.Empty<BarChartCategory>(), Array.Empty<BarChartSeries>()));
    }

    [Fact]
    public void Category_and_series_labels_must_be_present()
    {
        Assert.Throws<ArgumentException>(() => new BarChartCategory("id", " "));
        Assert.Throws<ArgumentException>(() => new BarChartSeries("id", "", Array.Empty<double?>()));
    }

    private static BarChartDefinition CreateChart() => new(
        "sales",
        "Quarterly sales",
        "Sales by quarter in thousands.",
        new[]
        {
            new BarChartCategory("Q1", "Quarter 1"),
            new BarChartCategory("Q2", "Quarter 2"),
            new BarChartCategory("Q3", "Quarter 3")
        },
        new[]
        {
            new BarChartSeries("revenue", "Revenue", new double?[] { 12, null, 18 })
        },
        "Quarter",
        "Revenue",
        "USD thousands");
}
