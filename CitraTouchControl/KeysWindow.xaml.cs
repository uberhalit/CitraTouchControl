using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace CitraTouchControl
{
    public partial class KeysWindow : Window
    {
        public KeysWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads user configuration to textboxes.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // get keys from virtual key codes
            tbA.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.A_KEY).ToString();
            tbB.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.B_KEY).ToString();
            tbX.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.X_KEY).ToString();
            tbY.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.Y_KEY).ToString();
            tbL.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.L_KEY).ToString();
            tbR.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.R_KEY).ToString();
            tbLeft.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.LEFT_KEY).ToString();
            tbRight.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.RIGHT_KEY).ToString();
            tbUp.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.UP_KEY).ToString();
            tbDown.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.DOWN_KEY).ToString();
            tbStart.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.START_KEY).ToString();
            tbSelect.Text = KeyInterop.KeyFromVirtualKey(GlobalVars.SELECT_KEY).ToString();
        }

        /// <summary>
        /// Custom PreviewKeyDown handler which sets control keys.
        /// </summary>
        private void tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            var textBox = sender as TextBox;
            if (textBox == null)
                return;
            // get virtual key code from presses key
            short vk = (short)KeyInterop.VirtualKeyFromKey(e.Key);
            switch (textBox.Name.Substring(2))  // remove 'tb'
            {
                case "A":
                    Properties.Settings.Default.A_KEY = vk;
                    GlobalVars.A_KEY = vk;
                    break;
                case "B":
                    Properties.Settings.Default.B_KEY = vk;
                    GlobalVars.B_KEY = vk;
                    break;
                case "X":
                    Properties.Settings.Default.X_KEY = vk;
                    GlobalVars.X_KEY = vk;
                    break;
                case "Y":
                    Properties.Settings.Default.Y_KEY = vk;
                    GlobalVars.Y_KEY = vk;
                    break;
                case "Left":
                    Properties.Settings.Default.LEFT_KEY = vk;
                    GlobalVars.LEFT_KEY = vk;
                    break;
                case "Right":
                    Properties.Settings.Default.RIGHT_KEY = vk;
                    GlobalVars.RIGHT_KEY = vk;
                    break;
                case "Up":
                    Properties.Settings.Default.UP_KEY = vk;
                    GlobalVars.UP_KEY = vk;
                    break;
                case "Down":
                    Properties.Settings.Default.DOWN_KEY = vk;
                    GlobalVars.DOWN_KEY = vk;
                    break;
                case "L":
                    Properties.Settings.Default.L_KEY = vk;
                    GlobalVars.L_KEY = vk;
                    break;
                case "R":
                    Properties.Settings.Default.R_KEY = vk;
                    GlobalVars.R_KEY = vk;
                    break;
                case "Start":
                    Properties.Settings.Default.START_KEY = vk;
                    GlobalVars.START_KEY = vk;
                    break;
                case "Select":
                    Properties.Settings.Default.SELECT_KEY = vk;
                    GlobalVars.SELECT_KEY = vk;
                    break;
                default:
                    return;
            }
            textBox.Text = e.Key.ToString();
        }
    }
}
