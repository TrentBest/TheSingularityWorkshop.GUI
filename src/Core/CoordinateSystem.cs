using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Defines the numeric range and origin used by a normalized two-dimensional coordinate space.
/// </summary>
/// <param name="Minimum">Inclusive lower bound for both coordinate axes.</param>
/// <param name="Maximum">Inclusive upper bound for both coordinate axes.</param>
/// <param name="OriginX">Horizontal coordinate of the declared origin.</param>
/// <param name="OriginY">Vertical coordinate of the declared origin.</param>
public readonly record struct CoordinateSystem(double Minimum=0,double Maximum=100,double OriginX=50,double OriginY=50)
{
    /// <summary>Gets the difference between the configured maximum and minimum.</summary>
    public double Range=>Maximum-Minimum;
    /// <summary>Returns whether both coordinates are within the configured inclusive range.</summary>
    /// <param name="x">Horizontal coordinate to test.</param><param name="y">Vertical coordinate to test.</param>
    public bool Contains(double x,double y)=>x>=Minimum&&x<=Maximum&&y>=Minimum&&y<=Maximum;
    /// <summary>Validates and returns a horizontal coordinate in the configured space.</summary>
    /// <param name="x">Horizontal coordinate to validate.</param>
    public double ToPanelX(double x)=>Validate(x);
    /// <summary>Validates and returns a vertical coordinate in the configured space.</summary>
    /// <param name="y">Vertical coordinate to validate.</param>
    public double ToPanelY(double y)=>Validate(y);
    /// <summary>Validates and returns a coordinate pair in the configured space.</summary>
    /// <param name="x">Horizontal coordinate to validate.</param><param name="y">Vertical coordinate to validate.</param>
    public (double X,double Y) ToPanel(double x,double y)=>(ToPanelX(x),ToPanelY(y));
    /// <summary>Validates and returns a coordinate pair supplied in panel space.</summary>
    /// <param name="x">Horizontal coordinate to validate.</param><param name="y">Vertical coordinate to validate.</param>
    public (double X,double Y) FromPanel(double x,double y)=>(Validate(x),Validate(y));
    /// <summary>Validates and returns a horizontal coordinate supplied in panel space.</summary>
    /// <param name="x">Horizontal coordinate to validate.</param>
    public double FromPanelX(double x)=>Validate(x);
    /// <summary>Validates and returns a vertical coordinate supplied in panel space.</summary>
    /// <param name="y">Vertical coordinate to validate.</param>
    public double FromPanelY(double y)=>Validate(y);
    /// <summary>Calculates the signed horizontal difference between two coordinates.</summary>
    /// <param name="fromX">Starting horizontal coordinate.</param><param name="toX">Ending horizontal coordinate.</param>
    public double DeltaX(double fromX,double toX)=>toX-fromX;
    /// <summary>Calculates the signed vertical difference between two coordinates.</summary>
    /// <param name="fromY">Starting vertical coordinate.</param><param name="toY">Ending vertical coordinate.</param>
    public double DeltaY(double fromY,double toY)=>toY-fromY;
    /// <summary>Gets the conventional 0–100 coordinate space with its origin at (50, 50).</summary>
    public static CoordinateSystem Normalized {get;}=new(0,100,50,50);
    private double Validate(double value)
    {
        if(double.IsNaN(value)||double.IsInfinity(value)) throw new ArgumentOutOfRangeException(nameof(value),"Coordinate must be finite.");
        if(value<Minimum||value>Maximum) throw new ArgumentOutOfRangeException(nameof(value),value,$"Coordinate must be between {Minimum} and {Maximum}.");
        return value;
    }
}