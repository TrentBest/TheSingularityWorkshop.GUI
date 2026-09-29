using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions;

/// <summary>Small platform utility context for applying WPF effects without exposing WPF through GUI Core.</summary>
public static class GuiContextWPF
{
    public static T WithOpacityPulse<T>(this T element, double min = 0.35, double max = 1.0, double durationSeconds = 1.5)
        where T : UIElement
    {
        if (min < 0 || max > 1 || min > max) throw new ArgumentOutOfRangeException();
        var animation = new DoubleAnimation(min, max, TimeSpan.FromSeconds(durationSeconds))
        {
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };
        element.BeginAnimation(UIElement.OpacityProperty, animation);
        return element;
    }

    public static T WithShadow<T>(this T element, Color color, double depth = 3, double blur = 8, double opacity = 0.35)
        where T : Control
    {
        element.Effect = new System.Windows.Media.Effects.DropShadowEffect
        {
            Color = color,
            Direction = 315,
            ShadowDepth = depth,
            BlurRadius = blur,
            Opacity = opacity
        };
        return element;
    }
}
