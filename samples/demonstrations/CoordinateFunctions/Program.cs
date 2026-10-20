using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

// Recipe 02: derive the formulas, sampled points, and curve geometry from one model.
// GUI.SVG manifests the semantic scene; it does not compute the mathematics itself.
const double sampleMinimum = -1.5;
const double sampleMaximum = 1.5;
const double coordinateMinimum = -2;
const double coordinateMaximum = 4;
const double verticalOffset = 0.5;
const double horizontalOffset = 0.5;
const double verticalScale = -0.5;
const int intervals = 32;
var samples = Enumerable.Range(0, intervals + 1)
    .Select(i =>
    {
        var x = sampleMinimum + (sampleMaximum - sampleMinimum) * i / intervals;
        var baseY = x * x;
        return new
        {
            X = x,
            Base = baseY,
            VerticalTranslation = baseY + verticalOffset,
            HorizontalTranslation = (x + horizontalOffset) * (x + horizontalOffset),
            Scaled = verticalScale * baseY
        };
    })
    .ToArray();

const double viewWidth = 100;
var viewHeight = 100d * 540 / 960;

string SvgPoints(IEnumerable<(double X, double Y)> points) =>
    string.Join(" ", points.Select(point =>
    {
        var svgX = (point.X - coordinateMinimum) / (coordinateMaximum - coordinateMinimum) * viewWidth;
        var svgY = viewHeight - (point.Y - coordinateMinimum) /
            (coordinateMaximum - coordinateMinimum) * viewHeight;
        return $"{svgX.ToString("0.###", CultureInfo.InvariantCulture)},{svgY.ToString("0.###", CultureInfo.InvariantCulture)}";
    }));

var scene = GuiBuilder.Create("coordinate-system", "cartesian-plane")
    .Text("Recipe 02: function transformations")
    .Property("minimum", coordinateMinimum.ToString("R", CultureInfo.InvariantCulture))
    .Property("maximum", coordinateMaximum.ToString("R", CultureInfo.InvariantCulture))
    .Property("origin-x", "0")
    .Property("origin-y", "0")
    .Property("grid", "visible")
    .Property("axes", "visible")
    .Property("origin", "visible")
    .Property("bounds", "visible")
    .Property("aria-label", "Cartesian plane comparing four transformations of f of x equals x squared")
    .Property("description", "The base curve, vertical translation, horizontal translation, and reflection are generated from one set of 33 x samples.")
    .Child("svg-text", "plot-title", title => title
        .Text("Function transformations")
        .Property("x", "4").Property("y", "6")
        .Property("font-size", "3.5").Property("font-weight", "700")
        .Property("fill", "#eef6ff"))
    .Child("svg-text", "formula-base", label => label
        .Text("f(x) = x²")
        .Property("x", "4").Property("y", "11")
        .Property("font-size", "2.7").Property("fill", "#7ee0bd"))
    .Child("svg-text", "formula-vertical", label => label
        .Text($"f(x) + {verticalOffset:0.##} = x² + {verticalOffset:0.##}")
        .Property("x", "4").Property("y", "15")
        .Property("font-size", "2.7").Property("fill", "#ffcf86"))
    .Child("svg-text", "formula-horizontal", label => label
        .Text($"f(x + {horizontalOffset:0.##}) = (x + {horizontalOffset:0.##})²")
        .Property("x", "4").Property("y", "19")
        .Property("font-size", "2.7").Property("fill", "#8bb8ff"))
    .Child("svg-text", "formula-scale", label => label
        .Text($"a f(x), a = {verticalScale:0.##} (reflection)")
        .Property("x", "4").Property("y", "23")
        .Property("font-size", "2.7").Property("fill", "#f29ac2"))
    .Child("svg-text", "x-label", label => label
        .Text("x")
        .Property("x", "96").Property("y", "51")
        .Property("font-size", "3").Property("fill", "#eef6ff"))
    .Child("svg-text", "y-label", label => label
        .Text("y")
        .Property("x", "53").Property("y", "29")
        .Property("font-size", "3").Property("fill", "#eef6ff"))
    .Child("svg-polyline", "curve-base", curve => curve
        .Property("points", SvgPoints(samples.Select(s => (s.X, s.Base))))
        .Property("fill", "none").Property("stroke", "#7ee0bd")
        .Property("stroke-width", "0.55").Property("stroke-linejoin", "round"))
    .Child("svg-polyline", "curve-vertical-translation", curve => curve
        .Property("points", SvgPoints(samples.Select(s => (s.X, s.VerticalTranslation))))
        .Property("fill", "none").Property("stroke", "#ffcf86")
        .Property("stroke-width", "0.55").Property("stroke-linejoin", "round"))
    .Child("svg-polyline", "curve-horizontal-translation", curve => curve
        .Property("points", SvgPoints(samples.Select(s => (s.X, s.HorizontalTranslation))))
        .Property("fill", "none").Property("stroke", "#8bb8ff")
        .Property("stroke-width", "0.55").Property("stroke-linejoin", "round"))
    .Child("svg-polyline", "curve-scaled-reflected", curve => curve
        .Property("points", SvgPoints(samples.Select(s => (s.X, s.Scaled))))
        .Property("fill", "none").Property("stroke", "#f29ac2")
        .Property("stroke-width", "0.55").Property("stroke-linejoin", "round"));

for (var i = 0; i < samples.Length; i++)
{
    var sample = samples[i];
    scene.Child("coordinate-point", $"sample-{i:D2}", point => point
        .Property("x", sample.X.ToString("R", CultureInfo.InvariantCulture))
        .Property("y", sample.Base.ToString("R", CultureInfo.InvariantCulture))
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
    throw new InvalidOperationException("Generated SVG does not contain all expected base-function samples.");

var axes = document.Descendants(ns + "path")
    .SingleOrDefault(e => (string?)e.Attribute("id") == "cartesian-plane-axes");
if (axes is null || (string?)axes.Attribute("d") != "M33.333 0V56.25M0 37.5H100")
    throw new InvalidOperationException("Cartesian axes do not cross at the configured mathematical origin (0, 0).");

var origin = document.Descendants(ns + "circle")
    .SingleOrDefault(e => (string?)e.Attribute("id") == "cartesian-plane-origin");
if ((string?)origin?.Attribute("cx") != "33.333" || (string?)origin?.Attribute("cy") != "37.5")
    throw new InvalidOperationException("Generated origin marker is not positioned at (0, 0).");

var expectedCurves = new[]
{
    "curve-base", "curve-vertical-translation",
    "curve-horizontal-translation", "curve-scaled-reflected"
};
foreach (var id in expectedCurves)
    if (!document.Descendants(ns + "polyline").Any(e => (string?)e.Attribute("id") == id))
        throw new InvalidOperationException($"Generated SVG is missing transformation curve '{id}'.");
if (document.Descendants(ns + "polyline").Count(e =>
        expectedCurves.Contains((string?)e.Attribute("id"), StringComparer.Ordinal)) != expectedCurves.Length)
    throw new InvalidOperationException("Generated SVG contains an unexpected transformation-curve count.");

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(Environment.CurrentDirectory, "02-coordinate-functions.generated.svg");
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, svg);
Console.WriteLine($"PASS: verified Cartesian origin, {intervals + 1} base samples, and {expectedCurves.Length} transformation curves.");
Console.WriteLine($"Parameters: vertical C={verticalOffset}, horizontal C={horizontalOffset}, scale a={verticalScale}.");
Console.WriteLine($"Generated: {outputPath}");
