using System.Windows.Media;

namespace Plainion.WpfBrushes
{
    class BrushDescriptor
    {
        public BrushDescriptor(string name, Brush brush)
        {
            Name = name;
            Brush = brush;
        }

        public string Name { get; private set; }

        public Brush Brush { get; private set; }
    }
}
