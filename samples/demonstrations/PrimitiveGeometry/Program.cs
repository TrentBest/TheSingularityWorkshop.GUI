using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

// This is the executable source of truth for demonstration 01.
// The adapter, not a hand-authored SVG, produces the output artifact.
var scene = GuiBuilder.Create("svg-scene", "scene")
    .Text("GUI.SVG demonstration 01: primitive geometry")
    .Child("svg-rect", "panel", panel => panel
        .Property("x", "10").Property("y", "5")
        .Property("width", "80").Property("height", "46")
        .Property("rx", "5")
        .Property("fill", "#173b4c")
        .Property("stroke", "#7ee0bd")
        .Property("stroke-width", "2"))
    .Child("svg-circle", "marker", marker => marker
        .Property("cx", "50").Property("cy", "28")
        .Property("r", "8")
        .Property("fill", "#ffb86c"))
    .Build();

var svg = new SvgGuiRenderer().Render(scene, width: 960, height: 540);
var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";

if (document.Root?.Name != ns + "svg")
    throw new InvalidOperationException("Adapter output does not have an SVG root.");
if (!document.Descendants(ns + "rect").Any(e => (string?)e.Attribute("id") == "panel"))
    throw new InvalidOperationException("Adapter output is missing the expected panel rectangle.");
if (!document.Descendants(ns + "circle").Any(e => (string?)e.Attribute("id") == "marker"))
    throw new InvalidOperationException("Adapter output is missing the expected marker circle.");

var viewBox = (string?)document.Root.Attribute("viewBox")
    ?? throw new InvalidOperationException("SVG root is missing viewBox.");
var bounds = viewBox.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    .Select(value => double.Parse(value, System.Globalization.CultureInfo.InvariantCulture))
    .ToArray();
if (bounds.Length != 4)
    throw new InvalidOperationException("SVG viewBox must contain four numbers.");
var panel = document.Descendants(ns + "rect").Single(e => (string?)e.Attribute("id") == "panel");
double Number(XElement element, string name) =>
    double.Parse((string?)element.Attribute(name) ?? throw new InvalidOperationException($"Missing {name}."), System.Globalization.CultureInfo.InvariantCulture);
var x = Number(panel, "x");
var y = Number(panel, "y");
var width = Number(panel, "width");
var height = Number(panel, "height");
var strokeWidth = Number(panel, "stroke-width");
if (x - strokeWidth / 2 < bounds[0] || y - strokeWidth / 2 < bounds[1] ||
    x + width + strokeWidth / 2 > bounds[0] + bounds[2] ||
    y + height + strokeWidth / 2 > bounds[1] + bounds[3])
    throw new InvalidOperationException("Panel stroke must fit fully inside the SVG viewBox.");

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(Environment.CurrentDirectory, "01-primitive-geometry.generated.svg");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, svg);
Console.WriteLine($"PASS: parsed SVG, verified panel + marker, and confirmed panel stroke fits inside viewBox.");
Console.WriteLine($"Generated: {outputPath}");
