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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI_3
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class HomePage : Page
    {
        BitmapImage BlackIceImg;
        BitmapImage DustLineImg;
        BitmapImage SkullRainImg;
        BitmapImage RedCrowImg;

        BitmapImage VelvetShellImg;
        BitmapImage HealthImg;
        BitmapImage BloodOrchidImg;
        BitmapImage WhiteNoiseImg;

        BitmapImage ChimeraImg;
        BitmapImage ParaBellumImg;
        BitmapImage GrimSkyImg;
        BitmapImage WindBastionImg;

        JObject SeasonData;

        string seasonFolder;
        string manifestWW;
        string manifestContent;

        public HomePage()
        {
            this.InitializeComponent();

            DownloadStuff();
            CheckHash();
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
        private void CheckHash()
        {
            using (WebClient client = new WebClient())
            {
                App.imageHashes = JObject.Parse(client.DownloadString("https://raw.githubusercontent.com/AKrisz2/r6cucc/main/seasons.json"));
            }

            string[] filePaths = Directory.GetFiles(App.appData + "\\images\\", "*.jpg", SearchOption.AllDirectories);
            foreach (string filePath in filePaths)
            {
                string[] name = filePath.Split("\\");
                if (App.imageHashes[name.Last().Split('.').First()]["md5"].ToString() != CalculateMD5(filePath))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(App.imageHashes[name.Last().Split('.').First()]["link"].ToString(), name.Last());
                        File.Copy(name.Last(), App.appData + "\\images\\" + name.Last(), true);
                        File.Delete(name.Last());
                    }
                }
            }
        }
        private void HideStuff()
        {
            PageScrollview.Visibility = Visibility.Collapsed;
            SeasonView.Visibility = Visibility.Visible;
            ButtonsBar.Visibility = Visibility.Visible;
        }
        private void DownloadStuff()
        {
            //Download Images
            if (!File.Exists(App.appData + "\\images\\blackice.jpg") || !File.Exists(App.appData + "\\images\\dustline.jpg") || !File.Exists(App.appData + "\\images\\skullrain.jpg") || !File.Exists(App.appData + "\\images\\redcrow.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%201/Year1.zip"), "year1.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year1.zip", App.appData + "\\images", true);
                    File.Delete("year1.zip");
                }
            }
            if (!File.Exists(App.appData + "\\images\\velvetshell.jpg") || !File.Exists(App.appData + "\\images\\health.jpg") || !File.Exists(App.appData + "\\images\\bloodorchid.jpg") || !File.Exists(App.appData + "\\images\\whitenoise.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%202/Year2.zip"), "year2.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year2.zip", App.appData + "\\images", true);
                    File.Delete("year2.zip");
                }
            }
            if (!File.Exists(App.appData + "\\images\\chimera.jpg") || !File.Exists(App.appData + "\\images\\parabellum.jpg") || !File.Exists(App.appData + "\\images\\grimsky.jpg") || !File.Exists(App.appData + "\\images\\windbastion.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%203/Year3.zip"), "year3.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year3.zip", App.appData + "\\images", true);
                    File.Delete("year3.zip");
                }
            }

            //Set Images
            BlackIceImg = new BitmapImage(new Uri(App.appData + "\\images\\blackice.jpg", UriKind.Absolute));
            DustLineImg = new BitmapImage(new Uri(App.appData + "\\images\\dustline.jpg", UriKind.Absolute));
            SkullRainImg = new BitmapImage(new Uri(App.appData + "\\images\\skullrain.jpg", UriKind.Absolute));
            RedCrowImg = new BitmapImage(new Uri(App.appData + "\\images\\redcrow.jpg", UriKind.Absolute));

            VelvetShellImg = new BitmapImage(new Uri(App.appData + "\\images\\velvetshell.jpg", UriKind.Absolute));
            HealthImg = new BitmapImage(new Uri(App.appData + "\\images\\health.jpg", UriKind.Absolute));
            BloodOrchidImg = new BitmapImage(new Uri(App.appData + "\\images\\bloodorchid.jpg", UriKind.Absolute));
            WhiteNoiseImg = new BitmapImage(new Uri(App.appData + "\\images\\whitenoise.jpg", UriKind.Absolute));

            ChimeraImg = new BitmapImage(new Uri(App.appData + "\\images\\chimera.jpg", UriKind.Absolute));
            ParaBellumImg = new BitmapImage(new Uri(App.appData + "\\images\\parabellum.jpg", UriKind.Absolute));
            GrimSkyImg = new BitmapImage(new Uri(App.appData + "\\images\\grimsky.jpg", UriKind.Absolute));
            WindBastionImg = new BitmapImage(new Uri(App.appData + "\\images\\windbastion.jpg", UriKind.Absolute));


            //Load Images
            BlackIceImage.Source = BlackIceImg;
            DustLineImage.Source = DustLineImg;
            SkullRainImage.Source = SkullRainImg;
            RedCrowImage.Source = RedCrowImg;

            VelvetShellImage.Source = VelvetShellImg;
            HealthImage.Source = HealthImg;
            BloodOrchidImage.Source = BloodOrchidImg;
            WhiteNoiseImage.Source = WhiteNoiseImg;

            ChimeraImage.Source = ChimeraImg;
            ParaBellumImage.Source = ParaBellumImg;
            GrimSkyImage.Source = GrimSkyImg;
            WindBastionImage.Source = WindBastionImg;

            //Load Season Descriptions
            using (WebClient client = new WebClient())
            {
                SeasonData = JObject.Parse(client.DownloadString("https://raw.githubusercontent.com/AKrisz2/r6cucc/main/seasons.json"));
            }

            //Download other stuff
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
        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("downloader.bat");
            File.AppendAllText("downloader.bat",
                "dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 377237 -manifest " + manifestWW + " -username " + App.settings["name"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                "dotnet DepotDownloader\\DepotDownloader.dll -app 359550 -depot 359551 -manifest " + manifestContent + " -username " + App.settings["name"].ToString() + " -remember-password -dir \"" + App.settings["folder"].ToString() + seasonFolder + "\" -validate -max-servers " + App.settings["maxDownloads"].ToString() + " -max-downloads " + App.settings["maxDownloads"].ToString() + "\r\n" +
                "pause");
            Process.Start("downloader.bat");
        }
        private void BlackIceButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = BlackIceImg;
            SeasonDescription.Text = SeasonData["blackice"]["description"].ToString();

            seasonFolder = "Y1S1_BlackIce";
            manifestWW = SeasonData["blackice"]["manifestWW"].ToString();
            manifestContent = SeasonData["blackice"]["manifestContent"].ToString();

            HideStuff();
        }
        private void DustLineButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = DustLineImg;
            SeasonDescription.Text = SeasonData["dustline"]["description"].ToString();

            seasonFolder = "Y1S2_DustLine";
            manifestWW = SeasonData["dustline"]["manifestWW"].ToString();
            manifestContent = SeasonData["dustline"]["manifestContent"].ToString();

            HideStuff();
        }
        private void SkullRainButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = SkullRainImg;
            SeasonDescription.Text = SeasonData["skullrain"]["description"].ToString();

            seasonFolder = "Y1S3_SkullRain";
            manifestWW = SeasonData["skullrain"]["manifestWW"].ToString();
            manifestContent = SeasonData["skullrain"]["manifestContent"].ToString();

            HideStuff();
        }
        private void RedCrowButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = RedCrowImg;
            SeasonDescription.Text = SeasonData["redcrow"]["description"].ToString();

            seasonFolder = "Y1S4_RedCrow";
            manifestWW = SeasonData["redcrow"]["manifestWW"].ToString();
            manifestContent = SeasonData["redcrow"]["manifestContent"].ToString();

            HideStuff();
        }
        private void VelvetShellButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = VelvetShellImg;
            SeasonDescription.Text = SeasonData["velvetshell"]["description"].ToString();

            seasonFolder = "Y2S1_VelvetShell";
            manifestWW = SeasonData["velvetshell"]["manifestWW"].ToString();
            manifestContent = SeasonData["velvetshell"]["manifestContent"].ToString();

            HideStuff();
        }
        private void HealthButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = HealthImg;
            SeasonDescription.Text = SeasonData["health"]["description"].ToString();

            seasonFolder = "Y2S2_Health";
            manifestWW = SeasonData["health"]["manifestWW"].ToString();
            manifestContent = SeasonData["health"]["manifestContent"].ToString();

            HideStuff();
        }
        private void BloodOrchidButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = BloodOrchidImg;
            SeasonDescription.Text = SeasonData["bloodorchid"]["description"].ToString();

            seasonFolder = "Y2S3_BloodOrchid";
            manifestWW = SeasonData["bloodorchid"]["manifestWW"].ToString();
            manifestContent = SeasonData["bloodorchid"]["manifestContent"].ToString();

            HideStuff();
        }
        private void WhiteNoiseButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = WhiteNoiseImg;
            SeasonDescription.Text = SeasonData["whitenoise"]["description"].ToString();

            seasonFolder = "Y2S4_WhiteNoise";
            manifestWW = SeasonData["whitenoise"]["manifestWW"].ToString();
            manifestContent = SeasonData["whitenoise"]["manifestContent"].ToString();

            HideStuff();
        }
        private void ChimeraButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = ChimeraImg;
            SeasonDescription.Text = SeasonData["chimera"]["description"].ToString();

            seasonFolder = "Y3S1_Chimera";
            manifestWW = SeasonData["chimera"]["manifestWW"].ToString();
            manifestContent = SeasonData["chimera"]["manifestContent"].ToString();

            HideStuff();
        }
        private void ParaBellumButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = ParaBellumImg;
            SeasonDescription.Text = SeasonData["parabellum"]["description"].ToString();

            seasonFolder = "Y3S2_ParaBellum";
            manifestWW = SeasonData["parabellum"]["manifestWW"].ToString();
            manifestContent = SeasonData["parabellum"]["manifestContent"].ToString();

            HideStuff();
        }
        private void GrimSkyButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = GrimSkyImg;
            SeasonDescription.Text = SeasonData["grimsky"]["description"].ToString();

            seasonFolder = "Y3S3_GrimSky";
            manifestWW = SeasonData["grimsky"]["manifestWW"].ToString();
            manifestContent = SeasonData["grimsky"]["manifestContent"].ToString();

            HideStuff();
        }
        private void WindBastionButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = WindBastionImg;
            SeasonDescription.Text = SeasonData["windbastion"]["description"].ToString();

            seasonFolder = "Y3S4_WindBastion";
            manifestWW = SeasonData["windbastion"]["manifestWW"].ToString();
            manifestContent = SeasonData["windbastion"]["manifestContent"].ToString();

            HideStuff();
        }
    }
}
