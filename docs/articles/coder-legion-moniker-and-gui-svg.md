# I May Have Just Created a Monster…

What started as a little fun making things come alive over the weekend has grown into something bigger: **GUI.SVG is on its way.**

But let's start with the fox.

## One fox. One sleeping sheepdog. One very visible jump.

![Animated fox walking along a number line, spotting a sleeping sheepdog, jumping over it, and turning around to repeat.](https://raw.githubusercontent.com/TrentBest/SimpleDemos/master/docs/assets/fox-dog-number-line.svg)

This is one of our first FSM_API examples, from [SimpleDemos](https://github.com/TrentBest/SimpleDemos). The fox walks along a number line, spots a sleeping sheepdog, jumps over it, lands, walks to the end, turns around, and repeats. The little red exclamation mark signals detection; the dog's sleepy Zs make the whole thing just a bit more alive.

The animated SVG above is the lightweight way to embed the scene in an article: it is an image, but the SVG itself contains animation. Want to experiment with the behavior? The repository also has an [interactive fox-and-sheepdog playground](https://github.com/TrentBest/SimpleDemos/blob/master/docs/fox-and-sheepdog.html) with controls for speed, jump height, detection distance, wake-up delay, and a distraction gag. To run that playground, clone or download SimpleDemos and open `docs/fox-and-sheepdog.html` in a browser; GitHub's file view shows the HTML source rather than running it as a live demo. The browser playground currently models the behavior in browser code; it does **not** execute the compiled FSM_API assembly. The separate [C# console demo](https://github.com/TrentBest/SimpleDemos/tree/master/QuickBrownFox) uses the published FSM_API package.

I also fixed a small but important bit of cartoon physics: the fox's jump follows an inverted-parabola arc, and it stays upright while travelling along that arc. It no longer rotates with the path tangent like a feather caught in a breeze. Sometimes the difference between *technically moving* and *looking right* is one little setting.

## GUI.SVG is coming soon

The fox-and-sheepdog scene is the sort of thing I want us to keep making easier to create, inspect, and reuse. **GUI.SVG is being prepared as a prerelease NuGet package.**

Here's another look at what the adapter can already do:

![GUI.SVG animated manifestation gallery](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.GUI/development/docs/assets/svg-adapter-gallery.svg)

The gallery demonstrates gradients, reusable symbols, patterns, filter effects, and declarative animation—including a pulsing marker and a point moving along a path. It is generated from the C# sample: a semantic GUI node tree is passed to `SvgGuiRenderer`, which manifests it as an SVG document. CI regenerates the gallery and checks it against the committed artifact, keeping the example connected to the code.

[Open the GUI.SVG gallery SVG](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.GUI/development/docs/assets/svg-adapter-gallery.svg) · [Read the SVG adapter guide](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/SVG_ADAPTER.md)

## Now let's see how far this goes

The SVG gallery and fox animation are the beginning, not the finish line. I want to challenge the other parts of the workshop to show what they can do:

- [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) — can the composition layer do something useful inside an SVG experience?
- [Renderer](https://github.com/TrentBest/TheSingularityWorkshop.Renderer) — can it contribute a meaningful visual scene or output?
- [Visualize](https://github.com/TrentBest/TheSingularityWorkshop.Visualize) — can we turn its visualization pipeline into another compelling SVG example?

Those are experiments to build and verify, not capabilities I'm claiming are already demonstrated here. Each repository needs to prove its own contribution. The fun is in connecting the pieces and seeing what emerges.

## Come play

If you'd like to explore the work or suggest the next thing we should make visible, start here:

- [SimpleDemos: explore the fox, sheepdog, and FSM_API](https://github.com/TrentBest/SimpleDemos)
- [Start here: explore GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/START_HERE.md)
- [Come play: try it, explore the idea, find a first contribution](https://github.com/TrentBest/TheSingularityWorkshop.GUI/blob/development/docs/COME_PLAY.md)
- [Browse TheSingularityWorkshop.GUI](https://github.com/TrentBest/TheSingularityWorkshop.GUI/tree/development)

Build something, make it visible, inspect what actually happened, then push the idea a little further. And if you have an idea for something you want to see made visible, tell me.

We might have just started a monster.
