using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace TheSingularityWorkshop.Workshop.Gui;

public static class BlazorGuiRenderer
{
    public static RenderFragment Render(GuiNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return builder => RenderNode(builder, node);
    }

    private static void RenderNode(RenderTreeBuilder builder, GuiNode node)
    {
        builder.OpenElement(0, ResolveTag(node.Kind));
        builder.AddAttribute(1, "id", node.Id);

        if (!string.IsNullOrWhiteSpace(node.Source))
            builder.AddAttribute(2, "src", node.Source);

        var sequence = 3;
        var style = new List<string>();

        foreach (var property in node.Properties)
        {
            if (property.Key.StartsWith("style:", StringComparison.Ordinal))
            {
                style.Add($"{property.Key["style:".Length..]}:{property.Value}");
                continue;
            }

            builder.AddAttribute(sequence++, property.Key, property.Value);
        }

        if (style.Count > 0)
            builder.AddAttribute(sequence++, "style", string.Join(";", style));

        if (node.Text is not null)
            builder.AddContent(sequence++, node.Text);

        foreach (var child in node.Children)
            RenderNode(builder, child);

        builder.CloseElement();
    }

    private static string ResolveTag(string kind) => kind switch
    {
        "Panel" => "div",
        "Button" => "button",
        "Image" => "img",
        "Text" => "span",
        "Warning" => "aside",
        _ => "div"
    };
}
