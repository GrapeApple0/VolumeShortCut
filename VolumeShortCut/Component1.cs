using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading.Tasks; 
using System.Windows.Forms;

namespace VolumeShortCut
{
    public partial class Component1 : Component
    {
        HotKey hotKey1;
        HotKey hotKey2;
        HotKey hotKey3;

        public Component1()
        {
            InitializeComponent();
            hotKey1 = new HotKey(MOD_KEY.CONTROL, Keys.F12);
            hotKey1.HotKeyPush += new EventHandler(VolumeUp);
            hotKey2 = new HotKey(MOD_KEY.CONTROL, Keys.F11);
            hotKey2.HotKeyPush += new EventHandler(VolumeDown);
            hotKey3 = new HotKey(MOD_KEY.CONTROL, Keys.F10);
            hotKey3.HotKeyPush += new EventHandler(Mute);
        }

        frmBackground fbg = null;

        public void VolumeUp(object sender, EventArgs e)
        {
            AudioManager.SetMasterVolume((int)AudioManager.GetMasterVolume() + 2);
            if (fbg == null || fbg.IsDisposed || fbg.isShow == false)
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
                fbg = new frmBackground(newImage);
                fbg.Show();
            }
            else
            {
                fbg.Show();
                frmBackground.enable_refresh = true;
            }
        }

        public void VolumeDown(object sender, EventArgs e)
        {
            AudioManager.SetMasterVolume((int)AudioManager.GetMasterVolume() - 2);

            if (fbg == null || fbg.IsDisposed || fbg.isShow == false)
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
                fbg = new frmBackground(newImage);
                fbg.Show();
            }
            else
            {
                fbg.Show();
                frmBackground.enable_refresh = true;
            }
        }

        public void Mute(object sender, EventArgs e)
        {
            AudioManager.SetMasterVolume(0f);
            if (fbg == null || fbg.IsDisposed || fbg.isShow == false)
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
                fbg = new frmBackground(newImage);
                fbg.Show();
            }
            else
            {
                fbg.Show();
                frmBackground.enable_refresh = true;
            }
        }
    }

    public static class AudioManager
    {
        public static void SetMasterVolume(float newLevel)
        {
            IAudioEndpointVolume masterVol = null;

            try
            {
                masterVol = GetMasterVolumeObject();

                if (masterVol == null)
                {
                    return;
                }

                masterVol.SetMasterVolumeLevelScalar(newLevel / 100, Guid.Empty);
            }
            finally
            {
                if (masterVol != null)
                {
                    Marshal.ReleaseComObject(masterVol);
                }
            }
        }

        public static float GetMasterVolume()
        {
            IAudioEndpointVolume masterVol = null;
            try
            {
                masterVol = GetMasterVolumeObject();
                if (masterVol == null)
                    return -1;

                float volumeLevel;
                masterVol.GetMasterVolumeLevelScalar(out volumeLevel);
                return volumeLevel * 100;
            }
            finally
            {
                if (masterVol != null)
                    Marshal.ReleaseComObject(masterVol);
            }
        }

        private static IAudioEndpointVolume GetMasterVolumeObject()
        {
            IMMDeviceEnumerator deviceEnumerator = null;
            IMMDevice speakers = null;

            try
            {
                deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
                deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

                Guid IID_IAudioEndpointVolume = typeof(IAudioEndpointVolume).GUID;

                speakers.Activate(ref IID_IAudioEndpointVolume, 0, IntPtr.Zero, out object endpointVolume);
                IAudioEndpointVolume masterVol = (IAudioEndpointVolume)endpointVolume;

                return masterVol;
            }
            finally
            {
                if (speakers != null) Marshal.ReleaseComObject(speakers);
                if (deviceEnumerator != null) Marshal.ReleaseComObject(deviceEnumerator);
            }
        }

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        internal class MMDeviceEnumerator
        {
        }

        internal enum EDataFlow
        {
            eRender,
            eCapture,
            eAll,
            EDataFlow_enum_count
        }

        internal enum ERole
        {
            eConsole,
            eMultimedia,
            eCommunications,
            ERole_enum_count
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMMDeviceEnumerator
        {
            int NotImpl1();

            [PreserveSig]
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IMMDevice
        {
            [PreserveSig]
            int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
        }

        [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionManager2
        {
            int NotImpl1();
            int NotImpl2();

            [PreserveSig]
            int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);
        }

        [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionEnumerator
        {
            [PreserveSig]
            int GetCount(out int SessionCount);

            [PreserveSig]
            int GetSession(int SessionCount, out IAudioSessionControl2 Session);
        }

        [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface ISimpleAudioVolume
        {
            [PreserveSig]
            int SetMasterVolume(float fLevel, ref Guid EventContext);

            [PreserveSig]
            int GetMasterVolume(out float pfLevel);

            [PreserveSig]
            int SetMute(bool bMute, ref Guid EventContext);

            [PreserveSig]
            int GetMute(out bool pbMute);
        }

        [Guid("bfb7ff88-7239-4fc9-8fa2-07c950be9c6d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAudioSessionControl2
        {
            [PreserveSig]
            int NotImpl0();

            [PreserveSig]
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetIconPath([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int SetIconPath([MarshalAs(UnmanagedType.LPWStr)] string Value, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int GetGroupingParam(out Guid pRetVal);

            [PreserveSig]
            int SetGroupingParam([MarshalAs(UnmanagedType.LPStruct)] Guid Override, [MarshalAs(UnmanagedType.LPStruct)] Guid EventContext);

            [PreserveSig]
            int NotImpl1();

            [PreserveSig]
            int NotImpl2();

            [PreserveSig]
            int GetSessionIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int GetSessionInstanceIdentifier([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

            [PreserveSig]
            int GetProcessId(out int pRetVal);

            [PreserveSig]
            int IsSystemSoundsSession();

            [PreserveSig]
            int SetDuckingPreference(bool optOut);
        }

        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAudioEndpointVolume
        {
            [PreserveSig]
            int NotImpl1();

            [PreserveSig]
            int NotImpl2();

            [PreserveSig]
            int GetChannelCount(
                [Out][MarshalAs(UnmanagedType.U4)] out UInt32 channelCount);

            [PreserveSig]
            int SetMasterVolumeLevel(
                [In][MarshalAs(UnmanagedType.R4)] float level,
                [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

            [PreserveSig]
            int SetMasterVolumeLevelScalar(
                [In][MarshalAs(UnmanagedType.R4)] float level,
                [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

            [PreserveSig]
            int GetMasterVolumeLevel(
                [Out][MarshalAs(UnmanagedType.R4)] out float level);

            [PreserveSig]
            int GetMasterVolumeLevelScalar(
                [Out][MarshalAs(UnmanagedType.R4)] out float level);

            [PreserveSig]
            int SetChannelVolumeLevel(
                [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
                [In][MarshalAs(UnmanagedType.R4)] float level,
                [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

            [PreserveSig]
            int SetChannelVolumeLevelScalar(
                [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
                [In][MarshalAs(UnmanagedType.R4)] float level,
                [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

            [PreserveSig]
            int GetChannelVolumeLevel(
                [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
                [Out][MarshalAs(UnmanagedType.R4)] out float level);

            [PreserveSig]
            int GetChannelVolumeLevelScalar(
                [In][MarshalAs(UnmanagedType.U4)] UInt32 channelNumber,
                [Out][MarshalAs(UnmanagedType.R4)] out float level);

            [PreserveSig]
            int SetMute(
                [In][MarshalAs(UnmanagedType.Bool)] Boolean isMuted,
                [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

            [PreserveSig]
            int GetMute(
                [Out][MarshalAs(UnmanagedType.Bool)] out Boolean isMuted);

            [PreserveSig]
            int GetVolumeStepInfo(
                [Out][MarshalAs(UnmanagedType.U4)] out UInt32 step,
                [Out][MarshalAs(UnmanagedType.U4)] out UInt32 stepCount);

            [PreserveSig]
            int VolumeStepUp(
                [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

            [PreserveSig]
            int VolumeStepDown(
                [In][MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);

            [PreserveSig]
            int QueryHardwareSupport(
                [Out][MarshalAs(UnmanagedType.U4)] out UInt32 hardwareSupportMask);

            [PreserveSig]
            int GetVolumeRange(
                [Out][MarshalAs(UnmanagedType.R4)] out float volumeMin,
                [Out][MarshalAs(UnmanagedType.R4)] out float volumeMax,
                [Out][MarshalAs(UnmanagedType.R4)] out float volumeStep);
        }
    }
}
