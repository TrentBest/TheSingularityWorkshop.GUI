namespace TheSingularityWorkshop.Workshop.Gui.Tests;

public sealed class ReflectionGuiBuilderTests
{
    [Fact]
    public void Create_ProjectsAnUnknownModelWithoutReferencingItsType()
    {
        var model = new TestExperience
        {
            Id = 42,
            Name = "Test Experience",
            Enabled = true,
            Weight = 1.5f,
            Settings = new NestedSettings { Label = "Nested" }
        };

        var root = ReflectionGuiBuilder.Create(model);

        Assert.Equal(GuiKinds.Object, root.Kind);
        Assert.Equal("42", root.Find("root.Id").Properties["value"]);
        Assert.Equal(GuiKinds.TextBox, root.Find("root.Name").Kind);
        Assert.Equal(GuiKinds.Toggle, root.Find("root.Enabled").Kind);
        Assert.Equal(GuiKinds.FloatSlider, root.Find("root.Weight").Kind);
        Assert.Equal("Nested", root.Find("root.Settings.Label").Properties["value"]);
    }

    [Fact]
    public void Create_HandlesCollectionsAndCycles()
    {
        var model = new CollectionModel();
        model.Self = model;
        model.Items.Add(7);
        model.Items.Add(11);

        var root = ReflectionGuiBuilder.Create(model);

        Assert.Equal("2", root.Find("root.Items").Properties["count"]);
        Assert.Equal("7", root.Find("root.Items[0]").Properties["value"]);
        Assert.Equal("11", root.Find("root.Items[1]").Properties["value"]);
        Assert.Equal("<cycle>", root.Find("root.Self").Properties["value"]);
    }

    private sealed class TestExperience
    {
        public ulong Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public bool Enabled { get; init; }
        public float Weight { get; init; }
        public NestedSettings Settings { get; init; } = new();
    }

    private sealed class NestedSettings
    {
        public string Label { get; init; } = string.Empty;
    }

    private sealed class CollectionModel
    {
        public List<int> Items { get; } = new();
        public CollectionModel? Self { get; set; }
    }
}
