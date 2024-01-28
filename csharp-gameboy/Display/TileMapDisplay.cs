using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using csharp_gameboy.Compute;
using csharp_gameboy.Util;
using Point = System.Drawing.Point;

namespace csharp_gameboy.Display
{
    public class TileMapDisplay : Window, IDisposable
    {
        public static readonly int ScreenWidth = 256 + 4 + 256;
        public static readonly int ScreenHeight = 256 + 4 + 192;
        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;
        private readonly Brush[,] _map1 = new Brush[256, 256];
        private readonly Brush[,] _map2 = new Brush[256, 256];

        private readonly int _scale;
        private readonly Brush[,] _tiles = new Brush[128, 192];
        private readonly Graphics _windowGraphics;

        private bool _isUpdating;

        public TileMapDisplay(int scale)
        {
            _scale = scale / 2;
            Title = "Tiles";
            ResizeMode = ResizeMode.NoResize;
            Show();
            while (!IsInitialized)
            {
            } // block until window is open

            var wih = new WindowInteropHelper(this);
            _windowGraphics = Graphics.FromHwnd(wih.Handle);
            var realWidth = ScreenWidth / (_windowGraphics.DpiX / 96f);
            var realHeight = ScreenHeight / (_windowGraphics.DpiY / 96f);

            _bitmap = new Bitmap(ScreenWidth * scale, ScreenHeight * scale);
            _graphics = Graphics.FromImage(_bitmap);

            Width = realWidth * _scale + 15; // account for window border
            Height = realHeight * _scale + 37; // account for window border and top bar
        }

        public void Dispose()
        {
            _graphics.Dispose();
        }

        public Brush ReadMap(int x, int y, bool map2 = false)
        {
            return map2 ? ReadMap2(x, y) : ReadMap1(x, y);
        }

        private Brush ReadMap1(int x, int y)
        {
            return _map1[x, y];
        }

        private Brush ReadMap2(int x, int y)
        {
            return _map2[x, y];
        }

        public void ClearTiles(Brush color)
        {
            for (var x = 0; x < 128; x++)
            for (var y = 0; y < 192; y++)
                _tiles[x, y] = color;
        }

        public void ClearTileMap1(Brush color)
        {
            for (var x = 0; x < 256; x++)
            for (var y = 0; y < 256; y++)
                _map1[x, y] = color;
        }

        public void ClearTileMap2(Brush color)
        {
            for (var x = 0; x < 256; x++)
            for (var y = 0; y < 256; y++)
                _map2[x, y] = color;
        }

        public void DrawTiles(Brush color, int x, int y)
        {
            _tiles[x, y] = color;
        }

        public void DrawTileMap1(Brush color, int x, int y)
        {
            _map1[x, y] = color;
        }

        public void DrawTileMap2(Brush color, int x, int y)
        {
            _map2[x, y] = color;
        }

        private void DrawScaledPixel(Brush color, int x, int y)
        {
            _graphics.FillRectangle(color, x * _scale, y * _scale, _scale, _scale);
        }

        public void Update(CPU cpu)
        {
            var tid = 0;
            for (var my = 0; my < 24; my++)
            for (var mx = 0; mx < 16; mx++)
            {
                for (var x = 0; x < 8; x++)
                for (var y = 0; y < 8; y++)
                    _tiles[mx * 8 + x, my * 8 + y] = DisplayAdapter.ByteToColor(cpu.MemoryManager.Memory.ReadTilePixel(tid, y, x));

                tid++;
            }

            var lcdc = cpu.ReadMem8(0XFF40);
            var tiles2 = MathUtil.IsBitSet(lcdc, 4);

            var add1 = 0;
            for (var y = 0; y < 32; y++)
            for (var x = 0; x < 32; x++)
            {
                var tile = cpu.ReadMem8((ushort)(0x9800 + add1));
                for (var tx = 0; tx < 8; tx++)
                for (var ty = 0; ty < 8; ty++)
                    _map1[x * 8 + tx, y * 8 + ty] =
                        DisplayAdapter.ByteToColor(cpu.MemoryManager.Memory.ReadTilePixel(tiles2 ? tile : 256 + (sbyte)tile, ty, tx));

                add1++;
            }

            var add2 = 0;
            for (var y = 0; y < 32; y++)
            for (var x = 0; x < 32; x++)
            {
                var tile = cpu.ReadMem8((ushort)(0x9C00 + add2));
                for (var tx = 0; tx < 8; tx++)
                for (var ty = 0; ty < 8; ty++)
                    _map2[x * 8 + tx, y * 8 + ty] =
                        DisplayAdapter.ByteToColor(cpu.MemoryManager.Memory.ReadTilePixel(tiles2 ? tile : 256 + (sbyte)tile, ty, tx));

                add2++;
            }
        }

        public void UpdateDisplay()
        {
            if (_isUpdating) return;
            _isUpdating = true;
            for (var x = 0; x < 256; x++)
            for (var y = 0; y < 256; y++)
                DrawScaledPixel(_map1[x, y], x, y);

            for (var y = 0; y < 256; y++)
            for (var x = 0; x < 256; x++)
                DrawScaledPixel(_map2[x, y], x + 256 + 4, y);

            for (var x = 0; x < 128; x++)
            for (var y = 0; y < 192; y++)
                DrawScaledPixel(_tiles[x, y], x, y + 256 + 4);

            _windowGraphics.DrawImage(_bitmap, Point.Empty);
            _isUpdating = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MainWindow.Instance.Close();
        }
    }
}