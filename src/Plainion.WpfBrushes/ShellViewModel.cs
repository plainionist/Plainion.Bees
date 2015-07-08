using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace Plainion.WpfBrushes
{
    class ShellViewModel
    {
        public ShellViewModel()
        {
            Brushes = typeof(Brushes).GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Select(p => new BrushDescriptor(p.Name, (Brush)p.GetValue(null)))
                .ToList();
        }

        public IEnumerable<BrushDescriptor> Brushes { get; private set; }
    }
}
