using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FoulzExternal.SDK;
using FoulzExternal.storage;

namespace FoulzExternal.features.games.universal.explorer
{
    public partial class ExplorerControl : UserControl
    {
        private readonly DispatcherTimer t;
        private readonly Dictionary<string, BitmapImage> cache = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<long> expanded = new();

        public ExplorerControl()
        {
            InitializeComponent();
            t = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            t.Tick += (s, e) => refresh();
            t.Start();
            tree.SelectedItemChanged += (s, e) => set_props();
        }

        private void set_props()
        {
            if (tree.SelectedItem is not TreeViewItem tvi || tvi.Tag is not long addr) return;
            var i = new Instance(addr);
            p_name.Text = i.GetName() ?? "Unnamed";
            p_class.Text = i.GetClass() ?? "???";
            p_addr.Text = $"0x{addr:X}";
            var hum = i.GetHumanoid();
            p_hp.Text = hum.IsValid ? $"{hum.GetHealth():0}/{hum.GetMaxHealth():0}" : "-";
        }

        private void refresh()
        {
            if (tree == null) return;
            if (!Storage.IsInitialized || !Storage.DataModelInstance.IsValid)
            {
                if (tree.Items.Count != 1) { tree.Items.Clear(); tree.Items.Add(new TreeViewItem { Header = "Attach to roblox first..." }); }
                return;
            }

            if (tree.Items.Count == 0 || (tree.Items[0] is TreeViewItem ti && ti.Header is string s && s.Contains("Wait")))
            {
                tree.Items.Clear();
                foreach (var i in Storage.DataModelInstance.GetChildren())
                {
                    var node = make_node(i);
                    if (node != null) tree.Items.Add(node);
                }
            }
        }

        private TreeViewItem? make_node(Instance i)
        {
            string n = i.GetName()?.Trim() ?? "";
            if (string.IsNullOrEmpty(n) || n == "???" || n == "[Unnamed]") return null;

            long addr = i.Address;
            var stack = new StackPanel { Orientation = Orientation.Horizontal };
            var img = new Image { Width = 14, Height = 14, Margin = new Thickness(0, 0, 5, 0), Source = get_ico(i.GetClass() ?? "") };
            var txt = new TextBlock { Text = n, VerticalAlignment = VerticalAlignment.Center, Foreground = (Brush)FindResource("item_grad") };

            stack.Children.Add(img);
            stack.Children.Add(txt);

            var tvi = new TreeViewItem { Header = stack, Tag = addr };
            try { if (i.GetChildren().Count > 0) tvi.Items.Add("..."); } catch { }

            tvi.Expanded += (s, e) => {
                expanded.Add(addr);
                if (tvi.Items.Count == 1 && tvi.Items[0] is string)
                {
                    tvi.Items.Clear();
                    Task.Run(() => {
                        var kids = new Instance(addr).GetChildren();
                        Dispatcher.Invoke(() => {
                            foreach (var k in kids)
                            {
                                var childNode = make_node(k);
                                if (childNode != null) tvi.Items.Add(childNode);
                            }
                        });
                    });
                }
            };

            tvi.Collapsed += (s, e) => expanded.Remove(addr);
            return tvi;
        }

        private BitmapImage? get_ico(string cls)
        {
            if (string.IsNullOrEmpty(cls)) return null;
            if (cache.TryGetValue(cls, out var b)) return b;

            byte[]? d = cls switch
            {
                "Workspace" => icons.workspace,
                "Folder" => icons.folder,
                "Camera" => icons.camera,
                "Humanoid" => icons.humanoid,
                "Part" => icons.part,
                "Players" => icons.players,
                "MeshPart" => icons.meshpart,
                "Player" => icons.player,
                "Model" => icons.model,
                "Terrain" => icons.terrain,
                "LocalScript" => icons.localscript,
                "LocalScripts" => icons.localscripts,
                "PlayerGui" => icons.playergui,
                "Stats" => icons.stats,
                "GuiService" => icons.guiservice,
                "VideoCapture" => icons.videocapture,
                "RunService" => icons.runservice,
                "Frame" => icons.frame,
                "ContentProvider" => icons.contentprovider,
                "NonReplicated" => icons.nonreplicated,
                "StarterGear" => icons.startergear,
                "TimerDevice" => icons.timerdevice,
                "Backpack" => icons.backpack,
                "MarketplaceService" => icons.marketplaceservice,
                "SoundService" => icons.soundservice,
                "LogService" => icons.logservice,
                "StatsItem" => icons.statsitem,
                "BoolValue" => icons.boolvalue,
                "IntValue" => icons.intvalue,
                "DoubleType" => icons.doubletype,
                "Type" => icons.typeshit,
                "AncientLogo" => icons.ancientlogo,
                "Lightning" => icons.lightning,
                _ => null
            };

            if (d == null || d.Length == 0) return null;
            try
            {
                using var ms = new MemoryStream(d);
                var img = new BitmapImage();
                img.BeginInit(); img.CacheOption = BitmapCacheOption.OnLoad; img.StreamSource = ms; img.DecodePixelWidth = 16; img.EndInit();
                img.Freeze(); cache[cls] = img; return img;
            }
            catch { return null; }
        }
    }
}