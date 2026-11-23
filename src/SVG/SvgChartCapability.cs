namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Describes one chart-family capability declared by the SVG adapter.</summary>
/// <param name="ChartKind">Stable, adapter-specific chart-kind identifier.</param>
/// <param name="IsSupported">Whether the adapter currently renders this chart kind.</param>
/// <param name="Constraint">The supported subset or reason the chart kind is unavailable.</param>
public sealed record SvgChartCapability(string ChartKind, bool IsSupported, string Constraint);
