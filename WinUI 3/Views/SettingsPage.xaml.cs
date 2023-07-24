// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.UI.Xaml.Controls;
using System.IO;
using Windows.Storage.AccessCache;
using Windows.Storage;
using Windows.Storage.Pickers;
using System;
using Microsoft.UI.Xaml;
using System.Globalization;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI_3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

            loadSettings();
        }

        private void loadSettings()
        {
            if (App.settings["folder"].ToString() != "")
            {
                downloadFolder.Text = App.settings["folder"].ToString();
            }
            maxDownloads.Value = float.Parse(App.settings["maxDownloads"].ToString(), CultureInfo.InvariantCulture.NumberFormat); ;
            rusToggle.IsOn = (bool)App.settings["rus"];
        }

        private async void changeDownloadFolder_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

            // Create a folder picker
            FolderPicker openPicker = new Windows.Storage.Pickers.FolderPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.Window);

            // Initialize the folder picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your folder picker
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a folder
            StorageFolder folder = await openPicker.PickSingleFolderAsync();
            if(folder != null)
            {
                downloadFolder.Text = folder.Path + "\\";
                App.settings["folder"] = folder.Path + "\\";
                File.WriteAllText("config.json", App.settings.ToString());
            }
        }

        private async void connectSteam_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Set Username and Password";
            dialog.PrimaryButtonText = "Save";
            dialog.CloseButtonText = "Cancel";
            dialog.Content = new ConnectSteamPage();
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                App.settings["name"] = ConnectSteamPage._usernameTB.Text;
                App.settings["password"] = ConnectSteamPage.passwordPB.Password;

                File.WriteAllText("config.json", App.settings.ToString());
            }
        }

        private void changeInGameName_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.settings["ingame"] = changeInGameName.Text;
            File.WriteAllText("config.json", App.settings.ToString());
        }

        private void rusToggle_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            App.settings["rus"] = rusToggle.IsOn;
            File.WriteAllText("config.json", App.settings.ToString());
        }

        private void maxDownloads_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            App.settings["maxDownloads"] = maxDownloads.Value;
            File.WriteAllText("config.json", App.settings.ToString());
        }
    }

}
