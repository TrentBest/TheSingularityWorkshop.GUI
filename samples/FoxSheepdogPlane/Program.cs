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
    .Property("description", "A regular orthogonal grid projected isometrically. A fox enters the cell beside a sleeping sheepdog; the dog wakes, reaches, a cartoon fight cloud hides the tussle, then a limp fox, RIP stone, and ascending harp angel show the outcome.");

scene.Child("svg-rect", "sky", n => n.Property("width", "1200").Property("height", "760").Property("fill", "#0b1423"));
scene.Child("svg-text", "heading", n => n.Text("THE FOX / SHEEPDOG CELL TEST")
    .Property("x", "40").Property("y", "42").Property("fill", "#eaf2fb").Property("font-size", "24").Property("font-weight", "800"));
scene.Child("svg-text", "subtitle", n => n.Text("GUI.SVG scene model · 10 × 10 cells · parallel grid families · one explicit encounter")
    .Property("x", "40").Property("y", "70").Property("fill", "#9fb6cc").Property("font-size", "14"));

scene.Child("svg-g", "plane", plane =>
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

    // Cell (5,5) is the sleeping sheepdog. The fox travels along row 5 and
    // reaches the adjacent cell (6,5), where the dog can reach out.
    plane.Child("svg-rect", "dog-cell-highlight", cell => cell
        .Property("x", "582").Property("y", "312").Property("width", "36").Property("height", "36")
        .Property("fill", "#ffcf86").Property("fill-opacity", "0.12").Property("stroke", "#ffcf86").Property("stroke-width", "1.5"));

    plane.Child("svg-g", "sleeping-dog", dog =>
    {
        dog.Property("transform", "translate(600 330)");
        dog.Child("svg-ellipse", "dog-shadow", e => e.Property("cx", "0").Property("cy", "13").Property("rx", "22").Property("ry", "5").Property("fill", "#07111d").Property("opacity", "0.45"));
        dog.Child("svg-path", "dog-body", p => p.Property("d", "M-20,5 Q-29,-4 -22,-13 Q-14,-23 -5,-14 Q4,-24 13,-14 Q25,-10 22,2 L15,10 L-16,10 Z").Property("fill", "#d7d4ca").Property("stroke", "#9ba7ae").Property("stroke-width", "1.5"));
        dog.Child("svg-ellipse", "dog-muzzle", e => e.Property("cx", "17").Property("cy", "1").Property("rx", "11").Property("ry", "8").Property("fill", "#f7f0e3").Property("stroke", "#aab0b2").Property("stroke-width", "1"));
        dog.Child("svg-path", "dog-closed-eyes", p => p.Property("d", "M11,0 q3,-3 6,0 M20,0 q3,-3 5,0").Property("fill", "none").Property("stroke", "#6d625a").Property("stroke-width", "1.5"));
        dog.Child("svg-text", "dog-zzz", t => t.Text("Z z z").Property("x", "17").Property("y", "-18").Property("fill", "#d5e8f8").Property("font-size", "11").Property("font-weight", "800").Property("class", "zzz"));
        dog.Child("svg-g", "dog-arm", arm => arm.Property("class", "dog-arm")
            .Child("svg-path", "arm-shape", p => p.Property("d", "M-8,2 Q-18,0 -22,7 L-31,10").Property("fill", "none").Property("stroke", "#eee9dd").Property("stroke-width", "6").Property("stroke-linecap", "round")));
    });

    plane.Child("svg-g", "fox-moving", fox =>
    {
        fox.Property("class", "fox-moving");
        fox.Child("svg-animate-motion", "fox-route", motion => motion
            .Property("path", "M456 258 L492 276 L528 294 L564 312 L600 330 L636 348")
            .Property("dur", "8s").Property("repeatCount", "indefinite").Property("rotate", "0"));
        fox.Child("svg-animate", "fox-hide-on-collision", animation => animation
            .Property("attributeName", "opacity").Property("values", "1;1;0;0;1")
            .Property("keyTimes", "0;0.43;0.5;0.94;1").Property("dur", "16s").Property("repeatCount", "indefinite"));
        AddFox(fox, "moving-fox-art");
    });

    plane.Child("svg-g", "fight-cloud", cloud =>
    {
        cloud.Property("class", "fight-cloud");
        cloud.Child("svg-circle", "cloud-puff-a", c => c.Property("cx", "615").Property("cy", "335").Property("r", "18").Property("fill", "#e7d5b4"));
        cloud.Child("svg-circle", "cloud-puff-b", c => c.Property("cx", "636").Property("cy", "329").Property("r", "22").Property("fill", "#d7c5a5"));
        cloud.Child("svg-circle", "cloud-puff-c", c => c.Property("cx", "652").Property("cy", "342").Property("r", "16").Property("fill", "#e7d5b4"));
        cloud.Child("svg-text", "fight-stars", t => t.Text("!  *  ?  !").Property("x", "610").Property("y", "320").Property("fill", "#a65b45").Property("font-size", "18").Property("font-weight", "900"));
    });

    plane.Child("svg-g", "limp-fox", dead =>
    {
        dead.Property("class", "limp-fox");
        dead.Child("svg-ellipse", "limp-body", e => e.Property("cx", "638").Property("cy", "351").Property("rx", "17").Property("ry", "8").Property("fill", "#e66b32").Property("stroke", "#d96832").Property("stroke-width", "1.5"));
        dead.Child("svg-path", "limp-tail", p => p.Property("d", "M624 350 Q608 355 614 342 Q619 338 628 345").Property("fill", "#f7eee0").Property("stroke", "#d77a3e").Property("stroke-width", "2"));
        dead.Child("svg-circle", "limp-head", c => c.Property("cx", "653").Property("cy", "350").Property("r", "8").Property("fill", "#f0a24e"));
        dead.Child("svg-path", "limp-ear", p => p.Property("d", "M650 344 L649 337 L655 343").Property("fill", "#ef8b43").Property("stroke", "#d96832").Property("stroke-width", "1"));
        dead.Child("svg-path", "limp-legs", p => p.Property("d", "M632 356 l-4 5 M641 357 l-1 5").Property("fill", "none").Property("stroke", "#f4b46b").Property("stroke-width", "3").Property("stroke-linecap", "round"));
    });

    plane.Child("svg-g", "rip-stone", stone =>
    {
        stone.Property("class", "rip-stone");
        stone.Child("svg-path", "stone-shape", p => p.Property("d", "M618 365 V351 Q618 340 630 340 Q642 340 642 351 V365 Z").Property("fill", "#8998a7").Property("stroke", "#d1dbe4").Property("stroke-width", "1.2"));
        stone.Child("svg-text", "stone-rip", t => t.Text("RIP").Property("x", "630").Property("y", "354").Property("fill", "#182638").Property("font-size", "7").Property("font-weight", "900").Property("text-anchor", "middle"));
    });

    plane.Child("svg-g", "fox-angel", angel =>
    {
        angel.Property("class", "fox-angel");
        angel.Child("svg-ellipse", "halo", e => e.Property("cx", "0").Property("cy", "-24").Property("rx", "9").Property("ry", "3").Property("fill", "none").Property("stroke", "#ffcf86").Property("stroke-width", "2"));
        angel.Child("svg-circle", "angel-head", c => c.Property("cx", "0").Property("cy", "-15").Property("r", "6").Property("fill", "#fff0d5").Property("stroke", "#d6e6f2").Property("stroke-width", "1"));
        angel.Child("svg-path", "angel-wings", p => p.Property("d", "M-6 -10 Q-19 -20 -20 -7 Q-16 2 -7 2 M6 -10 Q19 -20 20 -7 Q16 2 7 2").Property("fill", "#fff0d5").Property("stroke", "#ffcf86").Property("stroke-width", "1.5"));
        angel.Child("svg-path", "angel-body", p => p.Property("d", "M-7 -9 Q-12 0 -7 8 L0 3 L7 8 Q12 0 7 -9 Z").Property("fill", "#e8f5ff").Property("stroke", "#a7d4ed").Property("stroke-width", "1"));
        angel.Child("svg-path", "angel-harp", p => p.Property("d", "M10 -5 L17 -2 L15 7 M12 -2 L17 3 M12 1 L16 6").Property("fill", "none").Property("stroke", "#ffcf86").Property("stroke-width", "1.4"));
        angel.Child("svg-animate-motion", "angel-lift", motion => motion.Property("path", "M630 344 Q660 280 710 190").Property("dur", "4s").Property("begin", "9s").Property("repeatCount", "indefinite").Property("rotate", "0"));
        angel.Child("svg-animate", "angel-fade", animation => animation.Property("attributeName", "opacity").Property("values", "0;1;1;0").Property("keyTimes", "0;0.1;0.8;1").Property("dur", "4s").Property("begin", "9s").Property("repeatCount", "indefinite"));
    });
});

scene.Child("svg-g", "legend", legend =>
{
    legend.Child("svg-rect", "legend-panel", r => r.Property("x", "40").Property("y", "600").Property("width", "1120").Property("height", "110").Property("rx", "16").Property("fill", "#172b3d").Property("stroke", "#345064"));
    legend.Child("svg-text", "legend-grid", t => t.Text("10 × 10 regular grid · each intersection has a cell coordinate").Property("x", "64").Property("y", "634").Property("fill", "#dce8f3").Property("font-size", "16"));
    legend.Child("svg-text", "legend-encounter", t => t.Text("Wake → reach → dust/fight cloud → limp fox → RIP → harp angel").Property("x", "64").Property("y", "663").Property("fill", "#ffcf86").Property("font-size", "15"));
    legend.Child("svg-text", "legend-boundary", t => t.Text("SVG animation is presentation; behavior state is not yet a live FSM_API trace.").Property("x", "64").Property("y", "691").Property("fill", "#9fb6cc").Property("font-size", "12"));
});

scene.Child("svg-style", "scene-style", style => style.Text("""
text { font-family: ui-sans-serif, system-ui, sans-serif; }
.zzz { animation: zzz-float 1.5s ease-in-out infinite; transform-box: fill-box; transform-origin: center; }
@keyframes zzz-float { 0%,100% { transform: translate(0,0); opacity:1; } 50% { transform: translate(4px,-5px); opacity:.55; } }
.dog-arm { transform-box: fill-box; transform-origin: 0% 50%; animation: dog-reach 16s linear infinite; }
@keyframes dog-reach { 0%,42% { transform: translateX(0) scaleX(.2); } 46%,53% { transform: translateX(-7px) scaleX(1); } 58%,100% { transform: translateX(0) scaleX(.2); } }
.fight-cloud { opacity:0; animation: fight-cloud 16s linear infinite; }
@keyframes fight-cloud { 0%,43% { opacity:0; } 45%,57% { opacity:1; } 61%,100% { opacity:0; } }
.limp-fox { opacity:0; animation: limp-show 16s linear infinite; }
@keyframes limp-show { 0%,58% { opacity:0; } 62%,90% { opacity:1; } 96%,100% { opacity:0; } }
.rip-stone { opacity:0; animation: stone-show 16s linear infinite; }
@keyframes stone-show { 0%,61% { opacity:0; } 65%,94% { opacity:1; } 100% { opacity:0; } }
.fox-angel { opacity:0; transform-box: fill-box; transform-origin:center; animation: angel-rise 16s linear infinite; }
@keyframes angel-rise { 0%,67% { opacity:0; transform:translate(630px,344px) scale(.35); } 70% { opacity:1; } 88% { opacity:1; transform:translate(700px,190px) scale(1); } 96%,100% { opacity:0; transform:translate(730px,130px) scale(1.1); } }
@media (prefers-reduced-motion: reduce) { .zzz,.dog-arm,.fight-cloud,.limp-fox,.rip-stone,.fox-angel { animation:none; } .rip-stone { opacity:1; } }
"""));

var svg = new SvgGuiRenderer().Render(scene.Build(), 1200, 760);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, svg);
Console.WriteLine($"Wrote GUI.SVG scene to {output}");

static void AddFox(GuiBuilder fox, string prefix)
{
    fox.Child("svg-ellipse", $"{prefix}-shadow", e => e.Property("cx", "0").Property("cy", "9").Property("rx", "17").Property("ry", "4").Property("fill", "#07111d").Property("opacity", "0.4"));
    fox.Child("svg-path", $"{prefix}-tail", p => p.Property("d", "M-8 0 Q-28 -2 -26 -15 Q-22 -22 -14 -13 Q-18 -8 -10 -4 L-1 -3").Property("fill", "#f7eee0").Property("stroke", "#d77a3e").Property("stroke-width", "2"));
    fox.Child("svg-ellipse", $"{prefix}-body", e => e.Property("cx", "0").Property("cy", "0").Property("rx", "13").Property("ry", "8").Property("fill", "#e66b32"));
    fox.Child("svg-path", $"{prefix}-head", p => p.Property("d", "M5 -5 L11 -18 L21 -20 L27 -12 L20 -3 L10 -1 Z").Property("fill", "#f0a24e").Property("stroke", "#d96832").Property("stroke-width", "1.5"));
    fox.Child("svg-path", $"{prefix}-ears", p => p.Property("d", "M11 -17 L10 -27 L17 -19 M20 -19 L27 -27 L26 -12").Property("fill", "#ef8b43").Property("stroke", "#d96832").Property("stroke-width", "1.3"));
    fox.Child("svg-circle", $"{prefix}-eye", c => c.Property("cx", "21").Property("cy", "-13").Property("r", "1.5").Property("fill", "#182638"));
}
