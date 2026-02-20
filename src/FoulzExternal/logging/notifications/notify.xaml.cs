using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace FoulzExternal.logging.notifications
{
    public partial class notify : Window
    {
        private static notify me;

        public notify()
        {
            InitializeComponent();
            ShowActivated = false;
            Topmost = true;
            ShowInTaskbar = false;

            try { Owner = Application.Current?.MainWindow; } catch { }

            try
            {
                if (Application.Current != null)
                {
                    Application.Current.Exit += (s, e) => Dispatcher.Invoke(() => { if (IsLoaded) Close(); });
                    Application.Current.Dispatcher.ShutdownStarted += (s, e) => {
                        if (Dispatcher != null && !Dispatcher.HasShutdownStarted)
                            Dispatcher.BeginInvoke((Action)(() => { try { if (IsLoaded) Close(); } catch { } }));
                    };
                }
            }
            catch { }

            Closed += (s, e) => me = null;
        }

        private void place()
        {
            try
            {
                var work = SystemParameters.WorkArea;
                Width = 340;
                Height = Math.Min(600, work.Height);
                Left = work.Right - Width - 10;
                Top = work.Top + 10;
            }
            catch { }
        }

        public static void ShowNotifier()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (me == null || !me.IsLoaded)
                {
                    me = new notify();
                    me.place();
                    me.Show();
                }
                else
                {
                    me.place();
                    me.Activate();
                }
            });
        }

        public static void Notify(string title, string msg, int time = 3500)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (me == null || !me.IsLoaded)
                {
                    me = new notify();
                    me.place();
                    me.Show();
                }
                me.pop(title, msg, time);
            });
        }

        private void pop(string title, string msg, int time)
        {
            var box = new Border
            {
                CornerRadius = new CornerRadius(8),
                Margin = new Thickness(0, 6, 0, 0),
                Padding = new Thickness(12),
                Background = new SolidColorBrush(Color.FromArgb(230, 20, 20, 20)),
                Effect = new System.Windows.Media.Effects.DropShadowEffect { Color = Colors.Black, BlurRadius = 12, Opacity = 0.6, ShadowDepth = 2 }
            };

            var g = new Grid();
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(28) });

            var stack = new StackPanel { Orientation = Orientation.Vertical };
            stack.Children.Add(new TextBlock { Text = title, FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 13, Margin = new Thickness(0, 0, 0, 2) });
            stack.Children.Add(new TextBlock { Text = msg, TextWrapping = TextWrapping.Wrap, Foreground = Brushes.White, FontSize = 12, Opacity = 0.95 });

            var grad = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(1, 0) };
            grad.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 255), 0.0));
            grad.GradientStops.Add(new GradientStop(Color.FromArgb(60, 255, 255, 255), 0.25));
            grad.GradientStops.Add(new GradientStop(Color.FromArgb(20, 255, 255, 255), 0.5));
            grad.GradientStops.Add(new GradientStop(Color.FromArgb(60, 255, 255, 255), 0.75));
            grad.GradientStops.Add(new GradientStop(Color.FromArgb(255, 255, 255, 255), 1.0));

            var glass = new System.Windows.Shapes.Rectangle { Fill = grad, Opacity = 0.08, IsHitTestVisible = false, RadiusX = 6, RadiusY = 6 };

            g.Children.Add(stack);
            Grid.SetColumn(stack, 0);

            var close = new Button { Content = "✕", Width = 22, Height = 22, Background = Brushes.Transparent, BorderThickness = new Thickness(0), Foreground = Brushes.Gray, Cursor = System.Windows.Input.Cursors.Hand };
            close.Click += (s, e) => kill(box);
            g.Children.Add(close);
            Grid.SetColumn(close, 1);

            var host = new Grid();
            host.Children.Add(g);
            host.Children.Add(glass);
            box.Child = host;

            holder.Children.Insert(0, box);

            box.Loaded += (s, e) =>
            {
                try
                {
                    double off = box.ActualWidth + 40;
                    box.RenderTransform = new TranslateTransform(off, 0);
                    (box.RenderTransform as TranslateTransform).BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(off, 0, new Duration(TimeSpan.FromMilliseconds(350))) { EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut } });
                    box.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(300))));

                    var tt = new TranslateTransform(-1, 0);
                    grad.RelativeTransform = tt;
                    tt.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(-1.0, 1.0, new Duration(TimeSpan.FromSeconds(3))) { RepeatBehavior = RepeatBehavior.Forever, AutoReverse = true });
                }
                catch { }
            };

            _ = wait_and_kill(box, time);
        }

        private async Task wait_and_kill(Border b, int ms)
        {
            try
            {
                await Task.Delay(ms);
                double off = b.ActualWidth + 40;
                b.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(600))));
                var tt = b.RenderTransform as TranslateTransform ?? new TranslateTransform(0, 0);
                b.RenderTransform = tt;
                tt.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(0, off, new Duration(TimeSpan.FromMilliseconds(600))) { EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn } });
                await Task.Delay(600);
                Dispatcher.Invoke(() => holder.Children.Remove(b));
            }
            catch { }
        }

        private void kill(Border b)
        {
            try
            {
                b.BeginAnimation(UIElement.OpacityProperty, new DoubleAnimation(b.Opacity, 0, new Duration(TimeSpan.FromMilliseconds(250))));
                double off = b.ActualWidth + 40;
                var tt = b.RenderTransform as TranslateTransform ?? new TranslateTransform(0, 0);
                b.RenderTransform = tt;
                tt.BeginAnimation(TranslateTransform.XProperty, new DoubleAnimation(0, off, new Duration(TimeSpan.FromMilliseconds(250))));
                var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(260) };
                timer.Tick += (s, e) => { timer.Stop(); holder.Children.Remove(b); };
                timer.Start();
            }
            catch { }
        }
    }
}