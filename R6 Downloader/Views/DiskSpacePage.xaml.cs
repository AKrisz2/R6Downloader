using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace R6_Downloader.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiskSpacePage : Page
    {
        public static int totalInstallSize;
        public static int otherSpace;
        public (long,long) driveSize;
        public List<string> selectedSeasons;
        public DiskSpacePage()
        {
            this.InitializeComponent();

            selectedSeasons = new List<string>();

            totalInstallSize = 0;

            foreach (var season in HomePage.installedSeasons)
            {
                //Create grid to hold season
                Grid seasonGrid = new Grid();
                seasonGrid.Margin = new Thickness(5);
                seasonGrid.CornerRadius = new CornerRadius(15);
                seasonGrid.Height = 60;

                //Create background color
                Rectangle backgroundColor = new Rectangle();
                Color color = Color.FromArgb(255, 77, 77, 77);
                backgroundColor.Fill = new SolidColorBrush(color);
                backgroundColor.Opacity = 0.3;
                seasonGrid.Children.Add(backgroundColor);

                //Create StackPanel to hold the name and size
                StackPanel nameStack = new StackPanel();
                nameStack.Margin = new Thickness(20,0,0,0);
                nameStack.VerticalAlignment = VerticalAlignment.Center;
                nameStack.Orientation = Orientation.Horizontal;

                //Add season name
                TextBlock seasonName = new TextBlock();
                seasonName.Margin = new Thickness(0, 0, 5, 0);
                seasonName.Text = season.Name.Replace("_", " ");
                nameStack.Children.Add(seasonName);

                //Add size
                int size = (int)float.Parse(season.Size.Split(" ").First());
                TextBlock seasonSize = new TextBlock();
                seasonSize.Text = size.ToString() + " GB";
                nameStack.Children.Add(seasonSize);

                seasonGrid.Children.Add(nameStack);

                //Add manage button
                CheckBox checkSeason = new CheckBox();
                checkSeason.HorizontalAlignment = HorizontalAlignment.Right;
                checkSeason.Name = season.Path;
                checkSeason.Checked += CheckSeason_Checked;
                checkSeason.Unchecked += CheckSeason_Unchecked;
                seasonGrid.Children.Add(checkSeason);

                InstalledSeasonsStack.Children.Add(seasonGrid);

                totalInstallSize += (int)float.Parse(season.Size.Split(" ").First());
            }

            if(App.settings["folder"].ToString() == "")
            {
                driveSize = GetDriveSize("C");
            }
            else
            {
                driveSize = GetDriveSize(App.settings["folder"].ToString().Substring(0, 1));
            }
            CreateProgressBar();
            otherSpaceSubText.Text = "Other " + (driveSize.Item2 - driveSize.Item1 - totalInstallSize) + " GB";
            totalSizeSubText.Text = "Downloader " + totalInstallSize + " GB";
            freeSpaceSubText.Text = "Free " + driveSize.Item1 + " GB";
        }

        private void CheckSeason_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox boxChecked)
            {
                if (selectedSeasons.Count == 0)
                    UninstallButton.IsEnabled = true;
                selectedSeasons.Add(boxChecked.Name);
            }
        }

        private void CheckSeason_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox boxChecked)
            {
                selectedSeasons.Remove(boxChecked.Name);
                if (selectedSeasons.Count == 0)
                    UninstallButton.IsEnabled = false;
            }
        }

        private static (long, long) GetDriveSize(string driveLetter)
        {
            try
            {
                DriveInfo drive = new DriveInfo(driveLetter);

                if (drive.IsReady)
                {
                    long totalSizeBytes = drive.TotalSize;
                    long totalSizeGB = totalSizeBytes / (1024 * 1024 * 1024);

                    long freeSpaceBytes = drive.AvailableFreeSpace;
                    long freeSpaceGB = freeSpaceBytes / (1024 * 1024 * 1024);

                    return (freeSpaceGB, totalSizeGB);
                }
                else
                {
                    return (0, 0);
                }
            }
            catch (Exception ex)
            {
                return (0, 0);
            }
        }
        private void CreateProgressBar()
        {
            double oneUnit = (double)1277 / driveSize.Item2;
            List<double> sizeList = new List<double>();

            //Calculate space taken up on drive
            Rectangle otherSpace = new Rectangle();
            Color color = Color.FromArgb(255, 252, 186, 3);
            otherSpace.Fill = new SolidColorBrush(color);
            otherSpace.Width = (driveSize.Item2 - driveSize.Item1) * oneUnit;

            foreach(var season in HomePage.installedSeasons)
            {
                double seasonSize = Convert.ToDouble(season.Size.Split(" ").First());
                double seasonSizeUnit = oneUnit * seasonSize;
                sizeList.Add(seasonSizeUnit);
            }
            foreach(var size in sizeList)
            {
                otherSpace.Width -= size;
            }

            CustomProgressBar.Children.Add(otherSpace);

            foreach(var size in sizeList)
            {
                Rectangle seasonSpace = new Rectangle();
                seasonSpace.Fill = new SolidColorBrush(Color.FromArgb(255, 135, 206, 250));
                seasonSpace.Width = size;

                CustomProgressBar.Children.Add(seasonSpace);
            }
        }

        private async void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.XamlRoot = this.XamlRoot;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.Title = "Warning";
            dialog.PrimaryButtonText = "Yes";
            dialog.CloseButtonText = "No";
            dialog.Content = new UninstallWarningPage();
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                foreach (var folder in selectedSeasons)
                    try
                    {
                        Directory.Delete(folder, true);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Debug.WriteLine("Unauthorized access: " + ex.Message);

                        ContentDialog dialog2 = new ContentDialog();
                        dialog2.XamlRoot = this.XamlRoot;
                        dialog2.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
                        dialog2.Title = "Error";
                        dialog2.PrimaryButtonText = "OK";
                        dialog2.Content = new NoAccessErrorPage();
                        var result2 = await dialog2.ShowAsync();
                    }

                MainWindow._homeButton.IsSelected = true;
            }
        }
    }
}
