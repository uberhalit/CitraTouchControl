using System;
using System.IO;
using System.Windows;
using System.IO.Compression;

namespace CitraTouchControl
{
    public partial class MenuWindow : Window
    {
        internal MainWindow mainWindow;
        internal string savePath;

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
            if (GlobalVars.IsTapOnly)
                bTap.Content = "Touch: Tap & Hold";
            bDuration.Content = "KeyPress: " + GlobalVars.KeyPressDuration + "ms";
            if (mainWindow.userPath == null)
            {
                bSave.IsEnabled = false;
                bLoad.IsEnabled = false;
            }
            else
            {
                savePath = Path.Combine(Path.GetDirectoryName(mainWindow.userPath), "save01.zip");
            }
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
        /// Saves the current savestate.
        /// </summary>
        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(savePath))
            {
                var result = MessageBox.Show(this, "WARNING: Do you really wish to overwrite the last savegames?",
                    "CitraTouchControl", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                    return;
                File.Delete(savePath);
            }
            try
            {
                ZipFile.CreateFromDirectory(mainWindow.userPath, savePath, CompressionLevel.Fastest, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "EXCEPTION: Error while trying to save savegames.\n" + ex.Message, "CitraTouchControl", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Loads the current savestate.
        /// </summary>
        private void bLoad_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(savePath))
                return;
            var result = MessageBox.Show(this, "WARNING: Do you really wish to overwrite the current savegames?",
                "CitraTouchControl", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
                return;
            try
            {
                // delete all currently existing files which will be extracted from zip as ZipFile.ExtractToDirectory() can not overwrite any files...
                using (ZipArchive zipArchive = ZipFile.OpenRead(savePath))
                {
                    foreach (ZipArchiveEntry entry in zipArchive.Entries)
                    {
                        var path = Path.Combine(mainWindow.userPath, entry.FullName);
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                }
                ZipFile.ExtractToDirectory(savePath, mainWindow.userPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "EXCEPTION: Error while trying to load savegames.\n" + ex.Message, "CitraTouchControl", MessageBoxButton.OK);
            }
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
        /// Toggle TapMode.
        /// </summary>
        private void bTap_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalVars.IsTapOnly)
            {
                bTap.Content = "Touch: Tap & Hold";
                Properties.Settings.Default.IsTapOnly = true;
                GlobalVars.IsTapOnly = true;
            }
            else
            {
                bTap.Content = "Touch: Tap only";
                Properties.Settings.Default.IsTapOnly = false;
                GlobalVars.IsTapOnly = false;
            }
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
            if (GlobalVars.KeyPressDuration > 10)
                GlobalVars.KeyPressDuration -= 5;
            Properties.Settings.Default.KeyPressDuration = GlobalVars.KeyPressDuration;
            bDuration.Content = "KeyPress: " + GlobalVars.KeyPressDuration + "ms";
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
