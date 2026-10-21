using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

// The GUI model is the source; SvgGuiRenderer is the manifestation adapter.
// This is a concept storyboard, not a claim that the simulation runtime exists.
var output = args.Length > 0 ? Path.GetFullPath(args[0]) : Path.GetFullPath("docs/assets/drone-construction-vision.svg");
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
var scene = GuiBuilder.Create("svg-scene", "drone-construction-vision")
    .Text("Drone Construction — GUI.SVG vision board")
    .Property("viewBox", "0 0 1440 1000")
    .Property("aria-label", "Drone Construction concept storyboard")
    .Property("description", "Image-derived target, ten block emitters, construction agents, one-way skylanes, hover holding lanes, slipstream reservations, and a future live renderer observer. This is an authored concept scene, not live telemetry.");
scene.Child("svg-rect", "background", n => n.Property("width", "1440").Property("height", "1000").Property("fill", "#081525"));
Text(scene, "eyebrow", "THE SINGULARITY WORKSHOP • EXPERIENCE CONCEPT • GUI.SVG", 48, 48, 13, "#91abc3", "600");
Text(scene, "title", "DRONE CONSTRUCTION", 48, 96, 38, "#f2f7ff", "800");
Text(scene, "subtitle", "From a picture to a structure — one block, one reservation, one verified step at a time.", 48, 128, 17, "#b5c9dc");
Panel(scene, "source-panel", 48, 160, 250, 260, "#0d2034");
Text(scene, "source-title", "01 / SOURCE IMAGE", 68, 194, 18, "#d9eaf8", "700");
Rect(scene, "source-image", 72, 216, 202, 122, "#dce6ee", 8);
var heights = new[] { 22, 42, 62, 82, 62, 42, 22 };
for (var i = 0; i < heights.Length; i++) Rect(scene, $"source-pixel-{i}", 88 + i * 25, 330 - heights[i], 23, heights[i], "#354f65");
Text(scene, "source-copy", "Any image / height map", 68, 372, 15, "#c1d3e2");
Text(scene, "source-note", "Explicit, repeatable mapping", 68, 398, 13, "#91abc3");
Panel(scene, "target-panel", 326, 160, 260, 260, "#0d2034");
Text(scene, "target-title", "02 / TARGET GRID", 346, 194, 18, "#d9eaf8", "700");
for (var row = 0; row < 5; row++)
for (var col = 0; col < 7; col++) {
    var h = Math.Max(0, 4 - Math.Abs(col - 3));
    Rect(scene, $"target-cell-{col}-{row}", 350 + col * 28, 220 + row * 22, 26, 20, row >= 4 - h ? "#35c8f4" : "#15364f", 1);
}
Text(scene, "target-copy", "Required height per cell", 346, 372, 15, "#c1d3e2");
Text(scene, "target-note", "Tasks = missing block slots", 346, 398, 13, "#91abc3");
Line(scene, "source-to-target", 298, 284, 322, 284, "#9bb8d2");
Panel(scene, "emitters-panel", 614, 160, 300, 260, "#0d2034");
Text(scene, "emitters-title", "03 / TEN EMITTERS", 634, 194, 18, "#d9eaf8", "700");
for (var i = 0; i < 10; i++) {
    var x = 638 + (i % 5) * 50;
    var y = 220 + (i / 5) * 58;
    Rect(scene, $"emitter-{i + 1:00}", x, y, 42, 44, "#123d58", 7, "#32c5ff");
    Text(scene, $"emitter-label-{i + 1:00}", (i + 1).ToString("00"), x + 10, y + 27, 14, "#f0f8ff", "700");
}
Text(scene, "emitters-copy", "Finite inventory + pickup queues", 634, 372, 15, "#c1d3e2");
Text(scene, "emitters-note", "Resource conservation is invariant", 634, 398, 13, "#91abc3");
Panel(scene, "build-panel", 942, 160, 450, 260, "#0d2034");
Text(scene, "build-title", "04 / COORDINATED BUILD", 962, 194, 18, "#d9eaf8", "700");
for (var row = 0; row < 5; row++)
for (var col = 0; col < 8; col++) {
    var level = Math.Max(0, 4 - Math.Abs(col - 3));
    if (row < level) Rect(scene, $"block-{col}-{row}", 978 + col * 34, 332 - row * 22, 31, 19, row % 2 == 0 ? "#35c8f4" : "#1882ad", 2, "#9feaff");
}
foreach (var (x, y, id) in new[] { (1010, 238, "agent-a"), (1150, 226, "agent-b"), (1250, 264, "agent-c") }) {
    Circle(scene, id, x, y, 6, "#9feaff");
    Line(scene, id + "-rotor-x", x - 13, y, x + 13, y, "#c3f3ff");
    Line(scene, id + "-rotor-y", x, y - 13, x, y + 13, "#c3f3ff");
}
Text(scene, "build-copy", "Drones collect, route, and place", 962, 372, 15, "#c1d3e2");
Text(scene, "build-note", "Render committed state, never invented motion", 962, 398, 13, "#91abc3");
Panel(scene, "traffic-panel", 48, 460, 1344, 310, "#0d2034");
Text(scene, "traffic-title", "THE TRAFFIC SYSTEM / TOP-DOWN CELLULAR MODEL", 76, 504, 25, "#f2f7ff", "800");
Text(scene, "traffic-subtitle", "Reserve space, wait for clearance, then traverse a bounded transition.", 76, 532, 16, "#b5c9dc");
Panel(scene, "skylane-panel", 78, 562, 500, 150, "#0b3049", "#32c5ff");
Text(scene, "skylane-title", "SKYLANE · ONE-WAY GRID", 100, 594, 18, "#d9f5ff", "700");
for (var row = 0; row < 3; row++)
for (var col = 0; col < 8; col++) Rect(scene, $"sky-cell-{col}-{row}", 100 + col * 54, 612 + row * 24, 51, 22, "#103b55", 1, "#42627f");
for (var i = 0; i < 3; i++) {
    Circle(scene, $"sky-agent-{i}", 126 + i * 145, 623 + i * 24, 5, "#55d9ff");
    Line(scene, $"sky-arrow-{i}", 135 + i * 145, 623 + i * 24, 163 + i * 145, 623 + i * 24, "#a4edff");
}
Panel(scene, "slipstream-panel", 660, 562, 240, 150, "#29213f", "#bca1ff");
Text(scene, "slipstream-title", "SLIPSTREAM", 682, 594, 18, "#e8ddff", "700");
for (var i = 0; i < 5; i++) Rect(scene, $"slip-cell-{i}", 682 + i * 40, 612, 36, 34, "#543e78", 2, "#bca1ff");
Text(scene, "slipstream-note", "5-cell lookahead", 682, 682, 14, "#c7b5ee");
Panel(scene, "hover-panel", 982, 562, 382, 150, "#18372c", "#70df9a");
Text(scene, "hover-title", "HOVER LANE · HOLDING GRID", 1004, 594, 18, "#d9ffe8", "700");
for (var row = 0; row < 3; row++)
for (var col = 0; col < 6; col++) Rect(scene, $"hover-cell-{col}-{row}", 1004 + col * 54, 612 + row * 24, 51, 22, "#204332", 1, "#426b55");
Circle(scene, "waiting-agent", 1028, 623, 5, "#70df9a");
Line(scene, "sky-to-slip", 578, 632, 656, 632, "#9bb8d2");
Line(scene, "slip-to-hover", 900, 632, 978, 632, "#9bb8d2");
Text(scene, "clearance-note", "CLEARANCE CHECK → RESERVE → TRANSIT → RELEASE", 78, 746, 14, "#91abc3");
Panel(scene, "proof-panel", 48, 812, 1344, 118, "#10243a");
Text(scene, "proof-title", "WHAT MAKES THIS A REAL PROOF?", 76, 850, 19, "#d9eaf8", "700");
Text(scene, "proof-line", "One deterministic state source → validated changes → event snapshot → Renderer + GUI.SVG observers.", 76, 880, 16, "#b5c9dc");
Text(scene, "proof-caveat", "Concept scene only. Next: execute a small pickup → transit → placement loop; 100,000 agents remains unverified.", 76, 906, 13, "#91abc3");
Text(scene, "footer", "GENERATED FROM GuiNode SCENE DATA BY SvgGuiRenderer • NOT LIVE SIMULATION TELEMETRY", 48, 970, 12, "#91abc3", "600");
var svg = new SvgGuiRenderer().Render(scene.Build(), 1440, 1000);
File.WriteAllText(output, svg);
Console.WriteLine($"Wrote GUI.SVG concept scene: {output}");

static void Panel(GuiBuilder parent, string id, int x, int y, int width, int height, string fill, string stroke = "#355779") => Rect(parent, id, x, y, width, height, fill, 14, stroke, 2);
static void Rect(GuiBuilder parent, string id, int x, int y, int width, int height, string fill, int radius = 0, string? stroke = null, int strokeWidth = 1) =>
    parent.Child("svg-rect", id, n => n.Property("x", x.ToString()).Property("y", y.ToString()).Property("width", width.ToString()).Property("height", height.ToString()).Property("rx", radius.ToString()).Property("fill", fill).Property("stroke", stroke ?? "none").Property("stroke-width", strokeWidth.ToString()));
static void Text(GuiBuilder parent, string id, string text, int x, int y, int size, string fill, string weight = "400") =>
    parent.Child("svg-text", id, n => n.Text(text).Property("x", x.ToString()).Property("y", y.ToString()).Property("font-family", "Segoe UI, Arial, sans-serif").Property("font-size", size.ToString()).Property("font-weight", weight).Property("fill", fill));
static void Line(GuiBuilder parent, string id, int x1, int y1, int x2, int y2, string stroke) =>
    parent.Child("svg-line", id, n => n.Property("x1", x1.ToString()).Property("y1", y1.ToString()).Property("x2", x2.ToString()).Property("y2", y2.ToString()).Property("stroke", stroke).Property("stroke-width", "2"));
static void Circle(GuiBuilder parent, string id, int cx, int cy, int r, string fill) =>
    parent.Child("svg-circle", id, n => n.Property("cx", cx.ToString()).Property("cy", cy.ToString()).Property("r", r.ToString()).Property("fill", fill));
