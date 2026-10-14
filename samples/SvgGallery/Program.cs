using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

var output = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath("docs/assets/svg-adapter-gallery.svg");

var scene = GuiBuilder.Create("svg-scene", "gui-svg-gallery")
    .Text("GUI.SVG manifestation gallery")
    .Property("description", "A single semantic scene demonstrating vector geometry, reusable definitions, text, transforms, and declarative SVG animation.");

scene.Child("svg-rect", "background", node => node
    .Property("width", "100").Property("height", "56.25")
    .Property("fill", "#0b1725"));

scene.Child("svg-defs", "definitions", defs => defs
    .Child("svg-linear-gradient", "workshop-gradient", gradient => gradient
        .Property("x1", "0%").Property("y1", "0%").Property("x2", "100%").Property("y2", "100%")
        .Child("svg-stop", "gradient-start", stop => stop
            .Property("offset", "0%").Property("stop-color", "#173b4c"))
        .Child("svg-stop", "gradient-end", stop => stop
            .Property("offset", "100%").Property("stop-color", "#302b58")))
    .Child("svg-clip-path", "panel-clip", clip => clip
        .Child("svg-rect", "clip-panel", rect => rect
            .Property("x", "4").Property("y", "4").Property("width", "92").Property("height", "48.25")
            .Property("rx", "3")))
    .Child("svg-pattern", "circuit-pattern", pattern => pattern
        .Property("width", "8").Property("height", "8").Property("patternUnits", "userSpaceOnUse")
        .Child("svg-path", "pattern-line", path => path.Property("d", "M0 8L8 0")
            .Property("stroke", "#7ee0bd").Property("stroke-width", "0.3")))
    .Child("svg-marker", "arrowhead", marker => marker
        .Property("refX", "5").Property("refY", "5").Property("markerWidth", "4")
        .Property("markerHeight", "4").Property("orient", "auto")
        .Child("svg-polygon", "arrowhead-shape", polygon => polygon.Property("points", "0,0 10,5 0,10")
            .Property("fill", "#7ee0bd")))
    .Child("svg-filter", "soft-glow", filter => filter
        .Child("svg-fe-gaussian-blur", "glow-blur", blur => blur.Property("in", "SourceGraphic")
            .Property("stdDeviation", "0.5")))
    .Child("svg-filter", "surface-relief", filter => filter
        .Child("svg-fe-turbulence", "relief-noise", noise => noise
            .Property("baseFrequency", "0.025 0.06").Property("numOctaves", "2").Property("type", "fractalNoise"))
        .Child("svg-fe-displacement-map", "relief-displacement", displacement => displacement
            .Property("in", "SourceGraphic").Property("in2", "relief-noise").Property("scale", "1.5")
            .Property("xChannelSelector", "R").Property("yChannelSelector", "G"))
        .Child("svg-fe-component-transfer", "relief-tone", transfer => transfer
            .Child("svg-fe-func-r", "relief-red-curve", curve => curve.Property("type", "gamma")
                .Property("amplitude", "1").Property("exponent", "0.95").Property("offset", "0"))))
    .Child("svg-filter", "surface-lighting", filter => filter
        .Child("svg-fe-diffuse-lighting", "diffuse-study", lighting => lighting
            .Property("surfaceScale", "1.5").Property("diffuseConstant", "0.8")
            .Child("svg-fe-distant-light", "gallery-light", light => light
                .Property("azimuth", "225").Property("elevation", "40"))))
    .Child("svg-symbol", "star-symbol", symbol => symbol.Property("viewBox", "0 0 10 10")
        .Child("svg-polygon", "star-shape", polygon => polygon.Property("points", "5,0 6.5,3.5 10,4 7.5,6.5 8.2,10 5,8 1.8,10 2.5,6.5 0,4 3.5,3.5")
            .Property("fill", "#ffcf86")))
    .Child("svg-path", "orbit-path", path => path.Property("d", "M12 38 C24 12 42 48 58 24 S82 16 90 32")
        .Property("fill", "none")));

scene.Child("svg-rect", "panel", panel => panel
    .Property("x", "4").Property("y", "4").Property("width", "92").Property("height", "48.25")
    .Property("rx", "3").Property("fill", "url(#workshop-gradient)")
    .Property("stroke", "#426478").Property("stroke-width", "0.35")
    .Property("clip-path", "url(#panel-clip)"));

scene.Child("svg-path", "curve", path => path
    .Property("d", "M9 37 C22 14 34 49 47 28 S72 13 86 31")
    .Property("fill", "none").Property("stroke", "#7ee0bd")
    .Property("stroke-width", "0.7").Property("stroke-linecap", "round")
    .Property("marker-end", "url(#arrowhead)"));

scene.Child("svg-polygon", "facet", polygon => polygon
    .Property("points", "73,14 84,10 91,18 83,24")
    .Property("fill", "#ffcf86").Property("fill-opacity", "0.9")
    .Property("stroke", "#fff1ce").Property("stroke-width", "0.25")
    .Property("filter", "url(#surface-relief)"));

scene.Child("svg-circle", "glow-swatch", glow => glow
    .Property("cx", "65").Property("cy", "16").Property("r", "2")
    .Property("fill", "#7ee0bd").Property("filter", "url(#soft-glow)"));

scene.Child("svg-polygon", "lit-swatch", lit => lit
    .Property("points", "63,25 67,21 71,25")
    .Property("fill", "#ffcf86").Property("filter", "url(#surface-lighting)"));

scene.Child("svg-rect", "pattern-swatch", panel => panel
    .Property("x", "86").Property("y", "39").Property("width", "7").Property("height", "7")
    .Property("fill", "url(#circuit-pattern)").Property("stroke", "#426478").Property("stroke-width", "0.25"));

scene.Child("svg-use", "reused-star", use => use
    .Property("href", "#star-symbol").Property("x", "86").Property("y", "10")
    .Property("width", "7").Property("height", "7"));

scene.Child("svg-circle", "animated-marker", marker => marker
    .Property("cx", "47").Property("cy", "28").Property("r", "1.6")
    .Property("fill", "#ffcf86").Property("stroke", "#fff1ce").Property("stroke-width", "0.25")
    .Child("svg-animate", "marker-pulse", animation => animation
        .Property("attributeName", "r").Property("values", "1.6;2.8;1.6")
        .Property("keyTimes", "0;0.5;1").Property("dur", "2.4s")
        .Property("repeatCount", "indefinite"))
    .Child("svg-set", "marker-emphasis", set => set.Property("attributeName", "stroke-width")
        .Property("to", "0.5").Property("begin", "2s")));

scene.Child("svg-a", "heading-link", link => link
    .Property("href", "https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/SVG_ADAPTER.md")
    .Property("target", "_blank").Property("rel", "noopener")
    .Child("svg-text", "heading", text => text
        .Text("GUI.SVG")
        .Property("x", "9").Property("y", "12")
        .Property("font-family", "sans-serif").Property("font-size", "4")
        .Property("font-weight", "700").Property("fill", "#eef6ff")));

scene.Child("svg-circle", "motion-scout", scout => scout
    .Property("cx", "12").Property("cy", "38").Property("r", "0.7")
    .Property("fill", "#fff1ce").Property("stroke", "#0b1725").Property("stroke-width", "0.2")
    .Child("svg-animate-motion", "scout-motion", motion => motion
        .Property("dur", "5s").Property("repeatCount", "indefinite").Property("rotate", "auto")
        .Child("svg-mpath", "scout-route-reference", reference => reference.Property("href", "#orbit-path"))));

scene.Child("svg-text", "caption", text => text
    .Text("Semantic model → SVG manifestation")
    .Property("x", "9").Property("y", "47")
    .Property("font-family", "sans-serif").Property("font-size", "2.4")
    .Property("fill", "#d6e7f4"));

var svg = new SvgGuiRenderer().Render(scene.Build(), width: 1280, height: 720);
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
File.WriteAllText(output, svg);
Console.WriteLine($"GUI.SVG gallery generated: {output}");
