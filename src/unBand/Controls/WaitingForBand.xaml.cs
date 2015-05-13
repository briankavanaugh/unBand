using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace unBand.Controls
{
    /// <summary>
    ///     Interaction logic for WaitingForBand.xaml
    /// </summary>
    public partial class WaitingForBand : UserControl
    {
        private const int WM_CLOSE = 0x10;
        private const int WM_QUIT = 0x12;

        public WaitingForBand()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private async void btnCloseSyncApp_Click(object sender, RoutedEventArgs e)
        {
            // let's play nice and try to gracefully clear out all Sync processes
            var procs = Process.GetProcessesByName("Microsoft Band Sync");

            foreach (var proc in procs)
            {
                // lParam == Band Process Id, passed in below
                EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
                {
                    uint processId = 0;
                    GetWindowThreadProcessId(hWnd, out processId);

                    // Essentially: Find every hWnd associated with this process and ask it to go away
                    if (processId == (uint) lParam)
                    {
                        SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        SendMessage(hWnd, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                    }

                    return true;
                },
                    (IntPtr) proc.Id);
            }

            // let everything calm down
            await Task.Delay(1000);

            procs = Process.GetProcessesByName("Microsoft Band Sync");

            // ok, no more mister nice guy. Sadly.
            foreach (var proc in procs)
            {
                proc.Kill();
            }
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }
}