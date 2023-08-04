using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace R6_Downloader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectSteamPage : Page
    {
        public static TextBox _usernameTB;
        public static PasswordBox passwordPB;
        public ConnectSteamPage()
        {
            this.InitializeComponent();
            _usernameTB = usernameTB;
            passwordPB = passwordBox;

            usernameTB.Text = App.settings["name"].ToString();
            passwordBox.Password = App.settings["password"].ToString();
        }

        private void revealPasswordBox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealPasswordBox.IsChecked == true)
            {
                passwordBox.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                passwordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }




    }
}
