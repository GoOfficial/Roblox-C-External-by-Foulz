using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FoulzExternal.SDK.caches;
using FoulzExternal.storage;
using FoulzExternal.SDK;
using SDKInstance = FoulzExternal.SDK.Instance;

// teleport don't work, i'll fix ts later when I have the time

namespace FoulzExternal.features.games.universal.playerlist
{
    public partial class PlayerListControl : UserControl
    {
        private readonly DispatcherTimer t;
        private readonly Dictionary<string, TextBlock> stats = new(StringComparer.OrdinalIgnoreCase);
        private string spec_who = null;

        public event Action<string> tp_req;
        public event Action<string> spec_req;

        public PlayerListControl()
        {
            InitializeComponent();
            t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(800) };
            t.Tick += (s, e) => refresh();
            Loaded += (s, e) => t.Start();
            Unloaded += (s, e) => t.Stop();

            tp_req += async name => await tp(name);
            spec_req += name => spec(name);
            refresh();
        }

        private void refresh()
        {
            Dispatcher.Invoke(() => {
                plist.Items.Clear();
                stats.Clear();

                if (!Storage.IsInitialized)
                {
                    plist.Items.Add(new ListBoxItem { Content = new TextBlock { Text = "Attach to roblox first...", Foreground = System.Windows.Media.Brushes.Gray } });
                    return;
                }

                var snap = player.CachedPlayers;
                if (snap == null || snap.Count == 0) return;

                foreach (var i in snap)
                {
                    if (!i.IsValid) continue;
                    string name = i.GetName() ?? "???";

                    var g = new Grid { Margin = new Thickness(0, 4, 0, 4) };
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var info = new StackPanel();
                    var n_lbl = new TextBlock { Text = name, FontFamily = new("Consolas"), FontSize = 12, Foreground = System.Windows.Media.Brushes.White };
                    var s_lbl = new TextBlock { Text = "idle", FontSize = 10, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(154, 170, 170)) };
                    info.Children.Add(n_lbl);
                    info.Children.Add(s_lbl);
                    g.Children.Add(info);

                    var b_stack = new StackPanel { Orientation = Orientation.Horizontal };
                    var b_tp = new Button { Content = "Teleport", Tag = name, Style = (Style)FindResource("btn_style") };
                    var b_sp = new Button { Content = "Spectate", Tag = name, Style = (Style)FindResource("btn_style") };

                    b_tp.Click += (s, e) => tp_req?.Invoke(name);
                    b_sp.Click += (s, e) => spec_req?.Invoke(name);

                    b_stack.Children.Add(b_tp);
                    b_stack.Children.Add(b_sp);
                    Grid.SetColumn(b_stack, 1);
                    g.Children.Add(b_stack);

                    stats[name] = s_lbl;
                    if (name.Equals(spec_who, StringComparison.OrdinalIgnoreCase))
                    {
                        s_lbl.Text = "spectating";
                        s_lbl.Foreground = System.Windows.Media.Brushes.LightGreen;
                    }

                    plist.Items.Add(new ListBoxItem { Content = g });
                }
            });
        }

        private async Task tp(string name)
        {
            setstat(name, "teleporting...", System.Windows.Media.Brushes.Orange);
            var tar = playerobjects.CachedPlayerObjects.FirstOrDefault(p => p.Name == name);
            long tar_root = tar.HumanoidRootPart.IsValid ? tar.HumanoidRootPart.Address : 0;

            if (tar_root == 0)
            {
                var p_i = player.CachedPlayers.FirstOrDefault(p => p.GetName() == name);
                var chara = p_i.GetCharacter();
                if (chara.IsValid) tar_root = chara.FindFirstChild("HumanoidRootPart").Address;
            }

            if (tar_root == 0) { setstat(name, "no root", System.Windows.Media.Brushes.Gray); return; }

            long loc_root = 0;
            var loc_char = Storage.LocalPlayerInstance.GetCharacter();
            if (loc_char.IsValid) loc_root = loc_char.FindFirstChild("HumanoidRootPart").Address;

            if (loc_root == 0) return;

            try
            {
                var mem = SDKInstance.Mem;
                var pos = new SDKInstance(tar_root).GetPosition();
                long prim = mem.ReadPtr(loc_root + Offsets.BasePart.Primitive);

                if (prim != 0)
                {
                    await Task.Run(() => {
                        for (int i = 0; i < 6; i++)
                        {
                            mem.Write(prim + Offsets.Primitive.Position, pos);
                            Thread.Sleep(12);
                        }
                    });
                    setstat(name, "idle", System.Windows.Media.Brushes.Gray);
                }
            }
            catch { setstat(name, "err", System.Windows.Media.Brushes.Red); }
        }

        private void spec(string name)
        {
            if (name.Equals(spec_who, StringComparison.OrdinalIgnoreCase))
            {
                rescam();
                setstat(spec_who, "idle", System.Windows.Media.Brushes.Gray);
                spec_who = null;
                return;
            }

            var tar = playerobjects.CachedPlayerObjects.FirstOrDefault(p => p.Name == name);
            long sub = tar.Humanoid.IsValid ? tar.Humanoid.Address : 0;

            if (sub == 0)
            {
                var p_i = player.CachedPlayers.FirstOrDefault(p => p.GetName() == name);
                var hum = p_i.GetHumanoid();
                if (hum.IsValid) sub = hum.Address;
            }

            if (sub != 0 && Storage.CameraInstance.IsValid)
            {
                SDKInstance.Mem.Write(Storage.CameraInstance.Address + Offsets.Camera.CameraSubject, sub);
                spec_who = name;
                setstat(name, "spectating", System.Windows.Media.Brushes.LightGreen);
            }
        }

        private void rescam()
        {
            var hum = Storage.LocalPlayerInstance.GetHumanoid();
            if (hum.IsValid && Storage.CameraInstance.IsValid)
                SDKInstance.Mem.Write(Storage.CameraInstance.Address + Offsets.Camera.CameraSubject, hum.Address);
        }

        private void setstat(string name, string txt, System.Windows.Media.Brush col)
        {
            Dispatcher.Invoke(() => {
                if (stats.TryGetValue(name, out var tb))
                {
                    tb.Text = txt;
                    tb.Foreground = col;
                }
            });
        }
    }
}