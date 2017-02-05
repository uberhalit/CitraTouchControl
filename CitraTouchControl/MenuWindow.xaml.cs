using System;
using System.Windows;

namespace CitraTouchControl
{
    public partial class MenuWindow : Window
    {
        internal MainWindow mainWindow;

        public MenuWindow(MainWindow mW)
        {
            InitializeComponent();
            this.mainWindow = mW;
        }

        /// <summary>
        /// Loads user configuration to buttons.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalVars.IsTouchEnabled)
                bTouch.Content = "Disable Touch";
            if (GlobalVars.AreControlsHidden)
                bControls.Content = "Show Controls";
            bDuration.Content = "KeyPress: " + GlobalVars.KeyPressDuration + "ms";
        }

        /// <summary>
        /// Saves user configuration to file.
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mainWindow.isMenuWindow = false;
            // gets saved in C:\Users\*USER*\AppData\Local\CitraTouchControl\
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Resizes the overlay to match citra again.
        /// </summary>
        private void bResize_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.ResizeOverlay();
        }

        /// <summary>
        /// Toggles touch.
        /// </summary>
        private void bTouch_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVars.IsTouchEnabled)
            {
                mainWindow.ToggleTouch(true);
                bTouch.Content = "Disable Touch";
                Properties.Settings.Default.IsTouchEnabled = true;
                GlobalVars.IsTouchEnabled = true;
            }
            else
            {
                mainWindow.ToggleTouch(false);
                bTouch.Content = "Enable Touch";
                Properties.Settings.Default.IsTouchEnabled = false;
                GlobalVars.IsTouchEnabled = false;
            }
        }

        /// <summary>
        /// Toggles controls.
        /// </summary>
        private void bControls_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVars.AreControlsHidden)
            {
                mainWindow.ToggleControls(true);
                bControls.Content = "Show Controls";
                Properties.Settings.Default.AreControlsHidden = true;
                GlobalVars.AreControlsHidden = true;
            }
            else
            {
                mainWindow.ToggleControls(false);
                bControls.Content = "Hide Controls";
                Properties.Settings.Default.AreControlsHidden = false;
                GlobalVars.AreControlsHidden = false;
            }
        }

        /// <summary>
        /// Shows key binding window.
        /// </summary>
        private void bKeys_Click(object sender, RoutedEventArgs e)
        {
            KeysWindow kW = new KeysWindow();
            kW.Owner = this;
            kW.Show();
        }

        /// <summary>
        /// Increases KeyPressDuration by 5ms.
        /// </summary>
        private void bDPlus_Click(object sender, RoutedEventArgs e)
        {
            GlobalVars.KeyPressDuration += 5;
            Properties.Settings.Default.KeyPressDuration = GlobalVars.KeyPressDuration;
            bDuration.Content = "KeyPress: " + GlobalVars.KeyPressDuration + "ms";
        }

        /// <summary>
        /// Decreases KeyPressDuration by 5ms.
        /// </summary>
        private void bDMinus_Click(object sender, RoutedEventArgs e)
        {
            if (GlobalVars.KeyPressDuration > 20)
                GlobalVars.KeyPressDuration -= 5;
            Properties.Settings.Default.KeyPressDuration = GlobalVars.KeyPressDuration;
            bDuration.Content = "KeyPress: " + GlobalVars.KeyPressDuration + "ms";
        }

        /// <summary>
        /// Exits the whole application.
        /// </summary>
        private void bExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "WARNING: Do you really wish to close CitraTouchControl?", "CitraTouchControl", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                Environment.Exit(0);
        }

        /// <summary>
        /// Closes this window.
        /// </summary>
        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
