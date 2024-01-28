using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using csharp_gameboy.Compute;
using csharp_gameboy.Display;
using csharp_gameboy.Memory;
using Microsoft.Win32;

namespace csharp_gameboy
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int MaxLogLines = 40;
        private const int Scale = 4;
        private readonly List<string> _logLines = new List<string>();
        private ulong _frame;
        private bool _pause = true;
        private int _step;
        private bool _tiles;
        private CPU CPU;
        private DisplayAdapter DisplayAdapter;
        private MemoryManager MemoryManager;

        public MainWindow()
        {
            Instance = this;

            InitializeComponent();

            Log("Application Started");

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                    Dispatcher.Invoke(() =>
                    {
                        KeyE = Keyboard.IsKeyDown(Key.E);
                        KeyQ = Keyboard.IsKeyDown(Key.Q);
                        KeyDown = Keyboard.IsKeyDown(Key.Down);
                        KeyRight = Keyboard.IsKeyDown(Key.Right);
                        KeyS = Keyboard.IsKeyDown(Key.S);
                        KeyW = Keyboard.IsKeyDown(Key.W);
                        KeyA = Keyboard.IsKeyDown(Key.A);
                        KeyD = Keyboard.IsKeyDown(Key.D);
                    });
                }
            });
        }

        public bool KeyE { get; private set; }
        public bool KeyQ { get; private set; }
        public bool KeyDown { get; private set; }
        public bool KeyRight { get; private set; }
        public bool KeyS { get; private set; }
        public bool KeyW { get; private set; }
        public bool KeyA { get; private set; }
        public bool KeyD { get; private set; }

        public static MainWindow Instance { get; private set; }

        [STAThread]
        private void Main()
        {
            Log("- Loading Screen");

            Dispatcher.Invoke(() => { DisplayAdapter = new DisplayAdapter(Scale); });

            LogLast("+ Loaded Screen");

            Log("- Loading Events");
            Closed += (sender, args) =>
            {
                DisplayAdapter.DisplayWindow.Close();
                DisplayAdapter.TileMapDisplay.Close();
            };
            LogLast("+ Loaded Events");

            Log("- Loading Memory");
            MemoryManager = new MemoryManager();
            LogLast("+ Loaded Memory");

            Log("- Loading Rom");
            var ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "GB Rom (*.gb)|*.gb";
            ofd.Title = "Rom Select";
            if (ofd.ShowDialog() != true) Error("Load Rom", "Error: failed to load rom");

            MemoryManager.LoadRom(File.ReadAllBytes(ofd.FileName));
            LogLast("+ Loaded Rom");

            Log("- Loading Boot Rom");
            var ofd2 = new OpenFileDialog();
            ofd2.Multiselect = false;
            ofd2.Filter = "Binary File (*.bin)|*.bin";
            ofd2.Title = "Boot Rom Select";
            if (ofd2.ShowDialog() != true) Error("Load Boot Rom", "Error: failed to load boot rom");

            MemoryManager.LoadBootRom(File.ReadAllBytes(ofd2.FileName));
            LogLast("+ Loaded Boot Rom");

            Log("- Creating CPU");
            CPU = new CPU(MemoryManager);
            LogLast("+ Created CPU");

            Log("+ Starting Emulation");

            // Debug
            // var tests = new Dictionary<byte, bool>();

            // Gameboy Doctor
            /*
            var first = true;
            var file = "D:\\Documents\\GameboyDoctor\\mlog.txt";
            File.Open(file, FileMode.Truncate).Close();
            var logWriter = new StreamWriter(file);
            */

            while (true)
                try
                {
                    if (_step > 0 || !_pause)
                    {
                        // Debug
                        /*
                        var dadd = CPU.ReadMem8(CPU.ReadPC());
                        if (!MemoryManager.Memory.IsBooting())
                        {
                            if (!tests.ContainsKey(dadd)) tests[dadd] = true;
                        }
                        else
                        {
                            tests[dadd] = false;
                        }

                        if (CPU.ReadPC() == 0x0213)
                            foreach (var v in tests)
                                if (v.Value)
                                    Log($"0x{v.Key:x2}");

                        */

                        // Breakpoint (funny)

                        // if (CPU.ReadMem8(0xFF40) == 0xE3 && !MemoryManager.Memory.IsBooting()) _pause = true;
                        // if (CPU.ReadMem16(CPU.ReadSP()) == 0x0000 && !MemoryManager.Memory.IsBooting()) _pause = true;

                        // Gameboy Doctor
                        /*
                        if (!MemoryManager.Memory.IsBooting())
                        {
                            if (first)
                            {
                                CPU.SetFlagHalfCarry(true);
                                first = false;
                            }

                            logWriter.WriteLine(
                                $"A:{CPU.ReadA():x2} F:{CPU.ReadFlagsRegister():x2} B:{CPU.ReadB():x2} C:{CPU.ReadC():x2} D:{CPU.ReadD():x2} E:{CPU.ReadE():x2} H:{CPU.ReadH():x2} L:{CPU.ReadL():x2} SP:{CPU.ReadSP():x4} PC:{CPU.ReadPC():x4} PCMEM:{CPU.ReadMem8(CPU.ReadPC()):x2},{CPU.ReadMem8((ushort)(CPU.ReadPC() + 1)):x2},{CPU.ReadMem8((ushort)(CPU.ReadPC() + 2)):x2},{CPU.ReadMem8((ushort)(CPU.ReadPC() + 3)):x2}"
                                    .ToUpper());
                        }
                        */

                        if (_pause) Dispatcher.Invoke(() => { Debug.Text = Instructions.DisplayCurrentDebugInfo(CPU); });

                        if (_frame % (4190000 / 30) == 0)
                        {
                            Dispatcher.Invoke(() => { Debug.Text = Instructions.DisplayCurrentDebugInfo(CPU); });
                            DisplayAdapter.UpdateDisplay(CPU);
                            Thread.Sleep(1);
                        }

                        if (_frame % (4190000 / 4) == 0) DisplayAdapter.UpdateTileDisplay(CPU);

                        if (_tiles)
                        {
                            _tiles = false;
                            Log("- Loading Tiles");
                            DisplayAdapter.UpdateTileDisplay(CPU);
                            LogLast("+ Loaded Tiles");
                        }

                        if (!CPU.Step()) break;
                        MemoryManager.IO.Update(CPU);
                        _frame++;
                        _step--;
                        if (_step < 0) _step = 0;
                    }
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }

            Log("+ Stopping Emulation");
        }

        private void AddToLog(string text)
        {
            _logLines.AddRange(text.Split('\n'));
            while (_logLines.Count > MaxLogLines) _logLines.RemoveAt(0);
            UpdateLog();
        }

        private void UpdateLog()
        {
            Dispatcher.Invoke(() => { LogTextBlock.Text = string.Join("\n", _logLines); });
        }

        public void Log(string text)
        {
            AddToLog(text);
        }

        public void LogLast(string text)
        {
            _logLines[_logLines.Count - 1] = text;
            UpdateLog();
        }

        public void Error(string title, string text)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }

        public void ClearLog()
        {
            _logLines.Clear();
            UpdateLog();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Button.IsEnabled = false;
            Task.Run(Main);
        }

        private void Step_OnClick(object sender, RoutedEventArgs e)
        {
            _step++;
        }

        private void Pause_OnClick(object sender, RoutedEventArgs e)
        {
            _pause = !_pause;
            if (_pause) Pause.Content = "Resume";
            else Pause.Content = "Pause";
        }

        private void Step100_OnClick(object sender, RoutedEventArgs e)
        {
            _step += 100;
        }

        private void Step1000_OnClick(object sender, RoutedEventArgs e)
        {
            _step += 1000;
        }

        private void Tiles_OnClick(object sender, RoutedEventArgs e)
        {
            _tiles = true;
        }
    }
}