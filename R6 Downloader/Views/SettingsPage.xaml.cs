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
using System.Text.RegularExpressions;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace R6_Downloader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        HomePage homePage;
        public SettingsPage()
        {
            this.InitializeComponent();

            homePage = new HomePage();
            homePage.GetSeasons();

            loadSettings();
        }

        private void loadSettings()
        {
            if (App.settings["folder"].ToString() != "")
            {
                downloadFolder.Text = App.settings["folder"].ToString();
            }
            changeInGameName.Text = App.settings["ingame"].ToString();
            maxDownloads.Value = float.Parse(App.settings["maxDownloads"].ToString(), CultureInfo.InvariantCulture.NumberFormat);
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
            if (folder != null)
            {
                if (HasSpecialCharacters(folder.Path))
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.XamlRoot = this.XamlRoot;
                    dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    dialog.Title = "Error";
                    dialog.PrimaryButtonText = "OK";
                    dialog.Content = new DownloadFolderErrorPage();
                    var result = await dialog.ShowAsync();

                    changeDownloadFolder_Click(sender, e);
                }
                else
                {
                    downloadFolder.Text = folder.Path + "\\";
                    App.settings["folder"] = folder.Path + "\\";
                    File.WriteAllText("config.json", App.settings.ToString());
                }
            }
        }
        private bool HasSpecialCharacters(string input)
        {
            // Regular expression pattern to match allowed characters
            // Here, we allow only letters, numbers, spaces, underscores, and hyphens in the folder path
            string pattern = @"^[A-Za-z0-9.-_\s]+$";
            return !Regex.IsMatch(input, pattern);
        }

        private void changeInGameName_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.settings["ingame"] = changeInGameName.Text;
            File.WriteAllText("config.json", App.settings.ToString());
        }

        private void maxDownloads_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            App.settings["maxDownloads"] = maxDownloads.Value;
            File.WriteAllText("config.json", App.settings.ToString());
        }

        private void ShowStorageButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.contentFrame.Navigate(typeof(DiskSpacePage), string.Empty);
        }
    }

}
