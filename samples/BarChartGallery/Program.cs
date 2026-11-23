using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

var output = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath("artifacts/bar-chart-gallery.svg");

var chart = new BarChartDefinition(
    "quarterly-revenue",
    "Quarterly revenue",
    "Revenue by quarter in thousands of US dollars. Q2 is intentionally missing; the gap is not zero.",
    new[]
    {
        new BarChartCategory("q1", "Q1"),
        new BarChartCategory("q2", "Q2"),
        new BarChartCategory("q3", "Q3"),
        new BarChartCategory("q4", "Q4")
    },
    new[]
    {
        new BarChartSeries("revenue", "Revenue", new double?[] { 42, null, 58, 73 })
    },
    "Quarter",
    "Revenue",
    "USD thousands");

var svg = new BarChartSvgRenderer().Render(chart, width: 1280, height: 720);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, svg);
Console.WriteLine($"Renderer-neutral bar chart generated: {output}");
