using System.Drawing;
using System.Threading;
using csharp_gameboy.Compute;

namespace csharp_gameboy.Display
{
    public class DisplayAdapter
    {
        private static readonly Brush White = Brushes.White;
        private static readonly Brush DarkGray = Brushes.DarkGray;
        private static readonly Brush LightGray = Brushes.LightGray;
        private static readonly Brush Black = Brushes.Black;
        private static readonly Brush Red = Brushes.Red;

        public DisplayAdapter(int scale)
        {
            DisplayWindow = new DisplayWindow(scale);
            TileMapDisplay = new TileMapDisplay(scale);
            Thread.Sleep(25); // Give Window and Graphics time to load
        }

        public DisplayWindow DisplayWindow { get; }
        public TileMapDisplay TileMapDisplay { get; }

        public void UpdateDisplay(CPU cpu)
        {
            TileMapDisplay.Update(cpu);
            DisplayWindow.Draw(cpu, TileMapDisplay);
        }

        public void UpdateTileDisplay(CPU cpu)
        {
            TileMapDisplay.Update(cpu);
            TileMapDisplay.UpdateDisplay();
        }

        public static Brush ByteToColor(byte color)
        {
            switch (color)
            {
                case 0x00:
                    return White;
                case 0x01:
                    return DarkGray;
                case 0x02:
                    return LightGray;
                case 0x03:
                    return Black;
                default:
                    MainWindow.Instance.Log($"Invalid Color: {color:x2}");
                    return Red;
            }
        }
    }
}