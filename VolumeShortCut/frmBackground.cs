using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VolumeShortCut
{
    class frmBackground : AddEventForm
    {
        private static Form _parent;
        private int _offsetx;
        private int _offsety;
        private Bitmap _bitmap;
        private Timer timer = new Timer();
        private Timer timer2 = new Timer();
        public static bool enable_refresh = false;
        public static bool reshow = false;
        public bool isShow = false;

        public frmBackground(Bitmap bitmap, int offsetx = 0, int offsety = 0)
        {
            _parent = this;
            _bitmap = bitmap;
            isShow = true;
            this.TopMost = _parent.TopMost;
            if (TopMost)
                _parent.TopMost = true;
            _parent.ShowInTaskbar = false;
            this.ShowInTaskbar = false;
            this.Size = bitmap.Size;
            _bitmap = RoundBitmap.AdjustAlpha(_bitmap, 0.75f);
            this.SelectBitmap(_bitmap);
            _offsetx = offsetx;
            _offsety = offsety;
            this.Shown += Parent_Shown;
            this.Activated += Parent_Activated;
            timer.Tick += timer_Tick;
            timer2.Tick += timer2_Tick;
            timer.Interval = 2500;
            timer.Enabled = true;
            timer2.Interval = 10;
            timer2.Enabled = true;
            MaximumSize = Size;
            MinimumSize = Size;
            enable_refresh = true;
            int screen_x = Screen.FromControl(this).WorkingArea.X;
            int screen_y = Screen.FromControl(this).WorkingArea.Y;
            this.Left = screen_x;
            this.Top = screen_y;
            this.Left += (Screen.FromControl(this).WorkingArea.Width - this.Width) / 2;
            this.Top += (Screen.FromControl(this).WorkingArea.Height - this.Height) / 2 + 300;
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            for (float f = 0.75f; f > 0f; f -= 0.03f)
            {
                _bitmap = RoundBitmap.AdjustAlpha(_bitmap, f);
                this.SelectBitmap(_bitmap);
                await Task.Delay(30);
            }
            GC.Collect();
            this.Hide();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (enable_refresh)
            {
                Bitmap baseImage = new Bitmap(Properties.Resources.back);
                Bitmap logoImage = new Bitmap(Properties.Resources.vol3);
                Bitmap newImage = new Bitmap(baseImage);
                logoImage = RoundBitmap.ResizeBitmap(logoImage, 85, 85, InterpolationMode.NearestNeighbor);
                Graphics g = Graphics.FromImage(newImage);
                g.DrawImage(logoImage, baseImage.Width / 2 - logoImage.Width / 2, baseImage.Height / 2 - logoImage.Height / 2, logoImage.Width, logoImage.Height);
                g.Dispose();
                Graphics g2 = Graphics.FromImage(newImage);
                g2.FillRectangle(new SolidBrush(Color.FromArgb(64, 64, 64)), 25, baseImage.Height - 30, AudioManager.GetMasterVolume(), 10);
                g2.Dispose();
                baseImage.Dispose();
                logoImage.Dispose();
                newImage = RoundBitmap.AdjustAlpha(newImage, 0.75f);
                this.SelectBitmap(newImage);
                timer.Stop();
                timer.Start();
                enable_refresh = false;
                newImage.Dispose();
            }
            else if (reshow)
            {
                this.Show();
                reshow = false;
            }
        }

        private void Parent_Shown(object sender, EventArgs e)
        {
            FixLocation();
            this.Show();
            this.BringToFront();
            this.TopMost = true;
            this.Focus();
            this.TopMost = true;
            _parent.TopMost = true;
            _parent.Focus();
            _parent.TopMost = true;
        }

        private void Parent_FormClosing(object sender, FormClosingEventArgs e)
        {
            isShow = false;
            e.Cancel = true;
            Close();
            e.Cancel = false;
        }

        private void FixLocation()
        {
            Point point = _parent.Location;
            point.Offset(_offsetx, _offsety);
            this.Location = point;
        }

        private void Parent_Activated(object sender, EventArgs e)
        {
            var reorder = ApiHelper.GetWindow(_parent.Handle, ApiHelper.GetWindow_Cmd.GW_HWNDNEXT) != this.Handle;
            if (reorder)
            {
                BringToFront();
                _parent.BringToFront();
            }
        }

        public void SelectBitmap(Bitmap bitmap)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                throw new ApplicationException("The bitmap must be 32bpp with alpha-channel.");
            }

            IntPtr screenDc = ApiHelper.GetDC(IntPtr.Zero);
            IntPtr memDc = ApiHelper.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr hOldBitmap = IntPtr.Zero;
            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                hOldBitmap = ApiHelper.SelectObject(memDc, hBitmap);
                ApiHelper.ApiSize newApiSize = new ApiHelper.ApiSize(bitmap.Width, bitmap.Height);
                ApiHelper.ApiPoint sourceLocation = new ApiHelper.ApiPoint(0, 0);
                ApiHelper.ApiPoint newLocation = new ApiHelper.ApiPoint(this.Left, this.Top);
                ApiHelper.BLENDFUNCTION blend = new ApiHelper.BLENDFUNCTION();
                blend.BlendOp = ApiHelper.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = 255;
                blend.AlphaFormat = ApiHelper.AC_SRC_ALPHA;
                ApiHelper.UpdateLayeredWindow(Handle, screenDc, ref newLocation, ref newApiSize, memDc, ref sourceLocation, 0, ref blend, ApiHelper.ULW_ALPHA);
            }
            catch { }
            finally
            {
                ApiHelper.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    ApiHelper.SelectObject(memDc, hOldBitmap);
                    ApiHelper.DeleteObject(hBitmap);
                }
                ApiHelper.DeleteDC(memDc);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                p.ExStyle = p.ExStyle | ApiHelper.WS_EX_LAYERED;
                return p;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmBackground
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "frmBackground";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.frmBackground_Load);
            this.ResumeLayout(false);

        }

        private void frmBackground_Load(object sender, EventArgs e)
        {

        }

        private void frmBackground_Showing(object sender, EventArgs e)
        {

        }
    }

    public class ApiHelper
    {
        public const Int32 WS_EX_LAYERED = 524288;
        public const int WM_NCHITTEST = 132;
        public const int HTCLIENT = 1;
        public const int HTCAPTION = 2;
        public const Int32 ULW_ALPHA = 2;
        public const byte AC_SRC_OVER = 0;
        public const byte AC_SRC_ALPHA = 1;


        [StructLayout(LayoutKind.Sequential)]
        public struct ApiPoint
        {
            public int X;
            public int Y;
            public ApiPoint(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public ApiPoint(Point pt)
            {
                this.X = pt.X;
                this.Y = pt.Y;
            }
        }

        public enum BoolEnum
        {
            False = 0,
            True = 1
        }

        public enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ApiSize
        {
            public Int32 cx;
            public Int32 cy;
            public ApiSize(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        public enum GWL : int
        {
            ExStyle = -20
        }

        public enum WS_EX : int
        {
            Transparent = 32,
            Layered = 524288
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern BoolEnum UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref ApiPoint pptDst, ref ApiSize psize, IntPtr hdcSrc, ref ApiPoint pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern BoolEnum DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern BoolEnum DeleteObject(IntPtr hObject);
    }
}