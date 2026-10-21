namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Semantic factory methods for the canonical platform-neutral GUI builder.</summary>
public static class GuiBuilders
{
    /// <summary>Creates a panel builder.</summary>
    public static GuiBuilder Panel(string id) => GuiBuilder.Create("Panel", id);
    /// <summary>Creates a column builder.</summary>
    public static GuiBuilder Column(string id) => GuiBuilder.Create("Column", id);
    /// <summary>Creates a text builder.</summary>
    public static GuiBuilder Text(string id, string text) => GuiBuilder.Create("Text", id).Text(text);
    /// <summary>Creates a button builder.</summary>
    public static GuiBuilder Button(string id, string text) => GuiBuilder.Create("Button", id).Text(text);
    /// <summary>Creates a warning builder.</summary>
    public static GuiBuilder Warning(string id, string text) => GuiBuilder.Create("Warning", id).Text(text);
}
