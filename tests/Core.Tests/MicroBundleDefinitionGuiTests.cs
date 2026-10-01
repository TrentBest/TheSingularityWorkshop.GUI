using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Workshop.Gui.Tests;

public sealed class MicroBundleDefinitionGuiTests
{
    [Fact]
    public void Create_RecursivelyProjectsDefinitionIntoSemanticNodes()
    {
        var definition = new MicroBundleDefinition(
            "Hydrogen",
            new MicroBundleDescriptor(42, "1.0.0"),
            [
                new MicroBundleField("AtomicNumber", MicroBundleFieldKind.Integer, 1, 1, 118),
                new MicroBundleField(
                    "Physical",
                    MicroBundleFieldKind.Object,
                    children:
                    [
                        new MicroBundleField("Density", MicroBundleFieldKind.Float, 0.09, 0, 1000),
                        new MicroBundleField("Stable", MicroBundleFieldKind.Boolean, true)
                    ])
            ]);

        var root = MicroBundleDefinitionGui.Create(definition);

        Assert.Equal("Column", root.Kind);
        Assert.Equal("Hydrogen", root.Find("name").Text);

        var atomic = root.Find("field:AtomicNumber");
        Assert.Equal(GuiKinds.IntegerSlider, atomic.Kind);
        Assert.Equal("1", atomic.Properties["value"]);
        Assert.Equal("1", atomic.Properties["minimum"]);
        Assert.Equal("118", atomic.Properties["maximum"]);

        var density = root.Find("field:Physical.Density");
        Assert.Equal(GuiKinds.FloatSlider, density.Kind);
        Assert.Equal("0.09", density.Properties["value"]);

        Assert.Equal(GuiKinds.Toggle, root.Find("field:Physical.Stable").Kind);
    }
}
