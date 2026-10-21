using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: MandelbrotEscapeTime2d <physics-run-record.json> <output.svg>");
    return 2;
}

var inputPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
using var json = JsonDocument.Parse(await File.ReadAllTextAsync(inputPath));
var root = json.RootElement;

if (root.GetProperty("modelId").GetString() != "mathematics.mandelbrot-escape-time-2d" ||
    root.GetProperty("modelVersion").GetString() != "1" ||
    root.GetProperty("schemaVersion").GetInt32() != 1)
    throw new InvalidDataException("Unsupported Physics Mandelbrot identity or schema.");

var width = root.GetProperty("width").GetInt32();
var height = root.GetProperty("height").GetInt32();
var maxIterations = root.GetProperty("maxIterations").GetInt32();
var sampleCount = root.GetProperty("sampleCount").GetInt32();
if (width < 1 || height < 1 || (long)width * height > 262_144 ||
    maxIterations < 1 || maxIterations > 4_096 ||
    (long)width * height * maxIterations > 50_000_000 ||
    sampleCount != (long)width * height)
    throw new InvalidDataException("Grid dimensions, iteration limit, or work budget is invalid.");

var bounds = root.GetProperty("bounds");
var minReal = bounds.GetProperty("minReal").GetDouble();
var minImaginary = bounds.GetProperty("minImaginary").GetDouble();
var maxReal = bounds.GetProperty("maxReal").GetDouble();
var maxImaginary = bounds.GetProperty("maxImaginary").GetDouble();
if (bounds.GetProperty("convention").GetString() != "minimum-inclusive, maximum-exclusive; cell centers sampled" ||
    !double.IsFinite(minReal) || !double.IsFinite(minImaginary) ||
    !double.IsFinite(maxReal) || !double.IsFinite(maxImaginary) ||
    maxReal <= minReal || maxImaginary <= minImaginary ||
    !double.IsFinite(maxReal - minReal) || !double.IsFinite(maxImaginary - minImaginary))
    throw new InvalidDataException("Invalid or unsupported complex-plane bounds.");

var samples = root.GetProperty("samples").EnumerateArray().Select((item, index) =>
{
    var column = item.GetProperty("column").GetInt32();
    var row = item.GetProperty("row").GetInt32();
    var real = item.GetProperty("real").GetDouble();
    var imaginary = item.GetProperty("imaginary").GetDouble();
    var iterations = item.GetProperty("iterations").GetInt32();
    var escaped = item.GetProperty("escaped").GetBoolean();
    if (column != index % width || row != index / width)
        throw new InvalidDataException($"Sample {index} is missing, duplicated, or not in row-major order.");
    if (!double.IsFinite(real) || !double.IsFinite(imaginary) ||
        real < minReal || real >= maxReal || imaginary < minImaginary || imaginary >= maxImaginary)
        throw new InvalidDataException($"Sample {index} has invalid or out-of-window coordinates.");
    if (iterations < 1 || iterations > maxIterations || (!escaped && iterations != maxIterations))
        throw new InvalidDataException($"Sample {index} has an inconsistent iteration/escape classification.");
    return new Sample(column, row, real, imaginary, iterations, escaped);
}).ToArray();

if (samples.Length != sampleCount)
    throw new InvalidDataException($"Record declares {sampleCount} samples but contains {samples.Length}.");

const double plotLeft = 5;
const double plotTop = 7;
const double plotWidth = 90;
const double plotHeight = 38;
var cellWidth = plotWidth / width;
var cellHeight = plotHeight / height;
string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
string CellColor(Sample sample)
{
    if (!sample.Escaped)
        return "#10283c";
    var t = (sample.Iterations - 1d) / Math.Max(1, maxIterations - 1);
    var red = (int)Math.Round(24 + 70 * t);
    var green = (int)Math.Round(95 + 145 * t);
    var blue = (int)Math.Round(165 + 75 * (1 - t));
    return $"#{red:X2}{green:X2}{blue:X2}";
}

var escapedCount = samples.Count(sample => sample.Escaped);
var scene = GuiBuilder.Create("svg-scene", "physics-mandelbrot-escape-time")
    .Text("Mandelbrot escape-time field")
    .Property("aria-label", "GUI.SVG manifestation of the recorded Physics Mandelbrot escape-time samples")
    .Property("description", $"Physics model mathematics.mandelbrot-escape-time-2d v1; {width} by {height} cell-center samples; {escapedCount} escaped; maximum {maxIterations} iterations.")
    .Child("svg-rect", "plot-background", rect => rect
        .Property("x", "5").Property("y", "7").Property("width", "90").Property("height", "38")
        .Property("fill", "#07111d").Property("stroke", "#294253").Property("stroke-width", "0.15"))
    .Child("svg-text", "plot-title", text => text.Text("MANDELBROT · PHYSICS DATA")
        .Property("x", "5").Property("y", "4.5").Property("font-size", "2.2")
        .Property("font-weight", "700").Property("fill", "#eef6ff"))
    .Child("svg-text", "plot-summary", text => text.Text($"{width} × {height} samples · {escapedCount} escaped · source classifications preserved")
        .Property("x", "5").Property("y", "50").Property("font-size", "1.7").Property("fill", "#b8c8d8"))
    .Child("svg-text", "plot-bounds", text => text.Text($"Real [{F(minReal)}, {F(maxReal)}) · Imaginary [{F(minImaginary)}, {F(maxImaginary)}) · cap {maxIterations}")
        .Property("x", "5").Property("y", "54").Property("font-size", "1.5").Property("fill", "#7ee0bd"));

foreach (var sample in samples)
{
    scene.Child("svg-rect", $"mandelbrot-cell-{sample.Row:D3}-{sample.Column:D3}", rect => rect
        .Property("x", F(plotLeft + sample.Column * cellWidth))
        .Property("y", F(plotTop + sample.Row * cellHeight))
        .Property("width", F(cellWidth + 0.001))
        .Property("height", F(cellHeight + 0.001))
        .Property("fill", CellColor(sample)));
}

var svg = new SvgGuiRenderer().Render(scene.Build(), width: 1200, height: 720);
var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";
if (document.Root?.Name != ns + "svg")
    throw new InvalidOperationException("GUI.SVG did not emit an SVG root.");

var cells = document.Descendants(ns + "rect")
    .Where(element => ((string?)element.Attribute("id"))?.StartsWith("mandelbrot-cell-", StringComparison.Ordinal) == true)
    .ToDictionary(element => (string)element.Attribute("id")!, StringComparer.Ordinal);
if (cells.Count != samples.Length)
    throw new InvalidOperationException($"Expected {samples.Length} rendered cells, got {cells.Count}.");

foreach (var sample in samples)
{
    var id = $"mandelbrot-cell-{sample.Row:D3}-{sample.Column:D3}";
    if (!cells.TryGetValue(id, out var cell) ||
        (string?)cell.Attribute("x") != F(plotLeft + sample.Column * cellWidth) ||
        (string?)cell.Attribute("y") != F(plotTop + sample.Row * cellHeight) ||
        (string?)cell.Attribute("fill") != CellColor(sample))
        throw new InvalidOperationException($"SVG cell {id} does not match the corresponding Physics record sample.");
}

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
await File.WriteAllTextAsync(outputPath, svg);
Console.WriteLine($"PASS: rendered and verified {samples.Length} recorded Physics Mandelbrot samples.");
Console.WriteLine($"Model: mathematics.mandelbrot-escape-time-2d v1; {escapedCount} escaped; no fractal samples were recomputed.");
Console.WriteLine($"Generated: {outputPath}");
return 0;

internal sealed record Sample(int Column, int Row, double Real, double Imaginary, int Iterations, bool Escaped);
