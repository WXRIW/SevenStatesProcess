using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Lyricify.Helpers
{
    public static class MouseHelper
    {
        public static bool IsMouseInsideUIElement(FrameworkElement element)
        {
            Point point = Mouse.GetPosition(element);
            if (point.X >= 0 && point.Y >= 0 && point.X <= element.ActualWidth && point.Y <= element.ActualHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsMouseInsideUIElement(FrameworkElement element, MouseEventArgs e)
        {
            Point point = e.GetPosition(element);
            if (point.X >= 0 && point.Y >= 0 && point.X <= element.ActualWidth && point.Y <= element.ActualHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsMouseInsideUIElement(FrameworkElement element, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(element);
            if (point.X >= 0 && point.Y >= 0 && point.X <= element.ActualWidth && point.Y <= element.ActualHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsMouseInsideUIElement(object element, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition((FrameworkElement)element);
            if (point.X >= 0 && point.Y >= 0
                && point.X <= ((FrameworkElement)element).ActualWidth
                && point.Y <= ((FrameworkElement)element).ActualHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Direction MouseEnterDirection(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition((FrameworkElement)sender);
            double width = ((FrameworkElement)sender).ActualWidth;
            double height = ((FrameworkElement)sender).ActualHeight;
            double x = point.X - width / 2;
            double y = point.Y - height / 2;
            if (point.X <= width / 2 && Math.Abs(x / y) >= width / height)
            {
                return Direction.Left;
            }
            else if (point.X >= width / 2 && Math.Abs(x / y) >= width / height)
            {
                return Direction.Right;
            }
            else if (y >= 0)
            {
                return Direction.Bottom;
            }
            else
            {
                return Direction.Top;
            }
        }

        public static Direction MouseEnterDirectionVerticalSensitive(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition((FrameworkElement)sender);
            double width = ((FrameworkElement)sender).ActualWidth;
            double height = ((FrameworkElement)sender).ActualHeight;
            double x = point.X - width / 2;
            double y = point.Y - height / 2;
            if (point.X <= 5 && Math.Abs(x / y) >= width / height)
            {
                return Direction.Left;
            }
            else if (point.X >= width - 5 && Math.Abs(x / y) >= width / height)
            {
                return Direction.Right;
            }
            else if (y >= 0)
            {
                return Direction.Bottom;
            }
            else
            {
                return Direction.Top;
            }
        }

        public enum Direction
        {
            Left,
            Right,
            Top,
            Bottom
        }

        public struct POINT
        {
            public int X;
            public int Y;
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out POINT pt);
    }
}
