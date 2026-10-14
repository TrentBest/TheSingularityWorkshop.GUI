using System.Globalization;
using System.Security;
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
        var width = options.Width.ToString(CultureInfo.InvariantCulture);
        var height = options.Height.ToString(CultureInfo.InvariantCulture);
        sb.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" role=\"img\" viewBox=\"0 0 ")
          .Append(width).Append(' ').Append(height).Append("\" width=\"").Append(width)
          .Append("\" height=\"").Append(height).Append('"');

        if (!string.IsNullOrWhiteSpace(options.Title)) sb.Append(" aria-labelledby=\"svg-title\"");
        else if (!string.IsNullOrWhiteSpace(options.Description)) sb.Append(" aria-describedby=\"svg-description\"");
        sb.Append('>');

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

    // Returns the first y-coordinate available after this node and all descendants.
    private static int RenderNode(StringBuilder sb, GuiNode node, int x, int y, int width, int depth)
    {
        if (depth > 64) throw new InvalidOperationException("GUI tree exceeds the SVG renderer's maximum depth (64).");
        var label = node.Text ?? node.Id;
        var kind = node.Kind;
        var safeWidth = Math.Max(48, width - depth * 12);
        int nextY;

        switch (kind.ToLowerInvariant())
        {
            case "text":
                sb.Append("<text x=\"").Append(N(x)).Append("\" y=\"").Append(N(y)).Append("\">")
                  .Append(E(label)).Append("</text>");
                nextY = y + 28;
                break;
            case "panel":
            case "stack":
            case "row":
            case "column":
            case "object":
                sb.Append("<g aria-label=\"").Append(E(string.IsNullOrWhiteSpace(node.Text) ? kind : node.Text))
                  .Append("\"><rect x=\"").Append(N(x)).Append("\" y=\"").Append(N(y - 18))
                  .Append("\" width=\"").Append(N(Math.Min(safeWidth, 640))).Append("\" height=\"28\" rx=\"4\" fill=\"none\" stroke=\"currentColor\" stroke-opacity=\".22\"/>")
                  .Append("<text x=\"").Append(N(x + 8)).Append("\" y=\"").Append(N(y + 1)).Append("\" font-size=\"12\" opacity=\".7\">")
                  .Append(E(kind + ": " + (string.IsNullOrWhiteSpace(node.Text) ? node.Id : node.Text))).Append("</text></g>");
                nextY = y + 18;
                break;
            case "textbox":
                sb.Append("<g aria-label=\"").Append(E("Text input: " + label)).Append("\"><rect x=\"").Append(N(x)).Append("\" y=\"").Append(N(y - 20))
                  .Append("\" width=\"").Append(N(Math.Min(safeWidth, 320))).Append("\" height=\"34\" rx=\"4\" fill=\"none\" stroke=\"currentColor\" stroke-opacity=\".55\"/>")
                  .Append("<text x=\"").Append(N(x + 8)).Append("\" y=\"").Append(N(y + 2)).Append("\">").Append(E(label)).Append("</text></g>");
                nextY = y + 42;
                break;
            case "integerslider":
            case "floatslider":
                sb.Append("<g role=\"img\" aria-label=\"").Append(E(kind + ": " + label)).Append("\"><text x=\"").Append(N(x)).Append("\" y=\"").Append(N(y)).Append("\">")
                  .Append(E(label)).Append("</text><path d=\"M ").Append(N(x)).Append(' ').Append(N(y + 12)).Append(" h ")
                  .Append(N(Math.Min(safeWidth, 280))).Append("\" stroke=\"currentColor\" stroke-opacity=\".45\" stroke-width=\"4\" stroke-linecap=\"round\"/><circle cx=\"")
                  .Append(N(x + Math.Min(safeWidth, 280) / 2)).Append("\" cy=\"").Append(N(y + 12)).Append("\" r=\"7\" fill=\"#2563eb\"/></g>");
                nextY = y + 34;
                break;
            case "toggle":
            {
                var enabled = node.Properties.TryGetValue("Value", out var toggleValue) &&
                    (string.Equals(toggleValue, "true", StringComparison.OrdinalIgnoreCase) || toggleValue == "1");
                sb.Append("<g role=\"img\" aria-label=\"").Append(E("Toggle " + label + (enabled ? ", on" : ", off"))).Append("\"><rect x=\"")
                  .Append(N(x)).Append("\" y=\"").Append(N(y - 15)).Append("\" width=\"38\" height=\"20\" rx=\"10\" fill=\"")
                  .Append(enabled ? "#2563eb" : "#9ca3af").Append("\"/><circle cx=\"").Append(N(x + (enabled ? 27 : 11)))
                  .Append("\" cy=\"").Append(N(y - 5)).Append("\" r=\"7\" fill=\"white\"/><text x=\"").Append(N(x + 48))
                  .Append("\" y=\"").Append(N(y)).Append("\">").Append(E(label)).Append("</text></g>");
                nextY = y + 26;
                break;
            }
            case "image":
                sb.Append("<g role=\"img\" aria-label=\"").Append(E("Image: " + label)).Append("\"><rect x=\"").Append(N(x)).Append("\" y=\"")
                  .Append(N(y - 20)).Append("\" width=\"").Append(N(Math.Min(safeWidth, 240))).Append("\" height=\"72\" rx=\"4\" fill=\"#e5e7eb\" stroke=\"#9ca3af\"/>")
                  .Append("<text x=\"").Append(N(x + 8)).Append("\" y=\"").Append(N(y + 18)).Append("\" fill=\"#374151\">")
                  .Append(E(string.IsNullOrWhiteSpace(node.Source) ? label : "Image: " + label)).Append("</text></g>");
                nextY = y + 82;
                break;
            case "button":
                sb.Append("<g role=\"img\" aria-label=\"").Append(E("Button: " + label)).Append("\"><rect x=\"")
                  .Append(N(x)).Append("\" y=\"").Append(N(y - 20)).Append("\" width=\"").Append(N(Math.Min(safeWidth, 240)))
                  .Append("\" height=\"34\" rx=\"6\" fill=\"#1d4ed8\"/><text x=\"")
                  .Append(N(x + 12)).Append("\" y=\"").Append(N(y + 2))
                  .Append("\" fill=\"white\">").Append(E(label)).Append("</text></g>");
                nextY = y + 42;
                break;
            case "separator":
                sb.Append("<path role=\"img\" aria-label=\"").Append(E("Separator: " + label)).Append("\" d=\"M ")
                  .Append(N(x)).Append(' ').Append(N(y)).Append(" h ").Append(N(safeWidth))
                  .Append("\" stroke=\"currentColor\" stroke-opacity=\".25\"/>");
                nextY = y + 16;
                break;
            case "warning":
                sb.Append("<g><rect x=\"").Append(N(x)).Append("\" y=\"").Append(N(y - 20)).Append("\" width=\"")
                  .Append(N(Math.Min(safeWidth, 600))).Append("\" height=\"36\" rx=\"4\" fill=\"#fef3c7\" stroke=\"#d97706\"/>")
                  .Append("<text x=\"").Append(N(x + 10)).Append("\" y=\"").Append(N(y + 2))
                  .Append("\" fill=\"#78350f\">").Append(E(label)).Append("</text></g>");
                nextY = y + 44;
                break;
            default:
                sb.Append("<g><rect x=\"").Append(N(x)).Append("\" y=\"").Append(N(y - 20)).Append("\" width=\"")
                  .Append(N(Math.Min(safeWidth, 600))).Append("\" height=\"32\" rx=\"4\" fill=\"none\" stroke=\"currentColor\" stroke-opacity=\".3\"/>")
                  .Append("<text x=\"").Append(N(x + 8)).Append("\" y=\"").Append(N(y + 1)).Append("\">")
                  .Append(E(string.IsNullOrWhiteSpace(label) ? kind : label)).Append("</text></g>");
                nextY = y + 40;
                break;
        }

        // Layout is intentionally a simple vertical stack until Core exposes a shared layout contract.
        foreach (var child in node.Children)
            nextY = RenderNode(sb, child, x + 12, nextY + 4, Math.Max(48, safeWidth - 12), depth + 1);
        return nextY;
    }

    private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);
    private static string E(string? value) => SecurityElement.Escape(value ?? string.Empty) ?? string.Empty;
}