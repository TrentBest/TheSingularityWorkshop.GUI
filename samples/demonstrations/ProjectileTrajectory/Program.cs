using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: ProjectileTrajectory <physics-run-record.json> <output.svg>");
    return 2;
}

var inputPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
using var json = JsonDocument.Parse(File.ReadAllText(inputPath));
var root = json.RootElement;

var modelId = root.GetProperty("modelId").GetString();
var modelVersion = root.GetProperty("modelVersion").GetString();
var schemaVersion = root.GetProperty("schemaVersion").GetInt32();
if (modelId != "kinematics.constant-acceleration-projectile" || modelVersion != "1" || schemaVersion != 1)
    throw new InvalidDataException("Unsupported Physics projectile run-record identity or schema.");

var units = root.GetProperty("units");
if (units.GetProperty("position").GetString() != "m" ||
    units.GetProperty("elapsedTime").GetString() != "s")
    throw new InvalidDataException("The projectile run record must use metres and seconds.");

var frame = root.GetProperty("definition").GetProperty("coordinateFrame").GetString();
if (frame != "world, Z-up")
    throw new InvalidDataException($"Expected world, Z-up coordinates, got '{frame}'.");

var samples = root.GetProperty("samples").EnumerateArray().Select((sample, index) =>
{
    var position = sample.GetProperty("position");
    var elapsedTime = sample.GetProperty("elapsedTime").GetDouble();
    var x = position.GetProperty("x").GetDouble();
    var z = position.GetProperty("z").GetDouble();
    if (!double.IsFinite(elapsedTime) || !double.IsFinite(x) || !double.IsFinite(z))
        throw new InvalidDataException($"Sample {index} contains a non-finite value.");
    return new Sample(index, elapsedTime, x, z);
}).ToArray();

if (samples.Length < 2)
    throw new InvalidDataException("A trajectory needs at least two recorded samples.");
if (samples.Zip(samples.Skip(1)).Any(pair => pair.Second.ElapsedTime <= pair.First.ElapsedTime))
    throw new InvalidDataException("Sample elapsed times must be strictly increasing.");

var maxX = samples.Max(sample => sample.X);
var minX = samples.Min(sample => sample.X);
var maxZ = samples.Max(sample => sample.Z);
var minZ = samples.Min(sample => sample.Z);
if (maxX <= minX || maxZ <= minZ)
    throw new InvalidDataException("The run record needs non-zero X and Z spans to draw this trajectory.");

const double plotLeft = 10;
const double plotRight = 90;
const double plotTop = 8;
const double plotBottom = 47;
double MapX(double x) => plotLeft + (x - minX) / (maxX - minX) * (plotRight - plotLeft);
double MapZ(double z) => plotBottom - (z - minZ) / (maxZ - minZ) * (plotBottom - plotTop);
string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
var plottedPoints = string.Join(" ", samples.Select(sample => $"{F(MapX(sample.X))},{F(MapZ(sample.Z))}"));

var scene = GuiBuilder.Create("svg-scene", "physics-projectile-trajectory")
    .Text("Physics projectile trajectory")
    .Property("aria-label", "Trajectory plotted from the Physics repository's recorded projectile run")
    .Property("description", $"Model {modelId} version {modelVersion}; {samples.Length} recorded states; world Z-up; position in metres and time in seconds.")
    .Child("svg-text", "plot-title", text => text
        .Text("Projectile trajectory — recorded Physics run")
        .Property("x", "8").Property("y", "5")
        .Property("font-size", "3.0").Property("font-weight", "700")
        .Property("fill", "#eef6ff"))
    .Child("svg-text", "plot-subtitle", text => text
        .Text($"{samples.Length} states · {F(samples[0].ElapsedTime)}–{F(samples[^1].ElapsedTime)} s · X/Z in metres · world Z-up")
        .Property("x", "8").Property("y", "54")
        .Property("font-size", "2.0").Property("fill", "#b8c8d8"));

foreach (var x in Enumerable.Range(0, 5).Select(i => plotLeft + i * (plotRight - plotLeft) / 4))
{
    scene.Child("svg-line", $"grid-x-{F(x).Replace('.', '-')}", line => line
        .Property("x1", F(x)).Property("y1", F(plotTop))
        .Property("x2", F(x)).Property("y2", F(plotBottom))
        .Property("stroke", "#294253").Property("stroke-width", "0.2"));
}
foreach (var y in new[] { 10d, 20d, 30d, 40d })
{
    scene.Child("svg-line", $"grid-z-{F(y).Replace('.', '-')}", line => line
        .Property("x1", F(plotLeft)).Property("y1", F(y))
        .Property("x2", F(plotRight)).Property("y2", F(y))
        .Property("stroke", "#294253").Property("stroke-width", "0.2"));
}

scene.Child("svg-line", "axis-x", axis => axis
        .Property("x1", F(plotLeft)).Property("y1", F(plotBottom))
        .Property("x2", F(plotRight)).Property("y2", F(plotBottom))
        .Property("stroke", "#a7c1d2").Property("stroke-width", "0.35"))
    .Child("svg-line", "axis-z", axis => axis
        .Property("x1", F(plotLeft)).Property("y1", F(plotTop))
        .Property("x2", F(plotLeft)).Property("y2", F(plotBottom))
        .Property("stroke", "#a7c1d2").Property("stroke-width", "0.35"))
    .Child("svg-text", "axis-x-label", text => text
        .Text("X (m)").Property("x", "84").Property("y", "45")
        .Property("font-size", "2.3").Property("fill", "#eef6ff"))
    .Child("svg-text", "axis-z-label", text => text
        .Text("Z (m)").Property("x", "11").Property("y", "10")
        .Property("font-size", "2.3").Property("fill", "#eef6ff"))
    .Child("svg-polyline", "projectile-trajectory", line => line
        .Property("points", plottedPoints).Property("fill", "none")
        .Property("stroke", "#7ee0bd").Property("stroke-width", "0.7")
        .Property("stroke-linejoin", "round").Property("stroke-linecap", "round"));

foreach (var sample in samples)
{
    scene.Child("svg-circle", $"projectile-sample-{sample.Index:D2}", point => point
        .Property("cx", F(MapX(sample.X))).Property("cy", F(MapZ(sample.Z)))
        .Property("r", "0.8").Property("fill", "#ffcf86"));
}

var svg = new SvgGuiRenderer().Render(scene.Build(), width: 960, height: 540);
var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";
if (document.Root?.Name != ns + "svg")
    throw new InvalidOperationException("GUI.SVG did not emit an SVG root.");

var trajectory = document.Descendants(ns + "polyline")
    .SingleOrDefault(element => (string?)element.Attribute("id") == "projectile-trajectory");
if ((string?)trajectory?.Attribute("points") != plottedPoints)
    throw new InvalidOperationException("The SVG trajectory differs from the coordinates mapped from the Physics run record.");

var markers = document.Descendants(ns + "circle")
    .Where(element => ((string?)element.Attribute("id"))?.StartsWith("projectile-sample-", StringComparison.Ordinal) == true)
    .OrderBy(element => (string?)element.Attribute("id"), StringComparer.Ordinal)
    .ToArray();
if (markers.Length != samples.Length)
    throw new InvalidOperationException($"Expected {samples.Length} recorded-state markers, got {markers.Length}.");

for (var i = 0; i < samples.Length; i++)
{
    if ((string?)markers[i].Attribute("cx") != F(MapX(samples[i].X)) ||
        (string?)markers[i].Attribute("cy") != F(MapZ(samples[i].Z)))
        throw new InvalidOperationException($"SVG marker {i} does not match the corresponding Physics run-record sample.");
}

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, svg);
Console.WriteLine($"PASS: rendered {samples.Length} recorded Physics states without recomputing trajectory physics.");
Console.WriteLine($"Model: {modelId} v{modelVersion}; frame: {frame}; units: m and s.");
Console.WriteLine($"Generated: {outputPath}");
return 0;

internal sealed record Sample(int Index, double ElapsedTime, double X, double Z);
