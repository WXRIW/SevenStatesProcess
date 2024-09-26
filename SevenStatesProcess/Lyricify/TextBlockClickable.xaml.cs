using Lyricify.Helpers.General;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lyricify.Lite.Controls
{
    /// <summary>
    /// Interaction logic for TextBlockClickable.xaml
    /// </summary>
    public partial class TextBlockClickable : TextBlock
    {
        public TextBlockClickable()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextDecorations = new TextDecorationCollection
            {
                new TextDecoration() { PenOffset = 0.5 }
            };
            Cursor = Cursors.Hand;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextDecorations = new TextDecorationCollection();
            Cursor = null;
            isMouseDown = false;
        }

        bool isMouseDown = false;
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;
            CaptureMouse();
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            if (isMouseDown)
            {
                isMouseDown = false;

                if (!string.IsNullOrEmpty(Link))
                {
                    GeneralHelper.ProcessStartUrl(Link);
                }

                if (e.ChangedButton == MouseButton.Left)
                {
                    RoutedEventArgs _e = new()
                    {
                        RoutedEvent = ClickRoutedEvent,
                        Source = this
                    };
                    RaiseEvent(_e);
                }
                else if (e.ChangedButton == MouseButton.Right)
                {
                    RoutedEventArgs _e = new()
                    {
                        RoutedEvent = RightClickRoutedEvent,
                        Source = this
                    };
                    RaiseEvent(_e);
                }
            }
        }

        public string? Link { get; set; }

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickRoutedEvent, value); }
            remove { RemoveHandler(ClickRoutedEvent, value); }
        }
        public static readonly RoutedEvent ClickRoutedEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBlockClickable));

        public event RoutedEventHandler RightClick
        {
            add { AddHandler(RightClickRoutedEvent, value); }
            remove { RemoveHandler(RightClickRoutedEvent, value); }
        }
        public static readonly RoutedEvent RightClickRoutedEvent = EventManager.RegisterRoutedEvent("RightClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBlockClickable));
    }
}
