using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

// Recipe 02: a sampled function plotted through the real GUI.SVG adapter.
// The mathematical samples are computed here; the adapter maps coordinate points
// into the visible Cartesian system. No hand-authored SVG path is used.
const double sampleMinimum = -2;
const double sampleMaximum = 2;
const double coordinateMinimum = -2;
const double coordinateMaximum = 4;
const int intervals = 32;
var samples = Enumerable.Range(0, intervals + 1)
    .Select(i =>
    {
        var x = sampleMinimum + (sampleMaximum - sampleMinimum) * i / intervals;
        return (X: x, Y: x * x);
    })
    .ToArray();

// The adapter maps coordinate-point nodes. SVG polyline geometry uses SVG's own
// viewBox coordinates, so derive its vertices from those same mathematical samples.
const double viewWidth = 100;
var viewHeight = 100d * 540 / 960;
var svgCurvePoints = string.Join(" ", samples.Select(sample =>
{
    var svgX = (sample.X - coordinateMinimum) / (coordinateMaximum - coordinateMinimum) * viewWidth;
    var svgY = viewHeight - (sample.Y - coordinateMinimum) /
        (coordinateMaximum - coordinateMinimum) * viewHeight;
    return $"{svgX.ToString("0.###", CultureInfo.InvariantCulture)},{svgY.ToString("0.###", CultureInfo.InvariantCulture)}";
}));

var scene = GuiBuilder.Create("coordinate-system", "cartesian-plane")
    .Text("Recipe 02: Cartesian coordinates and f(x) = x²")
    .Property("minimum", coordinateMinimum.ToString("R", CultureInfo.InvariantCulture))
    .Property("maximum", coordinateMaximum.ToString("R", CultureInfo.InvariantCulture))
    // The renderer otherwise places both axes at the midpoint of the configured
    // range. Explicit zero keeps the picture mathematically honest for [-2, 4].
    .Property("origin-x", "0")
    .Property("origin-y", "0")
    .Property("grid", "visible")
    .Property("axes", "visible")
    .Property("origin", "visible")
    .Property("bounds", "visible")
    .Property("aria-label", "Cartesian plane plotting f of x equals x squared")
    .Property("description", "The curve is represented by sampled coordinate points generated from f(x) = x squared.")
    .Child("svg-text", "plot-title", title => title
        .Text("f(x) = x²")
        .Property("x", "4")
        .Property("y", "7")
        .Property("font-size", "3.5")
        .Property("font-weight", "700")
        .Property("fill", "#eef6ff"))
    .Child("svg-text", "x-label", label => label
        .Text("x")
        .Property("x", "96")
        .Property("y", "51")
        .Property("font-size", "3")
        .Property("fill", "#eef6ff"))
    .Child("svg-text", "y-label", label => label
        .Text("y")
        .Property("x", "53")
        .Property("y", "5")
        .Property("font-size", "3")
        .Property("fill", "#eef6ff"))
    .Child("svg-polyline", "function-curve", curve => curve
        .Property("points", svgCurvePoints)
        .Property("fill", "none")
        .Property("stroke", "#7ee0bd")
        .Property("stroke-width", "0.55")
        .Property("stroke-linejoin", "round"));

for (var i = 0; i < samples.Length; i++)
{
    var (x, y) = samples[i];
    scene.Child("coordinate-point", $"sample-{i:D2}", point => point
        .Property("x", x.ToString("R", CultureInfo.InvariantCulture))
        .Property("y", y.ToString("R", CultureInfo.InvariantCulture))
        .Property("radius", i % 4 == 0 ? "0.9" : "0.55")
        .Property("fill", "#7ee0bd"));
}

var svg = new SvgGuiRenderer().Render(scene.Build(), width: 960, height: 540);
var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";

if (document.Root?.Name != ns + "svg")
    throw new InvalidOperationException("Adapter output does not have an SVG root.");
if (document.Descendants(ns + "circle").Count(e =>
        ((string?)e.Attribute("id"))?.StartsWith("sample-", StringComparison.Ordinal) == true) != intervals + 1)
    throw new InvalidOperationException("Generated SVG does not contain all expected function samples.");
if (!document.Descendants(ns + "path").Any(e => (string?)e.Attribute("id") == "cartesian-plane-axes"))
    throw new InvalidOperationException("Generated SVG is missing the Cartesian axes.");
if (!document.Descendants(ns + "polyline").Any(e => (string?)e.Attribute("id") == "function-curve"))
    throw new InvalidOperationException("Generated SVG is missing the sampled function curve.");

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(Environment.CurrentDirectory, "02-coordinate-functions.generated.svg");
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, svg);
Console.WriteLine($"PASS: verified Cartesian axes and {intervals + 1} samples of f(x) = x².");
Console.WriteLine($"Generated: {outputPath}");
