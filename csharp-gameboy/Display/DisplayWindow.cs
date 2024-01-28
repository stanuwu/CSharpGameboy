using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using csharp_gameboy.Compute;
using csharp_gameboy.Util;
using Point = System.Drawing.Point;

namespace csharp_gameboy.Display
{
    public class DisplayWindow : Window, IDisposable
    {
        public static readonly int ScreenWidth = 160;
        public static readonly int ScreenHeight = 144;
        private readonly Bitmap _bitmap;
        private readonly Graphics _graphics;

        private readonly int _scale;
        private readonly Graphics _windowGraphics;

        public DisplayWindow(int scale)
        {
            _scale = scale;
            Title = "Screen";
            ResizeMode = ResizeMode.NoResize;
            Show();
            while (!IsInitialized)
            {
            } // block until window is open

            var wih = new WindowInteropHelper(this);
            _windowGraphics = Graphics.FromHwnd(wih.Handle);
            var realWidth = ScreenWidth / (_windowGraphics.DpiX / 96f);
            var realHeight = ScreenHeight / (_windowGraphics.DpiY / 96f);
            Width = realWidth * scale + 15; // account for window border
            Height = realHeight * scale + 37; // account for window border and top bar

            _bitmap = new Bitmap(ScreenWidth * scale, ScreenHeight * scale);
            _graphics = Graphics.FromImage(_bitmap);
        }

        public void Dispose()
        {
            _graphics.Dispose();
        }

        public void SetPixel(int x, int y, Brush brush)
        {
            _graphics.FillRectangle(brush, x * _scale, y * _scale, _scale, _scale);
        }

        public void FillRectangle(int x, int y, int width, int height, Brush brush)
        {
            _graphics.FillRectangle(brush, x * _scale, y * _scale, width * _scale, height * _scale);
        }

        public void Clear()
        {
            _graphics.Clear(Color.White);
        }

        public void Swap()
        {
            _windowGraphics.DrawImage(_bitmap, Point.Empty);
        }

        public void Draw(CPU cpu, TileMapDisplay tileMapDisplay)
        {
            // Hotfix for Pokemon LCDC
            // For some reason pokemon will not set lcdc back to $E3 value on right now
            // Until its fixed this makes it playable
            // TODO: Actually fix the issue
            cpu.WriteMem8(0xFF40, 0xE3);

            // Read
            var lcdc = cpu.ReadMem8(0xFF40);

            // Check if LCD is on;
            if (!MathUtil.IsBitSet(7, lcdc))
                // In theory this means lcd is off. We don't need to do that.
                // Clear();
                Swap();

            var scrollX = cpu.ReadMem8(0xFF42);
            var scrollY = cpu.ReadMem8(0xFF43);

            var bgEnable = MathUtil.IsBitSet(lcdc, 0);
            var objEnable = MathUtil.IsBitSet(lcdc, 1);
            var objSize16 = MathUtil.IsBitSet(lcdc, 2); // size 8x8 or 8x16
            var bgMap2 = MathUtil.IsBitSet(lcdc, 3);
            var windowEnable = MathUtil.IsBitSet(lcdc, 5);
            var windowMap2 = MathUtil.IsBitSet(lcdc, 6);

            var wy = cpu.ReadMem8(0xFF4A);
            var wx = cpu.ReadMem8(0xFF4B);

            // Draw Background and Window
            if (bgEnable)
            {
                // Draw Background
                for (var y = 0; y < 144; y++)
                for (var x = 0; x < 160; x++)
                {
                    var ry = MathUtil.WrapToByte(y + scrollX);
                    var rx = MathUtil.WrapToByte(x + scrollY);
                    SetPixel(x, y, tileMapDisplay.ReadMap(rx, ry, bgMap2));
                }

                // Draw Window
                if (windowEnable)
                    for (var y = 0; y < 144; y++)
                    for (var x = 0; x < 160; x++)
                    {
                        var ry = y + wy;
                        var rx = x + wx - 7;
                        SetPixel(rx, ry, tileMapDisplay.ReadMap(x, y, windowMap2));
                    }
            }
            else
            {
                Clear();
            }

            // Draw Objects
            // TODO: Palette
            // TODO: Priority

            if (objEnable)
                for (var i = 0; i < 40; i++)
                {
                    var oy = cpu.ReadMem8((ushort)(0xFE00 + i * 4)) - 16;
                    var ox = cpu.ReadMem8((ushort)(0xFE00 + i * 4 + 1)) - 8;
                    var tile = cpu.ReadMem8((ushort)(0xFE00 + i * 4 + 2));
                    var attribs = cpu.ReadMem8((ushort)(0xFE00 + i * 4 + 3));
                    var priority = MathUtil.IsBitSet(attribs, 7);
                    var flipY = MathUtil.IsBitSet(attribs, 6);
                    var flipX = MathUtil.IsBitSet(attribs, 5);

                    if (objSize16)
                    {
                        var tile1 = (byte)(tile & 0xFE);
                        var tile2 = (byte)(tile | 0x01);
                        for (var y = 0; y < 8; y++)
                        for (var x = 0; x < 8; x++)
                        {
                            var tx = x;
                            var ty = y;
                            var p1 = 0;
                            var p2 = 8;
                            if (flipX && flipY)
                            {
                                ty = 7 - ty;
                                tx = 7 - tx;
                                p1 = 8;
                                p2 = 0;
                            }
                            else if (flipX)
                            {
                                ty = 7 - ty;
                            }
                            else if (flipY)
                            {
                                p1 = 8;
                                p2 = 0;
                                tx = 7 - tx;
                            }

                            var c1 = cpu.MemoryManager.Memory.ReadTilePixel(tile1, tx, ty);
                            var c2 = cpu.MemoryManager.Memory.ReadTilePixel(tile2, tx, ty);
                            if (c1 != 0) SetPixel(y + ox, x + oy + p1, DisplayAdapter.ByteToColor(c1));
                            if (c2 != 0) SetPixel(y + ox, x + oy + p2, DisplayAdapter.ByteToColor(c2));
                        }
                    }
                    else
                    {
                        for (var y = 0; y < 8; y++)
                        for (var x = 0; x < 8; x++)
                        {
                            var tx = x;
                            var ty = y;
                            if (flipX && flipY)
                            {
                                ty = 7 - ty;
                                tx = 7 - tx;
                            }
                            else if (flipX)
                            {
                                ty = 7 - ty;
                            }
                            else if (flipY)
                            {
                                tx = 7 - tx;
                            }

                            var c = cpu.MemoryManager.Memory.ReadTilePixel(tile, tx, ty);
                            if (c != 0) SetPixel(y + ox, x + oy, DisplayAdapter.ByteToColor(c));
                        }
                    }
                }

            Swap();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            MainWindow.Instance.Close();
        }
    }
}