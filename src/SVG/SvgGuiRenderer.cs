using System.Globalization;
using System.Net;
using System.Text;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.SVG;

/// <summary>
/// Serializes the currently supported GUI.Core semantic tree as accessible SVG.
/// This is a deterministic first renderer, not a browser runtime or interaction engine.
/// </summary>
public sealed class SvgGuiRenderer
{
    /// <summary>Render a semantic GUI tree to a standalone SVG document.</summary>
    public string Render(GuiNode root, SvgRenderOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(root);
        options ??= new SvgRenderOptions();
        options.Validate();

        var sb = new StringBuilder();
        sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" role=\"img\" viewBox=\"0 0 ")
          .Append(options.Width.ToString(CultureInfo.InvariantCulture)).Append(' ')
          .Append(options.Height.ToString(CultureInfo.InvariantCulture)).Append("\" width=\"")
          .Append(options.Width.ToString(CultureInfo.InvariantCulture)).Append("\" height=\"")
          .Append(options.Height.ToString(CultureInfo.InvariantCulture)).Append("\"");

        if (!string.IsNullOrWhiteSpace(options.Title)) sb.Append(" aria-labelledby=\"svg-title\"");
        else if (!string.IsNullOrWhiteSpace(options.Description)) sb.Append(" aria-describedby=\"svg-description\"");
        sb.Append(">");

        if (!string.IsNullOrWhiteSpace(options.Title))
            sb.Append("<title id=\"svg-title\">").Append(E(options.Title)).Append("</title>");
        if (!string.IsNullOrWhiteSpace(options.Description))
            sb.Append("<desc id=\"svg-description\">").Append(E(options.Description)).Append("</desc>");
        if (options.Background is not null)
            sb.Append("<rect width=\"100%\" height=\"100%\" fill=\"").Append(E(options.Background)).Append("\"/>");

        sb.Append("<g font-family=\"system-ui, sans-serif\" font-size=\"16\" fill=\"currentColor\">");
        RenderNode(sb, root, 16, 28, Math.Max(100, options.Width - 32), 0);
        sb.Append("</g></svg>");
        return sb.ToString();
    }

    private static void RenderNode(StringBuilder sb, GuiNode node, int x, int y, int width, int depth)
    {
        if (depth > 64) throw new InvalidOperationException("GUI tree exceeds the SVG renderer's maximum depth (64).");
        var label = node.Text ?? node.Id;
        var kind = node.Kind ?? "Unknown";
        var safeWidth = Math.Max(48, width - depth * 12);

        switch (kind.ToLowerInvariant())
        {
            case "text":
                sb.Append("<text x=\"").Append(x).Append("\" y=\"").Append(y).Append("\">")
                  .Append(E(label)).Append("</text>");
                y += 28;
                break;
            case "button":
                sb.Append("<g role=\"img\" aria-label=\"Button: ").Append(E(label)).Append("\"><rect x=\"")
                  .Append(x).Append("\" y=\"").Append(y - 20).Append("\" width=\"").Append(Math.Min(safeWidth, 240))
                  .Append("\" height=\"34\" rx=\"6\" fill=\"#1d4ed8\"/><text x=\"")
                  .Append(x + 12).Append("\" y=\"").Append(y + 2)
                  .Append("\" fill=\"white\">").Append(E(label)).Append("</text></g>");
                y += 48;
                break;
            case "separator":
                sb.Append("<path d=\"M ").Append(x).Append(' ').Append(y).Append(" h ").Append(safeWidth)
                  .Append("\" stroke=\"currentColor\" stroke-opacity=\".25\"/>");
                y += 16;
                break;
            case "warning":
                sb.Append("<g><rect x=\"").Append(x).Append("\" y=\"").Append(y - 20).Append("\" width=\"")
                  .Append(Math.Min(safeWidth, 600)).Append("\" height=\"36\" rx=\"4\" fill=\"#fef3c7\" stroke=\"#d97706\"/>")
                  .Append("<text x=\"").Append(x + 10).Append("\" y=\"").Append(y + 2)
                  .Append("\" fill=\"#78350f\">").Append(E(label)).Append("</text></g>");
                y += 48;
                break;
            default:
                sb.Append("<g><rect x=\"").Append(x).Append("\" y=\"").Append(y - 20).Append("\" width=\"")
                  .Append(Math.Min(safeWidth, 600)).Append("\" height=\"32\" rx=\"4\" fill=\"none\" stroke=\"currentColor\" stroke-opacity=\".3\"/>")
                  .Append("<text x=\"").Append(x + 8).Append("\" y=\"").Append(y + 1).Append("\">")
                  .Append(E(string.IsNullOrWhiteSpace(label) ? kind : label)).Append("</text></g>");
                y += 42;
                break;
        }

        foreach (var child in node.Children)
        {
            RenderNode(sb, child, x + 12, y, safeWidth - 12, depth + 1);
            y += 44;
        }
    }

    private static string E(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
