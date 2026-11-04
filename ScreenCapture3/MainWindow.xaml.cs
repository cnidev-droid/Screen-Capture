using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

/*
 * WPF apps are System DPI aware by default, and need to declare themselves to be Per Monitor DPI aware via an app.manifest file. To add an app.manifest:
•Right click on your project folder, click Add -> New Item
•Click on General in the left pane, and chose Application Manifest File.
•Uncomment the following snippet of xml:

 * <application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <!-- The combination of below two tags have the following effect : 
      1) Per-Monitor for >= Windows 10 Anniversary Update
      2) System < Windows 10 Anniversary Update -->
      <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true</dpiAware>
    </windowsSettings>
  </application>
  */

namespace ScreenCapture3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static double capLeft = 150, capTop = 100, capWidth = 500, capHeight = 500;
        public static Window capWindow = null;
        bool saveAsBmp = false;
        string curDir, saveDir;
        string fileName;
        public static double scalingFactor=1;

        //To get mouse pos on screen
        DispatcherTimer timerMousePos = new DispatcherTimer();
        MouseEvents mouseEvents = new MouseEvents();
        bool isLocked = false;
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(string lpszDriver, string lpszDeviceName, string lpszOutput, IntPtr devMode);
        [DllImport("gdi32.dll")]
        public static extern ulong GetPixel(IntPtr hdc, int x, int y);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        //To include mouse icone in cap
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);
        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;
        bool withMousePointer = false;


        //autocap
        DispatcherTimer timerTakeCap = new DispatcherTimer();


        public MainWindow()
        {
            InitializeComponent();
            //alvays  use decimal .
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            curDir = Directory.GetCurrentDirectory();
            saveDir = curDir + "\\Files";
            TxtBoDir.Text = saveDir;

            if (Directory.Exists(saveDir) == false)
            {
                Directory.CreateDirectory(saveDir);
            }

            timerTakeCap.Tick += new EventHandler(timerTakeCap_Tick);
            timerTakeCap.Interval = TimeSpan.FromMilliseconds(1000);

            timerMousePos.Interval = TimeSpan.FromMilliseconds(100);
            timerMousePos.Tick += new EventHandler(timerMousePos_Tick);
            timerMousePos.Start();

        }

        //For auto cap
        private void timerTakeCap_Tick(object sender, EventArgs e)
        {
            if(autoCapActive)
            {
                takeCap(withMousePointer);
            }
            else
            {
                timerTakeCap.Stop();
            }
        }

        //*********************************
        //Get mouse pos
        //*********************************
        private void timerMousePos_Tick(object sender, EventArgs e)
        {
            if (!isLocked)
            {
                ulong cr;
                int x = 0, y = 0;
                //class MouseEvents DllImport user32.dll
                if (mouseEvents.getMousePos(ref x, ref y))
                {
                    TxtBlMousePosXY.Text = x.ToString() + " " + y.ToString();

                    //gdi32.dll
                    var hdc = CreateDC("DISPLAY", "", "", IntPtr.Zero);
                    cr = GetPixel(hdc, x, y);
                    byte[] rgb = BitConverter.GetBytes(cr);
                    DeleteDC(hdc);

                    string sout = rgb[0].ToString() + "," + rgb[1].ToString() + "," + rgb[2].ToString() + "  #" + rgb[0].ToString("X") + rgb[1].ToString("X") + rgb[2].ToString("X");
                    TxtBoRGB.Text = sout;
                }
            }
        }


        //**********************************
        //To be able to drop in Textbox
        //**********************************
        private void TxtBoDir_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        //**********************************
        //Drop saving directory to Textbox
        //**********************************
        private void TxtBoDir_PreviewDrop(object sender, DragEventArgs e)
        {
            System.Windows.Point mp = e.GetPosition(this.TxtBoDir);
            string[] dr = (string[])e.Data.GetData(DataFormats.FileDrop);

            DirectoryInfo dir = new DirectoryInfo(dr[0]);
            TxtBoDir.Text = dir.FullName;
            saveDir = dir.FullName;
        }

        //**********************************
        //Add saving directory from Textbox using Enter key
        //**********************************
        private void TxtBoDir_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                string fi = TxtBoDir.Text;
                if(!Directory.Exists(fi))
                {
                    try
                    {
                        Directory.CreateDirectory(fi);
                        saveDir = fi;
                    }
                    catch
                    {
                        MessageBox.Show("Can't create dir:" + fi);
                    }
                }
                else
                {
                    saveDir = fi;
                }
            }
        }

        //*********************************
        //Saving file im .bmp
        //*********************************
        private void chkBoxBmp_Checked(object sender, RoutedEventArgs e)
        {
            saveAsBmp = true;
        }

        //*********************************
        //Saving file im .jpg
        //*********************************
        private void chkBoxBmp_Unchecked(object sender, RoutedEventArgs e)
        {
            saveAsBmp = false;
        }

        //*********************************
        //Open saving directory in filehandler
        //*********************************
        private void BtnOpenLocation_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(saveDir);
        }

        //*********************************
        //Start auto capture of selected area
        //*********************************
        bool autoCapActive = false;
        private void ChkBoxAutoCap_Checked(object sender, RoutedEventArgs e)
        {
            autoCapActive = true;
            timerTakeCap.Start();
        }

        //*********************************
        //Stop auto capture of selected area
        //*********************************
        private void ChkBoxAutoCap_Unchecked(object sender, RoutedEventArgs e)
        {
            if (timerTakeCap.IsEnabled)
            {
                autoCapActive = false;
                timerTakeCap.Stop();
            }
        }

        //*********************************
        //Lock RGB value
        //*********************************
        private void ChkBoxLock_Checked(object sender, RoutedEventArgs e)
        {
            isLocked = true;
        }

        //*********************************
        //Unlock RGB value
        //*********************************
        private void ChkBoxLock_Unchecked(object sender, RoutedEventArgs e)
        {
            isLocked = false;
        }

        //*********************************
        //Save mouse pointer icone in cap file
        //*********************************
        private void ChkBoxMouse_Checked(object sender, RoutedEventArgs e)
        {
            withMousePointer = true;
        }

        //*********************************
        //Not save mouse pointer icone in cap file
        //*********************************
        private void ChkBoxMouse_Unchecked(object sender, RoutedEventArgs e)
        {
            withMousePointer = false;
        }

        //*********************************
        //Take single screen capture
        //*********************************
        private void BtnTakeCap_Click(object sender, RoutedEventArgs e)
        {
            string wh = TxtBoCapSize.Text;
            double w = capWidth;
            double h = capHeight;
            if(wh!="" && wh!="w*h")
            {
                string[] del = wh.Split('*');
                if(del.Length==2)
                {
                    try
                    {
                        capWidth = int.Parse(del[0]);
                        capHeight = int.Parse(del[1]);
                    }
                    catch
                    {
                        capHeight = h;
                        capWidth = w;
                    }
                }
            }
            capWindow = new CapWin() { Owner = this };
            capWindow.Closed += new EventHandler(capWindow_closed);
            capWindow.Show();
            this.Visibility = Visibility.Hidden;
        }

        //*********************************
        //Selection window closing.
        //Take kap of selected area
        //*********************************
        void capWindow_closed(object sender, EventArgs e)
        {
            this.Activate();
            capWindow = null;
            this.Visibility = Visibility.Visible;

            if (takeCap(withMousePointer) == 1)
            {
                TxtBlMsg.Text = "File on clipboard and saved as\n" + fileName;
            }

        }

        //*********************************
        //Methode for taking cap
        //*********************************
        int takeCap(bool CaptureMouse)
        {
            Bitmap bitmap = new Bitmap((int)(capWidth * scalingFactor), (int)(capHeight * scalingFactor));
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                try
                {
                    g.CopyFromScreen((int)(capLeft * scalingFactor), (int)(capTop * scalingFactor), 0, 0, bitmap.Size);

                    if (CaptureMouse)
                    {
                        CURSORINFO pci;
                        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                        if (GetCursorInfo(out pci))
                        {
                            if (pci.flags == CURSOR_SHOWING)
                            {
                                int left = pci.ptScreenPos.x - (int)capLeft;
                                int top = pci.ptScreenPos.y - (int)capTop;
                                DrawIcon(g.GetHdc(), left, top, pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }

                    string time = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss.fff");
                    if (saveAsBmp == false)
                    {
                        System.Drawing.Imaging.Encoder myEncoder;
                        EncoderParameter myEncoderParameter;
                        EncoderParameters myEncoderParameters;// system drawing + use System.Drawing.Imaging
                        ImageCodecInfo myImageCodecInfo;

                        // Get an ImageCodecInfo object that represents the JPEG codec.
                        myImageCodecInfo = GetEncoderInfo("image/jpeg");
                        // Create an Encoder object based on the GUID
                        // for the Quality parameter category.
                        myEncoder = System.Drawing.Imaging.Encoder.Quality;
                        // Create an EncoderParameters object.
                        // An EncoderParameters object has an array of EncoderParameter
                        // objects. In this case, there is only one
                        // EncoderParameter object in the array.
                        myEncoderParameters = new EncoderParameters(1);
                        // Save the bitmap as a JPEG file with quality level 90.
                        myEncoderParameter = new EncoderParameter(myEncoder, 90L);
                        myEncoderParameters.Param[0] = myEncoderParameter;

                        int fc = 0;
                        string givenName = TxtBoName.Text;
                        if (givenName != "")
                        {
                            fileName = saveDir + "\\" + givenName + ".jpg";
                            while(File.Exists(fileName))
                            {
                                fileName = saveDir + "\\" + givenName + "_" + fc.ToString("000") + ".jpg";
                                fc++;
                            }
                        }
                        else
                        {
                            fileName = saveDir + "\\Img_" + time + ".jpg";
                        }
                        bitmap.Save(fileName, myImageCodecInfo, myEncoderParameters); //saving
                    }
                    else
                    {
                        string givenName = TxtBoName.Text;
                        if (givenName != "")
                        {
                            fileName = saveDir + "\\" + givenName + ".bmp";
                        }
                        else
                        {
                            fileName = saveDir + "\\Img_" + time + ".bmp";
                        }
                        bitmap.Save(fileName, ImageFormat.Bmp); //saving
                    }
                    image.Source = ConvertBitmap(bitmap);
                    Clipboard.SetImage(ConvertBitmap(bitmap));
                }
                catch
                {
                    MessageBox.Show("Can't make CopyFromScreen!");
                    return -1;
                }

            }
            return 1;
        }
        //********************************
        //Bitmap to bitmapSource
        //********************************
        public static BitmapSource ConvertBitmap(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        //********************************
        //Get an ImageCodecInfo object here jpeg codec.
        //********************************
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

    }
}
