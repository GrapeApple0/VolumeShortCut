using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace VolumeShortCut
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {
            NotifyIcon trayIcon = new NotifyIcon();
            void TaskTray()
            {
                trayIcon.Visible = true;
                trayIcon.Text = "VolumeShortcut";
                ContextMenuStrip menu = new ContextMenuStrip();
                menu.Items.AddRange(new ToolStripMenuItem[]{
                    new ToolStripMenuItem("E&xit", null, (s,e)=>{Exit();}, "Exit")
                });
                trayIcon.ContextMenuStrip = menu;
            }

            void Exit()
            {
                CancelEventArgs e = new CancelEventArgs();
                trayIcon.Visible = false;
                Application.Exit(e);
            }

            string mutexName = "vsc";
            bool createdNew;
            System.Threading.Mutex mutex =
                new System.Threading.Mutex(true, mutexName, out createdNew);
            if (createdNew == false)
            {
                MessageBox.Show("すでに起動しているためアプリを終了します");
                mutex.Close();
                return;
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                trayIcon.Icon = Properties.Resources.vol;
                TaskTray();
                Component1 comp = new Component1();
                Application.Run();
            }
            finally
            {
                mutex.ReleaseMutex();
                mutex.Close();
            }

            if (restarting)
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath);
            }
        }

        private static bool restarting = false;
        /// <summary>
        /// アプリケーションを再起動する
        /// </summary>
        public static void RestartApplication()
        {
            restarting = true;
            Application.Exit();
        }
    }
}
