using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScreenCapture3
{
    /// <summary>
    /// Interaction logic for CapWin.xaml
    /// </summary>
    public partial class CapWin : Window
    {
        public CapWin()
        {
            InitializeComponent();
            Left = MainWindow.capLeft;
            Top = MainWindow.capTop;
            Width = MainWindow.capWidth;
            Height = MainWindow.capHeight;

            Topmost = true;
        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.scalingFactor = getScalingFactor();
            MainWindow.capLeft = (int)Left;
            MainWindow.capTop = (int)Top;
            MainWindow.capWidth = (int)Width;
            MainWindow.capHeight = (int)Height;
            this.Close();
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private double getScalingFactor()
        {
            //PresentationSource mainWindowPresentationSource = PresentationSource.FromVisual(cw);
            //Matrix m = mainWindowPresentationSource.CompositionTarget.TransformToDevice;
            Matrix m = PresentationSource.FromVisual(MyGrid).CompositionTarget.TransformToDevice;
            double dx = m.M11; // notice it's divided by 96 already
            double dy = m.M22; // notice it's divided by 96 already

            return dx; // 1.25 = 125%
        }

    }
}
