using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: SeededPointSet2d <physics-run-record.json> <output.svg>");
    return 2;
}

var inputPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
using var json = JsonDocument.Parse(await File.ReadAllTextAsync(inputPath));
var root = json.RootElement;

if (root.GetProperty("modelId").GetString() != "procedural.seeded-point-set-2d" ||
    root.GetProperty("modelVersion").GetString() != "1" ||
    root.GetProperty("schemaVersion").GetInt32() != 1)
    throw new InvalidDataException("Unsupported Physics point-set identity or schema.");

var seed = root.GetProperty("seed").GetUInt32();
var bounds = root.GetProperty("bounds");
var minX = bounds.GetProperty("minX").GetDouble();
var minY = bounds.GetProperty("minY").GetDouble();
var maxX = bounds.GetProperty("maxX").GetDouble();
var maxY = bounds.GetProperty("maxY").GetDouble();
if (bounds.GetProperty("convention").GetString() != "minimum-inclusive, maximum-exclusive" ||
    !double.IsFinite(minX) || !double.IsFinite(minY) ||
    !double.IsFinite(maxX) || !double.IsFinite(maxY) ||
    maxX <= minX || maxY <= minY ||
    !double.IsFinite(maxX - minX) || !double.IsFinite(maxY - minY))
    throw new InvalidDataException("Invalid or unsupported point-set bounds contract.");

var pointCount = root.GetProperty("pointCount").GetInt32();
if (pointCount < 0 || pointCount > 65_536)
    throw new InvalidDataException("Point count is outside the supported range.");

var points = root.GetProperty("points").EnumerateArray().Select((point, position) =>
{
    var index = point.GetProperty("index").GetInt32();
    var x = point.GetProperty("x").GetDouble();
    var y = point.GetProperty("y").GetDouble();
    if (index != position)
        throw new InvalidDataException($"Point index {index} is not in stable position {position}.");
    if (!double.IsFinite(x) || !double.IsFinite(y) ||
        x < minX || x >= maxX || y < minY || y >= maxY)
        throw new InvalidDataException($"Point {index} has non-finite or out-of-bounds coordinates.");
    return new Point(index, x, y);
}).ToArray();

if (points.Length != pointCount)
    throw new InvalidDataException($"Run record declares {pointCount} points but contains {points.Length}.");

const double plotLeft = 8;
const double plotRight = 92;
const double plotTop = 10;
const double plotBottom = 45;
double MapX(double x) => plotLeft + (x - minX) / (maxX - minX) * (plotRight - plotLeft);
double MapY(double y) => plotBottom - (y - minY) / (maxY - minY) * (plotBottom - plotTop);
string F(double value) => value.ToString("0.######", CultureInfo.InvariantCulture);

var scene = GuiBuilder.Create("svg-scene", "physics-seeded-point-set")
    .Text("Physics seeded 2D point set")
    .Property("aria-label", "GUI.SVG rendering of the recorded Physics seeded point set")
    .Property("description", $"Physics model procedural.seeded-point-set-2d v1; seed {seed}; {points.Length} recorded points; half-open bounds [{F(minX)}, {F(maxX)}) × [{F(minY)}, {F(maxY)}).")
    .Child("svg-rect", "plot-background", rect => rect
        .Property("x", "5").Property("y", "7").Property("width", "90").Property("height", "41")
        .Property("rx", "1").Property("fill", "#0b1725").Property("stroke", "#294253").Property("stroke-width", "0.3"))
    .Child("svg-text", "plot-title", text => text.Text("Seeded point set — recorded Physics output")
        .Property("x", "8").Property("y", "4.5").Property("font-size", "2.3")
        .Property("font-weight", "700").Property("fill", "#eef6ff"))
    .Child("svg-text", "plot-summary", text => text.Text($"{points.Length} points · seed {seed} · source coordinates preserved")
        .Property("x", "8").Property("y", "53").Property("font-size", "1.8").Property("fill", "#b8c8d8"));

for (var i = 0; i <= 4; i++)
{
    var x = plotLeft + (plotRight - plotLeft) * i / 4;
    scene.Child("svg-line", $"grid-x-{i}", line => line
        .Property("x1", F(x)).Property("y1", F(plotTop))
        .Property("x2", F(x)).Property("y2", F(plotBottom))
        .Property("stroke", "#294253").Property("stroke-width", "0.15"));
    var y = plotTop + (plotBottom - plotTop) * i / 4;
    scene.Child("svg-line", $"grid-y-{i}", line => line
        .Property("x1", F(plotLeft)).Property("y1", F(y))
        .Property("x2", F(plotRight)).Property("y2", F(y))
        .Property("stroke", "#294253").Property("stroke-width", "0.15"));
}

foreach (var point in points)
{
    scene.Child("svg-circle", $"seeded-point-{point.Index:D5}", circle => circle
        .Property("cx", F(MapX(point.X))).Property("cy", F(MapY(point.Y)))
        .Property("r", "0.48").Property("fill", "#7ee0bd"));
}

var svg = new SvgGuiRenderer().Render(scene.Build(), width: 960, height: 540);
var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";
if (document.Root?.Name != ns + "svg")
    throw new InvalidOperationException("GUI.SVG did not emit an SVG root.");

var markers = document.Descendants(ns + "circle")
    .Where(element => ((string?)element.Attribute("id"))?.StartsWith("seeded-point-", StringComparison.Ordinal) == true)
    .OrderBy(element => (string?)element.Attribute("id"), StringComparer.Ordinal)
    .ToArray();
if (markers.Length != points.Length)
    throw new InvalidOperationException($"Expected {points.Length} point markers, got {markers.Length}.");

for (var i = 0; i < points.Length; i++)
{
    if ((string?)markers[i].Attribute("id") != $"seeded-point-{points[i].Index:D5}" ||
        (string?)markers[i].Attribute("cx") != F(MapX(points[i].X)) ||
        (string?)markers[i].Attribute("cy") != F(MapY(points[i].Y)))
        throw new InvalidOperationException($"SVG marker {i} does not match the corresponding Physics record point.");
}

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
await File.WriteAllTextAsync(outputPath, svg);
Console.WriteLine($"PASS: rendered {points.Length} recorded Physics points and verified every SVG marker against the input JSON.");
Console.WriteLine($"Model: procedural.seeded-point-set-2d v1; seed: {seed}; bounds validated; no point set was regenerated.");
Console.WriteLine($"Generated: {outputPath}");
return 0;

internal sealed record Point(int Index, double X, double Y);
