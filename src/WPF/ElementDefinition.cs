namespace TheSingularityWorkshop.GUI.WPF;

public sealed class ElementDefinition
{
    public string TypeName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public Dictionary<string,string> Properties { get; set; } = new();
}
