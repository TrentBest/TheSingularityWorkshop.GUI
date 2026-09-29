namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Default platform-neutral GUI builder vocabulary.
/// These builders create semantic nodes only; manifestation belongs to a platform adapter.
/// </summary>
public static class GuiBuilders
{
    public static GuiBuilder Panel(string id) => GuiBuilder.Create(GuiKinds.Panel, id);

    public static GuiBuilder Stack(string id, string orientation = "Vertical") =>
        GuiBuilder.Create(GuiKinds.Stack, id).Property("orientation", orientation);

    public static GuiBuilder Row(string id) =>
        GuiBuilder.Create(GuiKinds.Row, id).Property("orientation", "Horizontal");

    public static GuiBuilder Column(string id) =>
        GuiBuilder.Create(GuiKinds.Column, id).Property("orientation", "Vertical");

    public static GuiBuilder Text(string id, string text) =>
        GuiBuilder.Create(GuiKinds.Text, id).Text(text);

    public static GuiBuilder Button(string id, string text) =>
        GuiBuilder.Create(GuiKinds.Button, id).Text(text);

    public static GuiBuilder Image(string id, string source) =>
        GuiBuilder.Create(GuiKinds.Image, id).Image(source);

    public static GuiBuilder Warning(string id, string text) =>
        GuiBuilder.Create(GuiKinds.Warning, id).Text(text);

    public static GuiBuilder Separator(string id) =>
        GuiBuilder.Create(GuiKinds.Separator, id);

    public static GuiBuilder TextBox(string id, string? text = null)
    {
        var builder = GuiBuilder.Create(GuiKinds.TextBox, id);
        if (text is not null)
            builder.Property("value", text);
        return builder;
    }
}
