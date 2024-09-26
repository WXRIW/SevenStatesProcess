using Lyricify.Helpers;
using ModernWpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;

namespace SevenStatesProcess
{
    public partial class MainWindow : Window
    {
        private enum ProcessState
        {
            New,
            ReadyActive,
            ReadySuspended,
            Running,
            BlockedActive,
            BlockedSuspended,
            Terminated
        }

        private ProcessState currentState = ProcessState.New;

        public MainWindow()
        {
            InitializeComponent();

            WindowChrome.CaptionHeight = 30;

            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
            {
                Wpf.Ui.Appearance.Background.Apply(
                    this,
                    Wpf.Ui.Appearance.BackgroundType.Mica);
                GridWindowControlButtons.Visibility = Visibility.Collapsed;
                WindowChrome.NonClientFrameEdges = NonClientFrameEdges.Left | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom;

                StateChanged += (o, e) =>
                {
                    if (WindowState == WindowState.Normal)
                    {
                        GridMain.Margin = new Thickness(0);
                    }
                    else if (WindowState == WindowState.Maximized)
                    {
                        GridMain.Margin = new Thickness(5);
                    }
                };
            }
            else
            {
                StateChanged += (o, e) =>
                {
                    if (WindowState == WindowState.Normal)
                    {
                        SymbolIconMaximize.Symbol = Wpf.Ui.Common.SymbolRegular.Maximize16;
                        GridMain.Margin = new Thickness(0);
                    }
                    else if (WindowState == WindowState.Maximized)
                    {
                        SymbolIconMaximize.Symbol = Wpf.Ui.Common.SymbolRegular.SquareMultiple16;
                        GridMain.Margin = new Thickness(5);
                    }
                };
            }

            UpdateButtonStates();
            TextBoxCurrentState.Text = GetName(currentState);
        }

        private void UpdateButtonStates()
        {
            ButtonToReadyActive.IsEnabled = false;
            ButtonToReadySuspended.IsEnabled = false;
            ButtonToRunning.IsEnabled = false;
            ButtonToBlockedActive.IsEnabled = false;
            ButtonToBlockedSuspended.IsEnabled = false;
            ButtonToTerminated.IsEnabled = false;

            switch (currentState)
            {
                case ProcessState.New:
                    ButtonToReadyActive.IsEnabled = true;
                    ButtonToReadySuspended.IsEnabled = true;
                    break;
                case ProcessState.ReadyActive:
                    ButtonToReadySuspended.IsEnabled = true;
                    ButtonToRunning.IsEnabled = true;
                    break;
                case ProcessState.ReadySuspended:
                    ButtonToReadyActive.IsEnabled = true;
                    break;
                case ProcessState.Running:
                    ButtonToBlockedActive.IsEnabled = true;
                    ButtonToReadyActive.IsEnabled = true;
                    ButtonToReadySuspended.IsEnabled = true;
                    ButtonToTerminated.IsEnabled = true;
                    break;
                case ProcessState.BlockedActive:
                    ButtonToReadyActive.IsEnabled = true;
                    ButtonToBlockedSuspended.IsEnabled = true;
                    break;
                case ProcessState.BlockedSuspended:
                    ButtonToBlockedActive.IsEnabled = true;
                    ButtonToReadySuspended.IsEnabled = true;
                    break;
                case ProcessState.Terminated:
                    break;
            }
        }

        private static string GetName(ProcessState state) => state switch
        {
            ProcessState.New => "创建",
            ProcessState.ReadyActive => "活动就绪",
            ProcessState.ReadySuspended => "静止就绪",
            ProcessState.Running => "执行",
            ProcessState.BlockedActive => "活动阻塞",
            ProcessState.BlockedSuspended => "静止阻塞",
            ProcessState.Terminated => "终止",
            _ => throw new NotImplementedException(),
        };

        private static (string, string, string) GetExplanation(string previousState, string currentState) => (previousState, currentState) switch
        {
            ("创建", "活动就绪") => (
                "许可",
                "系统完成初始化，分配资源，进程获得了运行许可，进入活动就绪状态，等待被调度。",
                "当用户决定排队体验 Apple Vision Pro 新产品，系统完成了必要的初始化并分配了所需资源，进程获得了运行许可，进入活动就绪状态。"
            ),
            ("创建", "静止就绪") => (
                "许可",
                "进程创建成功但暂时没有被激活，进入静止就绪状态，等待被激活或调度。",
                "当用户登记成功，但因名额已满，进程虽然被创建，但因未被激活而进入静止就绪状态。"
            ),
            ("活动就绪", "静止就绪") => (
                "挂起",
                "进程被暂停执行，保存其状态并进入静止就绪状态，等待恢复。",
                "在等待体验时，用户暂时决定离开等候区域，进程被暂时挂起，进入静止就绪状态。"
            ),
            ("活动就绪", "执行") => (
                "调度",
                "进程被调度器选中，分配到 CPU 开始执行。",
                "用户被叫到体验区，正式开始体验 Apple Vision Pro，进程被调度到 CPU，进入执行状态。"
            ),
            ("静止就绪", "活动就绪") => (
                "激活",
                "静止状态的进程被激活，重新进入活动就绪状态，准备被调度。",
                "用户决定继续排队，并重新进入活动就绪状态，准备体验产品。"
            ),
            ("执行", "活动阻塞") => (
                "请求I/O",
                "进程在执行过程中请求 I/O 操作，无法继续执行，进入活动阻塞状态，等待 I/O 完成。",
                "在体验过程中，用户请求某项特定功能，而该功能需要等待后台数据加载，进程进入活动阻塞状态。"
            ),
            ("执行", "活动就绪") => (
                "时间片完",
                "进程的时间片用完，回到活动就绪状态，等待下一次调度。",
                "用户体验时间结束，被要求返回等候区，进程在时间片结束后返回到活动就绪状态。"
            ),
            ("执行", "静止就绪") => (
                "挂起",
                "进程在执行过程中被挂起，保存状态并进入静止就绪状态，等待恢复。",
                "用户体验中途需要暂时离开，进程在执行过程中被挂起，进入静止就绪状态。"
            ),
            ("执行", "终止") => (
                "释放",
                "进程完成执行，所有资源释放，进入终止状态。",
                "用户完成了体验，关闭了体验程序，进程结束，释放所有与该体验相关的资源。"
            ),
            ("活动阻塞", "活动就绪") => (
                "释放",
                "I/O 操作完成，进程从阻塞状态回到活动就绪状态，等待调度。",
                "加载完成后，用户继续体验，进程从活动阻塞状态恢复为活动就绪状态。"
            ),
            ("活动阻塞", "静止阻塞") => (
                "挂起",
                "阻塞状态的进程被挂起，进入静止阻塞状态，等待恢复。",
                "用户在加载内容时决定离开等待区，进程进入静止阻塞状态，暂时不再进行操作。"
            ),
            ("静止阻塞", "活动阻塞") => (
                "激活",
                "静止状态的阻塞进程被激活，重新进入活动阻塞状态，等待 I/O 完成。",
                "用户返回体验后，进程从静止阻塞状态恢复为活动阻塞状态，继续等待内容加载。"
            ),
            ("静止阻塞", "静止就绪") => (
                "释放",
                "系统处理完成数据，进程从阻塞状态恢复为静止就绪状态。",
                "系统处理完数据，准备将用户重新激活，进程从静止阻塞状态恢复为静止就绪状态。"
            ),
            _ => ("未知", "未知过程解释", "未知转换的举例说明")
        };

        private void UpdateState(ProcessState state)
        {
            string previous = GetName(currentState);
            string current = GetName(state);
            currentState = state;
            TextBoxCurrentState.Text = current + "态";

            var text = GetExplanation(previous, current);
            TextBoxTransition.Text = $"{previous}态 --- {text.Item1} ---> {current}态";
            TextBoxExplanation.Text = text.Item2;
            TextBoxExample.Text = text.Item3;

            UpdateButtonStates();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            currentState = ProcessState.New;
            TextBoxCurrentState.Text = GetName(currentState) + "态";
            TextBoxTransition.Text = "";
            TextBoxExplanation.Text = "";
            TextBoxExample.Text = "";
            UpdateButtonStates();
        }

        private void ButtonToReadyActive_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(ProcessState.ReadyActive);
        }

        private void ButtonToReadySuspended_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(ProcessState.ReadySuspended);
        }

        private void ButtonToRunning_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(ProcessState.Running);
        }

        private void ButtonToBlockedActive_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(ProcessState.BlockedActive);
        }

        private void ButtonToBlockedSuspended_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(ProcessState.BlockedSuspended);
        }

        private void ButtonToTerminated_Click(object sender, RoutedEventArgs e)
        {
            UpdateState(ProcessState.Terminated);
        }

        #region Window Control Buttons

#nullable disable

        private void BorderControlButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ((UIElement)sender).CaptureMouse();
            }
        }

        private void BorderControlButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#4CFFFFFF");
        }

        private void BorderCloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#cd1a2b");
        }

        private void BorderControlButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Border)sender).Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#00000000");
        }

        public void UpdateControlButtonBackgroundTheme()
        {
            BorderMinimizeButton.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#00000000");
            BorderMaximizeButton.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#00000000");
            BorderCloseButton.Background = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#00000000");
        }

        private void BorderCloseButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)sender).ReleaseMouseCapture();
            if (MouseHelper.IsMouseInsideUIElement(sender, e))
            {
                Close();
            }
        }

        private void BorderMinimizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)sender).ReleaseMouseCapture();
            if (MouseHelper.IsMouseInsideUIElement(sender, e))
            {
                WindowState = WindowState.Minimized;
            }
        }

        private void BorderMaximizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)sender).ReleaseMouseCapture();
            WindowState = WindowState.Maximized == WindowState ? WindowState.Normal : WindowState.Maximized;
        }

#nullable enable

        #endregion Window Control Buttons

        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            if (GridAbout.Visibility == Visibility.Collapsed)
            {
                GridAbout.Visibility = Visibility.Visible;
                if (WindowState == WindowState.Normal) Width += GridAbout.Width;
            }
            else
            {
                GridAbout.Visibility = Visibility.Collapsed;
                if (WindowState == WindowState.Normal) Width -= GridAbout.Width;
            }
        }

        private void ButtonTheme_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonTheme.Icon == Wpf.Ui.Common.SymbolRegular.WeatherMoon16)
            {
                ButtonTheme.Icon = Wpf.Ui.Common.SymbolRegular.WeatherSunny16;

                Foreground = System.Windows.Media.Brushes.White;
                Background = System.Windows.Media.Brushes.Black;
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
            }
            else
            {
                ButtonTheme.Icon = Wpf.Ui.Common.SymbolRegular.WeatherMoon16;

                Foreground = System.Windows.Media.Brushes.Black;
                Background = System.Windows.Media.Brushes.White;
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
            }
            Wpf.Ui.Appearance.Background.Apply(
                this,
                Wpf.Ui.Appearance.BackgroundType.Mica);
        }
    }
}
