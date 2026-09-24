using System;

namespace TheSingularityWorkshop.Workshop.Gui;

public readonly record struct CoordinateSystem(double Minimum=0,double Maximum=100,double OriginX=50,double OriginY=50)
{
    public double Range=>Maximum-Minimum;
    public bool Contains(double x,double y)=>x>=Minimum&&x<=Maximum&&y>=Minimum&&y<=Maximum;
    public double ToPanelX(double x)=>Validate(x);
    public double ToPanelY(double y)=>Validate(y);
    public (double X,double Y) ToPanel(double x,double y)=>(ToPanelX(x),ToPanelY(y));
    public (double X,double Y) FromPanel(double x,double y)=>(Validate(x),Validate(y));
    public double FromPanelX(double x)=>Validate(x);
    public double FromPanelY(double y)=>Validate(y);
    public double DeltaX(double fromX,double toX)=>toX-fromX;
    public double DeltaY(double fromY,double toY)=>toY-fromY;
    public static CoordinateSystem Normalized {get;}=new(0,100,50,50);
    private double Validate(double value)
    {
        if(double.IsNaN(value)||double.IsInfinity(value)) throw new ArgumentOutOfRangeException(nameof(value),"Coordinate must be finite.");
        if(value<Minimum||value>Maximum) throw new ArgumentOutOfRangeException(nameof(value),value,$"Coordinate must be between {Minimum} and {Maximum}.");
        return value;
    }
}