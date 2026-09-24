namespace TheSingularityWorkshop.GUI.WPF;

[Serializable]
public sealed class GuiConfiguration
{
    public string Name { get; set; } = "DefaultLayout";
    public string PanelType { get; set; } = "StackPanel";
    public List<ElementDefinition> Elements { get; set; } = new();
}
