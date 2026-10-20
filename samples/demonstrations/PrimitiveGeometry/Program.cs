using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

// This is the executable source of truth for demonstration 01.
// The adapter, not a hand-authored SVG, produces the output artifact.
var scene = GuiBuilder.Create("svg-scene", "scene")
    .Text("GUI.SVG demonstration 01: primitive geometry")
    .Child("svg-rect", "panel", panel => panel
        .Property("x", "10").Property("y", "10")
        .Property("width", "80").Property("height", "60")
        .Property("rx", "5")
        .Property("fill", "#173b4c")
        .Property("stroke", "#7ee0bd")
        .Property("stroke-width", "2"))
    .Child("svg-circle", "marker", marker => marker
        .Property("cx", "50").Property("cy", "40")
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

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(Environment.CurrentDirectory, "01-primitive-geometry.generated.svg");

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, svg);
Console.WriteLine($"PASS: parsed SVG and verified panel + marker.");
Console.WriteLine($"Generated: {outputPath}");
