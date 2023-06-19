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

        BitmapImage BurntHorizonImg;
        BitmapImage PhantomSightImg;
        BitmapImage EmberRiseImg;
        BitmapImage ShiftingTidesImg;

        BitmapImage VoidEdgeImg;
        BitmapImage SteelWaveImg;
        BitmapImage ShadowLegacyImg;
        BitmapImage NeonDawnImg;

        BitmapImage CrimsonHeistImg;
        BitmapImage NorthStarImg;
        BitmapImage CrystalGuardImg;
        BitmapImage HighCalibreImg;

        BitmapImage DemonVeilImg;
        BitmapImage VectorGlareImg;
        BitmapImage BrutalSwarmImg;
        BitmapImage SolarRaidImg;

        BitmapImage CommandingForceImg;
        BitmapImage DreadFactorImg;

        JObject SeasonData;

        string seasonFolder;
        string manifestWW;
        string manifestContent;
        string manifest4K;

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
            if (!File.Exists(App.appData + "\\images\\burnthorizon.jpg") || !File.Exists(App.appData + "\\images\\phantomsight.jpg") || !File.Exists(App.appData + "\\images\\emberrise.jpg") || !File.Exists(App.appData + "\\images\\shiftingtides.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%204/Year4.zip"), "year4.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year4.zip", App.appData + "\\images", true);
                    File.Delete("year4.zip");
                }
            }
            if (!File.Exists(App.appData + "\\images\\voidedge.jpg") || !File.Exists(App.appData + "\\images\\steelwave.jpg") || !File.Exists(App.appData + "\\images\\shadowlegacy.jpg") || !File.Exists(App.appData + "\\images\\neondawn.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%205/Year5.zip"), "year5.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year5.zip", App.appData + "\\images", true);
                    File.Delete("year5.zip");
                }
            }
            if (!File.Exists(App.appData + "\\images\\crimsonheist.jpg") || !File.Exists(App.appData + "\\images\\northstar.jpg") || !File.Exists(App.appData + "\\images\\crystalguard.jpg") || !File.Exists(App.appData + "\\images\\highcalibre.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%206/Year6.zip"), "year6.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year6.zip", App.appData + "\\images", true);
                    File.Delete("year6.zip");
                }
            }
            if (!File.Exists(App.appData + "\\images\\demonveil.jpg") || !File.Exists(App.appData + "\\images\\vectorglare.jpg") || !File.Exists(App.appData + "\\images\\brutalswarm.jpg") || !File.Exists(App.appData + "\\images\\solarraid.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%207/Year7.zip"), "year7.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year7.zip", App.appData + "\\images", true);
                    File.Delete("year7.zip");
                }
            }
            if (!File.Exists(App.appData + "\\images\\commandingforce.jpg") || !File.Exists(App.appData + "\\images\\dreadfactor.jpg"))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(new Uri("https://github.com/AKrisz2/r6cucc/raw/main/Year%208/Year8.zip"), "year8.zip");
                    System.IO.Compression.ZipFile.ExtractToDirectory("year8.zip", App.appData + "\\images", true);
                    File.Delete("year8.zip");
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

            BurntHorizonImg = new BitmapImage(new Uri(App.appData + "\\images\\burnthorizon.jpg", UriKind.Absolute));
            PhantomSightImg = new BitmapImage(new Uri(App.appData + "\\images\\phantomsight.jpg", UriKind.Absolute));
            EmberRiseImg = new BitmapImage(new Uri(App.appData + "\\images\\emberrise.jpg", UriKind.Absolute));
            ShiftingTidesImg = new BitmapImage(new Uri(App.appData + "\\images\\shiftingtides.jpg", UriKind.Absolute));

            VoidEdgeImg = new BitmapImage(new Uri(App.appData + "\\images\\voidedge.jpg", UriKind.Absolute));
            SteelWaveImg = new BitmapImage(new Uri(App.appData + "\\images\\steelwave.jpg", UriKind.Absolute));
            ShadowLegacyImg = new BitmapImage(new Uri(App.appData + "\\images\\shadowlegacy.jpg", UriKind.Absolute));
            NeonDawnImg = new BitmapImage(new Uri(App.appData + "\\images\\neondawn.jpg", UriKind.Absolute));

            CrimsonHeistImg = new BitmapImage(new Uri(App.appData + "\\images\\crimsonheist.jpg", UriKind.Absolute));
            NorthStarImg = new BitmapImage(new Uri(App.appData + "\\images\\northstar.jpg", UriKind.Absolute));
            CrystalGuardImg = new BitmapImage(new Uri(App.appData + "\\images\\crystalguard.jpg", UriKind.Absolute));
            HighCalibreImg = new BitmapImage(new Uri(App.appData + "\\images\\highcalibre.jpg", UriKind.Absolute));

            DemonVeilImg = new BitmapImage(new Uri(App.appData + "\\images\\demonveil.jpg", UriKind.Absolute));
            VectorGlareImg = new BitmapImage(new Uri(App.appData + "\\images\\vectorglare.jpg", UriKind.Absolute));
            BrutalSwarmImg = new BitmapImage(new Uri(App.appData + "\\images\\brutalswarm.jpg", UriKind.Absolute));
            SolarRaidImg = new BitmapImage(new Uri(App.appData + "\\images\\solarraid.jpg", UriKind.Absolute));

            CommandingForceImg = new BitmapImage(new Uri(App.appData + "\\images\\commandingforce.jpg", UriKind.Absolute));
            DreadFactorImg = new BitmapImage(new Uri(App.appData + "\\images\\dreadfactor.jpg", UriKind.Absolute));


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

            BurntHorizonImage.Source = BurntHorizonImg;
            PhantomSightImage.Source = PhantomSightImg;
            EmberRiseImage.Source = EmberRiseImg;
            ShiftingTidesImage.Source = ShiftingTidesImg;

            VoidEdgeImage.Source = VoidEdgeImg;
            SteelWaveImage.Source = SteelWaveImg;
            ShadowLegacyImage.Source = ShadowLegacyImg;
            NeonDawnImage.Source = NeonDawnImg;

            CrimsonHeistImage.Source = CrimsonHeistImg;
            NorthStarImage.Source = NorthStarImg;
            CrystalGuardImage.Source = CrystalGuardImg;
            HighCalibreImage.Source = HighCalibreImg;

            DemonVeilImage.Source = DemonVeilImg;
            VectorGlareImage.Source = VectorGlareImg;
            BrutalSwarmImage.Source = BrutalSwarmImg;
            SolarRaidImage.Source = SolarRaidImg;

            CommandingForceImage.Source = CommandingForceImg;
            DreadFactorImage.Source = DreadFactorImg;

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
        private void BlackIceButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = BlackIceImg;
            SeasonDescription.Text = SeasonData["blackice"]["description"].ToString();

            seasonFolder = "Y1S1_BlackIce";
            manifestWW = SeasonData["blackice"]["manifestWW"].ToString();
            manifestContent = SeasonData["blackice"]["manifestContent"].ToString();
            manifest4K = SeasonData["blackice"]["manifest4K"].ToString();

            HideStuff();
        }
        private void DustLineButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = DustLineImg;
            SeasonDescription.Text = SeasonData["dustline"]["description"].ToString();

            seasonFolder = "Y1S2_DustLine";
            manifestWW = SeasonData["dustline"]["manifestWW"].ToString();
            manifestContent = SeasonData["dustline"]["manifestContent"].ToString();
            manifest4K = SeasonData["dustline"]["manifest4K"].ToString();

            HideStuff();
        }
        private void SkullRainButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = SkullRainImg;
            SeasonDescription.Text = SeasonData["skullrain"]["description"].ToString();

            seasonFolder = "Y1S3_SkullRain";
            manifestWW = SeasonData["skullrain"]["manifestWW"].ToString();
            manifestContent = SeasonData["skullrain"]["manifestContent"].ToString();
            manifest4K = SeasonData["skullrain"]["manifest4K"].ToString();

            HideStuff();
        }
        private void RedCrowButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = RedCrowImg;
            SeasonDescription.Text = SeasonData["redcrow"]["description"].ToString();

            seasonFolder = "Y1S4_RedCrow";
            manifestWW = SeasonData["redcrow"]["manifestWW"].ToString();
            manifestContent = SeasonData["redcrow"]["manifestContent"].ToString();
            manifest4K = SeasonData["redcrow"]["manifest4K"].ToString();

            HideStuff();
        }
        private void VelvetShellButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = VelvetShellImg;
            SeasonDescription.Text = SeasonData["velvetshell"]["description"].ToString();

            seasonFolder = "Y2S1_VelvetShell";
            manifestWW = SeasonData["velvetshell"]["manifestWW"].ToString();
            manifestContent = SeasonData["velvetshell"]["manifestContent"].ToString();
            manifest4K = SeasonData["velvetshell"]["manifest4K"].ToString();

            HideStuff();
        }
        private void HealthButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = HealthImg;
            SeasonDescription.Text = SeasonData["health"]["description"].ToString();

            seasonFolder = "Y2S2_Health";
            manifestWW = SeasonData["health"]["manifestWW"].ToString();
            manifestContent = SeasonData["health"]["manifestContent"].ToString();
            manifest4K = SeasonData["health"]["manifest4K"].ToString();

            HideStuff();
        }
        private void BloodOrchidButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = BloodOrchidImg;
            SeasonDescription.Text = SeasonData["bloodorchid"]["description"].ToString();

            seasonFolder = "Y2S3_BloodOrchid";
            manifestWW = SeasonData["bloodorchid"]["manifestWW"].ToString();
            manifestContent = SeasonData["bloodorchid"]["manifestContent"].ToString();
            manifest4K = SeasonData["bloodorchid"]["manifest4K"].ToString();

            HideStuff();
        }
        private void WhiteNoiseButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = WhiteNoiseImg;
            SeasonDescription.Text = SeasonData["whitenoise"]["description"].ToString();

            seasonFolder = "Y2S4_WhiteNoise";
            manifestWW = SeasonData["whitenoise"]["manifestWW"].ToString();
            manifestContent = SeasonData["whitenoise"]["manifestContent"].ToString();
            manifest4K = SeasonData["whitenoise"]["manifest4K"].ToString();

            HideStuff();
        }
        private void ChimeraButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = ChimeraImg;
            SeasonDescription.Text = SeasonData["chimera"]["description"].ToString();

            seasonFolder = "Y3S1_Chimera";
            manifestWW = SeasonData["chimera"]["manifestWW"].ToString();
            manifestContent = SeasonData["chimera"]["manifestContent"].ToString();
            manifest4K = SeasonData["chimera"]["manifest4K"].ToString();

            HideStuff();
        }
        private void ParaBellumButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = ParaBellumImg;
            SeasonDescription.Text = SeasonData["parabellum"]["description"].ToString();

            seasonFolder = "Y3S2_ParaBellum";
            manifestWW = SeasonData["parabellum"]["manifestWW"].ToString();
            manifestContent = SeasonData["parabellum"]["manifestContent"].ToString();
            manifest4K = SeasonData["parabellum"]["manifest4K"].ToString();

            HideStuff();
        }
        private void GrimSkyButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = GrimSkyImg;
            SeasonDescription.Text = SeasonData["grimsky"]["description"].ToString();

            seasonFolder = "Y3S3_GrimSky";
            manifestWW = SeasonData["grimsky"]["manifestWW"].ToString();
            manifestContent = SeasonData["grimsky"]["manifestContent"].ToString();
            manifest4K = SeasonData["grimsky"]["manifest4K"].ToString();

            HideStuff();
        }
        private void WindBastionButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = WindBastionImg;
            SeasonDescription.Text = SeasonData["windbastion"]["description"].ToString();

            seasonFolder = "Y3S4_WindBastion";
            manifestWW = SeasonData["windbastion"]["manifestWW"].ToString();
            manifestContent = SeasonData["windbastion"]["manifestContent"].ToString();
            manifest4K = SeasonData["windbastion"]["manifest4K"].ToString();

            HideStuff();
        }
        private void BurntHorizonButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = BurntHorizonImg;
            SeasonDescription.Text = SeasonData["burnthorizon"]["description"].ToString();

            seasonFolder = "Y4S1_BurntHorizon";
            manifestWW = SeasonData["burnthorizon"]["manifestWW"].ToString();
            manifestContent = SeasonData["burnthorizon"]["manifestContent"].ToString();
            manifest4K = SeasonData["burnthorizon"]["manifest4K"].ToString();

            HideStuff();
        }
        private void PhantomSightButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = PhantomSightImg;
            SeasonDescription.Text = SeasonData["phantomsight"]["description"].ToString();

            seasonFolder = "Y4S2_PhantomSight";
            manifestWW = SeasonData["phantomsight"]["manifestWW"].ToString();
            manifestContent = SeasonData["phantomsight"]["manifestContent"].ToString();
            manifest4K = SeasonData["phantomsight"]["manifest4K"].ToString();

            HideStuff();
        }
        private void EmberRiseButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = EmberRiseImg;
            SeasonDescription.Text = SeasonData["emberrise"]["description"].ToString();

            seasonFolder = "Y4S3_EmberRise";
            manifestWW = SeasonData["emberrise"]["manifestWW"].ToString();
            manifestContent = SeasonData["emberrise"]["manifestContent"].ToString();
            manifest4K = SeasonData["emberrise"]["manifest4K"].ToString();

            HideStuff();
        }
        private void ShiftingTidesButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = ShiftingTidesImg;
            SeasonDescription.Text = SeasonData["shiftingtides"]["description"].ToString();

            seasonFolder = "Y4S4_ShiftingTides";
            manifestWW = SeasonData["shiftingtides"]["manifestWW"].ToString();
            manifestContent = SeasonData["shiftingtides"]["manifestContent"].ToString();
            manifest4K = SeasonData["shiftingtides"]["manifest4K"].ToString();

            HideStuff();
        }
        private void VoidEdgeButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = VoidEdgeImg;
            SeasonDescription.Text = SeasonData["voidedge"]["description"].ToString();

            seasonFolder = "Y5S1_VoidEdge";
            manifestWW = SeasonData["voidedge"]["manifestWW"].ToString();
            manifestContent = SeasonData["voidedge"]["manifestContent"].ToString();
            manifest4K = SeasonData["voidedge"]["manifest4K"].ToString();

            HideStuff();
        }
        private void SteelWaveButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = SteelWaveImg;
            SeasonDescription.Text = SeasonData["steelwave"]["description"].ToString();

            seasonFolder = "Y5S2_SteelWave";
            manifestWW = SeasonData["steelwave"]["manifestWW"].ToString();
            manifestContent = SeasonData["steelwave"]["manifestContent"].ToString();
            manifest4K = SeasonData["steelwave"]["manifest4K"].ToString();

            HideStuff();
        }
        private void ShadowLegacyButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = ShadowLegacyImg;
            SeasonDescription.Text = SeasonData["shadowlegacy"]["description"].ToString();

            seasonFolder = "Y5S3_ShadowLegacy";
            manifestWW = SeasonData["shadowlegacy"]["manifestWW"].ToString();
            manifestContent = SeasonData["shadowlegacy"]["manifestContent"].ToString();
            manifest4K = SeasonData["shadowlegacy"]["manifest4K"].ToString();

            HideStuff();
        }
        private void NeonDawnButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = NeonDawnImg;
            SeasonDescription.Text = SeasonData["neondawn"]["description"].ToString();

            seasonFolder = "Y5S4_NeonDawn";
            manifestWW = SeasonData["neondawn"]["manifestWW"].ToString();
            manifestContent = SeasonData["neondawn"]["manifestContent"].ToString();
            manifest4K = SeasonData["neondawn"]["manifest4K"].ToString();

            HideStuff();
        }
        private void CrimsonHeistButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = CrimsonHeistImg;
            SeasonDescription.Text = SeasonData["crimsonheist"]["description"].ToString();

            seasonFolder = "Y6S1_CrimsonHeist";
            manifestWW = SeasonData["crimsonheist"]["manifestWW"].ToString();
            manifestContent = SeasonData["crimsonheist"]["manifestContent"].ToString();
            manifest4K = SeasonData["crimsonheist"]["manifest4K"].ToString();

            HideStuff();
        }
        private void NorthStarButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = NorthStarImg;
            SeasonDescription.Text = SeasonData["northstar"]["description"].ToString();

            seasonFolder = "Y6S2_NorthStar";
            manifestWW = SeasonData["northstar"]["manifestWW"].ToString();
            manifestContent = SeasonData["northstar"]["manifestContent"].ToString();
            manifest4K = SeasonData["northstar"]["manifest4K"].ToString();

            HideStuff();
        }
        private void CrystalGuardButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = CrystalGuardImg;
            SeasonDescription.Text = SeasonData["crystalguard"]["description"].ToString();

            seasonFolder = "Y6S3_CrystalGuard";
            manifestWW = SeasonData["crystalguard"]["manifestWW"].ToString();
            manifestContent = SeasonData["crystalguard"]["manifestContent"].ToString();
            manifest4K = SeasonData["crystalguard"]["manifest4K"].ToString();

            HideStuff();
        }
        private void HighCalibreButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = HighCalibreImg;
            SeasonDescription.Text = SeasonData["highcalibre"]["description"].ToString();

            seasonFolder = "Y6S4_HighCalibre";
            manifestWW = SeasonData["highcalibre"]["manifestWW"].ToString();
            manifestContent = SeasonData["highcalibre"]["manifestContent"].ToString();
            manifest4K = SeasonData["highcalibre"]["manifest4K"].ToString();

            HideStuff();
        }
        private void DemonVeilButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = DemonVeilImg;
            SeasonDescription.Text = SeasonData["demonveil"]["description"].ToString();

            seasonFolder = "Y7S1_DemonVeil";
            manifestWW = SeasonData["demonveil"]["manifestWW"].ToString();
            manifestContent = SeasonData["demonveil"]["manifestContent"].ToString();
            manifest4K = SeasonData["demonveil"]["manifest4K"].ToString();

            HideStuff();
        }
        private void VectorGlareButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = VectorGlareImg;
            SeasonDescription.Text = SeasonData["vectorglare"]["description"].ToString();

            seasonFolder = "Y7S2_VectorGlare";
            manifestWW = SeasonData["vectorglare"]["manifestWW"].ToString();
            manifestContent = SeasonData["vectorglare"]["manifestContent"].ToString();
            manifest4K = SeasonData["vectorglare"]["manifest4K"].ToString();

            HideStuff();
        }
        private void BrutalSwarmButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = BrutalSwarmImg;
            SeasonDescription.Text = SeasonData["brutalswarm"]["description"].ToString();

            seasonFolder = "Y7S3_BrutalSwarm";
            manifestWW = SeasonData["brutalswarm"]["manifestWW"].ToString();
            manifestContent = SeasonData["brutalswarm"]["manifestContent"].ToString();
            manifest4K = SeasonData["brutalswarm"]["manifest4K"].ToString();

            HideStuff();
        }
        private void SolarRaidButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = SolarRaidImg;
            SeasonDescription.Text = SeasonData["solarraid"]["description"].ToString();

            seasonFolder = "Y7S4_SolarRaid";
            manifestWW = SeasonData["solarraid"]["manifestWW"].ToString();
            manifestContent = SeasonData["solarraid"]["manifestContent"].ToString();
            manifest4K = SeasonData["solarraid"]["manifest4K"].ToString();

            HideStuff();
        }
        private void CommandingForceButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = CommandingForceImg;
            SeasonDescription.Text = SeasonData["commandingforce"]["description"].ToString();

            seasonFolder = "Y8S1_CommandingForce";
            manifestWW = SeasonData["commandingforce"]["manifestWW"].ToString();
            manifestContent = SeasonData["commandingforce"]["manifestContent"].ToString();
            manifest4K = SeasonData["commandingforce"]["manifest4K"].ToString();

            HideStuff();
        }
        private void DreadFactorButton_Click(object sender, RoutedEventArgs e)
        {
            SeasonImage.Source = DreadFactorImg;
            SeasonDescription.Text = SeasonData["dreadfactor"]["description"].ToString();

            seasonFolder = "Y8S2_DreadFactor";
            manifestWW = SeasonData["dreadfactor"]["manifestWW"].ToString();
            manifestContent = SeasonData["dreadfactor"]["manifestContent"].ToString();
            manifest4K = SeasonData["dreadfactor"]["manifest4K"].ToString();

            HideStuff();
        }
    }
}
