using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCapture3
{
    public class MouseEvents
    {
        [DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPos);
        [StructLayout(LayoutKind.Sequential)]
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

        //to get mouse pos on screen
        public bool getMousePos(ref int x, ref int y)
        {
            POINT p;
            if (GetCursorPos(out p))
            {
                x = p.X;
                y = p.Y;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
