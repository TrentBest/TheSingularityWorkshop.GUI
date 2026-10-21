using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Renders the generic DataCard hierarchy as a static, inspectable SVG diagram.</summary>
/// <remarks>
/// This renderer consumes an already-built card model; it does not parse source code or authorize edits.
/// The edit-mode flag only controls whether a permitted insertion affordance is shown. A domain command
/// handler must validate any requested mutation. Expansion state is supplied by the presentation.
/// </remarks>
public sealed class SvgDataCardRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private const double Left = 18;
    private const double Top = 18;
    private const double HeaderHeight = 34;
    private const double ChildGap = 10;
    private const double NestIndent = 18;
    private const double BottomPadding = 12;
    private const double SubtitleHeight = 16;

    /// <summary>Renders a card tree into a self-contained SVG document.</summary>
    /// <param name="root">The platform-neutral card hierarchy.</param>
    /// <param name="editMode">Whether edit-only affordances may be shown.</param>
    /// <param name="expansionState">Optional current expansion state keyed by stable card ID.</param>
    /// <param name="width">SVG width in pixels.</param>
    /// <returns>An SVG document string.</returns>
    public string Render(
        DataCard root,
        bool editMode = false,
        IReadOnlyDictionary<string, bool>? expansionState = null,
        int width = 960)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (width < 240)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 240 pixels.");

        var height = Measure(root, 0, expansionState) + Top * 2;
        var svg = new XElement(Svg + "svg",
            new XAttribute("width", width),
            new XAttribute("height", Math.Ceiling(height)),
            new XAttribute("viewBox", $"0 0 {width} {F(height)}"),
            new XAttribute("role", "img"),
            new XAttribute("aria-label", $"Data-card hierarchy: {root.Title}"),
            new XAttribute("font-family", "Segoe UI, sans-serif"),
            new XAttribute("font-size", "12"),
            new XElement(Svg + "title", $"Data-card hierarchy: {root.Title}"),
            new XElement(Svg + "desc", "Nested cards represent containment. Collapsed summaries show direct child counts. Add affordances appear only in edit mode when the model permits insertion."));

        DrawCard(svg, root, Left, Top, width - Left * 2, 0, editMode, expansionState);
        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg)
            .ToString(SaveOptions.DisableFormatting);
    }

    private static double Measure(DataCard card, int depth, IReadOnlyDictionary<string, bool>? state)
    {
        var expanded = IsExpanded(card, state);
        var height = HeaderHeight + (string.IsNullOrWhiteSpace(card.Subtitle) ? 0 : SubtitleHeight) + BottomPadding;
        if (!expanded)
            return height;

        foreach (var child in card.Children)
            height += ChildGap + Measure(child, depth + 1, state);
        return height;
    }

    private static double DrawCard(
        XElement parent,
        DataCard card,
        double x,
        double y,
        double availableWidth,
        int depth,
        bool editMode,
        IReadOnlyDictionary<string, bool>? state)
    {
        var expanded = IsExpanded(card, state);
        var cardWidth = Math.Max(120, availableWidth - (depth == 0 ? 0 : NestIndent));
        var summary = card.GetCollapsedSummary(expanded);
        var headerText = summary is null ? card.Title : $"{card.Title}  ·  {summary}";
        var group = new XElement(Svg + "g",
            new XAttribute("data-card-id", card.Id),
            new XAttribute("data-card-kind", card.Kind),
            new XAttribute("role", "group"),
            new XAttribute("aria-label", headerText));

        var palette = PaletteFor(card.Kind);
        group.Add(new XElement(Svg + "rect",
            new XAttribute("x", F(x)),
            new XAttribute("y", F(y)),
            new XAttribute("width", F(cardWidth)),
            new XAttribute("height", F(HeaderHeight)),
            new XAttribute("rx", "2"),
            new XAttribute("fill", palette.Fill),
            new XAttribute("stroke", palette.Stroke),
            new XAttribute("stroke-width", palette.StrokeWidth)));

        group.Add(new XElement(Svg + "text",
            new XAttribute("x", F(x + 10)),
            new XAttribute("y", F(y + 21)),
            new XAttribute("fill", palette.Text),
            new XAttribute("font-weight", "600"),
            headerText));

        if (!string.IsNullOrWhiteSpace(card.Subtitle))
            group.Add(new XElement(Svg + "text",
                new XAttribute("x", F(x + 10)),
                new XAttribute("y", F(y + HeaderHeight + 12)),
                new XAttribute("fill", palette.Text),
                card.Subtitle));

        if (editMode && card.CanRequestChildInsertion)
        {
            group.Add(new XElement(Svg + "text",
                new XAttribute("x", F(x + cardWidth - 18)),
                new XAttribute("y", F(y + 21)),
                new XAttribute("fill", "#7ee0bd"),
                new XAttribute("font-size", "16"),
                new XAttribute("font-weight", "700"),
                new XAttribute("text-anchor", "middle"),
                new XAttribute("aria-label", $"Request child insertion for {card.Title}"),
                "+"));
        }

        parent.Add(group);
        var nextY = y + HeaderHeight + (string.IsNullOrWhiteSpace(card.Subtitle) ? 0 : SubtitleHeight) + BottomPadding;
        if (!expanded)
            return nextY;

        foreach (var child in card.Children)
        {
            nextY += ChildGap;
            nextY = DrawCard(group, child, x + NestIndent, nextY,
                cardWidth - NestIndent, depth + 1, editMode, state);
        }

        return nextY;
    }

    // UML containment uses semantic roles rather than depth-only styling.
    // Namespace, type, and member cards keep their identity at every nesting level.
    private static (string Fill, string Stroke, string StrokeWidth, string Text) PaletteFor(string kind) =>
        kind.ToLowerInvariant() switch
        {
            // Pastel fills are deliberately paired with dark text for legibility.
            "namespace" => ("#eef0ff", "#4f46e5", "4", "#20205c"),
            "class" or "record" or "struct" or "interface" or "enum" or "type" =>
                ("#fff0e5", "#ff823d", "4", "#6b2e0c"),
            "property" => ("#fffde0", "#f2e83d", "4", "#514d00"),
            "field" => ("#eaf4ff", "#4a90e2", "3", "#173c66"),
            "method" or "constructor" or "event" => ("#e9fbf1", "#2e9d70", "3", "#15563c"),
            "source-file" => ("#f1f5f9", "#64748b", "2", "#334155"),
            _ => ("#f8fafc", "#94a3b8", "2", "#334155")
        };

    private static bool IsExpanded(DataCard card, IReadOnlyDictionary<string, bool>? state) =>
        state is not null && state.TryGetValue(card.Id, out var expanded)
            ? expanded
            : card.IsExpandedByDefault;

    private static string F(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
