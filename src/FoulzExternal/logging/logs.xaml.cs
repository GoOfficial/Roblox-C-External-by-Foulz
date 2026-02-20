using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Collections.Concurrent;
using System.Windows.Threading;
using System.Collections.Generic;

namespace FoulzExternal.logging
{
    public partial class LogsWindow : Window
    {
        private static LogsWindow me;
        private static readonly ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private static readonly List<string> history = new List<string>();
        private DispatcherTimer timer;

        private static readonly TimeSpan delay = TimeSpan.FromSeconds(1);
        private static DateTime last_sent = DateTime.MinValue;
        private static readonly object safety = new object();
        private static int lines = 0;
        private const int batch = 8;

        public LogsWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => {
                console_out?.Children.Clear();
                lines = 0;
                foreach (var msg in history) add_row(msg);
            };
        }

        private void add_row(string msg)
        {
            string ts = "", txt = msg;
            if (msg.StartsWith("[") && msg.Contains("]"))
            {
                int i = msg.IndexOf(']');
                ts = msg.Substring(0, i + 1);
                txt = msg.Substring(i + 2);
            }

            var row = new Border
            {
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 4, 6, 4),
                Margin = new Thickness(0, 1, 0, 1),
                Background = (lines % 2 == 0) ? new SolidColorBrush(Color.FromArgb(15, 255, 255, 255)) : Brushes.Transparent
            };

            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var t_txt = new TextBlock { Text = ts, FontFamily = new FontFamily("Consolas"), FontSize = 11, Margin = new Thickness(0, 0, 8, 0), VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(Color.FromRgb(100, 100, 100)) };
            var m_txt = new TextBlock { Text = txt, FontFamily = new FontFamily("Consolas"), FontSize = 12, TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center };

            var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
            brush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            brush.GradientStops.Add(new GradientStop(Color.FromRgb(180, 180, 180), 0.5));
            brush.GradientStops.Add(new GradientStop(Colors.White, 1.0));

            var move = new TranslateTransform(-1, 0);
            brush.RelativeTransform = move;
            move.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-1, 1, new Duration(TimeSpan.FromSeconds(3))) { RepeatBehavior = RepeatBehavior.Forever, AutoReverse = true });

            m_txt.Foreground = brush;

            Grid.SetColumn(t_txt, 0);
            Grid.SetColumn(m_txt, 1);
            g.Children.Add(t_txt);
            g.Children.Add(m_txt);
            row.Child = g;

            if (console_out != null)
            {
                console_out.Children.Add(row);
                row.BringIntoView();
            }
            lines++;
        }

        private void start_timer()
        {
            if (timer != null) return;
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            timer.Tick += (s, e) => drain();
            timer.Start();
        }

        public static void ShowConsole()
        {
            if (Application.Current == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (me == null || !me.IsLoaded)
                {
                    me = new LogsWindow();
                    me.Show();
                }
                else
                {
                    me.Activate();
                    if (me.WindowState != WindowState.Normal) me.WindowState = WindowState.Normal;
                }
                me.start_timer();
                drain();
            });
        }

        public static void Log(string fmt, params object[] args)
        {
            try
            {
                string msg = $"[{DateTime.Now:HH:mm:ss}] {string.Format(fmt, args)}";
                queue.Enqueue(msg);
                history.Add(msg);

                if (me != null && me.IsLoaded)
                    me.Dispatcher.BeginInvoke(new Action(() => drain()));
            }
            catch { }
        }

        private static void drain()
        {
            if (me == null || !me.IsLoaded) return;

            lock (safety)
            {
                if ((DateTime.UtcNow - last_sent) < delay) return;

                int count = 0;
                while (count < batch && queue.TryDequeue(out string msg))
                {
                    me.add_row(msg);
                    count++;
                }
                if (count > 0) last_sent = DateTime.UtcNow;
            }
        }

        private void move_it(object s, MouseButtonEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) DragMove(); }
        private void bye(object s, RoutedEventArgs e) => Close();
    }
}