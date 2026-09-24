using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions
{
    public interface IComponentWPF
    {
        // Identification
        string Id { get; }

        // Lifecycle & Hierarchy
        void SetParent(IComponentWPF? parent);
        IComponentWPF? GetParent();

        // The "Agent" Communication (The Bus)
        void Publish(string topic, object payload);
        void Receive(string topic, object payload);

        // Manifestation
        FrameworkElement Build();
    }
}
