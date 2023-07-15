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
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Text;

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
        
        private Process process;

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
        }
        private async Task GetSeasons()
        {
            using (WebClient client = new WebClient())
            {
                json = JObject.Parse(await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/AKrisz2/r6cucc/main/seasonteszt.json"));
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

                        if (File.Exists(App.appData + "\\images\\" + items[i].ElementAt(j).First["nameShort"] + ".jpg") && CalculateMD5(App.appData + "\\images\\" + items[i].ElementAt(j).First["nameShort"].ToString() + ".jpg") == items[i].ElementAt(j).First["md5"].ToString())
                        {
                            image.UriSource = new Uri(App.appData + "\\images\\" + items[i].ElementAt(j).First["nameShort"] + ".jpg", UriKind.Absolute);
                        }
                        else if (!File.Exists(App.appData + "\\images\\" + items[i].ElementAt(j).First["nameShort"] + ".jpg") || CalculateMD5(App.appData + "\\images\\" + items[i].ElementAt(j).First["nameShort"].ToString() + ".jpg") != items[i].ElementAt(j).First["md5"].ToString())
                        {
                            using (var client = new WebClient())
                            {
                                client.DownloadFile(new Uri(items[i].ElementAt(j).First["link"].ToString()), items[i].ElementAt(j).First["nameShort"].ToString() + ".jpg");
                                File.Copy(items[i].ElementAt(j).First["nameShort"].ToString() + ".jpg", App.appData + "\\images\\" + items[i].ElementAt(j).First["nameShort"].ToString() + ".jpg", true);
                                File.Delete(items[i].ElementAt(j).First["nameShort"].ToString() + ".jpg");
                                image.UriSource = new Uri(App.appData + "\\images\\" + items[i].ElementAt(j).First["nameShort"] + ".jpg", UriKind.Absolute);
                            }
                        }

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

            DownloadStuff();

            loadingStack.Visibility = Visibility.Collapsed;
            yearViewer.Visibility = Visibility.Visible;
            MainWindow._settingsButton.Visibility = Visibility.Visible;
        }
        public static void SeasonButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int seasonNumber = int.Parse(button.Name);

            BitmapImage image = new BitmapImage();
            image.UriSource = new Uri(App.appData + "\\images\\" + seasons[seasonNumber].First["nameShort"] + ".jpg", UriKind.Absolute);
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
        }
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory("depotdownloader.zip", App.appFolder + "\\DepotDownloader\\");
            File.Delete("depotdownloader.zip");
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonView.Visibility = Visibility.Collapsed;
            ButtonsBar.Visibility = Visibility.Collapsed;
            PageScrollview.Visibility = Visibility.Visible;

            SeasonImage.Source = null;
            SeasonDescription.Text = null;
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GetSeasons();
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
            else if(App.password == null)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Please enter Steam password!";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new PasswordPage();

                var result = await dialog.ShowAsync();
            }
            else
            {
                File.Delete("downloader.bat"); 

                ContentDialog dialog = new ContentDialog();
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Warning!";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new DownloadWarning();

                var result = await dialog.ShowAsync();

                if (_4kCheckbox.IsChecked == true)
                {
                    File.AppendAllText("downloader.bat",
                    "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377239 -manifest " + manifest4K + " -username " + App.settings["name"].ToString() + " -password " + App.password + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n");
                }
                File.AppendAllText("downloader.bat",
                    "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377237 -manifest " + manifestWW + " -username " + App.settings["name"].ToString() + " -password " + App.password + " -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                    "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 359551 -manifest " + manifestContent + " -username " + App.settings["name"].ToString() + " -password " + App.password + " -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                    "@echo Download Complete!");

                string batchFilePath = "downloader.bat";
                string workingDirectory = App.appFolder;

                SeasonDescription.Visibility = Visibility.Collapsed;
                DownloadScroller.Visibility = Visibility.Visible;
                _4kCheckbox.IsEnabled = false;
                BackButton.IsEnabled = false;
                DownloadButton.IsEnabled = false;

                SynchronizationContext synchronizationContext = SynchronizationContext.Current;
                process = new Process();
                string output = await RunCommand(batchFilePath, workingDirectory, synchronizationContext);

                UpdateOutputInUI(synchronizationContext, output);

                // Scroll to the bottom
                DownloadText.UpdateLayout();
                DownloadScroller.ScrollToVerticalOffset(DownloadScroller.ExtentHeight);
            }
        }

        private void UpdateOutputInUI(SynchronizationContext synchronizationContext, string output)
        {
            synchronizationContext.Post(_ =>
            {
                DownloadText.Text += output + Environment.NewLine; // Append the output with a new line

                // Scroll to the bottom
                DownloadText.UpdateLayout();
                DownloadScroller.ScrollToVerticalOffset(DownloadScroller.ExtentHeight);
            }, null);
        }

        public async Task<string> RunCommand(string batchFilePath, string workingDirectory, SynchronizationContext synchronizationContext)
        {
            StringBuilder outputBuilder = new StringBuilder();

            using (Process process = new Process())
            {
                process.StartInfo.FileName = batchFilePath;
                process.StartInfo.WorkingDirectory = workingDirectory;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        outputBuilder.AppendLine(e.Data);
                        UpdateOutputInUI(synchronizationContext, e.Data); // Update the output in the UI
                    }
                };

                process.Start();
                process.BeginOutputReadLine();

                await process.WaitForExitAsync();
            }

            // Get the captured output as a string
            string output = outputBuilder.ToString().Trim();

            return output;
        }
    }
}
