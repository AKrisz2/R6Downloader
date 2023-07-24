using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Threading;
using System.Net;
using System.Security.Cryptography;
using System.Diagnostics;
using WinUI_3.Views;
using Microsoft.UI.Text;
using System.Threading.Tasks;
using System.Text;
using Windows.UI.Core;
using System.Drawing;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;
using Path = System.IO.Path;
using Windows.UI.ViewManagement;

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

        public static Button _downloadButton;
        public static Button _playButton;
        public static Button _verifyButton;
        public static Button _backButton;
        public static Button _refreshNameButton;
        public static Button _closeGameButton;

        public static int seasonNumber = 0;

        public static int selectedSeason;
        public static string seasonFolder;
        public static int crackType;
        public static string manifestWW;
        public static string manifestContent;
        public static string manifest4K;
        public static string manifestRus;

        public static Process process;

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
            _playButton = PlayButton;
            _verifyButton = VerifyButton;
            _downloadButton = DownloadButton;
            _backButton = BackButton;
            _refreshNameButton = RefreshNameButton;
            _closeGameButton = CloseGameButton;

            process = new Process();
        }
        private async Task GetSeasons()
        {
            if (IsDirectoryEmpty(App.appData + "\\images\\"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFileCompleted += ImagesDownloaded;
                    client.DownloadFile(new Uri("https://raw.githubusercontent.com/AKrisz2/r6cucc/main/images.zip"), "images.zip");
                }
            }
            selectedSeason = 0;
            seasons.Clear();
            seasonNumber = 0;
            using (WebClient client = new WebClient())
            {
                json = JObject.Parse(await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/AKrisz2/r6cucc/main/seasons.json"));
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

                        Grid imageContainer = new Grid();
                        if (File.Exists(App.settings["folder"].ToString() + items[i].ElementAt(j).First["name"] + "\\CPlay.ini") || File.Exists(App.settings["folder"].ToString() + items[i].ElementAt(j).First["name"] + "\\uplay_r2.ini"))
                        {
                            var uiSettings = new UISettings();
                            var accentColor = uiSettings.GetColorValue(UIColorType.Accent);

                            Microsoft.UI.Xaml.Shapes.Rectangle background = new Microsoft.UI.Xaml.Shapes.Rectangle();
                            SolidColorBrush brush = new SolidColorBrush(accentColor);
                            background.Fill = brush;
                            background.Height = 15;
                            background.VerticalAlignment = VerticalAlignment.Bottom;

                            TextBlock installedText = new TextBlock();
                            installedText.Text = "Installed";
                            installedText.HorizontalAlignment = HorizontalAlignment.Center;
                            installedText.VerticalAlignment = VerticalAlignment.Bottom;
                            installedText.Margin = new Thickness(0, 0, 0, 1);
                            installedText.FontSize = 10;
                            installedText.FontWeight = FontWeights.Bold;

                            imageContainer.Children.Add(background);
                            imageContainer.Children.Add(installedText);

                            seasonImage.Margin = new Thickness(0, 0, 0, background.Height);
                        }
                        else
                        {
                            seasonImage.Height = 190;
                        }
                        imageContainer.Children.Add(seasonImage);

                        seasonGrid.Children.Add(imageContainer);

                        Button seasonButton = new Button();
                        seasonButton.Width = 300;
                        seasonButton.Height = 190;
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

            if (File.Exists("images.zip"))
            {
                File.Delete("images.zip");
            }

            loadingStack.Visibility = Visibility.Collapsed;
            yearViewer.Visibility = Visibility.Visible;
            MainWindow._settingsButton.Visibility = Visibility.Visible;
        }
        public static void SeasonButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int seasonNumber = int.Parse(button.Name);
            selectedSeason = seasonNumber;

            BitmapImage image = new BitmapImage();
            image.UriSource = new Uri(App.appData + "\\images\\" + seasons[seasonNumber].First["nameShort"] + ".jpg", UriKind.Absolute);
            _seasonImage.Source = image;
            _seasonDescription.Text = seasons[seasonNumber].First["description"].ToString();
            manifest4K = seasons[seasonNumber].First["manifest4K"].ToString();
            manifestContent = seasons[seasonNumber].First["manifestContent"].ToString();
            manifestWW = seasons[seasonNumber].First["manifestWW"].ToString();
            manifestRus = seasons[seasonNumber].First["manifestRus"].ToString();
            seasonFolder = seasons[seasonNumber].First["name"].ToString();
            crackType = Convert.ToInt16(seasons[seasonNumber].First["crackType"].ToString());

            yearViewer.Visibility = Visibility.Collapsed;
            _seasonView.Visibility = Visibility.Visible;
            _buttonsBar.Visibility = Visibility.Visible;

            if(File.Exists(App.settings["folder"].ToString() + seasonFolder + "\\CPlay.ini") || File.Exists(App.settings["folder"].ToString() + seasonFolder + "\\uplay_r2.ini"))
            {
                ShowPlayButton();
            }
            else
            {
                HidePlayButton();
            }
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
                client.DownloadFileCompleted += DepotDownloaderDownloaded;
                client.DownloadFileAsync(new Uri("https://github.com/SteamRE/DepotDownloader/releases/download/DepotDownloader_2.5.0/depotdownloader-2.5.0.zip"), "depotdownloader.zip");
            }
            if (!Directory.Exists("Cracks"))
            using (var client = new WebClient())
            {
                client.DownloadFileCompleted += CracksDownloaded;
                client.DownloadFileAsync(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Cracks.zip"), "cracks.zip");
            }
        }
        private void DepotDownloaderDownloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory("depotdownloader.zip", App.appFolder + "\\DepotDownloader\\");
            File.Delete("depotdownloader.zip");
        }
        private void ImagesDownloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory("images.zip", App.appData + "\\images\\");
            File.Delete("images.zip");
        }
        private void CracksDownloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory("cracks.zip", App.appFolder + "\\Cracks\\");
            File.Delete("cracks.zip");
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
            _verifyButton.IsEnabled = false;
            _refreshNameButton.IsEnabled = false;
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
                if (App.settings["password"].ToString() == null)
                {
                    ContentDialog dialog1 = new ContentDialog();
                    dialog1.XamlRoot = this.XamlRoot;
                    dialog1.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                    dialog1.Title = "Please enter Steam password!";
                    dialog1.PrimaryButtonText = "OK";
                    dialog1.DefaultButton = ContentDialogButton.Primary;
                    dialog1.Content = new PasswordPage();

                    var result1 = await dialog1.ShowAsync();
                    if (result1 == ContentDialogResult.Primary)
                    {
                        App.settings["password"] = PasswordPage._passwordBox.Password;
                        File.WriteAllText("config.json", App.settings.ToString());
                    }
                }
                
                File.Delete("downloader.bat");
                ContentDialog dialog = new ContentDialog();
                dialog.XamlRoot = this.XamlRoot;
                dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                dialog.Title = "Warning!";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new DownloadWarning();

                var result = await dialog.ShowAsync();

                if (!(bool)App.settings["rus"])
                {
                    File.AppendAllText("downloader.bat",
                        "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377237 -manifest " + manifestWW + " -username " + App.settings["name"].ToString() + " -password " + App.settings["password"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                        "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 359551 -manifest " + manifestContent + " -username " + App.settings["name"].ToString() + " -password " + App.settings["password"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n");
                }
                else if ((bool)App.settings["rus"])
                {
                    File.AppendAllText("downloader.bat",
                        "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377238 -manifest " + manifestRus + " -username " + App.settings["name"].ToString() + " -password " + App.settings["password"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                        "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 359551 -manifest " + manifestContent + " -username " + App.settings["name"].ToString() + " -password " + App.settings["password"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n");
                }
                if (_4kCheckbox.IsChecked == true)
                {
                    File.AppendAllText("downloader.bat",
                    "@dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377239 -manifest " + manifest4K + " -username " + App.settings["name"].ToString() + " -password " + App.settings["password"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n");
                }

                File.AppendAllText("downloader.bat", "@echo Download Complete!");

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
            _playButton.IsEnabled = false;
            _backButton.IsEnabled = false;
            MainWindow._settingsButton.IsEnabled = false;
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

                //Download Done
                CopyCrack(selectedSeason, App.settings["folder"].ToString() + seasonFolder);
                GenerateStreaminginstall(App.settings["folder"].ToString() + seasonFolder);
                MainWindow._settingsButton.IsEnabled = true;
                _4kCheckbox.IsEnabled = true;
                _verifyButton.IsEnabled = false;
                _refreshNameButton.IsEnabled = false;
                ShowPlayButton();
            }

            // Get the captured output as a string
            string output = outputBuilder.ToString().Trim();

            return output;
        }
        public void CopyCrack(int seasonNum, string folder)
        {
            if (crackType == 1) //Y1S1-Y6S2
            {
                string sourceFolder = "Cracks\\Y1SX-Y6S2";
                string destinationFolder = folder;

                string[] files = Directory.GetFiles(sourceFolder);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(destinationFolder, fileName);
                    File.Copy(file, destinationPath, true);
                }
                ReplaceLineStartsWith(destinationFolder + "\\CPlay.ini", "Username = ", App.settings["ingame"].ToString());
                ReplaceLineStartsWith(destinationFolder + "\\CPlay.ini", "UserId = ", App.settings["userId"].ToString());
                ReplaceStringInFile(destinationFolder + "\\CODEX.ini", "CHANGEGAMENAME", seasonFolder);
            }
            else if (crackType == 2) //Y6S3
            {
                string sourceFolder = "Cracks\\Y6S3";
                string destinationFolder = folder;

                string[] files = Directory.GetFiles(sourceFolder);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(destinationFolder, fileName);
                    File.Copy(file, destinationPath, true);
                }
                ReplaceLineStartsWith(destinationFolder + "\\CPlay.ini", "Username = ", App.settings["ingame"].ToString());
                ReplaceLineStartsWith(destinationFolder + "\\CPlay.ini", "UserId = ", App.settings["userId"].ToString());
                ReplaceStringInFile(destinationFolder + "\\CODEX.ini", "RainbowSixSiegeYS", seasonFolder);
            }
            else if (crackType == 3) //Y6S4+
            {
                string sourceFolder = "Cracks\\Y6S4-Y8SX";
                string destinationFolder = folder;

                string[] files = Directory.GetFiles(sourceFolder);

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    string destinationPath = Path.Combine(destinationFolder, fileName);
                    File.Copy(file, destinationPath, true);
                }
                ReplaceLineStartsWith(destinationFolder + "\\uplay_r2.ini", "Username = ", App.settings["ingame"].ToString());
                ReplaceLineStartsWith(destinationFolder + "\\uplay_r2.ini", "UserId = ", App.settings["userId"].ToString());
            }
        }
        public void ChangeIngameName(int seasonNum, string folder)
        {
            if (crackType == 1) //Y1S1-Y6S2
            {
                string destinationFolder = folder;

                ReplaceLineStartsWith(destinationFolder + "\\CPlay.ini", "Username = ", App.settings["ingame"].ToString());
            }
            else if (crackType == 2) //Y6S3
            {
                string destinationFolder = folder;
                ReplaceLineStartsWith(destinationFolder + "\\CPlay.ini", "Username = ", App.settings["ingame"].ToString());
            }
            else if (crackType == 3) //Y6S4+
            {
                string destinationFolder = folder;
                ReplaceLineStartsWith(destinationFolder + "\\uplay_r2.ini", "Username = ", App.settings["ingame"].ToString());
            }
        }
        void ReplaceStringInFile(string filePath, string oldText, string newText)
        {
            string fileContent = File.ReadAllText(filePath);

            string updatedContent = fileContent.Replace(oldText, newText);

            File.WriteAllText(filePath, updatedContent);
        }
        void ReplaceLineStartsWith(string filePath, string lineStart, string newText)
        {
            string[] lines = File.ReadAllLines(filePath);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(lineStart))
                {
                    lines[i] = lineStart + newText;
                    break;
                }
            }
            File.WriteAllLines(filePath, lines);
        }
        public void GenerateStreaminginstall(string gameFolder)
        {
            File.Delete(gameFolder + "\\streaminginstall.ini");
            string streaminginstallText = "[MissionToChunk]\r\n1=0,1,2,3,4,5,6,7,8,9,47\r\n2=10,11,12,13,14,15,16,17,18,19,48\r\n3=20\r\n4=21\r\n5=22,23,24,25,26,27,28,29,30,31,49\r\n6=32\r\n7=33\r\n8=34\r\n9=35\r\n10=36\r\n11=37,38,39,40,41,42,43,44,45,46,50\r\n[FileToChunk]";

            List<string> files = ListFiles(gameFolder);

            File.WriteAllLines(gameFolder + "\\streaminginstall.ini", new[] { streaminginstallText }.Concat(files.Skip(1)));
        }
        static List<string> ListFiles(string directoryPath)
        {
            string[] excludedDirectories = { ".DepotDownloader", "MatchReplay", "SAVE_GAMES", "Support", "BattlEye", "download", "Temp" };

            return Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
                .Select(filePath => Path.GetRelativePath(directoryPath, filePath))
                .Where(relativePath => !IsExcludedDirectory(relativePath, excludedDirectories))
                .Select(relativePath => relativePath.Replace("\\", "/") + "=0")
                .ToList();
        }
        static bool IsDirectoryEmpty(string path)
        {
            // Enumerate files and directories in the specified path
            foreach (var entry in Directory.EnumerateFileSystemEntries(path))
            {
                // If there's at least one entry, the directory is not empty
                return false;
            }

            // If no entries are found, the directory is empty
            return true;
        }

        static bool IsExcludedDirectory(string path, string[] excludedDirectories)
        {
            return excludedDirectories.Any(excludedDir =>
                path.StartsWith(excludedDir + Path.DirectorySeparatorChar) || path.Equals(excludedDir));
        }
        public static void ShowPlayButton()
        {
            _downloadButton.Visibility = Visibility.Collapsed;

            _playButton.IsEnabled = true;
            _playButton.Visibility = Visibility.Visible;

            _verifyButton.Visibility = Visibility.Visible;

            _backButton.Visibility = Visibility.Visible;
            _backButton.IsEnabled = true;

            _refreshNameButton.Visibility = Visibility.Visible;
        }
        public static void HidePlayButton()
        {
            _downloadButton.Visibility = Visibility.Visible;
            _playButton.Visibility = Visibility.Collapsed;
            _verifyButton.Visibility = Visibility.Collapsed;
            _backButton.Visibility = Visibility.Visible;
            _refreshNameButton.Visibility= Visibility.Collapsed;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(App.settings["folder"].ToString() + seasonFolder + "\\RainbowSixGame.exe"))
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = App.settings["folder"].ToString() + seasonFolder + "\\RainbowSixGame.exe",
                    UseShellExecute = true,
                    Arguments = "/belaunch"
                };
            }
            else if (File.Exists(App.settings["folder"].ToString() + seasonFolder + "\\RainbowSix.exe"))
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = App.settings["folder"].ToString() + seasonFolder + "\\RainbowSix.exe",
                    UseShellExecute = true,
                    Arguments = "/belaunch"
                };
            }

            process.EnableRaisingEvents = true;
            process.Exited += ProcessExited;
            process.Start();

            _playButton.Visibility = Visibility.Collapsed;
            _playButton.IsEnabled = false;

            _refreshNameButton.IsEnabled = false;
            _verifyButton.IsEnabled = false;

            _closeGameButton.Visibility = Visibility.Visible;
            _closeGameButton.IsEnabled = true;

            _backButton.IsEnabled = false;

            MainWindow._settingsButton.IsEnabled = false;
        }
        private void ProcessExited(object sender, EventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                _playButton.Visibility = Visibility.Visible;
                _playButton.IsEnabled = true;

                _refreshNameButton.IsEnabled = true;
                _verifyButton.IsEnabled = true;

                _closeGameButton.Visibility = Visibility.Collapsed;
                _closeGameButton.IsEnabled = false;

                _backButton.IsEnabled = true;

                MainWindow._settingsButton.IsEnabled = true;
            });
            process = null;
            process = new Process();
        }
        private void KillProcessByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process process in processes)
            {
                process.Kill();
            }
        }
        private void RefreshNameButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeIngameName(selectedSeason, App.settings["folder"].ToString() + seasonFolder);
        }

        private void CloseGameButton_Click(object sender, RoutedEventArgs e)
        {
            KillProcessByName("RainbowSix");
            KillProcessByName("rainbowsix");
            KillProcessByName("RainbowSixGame");
            KillProcessByName("rainbowsixgame");
        }
    }
}
