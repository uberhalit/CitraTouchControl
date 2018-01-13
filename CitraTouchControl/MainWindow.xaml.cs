using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Management;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CitraTouchControl
{
    public partial class MainWindow : Window
    {
        internal Mutex mutex;
        internal bool isMenuWindow = false;
        internal string userPath;
        private static IntPtr citraHwnd = IntPtr.Zero;
        private static IntPtr citraMainControlHwnd = IntPtr.Zero;
        private List<IntPtr> pressedControls = new List<IntPtr>();
        
        public MainWindow()
        {
            // check if we are the only instance of CitraTouchControl
            bool isNew = false;
            mutex = new Mutex(true, "CitraTouchControl", out isNew);
            if (!isNew)
            {
                MessageBox.Show(this, "ERROR: There is already an instance of CitraTouchControl running.", "CitraTouchControl");
                Environment.Exit(0);
            }
            GC.KeepAlive(mutex);

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // check if citra is running
            Process[] processes = Process.GetProcessesByName("citra-qt");
            if (processes.Length < 1)
            {
                MessageBox.Show(this, "ERROR: Citra not running!\nPlease start Citra (citra-qt.exe) first.", "CitraTouchControl");
                Environment.Exit(0);
            }
            // get citra process handle
            citraHwnd = processes[0].MainWindowHandle;

            // check if citra is minimized
            if (IsIconic(citraHwnd))
            {
                MessageBox.Show(this, "ERROR: Citra is minimized!\nPlease restore the Citra window first.", "CitraTouchControl");
                Environment.Exit(0);
            }
            // check if active citra window is Qt-GUI
            IntPtr citraIsQt = FindWindow("Qt5QWindowIcon", processes[0].MainWindowTitle);
            if (citraIsQt == IntPtr.Zero)
            {
                // active citra window is console
                MessageBox.Show(this, "ERROR: Active Citra window not Qt-GUI!\nPlease bring the Citra Qt-GUI to the front first.", "CitraTouchControl");
                Environment.Exit(0);
            }

            // get Citra savegame folder
            userPath = Path.Combine(Path.GetDirectoryName(GetProcessPath(processes[0])), "user\\sdmc");
            if (!Directory.Exists(userPath))
            {
                userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Citra\\sdmc");
                if (!Directory.Exists(userPath))
                {
                    MessageBox.Show(this, "WARNING: Savegame folder could not be located!\nSavestate functionality will not work.", "CitraTouchControl");
                    userPath = null;
                }
            }

            ReloadOverlay();

            // load user settings into fast accessable global vars
            GlobalVars.A_KEY = Properties.Settings.Default.A_KEY;
            GlobalVars.B_KEY = Properties.Settings.Default.B_KEY;
            GlobalVars.X_KEY = Properties.Settings.Default.X_KEY;
            GlobalVars.Y_KEY = Properties.Settings.Default.Y_KEY;
            GlobalVars.L_KEY = Properties.Settings.Default.L_KEY;
            GlobalVars.R_KEY = Properties.Settings.Default.R_KEY;
            GlobalVars.LEFT_KEY = Properties.Settings.Default.LEFT_KEY;
            GlobalVars.RIGHT_KEY = Properties.Settings.Default.RIGHT_KEY;
            GlobalVars.UP_KEY = Properties.Settings.Default.UP_KEY;
            GlobalVars.DOWN_KEY = Properties.Settings.Default.DOWN_KEY;
            GlobalVars.START_KEY = Properties.Settings.Default.START_KEY;
            GlobalVars.SELECT_KEY = Properties.Settings.Default.SELECT_KEY;
            if (Properties.Settings.Default.IsTouchEnabled)
            {
                GlobalVars.IsTouchEnabled = true;
                ToggleTouch(true);
            }
            if (Properties.Settings.Default.AreControlsHidden)
            {
                GlobalVars.AreControlsHidden = true;
                ToggleControls(true);
            }
            GlobalVars.IsTapOnly = Properties.Settings.Default.IsTapOnly;
            GlobalVars.KeyPressDuration = Properties.Settings.Default.KeyPressDuration;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// OnTouchDown handler.
        /// </summary>
        private void controlButton_TouchDown(object sender, TouchEventArgs e)
        {
            ControlButtonDown(sender);
        }

        /// <summary>
        /// OnTouchUp handler.
        /// </summary>
        private void controlButton_TouchUp(object sender, TouchEventArgs e)
        {
            // ignore TouchUp in TapOnly mode
            if (GlobalVars.IsTapOnly)
                return;
            e.Handled = true;
            ControlButtonUp(sender);
        }

        /// <summary>
        /// OnTouchLeave handler as OnTouchUp does not fire if user drags touch outside of control.
        /// </summary>
        private void controlButton_TouchLeave(object sender, TouchEventArgs e)
        {
            // ignore TouchLeave in TapOnly mode
            if (GlobalVars.IsTapOnly)
                return;
            ControlButtonUp(sender);
        }

        /// <summary>
        /// Shows overlay menu with normal click.
        /// </summary>
        private void ibMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isMenuWindow)
                return;
            isMenuWindow = true;
            ShowMenu();
        }

        /// <summary>
        /// Shows overlay menu with touch.
        /// </summary>
        private void ibMenu_TouchDown(object sender, TouchEventArgs e)
        {
            if (isMenuWindow)
                return;
            isMenuWindow = true;
            ShowMenu();
        }
    
        /// <summary>
        /// Control down handler.
        /// </summary>
        /// <param name="sender">The caller UI element.</param>
        private void ControlButtonDown(object sender)
        {
            // get calling button
            var button = (sender as System.Windows.Controls.Image);
            if (button == null)
                return;
            // select key to send accordingly, remove 'ib' from sender name
            IntPtr key = GetKeyFromControl(button.Name.Substring(2));
            if (key == IntPtr.Zero)
                return;

            // actually send keydown
            SendMessage(citraMainControlHwnd, WM_KEYDOWN, key, IntPtr.Zero);

            // if TapMode is tap only then send KeyUp directly
            if (GlobalVars.IsTapOnly)
            {
                SendButtonUp(key);
            }
            else
            {
                // add pressed key to list
                if (!pressedControls.Contains(key))
                    pressedControls.Add(key);
            }
        }

        /// <summary>
        /// Control leave handler which supports asynchronous waiting.
        /// </summary>
        /// <param name="sender">The caller UI element.</param>
        private void ControlButtonUp(object sender)
        {
            // get calling button
            var button = (sender as System.Windows.Controls.Image);
            if (button == null)
                return;
            // select key to send accordingly, remove 'ib' from sender name
            IntPtr key = GetKeyFromControl(button.Name.Substring(2));
            if (key == IntPtr.Zero)
                return;

            // remove key from list
            if (!pressedControls.Contains(key))
                return;
            pressedControls.Remove(key);

            SendButtonUp(key);
        }

        /// <summary>
        /// Sends a KeyButtonUp after a short delay.
        /// </summary>
        /// <param name="key">The IntPtr of the key.</param>
        private async void SendButtonUp(IntPtr key)
        {
            // wait 50ms asynchronously, then send keyup of selected button
            await Task.Delay(GlobalVars.KeyPressDuration);
            SendMessage(citraMainControlHwnd, WM_KEYUP, key, IntPtr.Zero);
        }

        /// <summary>
        /// Returns a key as IntPtr for SendMessage from a controlName.
        /// </summary>
        /// <param name="controlName">The name of the Key.</param>
        /// <returns></returns>
        private IntPtr GetKeyFromControl(string controlName)
        {
            IntPtr key = IntPtr.Zero;
            // check if MainControlHandle is valid
            if (citraMainControlHwnd == IntPtr.Zero)
            {
                citraMainControlHwnd = FindWindowEx(citraHwnd, IntPtr.Zero, "Qt5QWindowIcon", "centralwidgetWindow");
                //citraMainControlHwnd = FindChildWindow(citraHwnd, IntPtr.Zero, "Qt5QWindowIcon", "centralwidgetWindow");
                //citraMainControlHwnd = FindChildWindow(citraHwnd, IntPtr.Zero, "Qt5QWindowIcon", "GRenderWindowClassWindow");
                if (citraMainControlHwnd == IntPtr.Zero)
                {
                    citraMainControlHwnd = FindChildWindow(citraHwnd, IntPtr.Zero, "Qt5QWindowOwnDCIcon", null);
                    //citraMainControlHwnd = FindChildWindow(citraHwnd, IntPtr.Zero, "Qt5QWindowOwnDCIcon", "Citra");
                    if (citraMainControlHwnd == IntPtr.Zero)
                    {
                        MessageBox.Show(this,"ERROR: Could not get mainControlHandle!\nPlease try to restart Citra and this application.", "CitraTouchControl");
                        return key;
                    }
                }
            }

            switch (controlName)
            {
                case "Up":
                    key = new IntPtr(GlobalVars.UP_KEY);
                    break;
                case "Down":
                    key = new IntPtr(GlobalVars.DOWN_KEY);
                    break;
                case "Left":
                    key = new IntPtr(GlobalVars.LEFT_KEY);
                    break;
                case "Right":
                    key = new IntPtr(GlobalVars.RIGHT_KEY);
                    break;
                case "X":
                    key = new IntPtr(GlobalVars.X_KEY);
                    break;
                case "Y":
                    key = new IntPtr(GlobalVars.Y_KEY);
                    break;
                case "A":
                    key = new IntPtr(GlobalVars.A_KEY);
                    break;
                case "B":
                    key = new IntPtr(GlobalVars.B_KEY);
                    break;
                case "R":
                    key = new IntPtr(GlobalVars.R_KEY);
                    break;
                case "L":
                    key = new IntPtr(GlobalVars.L_KEY);
                    break;
                case "Start":
                    key = new IntPtr(GlobalVars.START_KEY);
                    break;
                case "Select":
                    key = new IntPtr(GlobalVars.SELECT_KEY);
                    break;
                default:
                    return key;
            }

            return key;
        }

        /// <summary>
        /// Shows MenuWindow and pass this window as parameter.
        /// </summary>
        private void ShowMenu()
        {
            MenuWindow mW = new MenuWindow(this);
            mW.Owner = this;
            mW.Show();
        }

        /// <summary>
        /// Resets citra handle, gets citra window size and position and place overlay accordingly.
        /// </summary>
        internal void ReloadOverlay()
        {
            citraMainControlHwnd = IntPtr.Zero;
            RECT rect = new RECT();
            if (GetWindowActualRect(citraHwnd, out rect))
            {
                this.Width = rect.Right - rect.Left;
                this.Height = rect.Bottom - rect.Top;
                this.Left = rect.Left;
                this.Top = rect.Top;
            }
        }

        /// <summary>
        /// Toggles touch functionality.
        /// </summary>
        /// <param name="enable">A boolean to determine if touch should be enabled or not.</param>
        internal void ToggleTouch(bool enable)
        {
            // fully transparent background makes window hittest invisible so no mouse clicks will get processed outside of controls
            if (enable)
                this.Background = Brushes.Transparent;
            else
                this.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#01FFFFFF");
        }

        /// <summary>
        /// Toggles overlay controls except menu button.
        /// </summary>
        /// <param name="hide">A boolean to determine if controls should be hidden or not.</param>
        internal void ToggleControls(bool hide)
        {
            gMainGrid.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Retrieves the full path to the executable of a process.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <returns></returns>
        internal static string GetProcessPath(Process process)
        {
            string MethodResult = "";
            string Query = "SELECT ExecutablePath FROM Win32_Process WHERE ProcessId = " + process.Id;

            using (ManagementObjectSearcher mos = new ManagementObjectSearcher(Query))
            {
                using (ManagementObjectCollection moc = mos.Get())
                {
                    string ExecutablePath = (from mo in moc.Cast<ManagementObject>() select mo["ExecutablePath"]).First().ToString();
                    MethodResult = ExecutablePath;
                }
            }
            return MethodResult;
        }

        /// <summary>
        /// Uses FindWindowEx() to recursively search for a child window with the given class and/or title,
        /// starting after a specified child window.
        /// If lpszClass is null, it will match any class name. It's not case-sensitive.
        /// If lpszTitle is null, it will match any window title.
        /// </summary>
        public static IntPtr FindChildWindow(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszTitle)
        {
            // Try to find a match.
            IntPtr hwnd = FindWindowEx(hwndParent, IntPtr.Zero, lpszClass, lpszTitle);
            if (hwnd == IntPtr.Zero)
            {
                // Search inside the children.
                IntPtr hwndChild = FindWindowEx(hwndParent, IntPtr.Zero, null, null);
                while (hwndChild != IntPtr.Zero && hwnd == IntPtr.Zero)
                {
                    hwnd = FindChildWindow(hwndChild, IntPtr.Zero, lpszClass, lpszTitle);
                    if (hwnd == IntPtr.Zero)
                    {
                        // If we didn't find it yet, check the next child.
                        hwndChild = FindWindowEx(hwndParent, hwndChild, null, null);
                    }
                }
            }
            return hwnd;
        }

        #region IMPORTS

        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport(@"dwmapi.dll")]
        private static extern Int32 DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        /// <summary>
        /// Gets the actual position including Aero borders on > Windows 7.
        /// </summary>
        /// <param name="handle">The handle to the window.</param>
        /// <param name="rect">Output parameter for RECT struct.</param>
        /// <returns>True if call was successfull, false otherwise.</returns>
        private static bool GetWindowActualRect(IntPtr handle, out RECT rect)
        {
            int result = DwmGetWindowAttribute(handle, DWMWA_EXTENDED_FRAME_BOUNDS, out rect, Marshal.SizeOf(typeof(RECT)));
            return result >= 0;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #endregion
    }
}
