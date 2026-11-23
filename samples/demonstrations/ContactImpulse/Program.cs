using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: ContactImpulse <physics-run-record.json> <output.svg>");
    return 2;
}

var inputPath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
using var json = JsonDocument.Parse(File.ReadAllText(inputPath));
var root = json.RootElement;

if (root.GetProperty("schemaVersion").GetInt32() != 1 ||
    root.GetProperty("modelId").GetString() != "collision.single-contact-normal-impulse" ||
    root.GetProperty("modelVersion").GetInt32() != 1)
    throw new InvalidDataException("Unsupported Physics contact-impulse run-record identity or schema.");

var units = root.GetProperty("units");
if (units.GetProperty("mass").GetString() != "kg" ||
    units.GetProperty("velocity").GetString() != "m/s" ||
    units.GetProperty("impulse").GetString() != "N*s")
    throw new InvalidDataException("The contact-impulse record must use kg, m/s, and N*s.");

var definition = root.GetProperty("definition");
var result = root.GetProperty("result");
var massA = definition.GetProperty("massAKilograms").GetDouble();
var massB = definition.GetProperty("massBKilograms").GetDouble();
var restitution = definition.GetProperty("restitution").GetDouble();
var impulse = result.GetProperty("normalImpulseNewtonSeconds").GetDouble();
var physicalTimeAdvanced = result.GetProperty("physicalTimeAdvancedSeconds").GetDouble();
var initialA = ReadVector(definition.GetProperty("bodyAInitialVelocityMetresPerSecond"));
var initialB = ReadVector(definition.GetProperty("bodyBInitialVelocityMetresPerSecond"));
var finalA = ReadVector(result.GetProperty("bodyAFinalVelocityMetresPerSecond"));
var finalB = ReadVector(result.GetProperty("bodyBFinalVelocityMetresPerSecond"));
var normal = ReadVector(definition.GetProperty("contactNormalFromAToB"));

if (massA <= 0 || massB <= 0 || impulse < 0 || physicalTimeAdvanced != 0 ||
    !double.IsFinite(restitution) || restitution < 0 || restitution > 1)
    throw new InvalidDataException("The contact-impulse record contains invalid mass, impulse, time, or restitution values.");
if (Math.Abs(normal.Length - 1d) > 1e-9)
    throw new InvalidDataException("The contact normal must be a unit vector.");
if (new[] { initialA, initialB, finalA, finalB, normal }.SelectMany(v => new[] { v.X, v.Y, v.Z }).Any(v => !double.IsFinite(v)))
    throw new InvalidDataException("The contact-impulse record contains a non-finite vector component.");

var scene = GuiBuilder.Create("svg-scene", "physics-contact-impulse")
    .Text("Contact impulse — recorded Physics result")
    .Property("aria-label", "Two-body contact impulse, with initial velocities, equal and opposite normal impulses, and final velocities from a recorded Physics run")
    .Property("description", $"Model collision.single-contact-normal-impulse version 1. Masses {F(massA)} kg and {F(massB)} kg; restitution {F(restitution)}; normal impulse {F(impulse)} N*s. The resolver advances physical time by {F(physicalTimeAdvanced)} seconds. Arrows are schematic, not to scale.")
    .Property("viewBox", "0 0 100 56.25")
    .Child("svg-text", "title", t => t.Text("A single contact changes normal velocity")
        .Property("x", "50").Property("y", "5").Property("text-anchor", "middle")
        .Property("font-size", "3.2").Property("font-weight", "700").Property("fill", "#eef6ff"))
    .Child("svg-text", "subtitle", t => t.Text("Recorded resolver output · impulse is not a force · no physical time advanced")
        .Property("x", "50").Property("y", "9").Property("text-anchor", "middle")
        .Property("font-size", "1.8").Property("fill", "#b8c8d8"));

AddBody("body-a", "Body A", 27, "#58b6e8", massA, initialA, finalA);
AddBody("body-b", "Body B", 73, "#f3b45f", massB, initialB, finalB);

scene.Child("svg-line", "impulse-a", line => line
        .Property("x1", "44").Property("y1", "27").Property("x2", "35").Property("y2", "27")
        .Property("stroke", "#7ee0bd").Property("stroke-width", "1.1").Property("stroke-linecap", "round"))
    .Child("svg-polygon", "impulse-a-head", p => p
        .Property("points", "34,27 37,25.7 37,28.3").Property("fill", "#7ee0bd"))
    .Child("svg-line", "impulse-b", line => line
        .Property("x1", "56").Property("y1", "27").Property("x2", "65").Property("y2", "27")
        .Property("stroke", "#7ee0bd").Property("stroke-width", "1.1").Property("stroke-linecap", "round"))
    .Child("svg-polygon", "impulse-b-head", p => p
        .Property("points", "66,27 63,25.7 63,28.3").Property("fill", "#7ee0bd"))
    .Child("svg-text", "impulse-label", t => t.Text($"Equal/opposite normal impulse: {F(impulse)} N*s")
        .Property("x", "50").Property("y", "23").Property("text-anchor", "middle")
        .Property("font-size", "1.9").Property("font-weight", "700").Property("fill", "#7ee0bd"))
    .Child("svg-text", "normal-label", t => t.Text($"Contact normal A → B: ({F(normal.X)}, {F(normal.Y)}, {F(normal.Z)})")
        .Property("x", "50").Property("y", "32").Property("text-anchor", "middle")
        .Property("font-size", "1.5").Property("fill", "#b8c8d8"))
    .Child("svg-text", "initial-caption", t => t.Text("BEFORE · recorded initial velocity")
        .Property("x", "50").Property("y", "38").Property("text-anchor", "middle")
        .Property("font-size", "1.8").Property("font-weight", "700").Property("fill", "#eef6ff"))
    .Child("svg-text", "final-caption", t => t.Text($"AFTER · final normal velocities {F(finalA.X)} / {F(finalB.X)} m/s")
        .Property("x", "50").Property("y", "43").Property("text-anchor", "middle")
        .Property("font-size", "1.8").Property("fill", "#eef6ff"))
    .Child("svg-text", "limits", t => t.Text("Pose preserved · resolver does not separate bodies, integrate forces, or model energy conversion")
        .Property("x", "50").Property("y", "51").Property("text-anchor", "middle")
        .Property("font-size", "1.35").Property("fill", "#b8c8d8"));

var svg = new SvgGuiRenderer().Render(scene.Build(), width: 960, height: 540);
var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";
if (document.Root?.Name != ns + "svg")
    throw new InvalidOperationException("GUI.SVG did not emit an SVG root.");

foreach (var id in new[] { "body-a", "body-b", "impulse-a", "impulse-b", "impulse-label", "initial-caption", "final-caption" })
{
    if (document.Descendants().All(element => (string?)element.Attribute("id") != id))
        throw new InvalidOperationException($"Generated SVG is missing required evidence element '{id}'.");
}

if (!svg.Contains("1 N*s", StringComparison.Ordinal) ||
    !svg.Contains("no physical time advanced", StringComparison.OrdinalIgnoreCase))
    throw new InvalidOperationException("Generated SVG is missing the recorded impulse magnitude or time boundary.");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, svg);
Console.WriteLine($"PASS: manifested recorded contact impulse {F(impulse)} N*s without recomputing the Physics response.");
Console.WriteLine($"Masses: {F(massA)} kg / {F(massB)} kg; restitution: {F(restitution)}; physical time advanced: {F(physicalTimeAdvanced)} s.");
Console.WriteLine($"Generated: {outputPath}");
return 0;

void AddBody(string id, string label, double x, string fill, double mass, Vector velocityBefore, Vector velocityAfter)
{
    scene.Child("svg-circle", id, body => body
            .Property("cx", F(x)).Property("cy", "27").Property("r", "5")
            .Property("fill", fill).Property("stroke", "#ffffff").Property("stroke-width", "0.4"))
        .Child("svg-text", $"{id}-label", t => t.Text($"{label} · {F(mass)} kg")
            .Property("x", F(x)).Property("y", "17").Property("text-anchor", "middle")
            .Property("font-size", "2").Property("font-weight", "700").Property("fill", "#eef6ff"))
        .Child("svg-text", $"{id}-initial-velocity", t => t.Text($"v₀ = ({F(velocityBefore.X)}, {F(velocityBefore.Y)}, {F(velocityBefore.Z)}) m/s")
            .Property("x", F(x)).Property("y", "14").Property("text-anchor", "middle")
            .Property("font-size", "1.35").Property("fill", "#b8c8d8"))
        .Child("svg-text", $"{id}-final-velocity", t => t.Text($"v₁ = ({F(velocityAfter.X)}, {F(velocityAfter.Y)}, {F(velocityAfter.Z)}) m/s")
            .Property("x", F(x)).Property("y", "47").Property("text-anchor", "middle")
            .Property("font-size", "1.35").Property("fill", "#b8c8d8"));
}

static Vector ReadVector(JsonElement value) => new(
    value.GetProperty("x").GetDouble(),
    value.GetProperty("y").GetDouble(),
    value.GetProperty("z").GetDouble());

static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

internal readonly record struct Vector(double X, double Y, double Z)
{
    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);
}
