using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading;
using System.Net;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Diagnostics;
using WinUI_3.Views;
using Microsoft.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI_3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class HomePage : Page
    {
        public static StackPanel parentStack;
        public static JObject json;
        public static List<JToken> seasons = new List<JToken>();

        public static StackPanel loadingStack;
        public static ScrollViewer yearViewer;
        public static StackPanel seasonStack;

        public static Image _seasonImage;
        public static TextBlock _seasonDescription;
        public static StackPanel _seasonView;
        public static Grid _buttonsBar;

        public static int seasonNumber = 0;

        public static string seasonFolder;
        public static string manifestWW;
        public static string manifestContent;
        public static string manifest4K;

        public HomePage()
        {
            this.InitializeComponent();

            parentStack = YourParentControl;
            loadingStack = LoadingView;
            yearViewer = PageScrollview;
            seasonStack = SeasonView;
            _seasonImage = SeasonImage;
            _seasonDescription = SeasonDescription;
            _seasonView = SeasonView;
            _buttonsBar = ButtonsBar;

            GetSeasons();
        }
        public static async void GetSeasons()
        {
            using (WebClient client = new WebClient())
            {
                json = JObject.Parse(client.DownloadString("https://raw.githubusercontent.com/AKrisz2/r6cucc/main/seasonteszt.json"));
            }
            foreach (var property in json.Properties())
            {
                //Go through each year and get their name
                string objectName = property.Name;
                JArray items = (JArray)json[objectName];

                //Add year name from JSON
                var yearParent = new TextBlock();
                yearParent.FontSize = 36;
                yearParent.FontWeight = FontWeights.ExtraBold;
                yearParent.Text = objectName;
                parentStack.Children.Add(yearParent);

                //Add seasons from JSON
                StackPanel yearPanel = new StackPanel();
                yearPanel.Orientation = Orientation.Horizontal;
                yearPanel.Padding = new Thickness(0, 25, 25, 25);
                parentStack.Children.Add(yearPanel);

                //Go through each season for that year
                for (int i = 0; i < items.Count; i++)
                {
                    var item = (JObject)items[i];
                    for (int j = 0; j < items[i].Count(); j++)
                    {
                        //Creates season button thingy
                        Grid seasonGrid = new Grid();
                        seasonGrid.Margin = new Thickness(0, 0, 25, 0);
                        seasonGrid.CornerRadius = new CornerRadius(20);

                        Image seasonImage = new Image();
                        seasonImage.Width = 300;
                        seasonImage.Height = 175;
                        seasonImage.Stretch = Stretch.Fill;
                        BitmapImage image = new BitmapImage();
                        image.UriSource = new Uri(items[i].ElementAt(j).First["link"].ToString());
                        seasonImage.Source = image;
                        seasonGrid.Children.Add(seasonImage);

                        Button seasonButton = new Button();
                        seasonButton.Width = 300;
                        seasonButton.Height = 175;
                        seasonButton.Name = seasonNumber.ToString();
                        seasonButton.Click += SeasonButton_Click;
                        seasonGrid.Children.Add(seasonButton);

                        yearPanel.Children.Add(seasonGrid);

                        seasons.Add(items[i].ElementAt(j));
                        seasonNumber++;
                    }
                }
            }
            loadingStack.Visibility = Visibility.Collapsed;
            yearViewer.Visibility = Visibility.Visible;
            MainWindow._settingsButton.Visibility = Visibility.Visible;
        }
        public static void SeasonButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int seasonNumber = int.Parse(button.Name);

            BitmapImage image = new BitmapImage();
            image.UriSource = new Uri(seasons[seasonNumber].First["link"].ToString());
            _seasonImage.Source = image;
            _seasonDescription.Text = seasons[seasonNumber].First["description"].ToString();
            manifest4K = seasons[seasonNumber].First["manifest4K"].ToString();
            manifestContent = seasons[seasonNumber].First["manifestContent"].ToString();
            manifestWW = seasons[seasonNumber].First["manifestWW"].ToString();
            seasonFolder = seasons[seasonNumber].First["name"].ToString();

            yearViewer.Visibility = Visibility.Collapsed;
            _seasonView.Visibility = Visibility.Visible;
            _buttonsBar.Visibility = Visibility.Visible;
        }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        private void DownloadStuff()
        {
            if (!File.Exists("DepotDownloader\\DepotDownloader.dll"))
            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri("https://github.com/SteamRE/DepotDownloader/releases/download/DepotDownloader_2.5.0/depotdownloader-2.5.0.zip"), "depotdownloader.zip");
            }
            else
            {
                MainWindow._settingsButton.Visibility = Visibility.Visible;
                LoadingView.Visibility = Visibility.Collapsed;
                PageScrollview.Visibility = Visibility.Visible;
            }
        }
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory("depotdownloader.zip", App.appFolder + "\\DepotDownloader\\");
            File.Delete("depotdownloader.zip");

            MainWindow._settingsButton.Visibility = Visibility.Visible;
            LoadingView.Visibility = Visibility.Collapsed;
            PageScrollview.Visibility = Visibility.Visible;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonView.Visibility = Visibility.Collapsed;
            ButtonsBar.Visibility = Visibility.Collapsed;
            PageScrollview.Visibility = Visibility.Visible;

            SeasonImage.Source = null;
            SeasonDescription.Text = null;
        }
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.settings["name"].ToString() == "" || App.settings["folder"].ToString() == "")
            {
                ContentDialog dialog = new ContentDialog();
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Please check settings!";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new SettingsErrorPage();

                var result = await dialog.ShowAsync();
            }
            else
            {
                File.Delete("downloader.bat");
                if (_4kCheckbox.IsChecked == true)
                {
                    File.AppendAllText("downloader.bat",
                    "dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377239 -manifest " + manifest4K + " -username " + App.settings["name"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n");
                }
                File.AppendAllText("downloader.bat",
                    "dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377237 -manifest " + manifestWW + " -username " + App.settings["name"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                    "dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 359551 -manifest " + manifestContent + " -username " + App.settings["name"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                    "pause");
                Process.Start("downloader.bat");
            }
        }
    }
}
