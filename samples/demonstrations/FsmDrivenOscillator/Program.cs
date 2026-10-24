using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.FSM_API;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

const string group = "OscillatorDemo";
const string fsmName = "BoundedFunctionOscillator";
const double minimum = 0;
const double maximum = 10;
const double step = 3;
var trace = new List<(double Value, string State)>();

// Reject invalid configuration before handing the context to FSM_API.
ExpectOutOfRange(() => _ = new OscillatorContext("bad-bounds", 2, 2, 1), "minimum == maximum");
ExpectOutOfRange(() => _ = new OscillatorContext("bad-step", 0, 10, 0), "zero step");
ExpectOutOfRange(() => _ = new OscillatorContext("negative-step", 0, 10, -1), "negative step");

// The FSM owns motion. The renderer below only draws the state it receives.
var context = new OscillatorContext("main-oscillator", minimum, maximum, step);
FSM_API.Create.CreateFiniteStateMachine(fsmName, processRate: -1, processingGroup: group)
    .State("Forward",
        onEnter: null,
        onUpdate: state => Advance((OscillatorContext)state, +1),
        onExit: null)
    .State("Reverse",
        onEnter: null,
        onUpdate: state => Advance((OscillatorContext)state, -1),
        onExit: null)
    .WithInitialState("Forward")
    .Transition("Forward", "Reverse", state => ((OscillatorContext)state).Value >= ((OscillatorContext)state).Maximum)
    .Transition("Reverse", "Forward", state => ((OscillatorContext)state).Value <= ((OscillatorContext)state).Minimum)
    .BuildDefinition();

var handle = FSM_API.Create.CreateInstance(fsmName, context, group);
for (var tick = 0; tick < 12; tick++)
{
    FSM_API.Interaction.Update(group);
    trace.Add((context.Value, handle.CurrentState));
}

var expectedValues = new[] { 3d, 6d, 9d, 10d, 7d, 4d, 1d, 0d, 3d, 6d, 9d, 10d };
var expectedStates = new[] { "Forward", "Forward", "Forward", "Reverse", "Reverse", "Reverse", "Reverse", "Forward", "Forward", "Forward", "Forward", "Reverse" };
for (var i = 0; i < expectedValues.Length; i++)
{
    if (Math.Abs(trace[i].Value - expectedValues[i]) > 1e-9 || trace[i].State != expectedStates[i])
        throw new InvalidOperationException($"Tick {i + 1}: expected ({expectedValues[i]}, {expectedStates[i]}), got ({trace[i].Value}, {trace[i].State}).");
}

// Exercise a step larger than the entire range: it must clamp to the far endpoint,
// reverse, then clamp back to the near endpoint rather than wrapping or getting stuck.
var largeStepContext = new OscillatorContext("large-step-oscillator", 0, 2, 5);
const string largeGroup = "OscillatorLargeStep";
const string largeName = "LargeStepOscillator";
FSM_API.Create.CreateFiniteStateMachine(largeName, processRate: -1, processingGroup: largeGroup)
    .State("Forward", null, state => Advance((OscillatorContext)state, +1), null)
    .State("Reverse", null, state => Advance((OscillatorContext)state, -1), null)
    .WithInitialState("Forward")
    .Transition("Forward", "Reverse", state => ((OscillatorContext)state).Value >= ((OscillatorContext)state).Maximum)
    .Transition("Reverse", "Forward", state => ((OscillatorContext)state).Value <= ((OscillatorContext)state).Minimum)
    .BuildDefinition();
var largeHandle = FSM_API.Create.CreateInstance(largeName, largeStepContext, largeGroup);
FSM_API.Interaction.Update(largeGroup);
if (largeStepContext.Value != 2 || largeHandle.CurrentState != "Reverse")
    throw new InvalidOperationException("Oversized step did not clamp at the maximum and reverse.");
FSM_API.Interaction.Update(largeGroup);
if (largeStepContext.Value != 0 || largeHandle.CurrentState != "Forward")
    throw new InvalidOperationException("Oversized reverse step did not clamp at the minimum and reverse.");

var points = Enumerable.Range(0, 101).Select(i =>
{
    var x = minimum + (maximum - minimum) * i / 100d;
    var y = Math.Sin(x);
    var sx = 10 + (x - minimum) / (maximum - minimum) * 80;
    var sy = 28.125 - y * 18;
    return (X: sx, Y: sy);
}).ToArray();
string SvgPoints(IEnumerable<(double X, double Y)> values) =>
    string.Join(" ", values.Select(p => $"{p.X.ToString("0.###", CultureInfo.InvariantCulture)},{p.Y.ToString("0.###", CultureInfo.InvariantCulture)}"));

var last = trace[^1];
var markerX = 10 + (last.Value - minimum) / (maximum - minimum) * 80;
var markerY = 28.125 - Math.Sin(last.Value) * 18;
var scene = GuiBuilder.Create("bounded-oscillator", "cartesian-plane")
    .Text("FSM-driven bounded oscillator")
    .Property("minimum", "0")
    .Property("maximum", "10")
    .Property("grid", "visible")
    .Property("axes", "visible")
    .Property("aria-label", $"Sine curve with current x={last.Value.ToString("0.###", CultureInfo.InvariantCulture)} moving {last.State.ToLowerInvariant()}")
    .Child("svg-text", "plot-title", label => label.Text("y = sin(x), 0 ≤ x ≤ 10")
        .Property("x", "7").Property("y", "5").Property("font-size", "3").Property("fill", "#eef6ff"))
    .Child("svg-polyline", "sine-curve", curve => curve
        .Property("points", SvgPoints(points))
        .Property("fill", "none").Property("stroke", "#7ee0bd").Property("stroke-width", "0.7"))
    .Child("svg-circle", "current-marker", marker => marker
        .Property("cx", markerX.ToString("0.###", CultureInfo.InvariantCulture))
        .Property("cy", markerY.ToString("0.###", CultureInfo.InvariantCulture))
        .Property("r", "1.8").Property("fill", "#ffcf86"))
    .Child("svg-text", "current-value", label => label
        .Text($"x = {last.Value:0.###} | {last.State}")
        .Property("x", "7").Property("y", "52").Property("font-size", "3").Property("fill", "#ffcf86"));

var svg = new SvgGuiRenderer().Render(scene.Build(), width: 960, height: 540);
var document = XDocument.Parse(svg);
XNamespace ns = "http://www.w3.org/2000/svg";
if (document.Root?.Name != ns + "svg")
    throw new InvalidOperationException("Renderer output is not a valid SVG root.");
if (!document.Descendants(ns + "polyline").Any(e => (string?)e.Attribute("id") == "sine-curve"))
    throw new InvalidOperationException("Generated SVG is missing the sampled function curve.");
if (!document.Descendants(ns + "circle").Any(e => (string?)e.Attribute("id") == "current-marker"))
    throw new InvalidOperationException("Generated SVG is missing the runtime-state marker.");

var outputPath = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.Combine(Environment.CurrentDirectory, "07-fsm-driven-oscillator.generated.svg");
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, svg);
Console.WriteLine("PASS: FSM_API trace verified forward/reverse motion, both endpoints, repeated travel, and oversized-step clamping.");
Console.WriteLine("PASS: invalid bounds and non-positive steps are rejected.");
Console.WriteLine("Trace: " + string.Join(" → ", trace.Select(p => $"{p.Value:0.###} ({p.State})")));
Console.WriteLine("PASS: GUI.SVG generated and XML-validated the curve and current-state marker.");
Console.WriteLine($"Generated: {outputPath}");

static void Advance(OscillatorContext context, int direction)
{
    var proposed = context.Value + direction * context.StepMagnitude;
    context.Value = Math.Clamp(proposed, context.Minimum, context.Maximum);
}

static void ExpectOutOfRange(Action action, string scenario)
{
    try
    {
        action();
    }
    catch (ArgumentOutOfRangeException)
    {
        return;
    }

    throw new InvalidOperationException($"Invalid oscillator configuration was accepted: {scenario}.");
}

sealed class OscillatorContext : IStateContext
{
    public OscillatorContext(string name, double minimum, double maximum, double stepMagnitude)
    {
        if (!double.IsFinite(minimum) || !double.IsFinite(maximum) || minimum >= maximum)
            throw new ArgumentOutOfRangeException(nameof(minimum), "Oscillator bounds must be finite and minimum must be less than maximum.");
        if (!double.IsFinite(stepMagnitude) || stepMagnitude <= 0)
            throw new ArgumentOutOfRangeException(nameof(stepMagnitude), "Step magnitude must be finite and greater than zero.");

        Name = name;
        Minimum = minimum;
        Maximum = maximum;
        StepMagnitude = stepMagnitude;
        Value = minimum;
    }

    public string Name { get; set; }
    public double Minimum { get; }
    public double Maximum { get; }
    public double StepMagnitude { get; }
    public double Value { get; set; }
    public bool IsValid { get; set; } = true;
}
