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

        foreach (var property in node.Properties)
            builder.AddAttribute(3, property.Key, property.Value);

        if (node.Text is not null)
            builder.AddContent(4, node.Text);

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