using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

// This is the canonical scene model. The checked-in SVG is the inspectable manifestation.
var output = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath("docs/assets/fox-sheepdog-10x10.svg");

var scene = GuiBuilder.Create("svg-scene", "fox-sheepdog-plane")
    .Text("GUI.SVG · 10×10 fox and sheepdog test plane")
    .Property("viewBox", "0 0 1200 760")
    .Property("aria-label", "Ten by ten isometric fox and sheepdog behavior test plane")
    .Property("description", "A regular orthogonal grid projected isometrically. Four sleeping dogs are scattered across a 10 by 10 isometric grid. Four foxes traverse different grid axes and use curved SVG motion paths to jump above detected dogs.");

scene.Child("svg-defs", "definitions", defs => defs
    .Child("svg-linear-gradient", "background", gradient => gradient
        .Property("x1", "0%").Property("y1", "0%").Property("x2", "100%").Property("y2", "100%")
        .Child("svg-stop", "background-start", stop => stop.Property("offset", "0%").Property("stop-color", "#0b1423"))
        .Child("svg-stop", "background-end", stop => stop.Property("offset", "100%").Property("stop-color", "#1b3043")))
    .Child("svg-linear-gradient", "plane", gradient => gradient
        .Property("x1", "0%").Property("y1", "0%").Property("x2", "100%").Property("y2", "100%")
        .Child("svg-stop", "plane-start", stop => stop.Property("offset", "0%").Property("stop-color", "#1e4550"))
        .Child("svg-stop", "plane-end", stop => stop.Property("offset", "100%").Property("stop-color", "#142b3a")))
    .Child("svg-linear-gradient", "fox-fur", gradient => gradient
        .Property("x1", "0%").Property("y1", "0%").Property("x2", "100%").Property("y2", "100%")
        .Child("svg-stop", "fox-start", stop => stop.Property("offset", "0%").Property("stop-color", "#ffc778"))
        .Child("svg-stop", "fox-end", stop => stop.Property("offset", "100%").Property("stop-color", "#e66b32"))));
scene.Child("svg-rect", "sky", n => n.Property("width", "1200").Property("height", "760").Property("fill", "url(#background)"));
scene.Child("svg-text", "heading", n => n.Text("THE FOX / SHEEPDOG CELL TEST")
    .Property("x", "40").Property("y", "42").Property("fill", "#eaf2fb").Property("font-size", "24").Property("font-weight", "800"));
scene.Child("svg-text", "subtitle", n => n.Text("GUI.SVG scene model · 10 × 10 cells · parallel grid families · four sleeping dogs · four crossing fox routes · jump-on-detection")
    .Property("x", "40").Property("y", "70").Property("fill", "#9fb6cc").Property("font-size", "14"));

scene.Child("svg-g", "plane-scene", plane =>
{
    plane.Child("svg-polygon", "plane-surface", p => p.Property("points", "600,150 960,330 600,510 240,330")
        .Property("fill", "#163742").Property("stroke", "#527d8b").Property("stroke-width", "2"));

    // Isometric projection of a true 10×10 orthogonal grid:
    // screenX = 600 + (column - row) * 36; screenY = 150 + (column + row) * 18.
    for (var column = 0; column <= 10; column++)
    {
        var x1 = 600 + column * 36;
        var y1 = 150 + column * 18;
        var x2 = 600 + (column - 10) * 36;
        var y2 = 150 + (column + 10) * 18;
        plane.Child("svg-line", $"grid-column-{column}", line => line
            .Property("x1", x1.ToString()).Property("y1", y1.ToString())
            .Property("x2", x2.ToString()).Property("y2", y2.ToString())
            .Property("stroke", "#74a5b4").Property("stroke-opacity", "0.5").Property("stroke-width", "1"));
    }

    for (var row = 0; row <= 10; row++)
    {
        var x1 = 600 - row * 36;
        var y1 = 150 + row * 18;
        var x2 = 600 + (10 - row) * 36;
        var y2 = 150 + (row + 10) * 18;
        plane.Child("svg-line", $"grid-row-{row}", line => line
            .Property("x1", x1.ToString()).Property("y1", y1.ToString())
            .Property("x2", x2.ToString()).Property("y2", y2.ToString())
            .Property("stroke", "#74a5b4").Property("stroke-opacity", "0.5").Property("stroke-width", "1"));
    }

    // Four scattered sleeping dogs occupy cells on four distinct grid-aligned fox routes.
    // The layout is intentionally fixed for reproducible screenshots; each route jumps over its sensed dog.
    AddDog(plane, "dog-a", 600, 258, 0.78);
    AddDog(plane, "dog-b", 708, 348, 0.78);
    AddDog(plane, "dog-c", 492, 348, 0.78);
    AddDog(plane, "dog-d", 600, 438, 0.78);

    AddMovingFox(plane, "fox-east-west", "M492 204 L564 240 Q600 160 636 276 L852 384", "18s", "0s");
    AddMovingFox(plane, "fox-north-south", "M888 294 L636 420 Q600 315 564 456 L528 474", "21s", "4s");
    AddMovingFox(plane, "fox-cross-axis", "M852 276 L744 330 Q708 230 672 366 L492 456", "19s", "8s");
    AddMovingFox(plane, "fox-lower-axis", "M348 276 L456 330 Q492 220 528 366 L708 456", "23s", "12s");

});

scene.Child("svg-g", "legend", legend =>
{
    legend.Child("svg-rect", "legend-panel", r => r.Property("x", "40").Property("y", "600").Property("width", "1120").Property("height", "110").Property("rx", "16").Property("fill", "#172b3d").Property("stroke", "#345064"));
    legend.Child("svg-text", "legend-grid", t => t.Text("10 × 10 regular grid · four scattered sleeping dogs · four foxes on crossing axes").Property("x", "64").Property("y", "634").Property("fill", "#dce8f3").Property("font-size", "16"));
    legend.Child("svg-text", "legend-encounter", t => t.Text("Detect dog → jump arc → continue along the grid-aligned route").Property("x", "64").Property("y", "663").Property("fill", "#ffcf86").Property("font-size", "15"));
    legend.Child("svg-text", "legend-boundary", t => t.Text("Curved SVG paths illustrate detection and jumping; not a live FSM_API or physics trace.").Property("x", "64").Property("y", "691").Property("fill", "#9fb6cc").Property("font-size", "12"));
});

scene.Child("svg-style", "scene-style", style => style.Text("""
text { font-family: ui-sans-serif, system-ui, sans-serif; }
.dog-zzz { transform-box: fill-box; transform-origin: center; animation: snooze 1.5s ease-in-out infinite; }
@keyframes snooze { 0%,100% { transform: translate(0,0); opacity:1; } 50% { transform: translate(4px,-5px); opacity:.55; } }
.moving-fox { animation: fox-arrive 18s linear infinite; }
@keyframes fox-arrive { 0%,3% { opacity:0; } 6%,90% { opacity:1; } 96%,100% { opacity:0; } }
@media (prefers-reduced-motion:reduce) { .moving-fox,.dog-zzz { animation:none; } .moving-fox { opacity:1; } }
"""));

var svg = new SvgGuiRenderer().Render(scene.Build(), 1200, 760);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, svg);
Console.WriteLine($"Wrote GUI.SVG scene to {output}");

static void AddDog(GuiBuilder plane, string id, int x, int y, double scale)
{
    plane.Child("svg-g", id, dog =>
    {
        dog.Property("transform", $"translate({x} {y}) scale({scale})");
        dog.Child("svg-use", $"{id}-art", use => use.Property("href", "#dog-actor"));
        dog.Child("svg-text", $"{id}-zzz", t => t.Text("Z z z")
            .Property("x", "25").Property("y", "-23").Property("fill", "#d5e8f8")
            .Property("font-size", "11").Property("font-weight", "900")
            .Property("class", "dog-zzz"));
    });
}

static void AddMovingFox(GuiBuilder plane, string id, string path, string duration, string delay)
{
    plane.Child("svg-g", id, fox =>
    {
        fox.Property("class", "moving-fox");
        fox.Child("svg-animate-motion", $"{id}-route", motion => motion
            .Property("path", path).Property("dur", duration).Property("begin", delay)
            .Property("repeatCount", "indefinite").Property("rotate", "auto")
            .Property("calcMode", "linear"));
        AddFox(fox, $"{id}-art");
    });
}

static void AddFox(GuiBuilder fox, string prefix)
{
    fox.Child("svg-ellipse", $"{prefix}-shadow", e => e.Property("cx", "0").Property("cy", "9").Property("rx", "17").Property("ry", "4").Property("fill", "#07111d").Property("opacity", "0.4"));
    fox.Child("svg-path", $"{prefix}-tail", p => p.Property("d", "M-8 0 Q-28 -2 -26 -15 Q-22 -22 -14 -13 Q-18 -8 -10 -4 L-1 -3").Property("fill", "#f7eee0").Property("stroke", "#d77a3e").Property("stroke-width", "2"));
    fox.Child("svg-ellipse", $"{prefix}-body", e => e.Property("cx", "0").Property("cy", "0").Property("rx", "13").Property("ry", "8").Property("fill", "#e66b32"));
    fox.Child("svg-path", $"{prefix}-head", p => p.Property("d", "M5 -5 L11 -18 L21 -20 L27 -12 L20 -3 L10 -1 Z").Property("fill", "#f0a24e").Property("stroke", "#d96832").Property("stroke-width", "1.5"));
    fox.Child("svg-path", $"{prefix}-ears", p => p.Property("d", "M11 -17 L10 -27 L17 -19 M20 -19 L27 -27 L26 -12").Property("fill", "#ef8b43").Property("stroke", "#d96832").Property("stroke-width", "1.3"));
    fox.Child("svg-circle", $"{prefix}-eye", c => c.Property("cx", "21").Property("cy", "-13").Property("r", "1.5").Property("fill", "#182638"));
}
