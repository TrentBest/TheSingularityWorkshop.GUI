using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Builds a semantic node describing a coordinate system.</summary>
public sealed class CoordinateSystemGuiBuilder
{
    private readonly GuiBuilder _builder;
    private readonly CoordinateSystem _coordinates;
    private CoordinateSystemGuiBuilder(string id,CoordinateSystem coordinates)
    {
        _coordinates=coordinates;
        _builder=GuiBuilder.Create("coordinate-system",id)
            .Property("coordinate-space","cartesian")
            .Property("minimum",coordinates.Minimum.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Property("maximum",coordinates.Maximum.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Property("origin-x",coordinates.OriginX.ToString(System.Globalization.CultureInfo.InvariantCulture))
            .Property("origin-y",coordinates.OriginY.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
    /// <summary>Creates a builder or adapter for the supplied root or element.</summary>
    public static CoordinateSystemGuiBuilder Create(string id)=>new(id,CoordinateSystem.Normalized);
    /// <summary>Creates a builder or adapter for the supplied root or element.</summary>
    public static CoordinateSystemGuiBuilder Create(string id,CoordinateSystem coordinates)=>new(id,coordinates);
    /// <summary>Sets or adds text content.</summary>
    public CoordinateSystemGuiBuilder Text(string text){_builder.Text(text);return this;}
    /// <summary>Provides the Axes member.</summary>
    public CoordinateSystemGuiBuilder Axes(bool visible=true){_builder.Property("axes",visible?"visible":"hidden");return this;}
    /// <summary>Provides the Grid member.</summary>
    public CoordinateSystemGuiBuilder Grid(bool visible=true){_builder.Property("grid",visible?"visible":"hidden");return this;}
    /// <summary>Provides the Origin member.</summary>
    public CoordinateSystemGuiBuilder Origin(bool visible=true){_builder.Property("origin",visible?"visible":"hidden");return this;}
    /// <summary>Provides the Bounds member.</summary>
    public CoordinateSystemGuiBuilder Bounds(bool visible=true){_builder.Property("bounds",visible?"visible":"hidden");return this;}
    /// <summary>Provides the Point member.</summary>
    public CoordinateSystemGuiBuilder Point(string id,double x,double y){var panel=_coordinates.ToPanel(x,y);_builder.Child("coordinate-point",id,p=>p.Property("x",panel.X.ToString(System.Globalization.CultureInfo.InvariantCulture)).Property("y",panel.Y.ToString(System.Globalization.CultureInfo.InvariantCulture)));return this;}
    /// <summary>Builds and returns the configured GUI node or render fragment.</summary>
    public GuiNode Build()=>_builder.Build();
}