using TheSingularityWorkshop.MicroBundleDomain;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Recursively manifests a MicroBundleDefinition as a platform-neutral semantic GUI tree.
/// The definition supplies the schema; this type supplies no MicroBundle-specific editor layout.
/// </summary>
public static class MicroBundleDefinitionGui
{
    public static GuiNode Create(MicroBundleDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var root = GuiBuilders.Column($"microbundle:{definition.Id}")
            .Child(GuiBuilders.Text("name", definition.Name))
            .Child(GuiBuilders.Text("version", definition.Version));

        foreach (var field in definition.Fields)
            root.Child(CreateField(field, $"field:{field.Name}"));

        return root.Build();
    }

    private static GuiBuilder CreateField(MicroBundleField field, string id)
    {
        var builder = field.Kind switch
        {
            MicroBundleFieldKind.Integer => GuiBuilder.Create(GuiKinds.IntegerSlider, id),
            MicroBundleFieldKind.Float => GuiBuilder.Create(GuiKinds.FloatSlider, id),
            MicroBundleFieldKind.Boolean => GuiBuilder.Create(GuiKinds.Toggle, id),
            MicroBundleFieldKind.Object => GuiBuilder.Create(GuiKinds.Object, id),
            _ => GuiBuilder.Create(GuiKinds.TextBox, id)
        };

        builder.Property("label", field.Name);
        builder.Property("type", field.Kind.ToString());

        if (field.DefaultValue is not null)
            builder.Property("value", field.DefaultValue.ToString()!);

        if (field.Minimum is not null)
            builder.Property("minimum", field.Minimum.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));

        if (field.Maximum is not null)
            builder.Property("maximum", field.Maximum.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));

        foreach (var child in field.Children)
            builder.Child(CreateField(child, $"{id}.{child.Name}"));

        return builder;
    }
}
