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

namespace WinUI_3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DiskSpacePage : Page
    {
        public static int totalInstallSize;
        public (long,long) driveSize;
        public List<Color> colors;
        public DiskSpacePage()
        {
            this.InitializeComponent();
            totalInstallSize = 0;

            colors = new List<Color>();
            colors.Clear();

            foreach (var season in HomePage.installedSeasons)
            {
                colors.Add(GenerateRandomColor());

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

                //Add color to season name
                Rectangle seasonColor = new Rectangle();
                seasonColor.Height = 10;
                seasonColor.Width = 10;
                seasonColor.Margin = new Thickness(0, 0, 10, 0);
                seasonColor.RadiusX = 10;
                seasonColor.RadiusY = 10;
                seasonColor.Fill = new SolidColorBrush(colors.Last());
                nameStack.Children.Add(seasonColor);

                //Add season name
                TextBlock seasonName = new TextBlock();
                seasonName.Margin = new Thickness(0, 0, 5, 0);
                seasonName.Text = season.Name.Replace("_", " ");
                nameStack.Children.Add(seasonName);

                //Add size
                TextBlock seasonSize = new TextBlock();
                seasonSize.Text = season.Size;
                nameStack.Children.Add(seasonSize);

                seasonGrid.Children.Add(nameStack);

                //Add manage button
                Button manageButton = new Button();
                manageButton.Content = "Manage Install";
                manageButton.HorizontalAlignment = HorizontalAlignment.Right;
                manageButton.Width = 200;
                manageButton.Margin = new Thickness(0, 0, 20, 0);
                manageButton.Name = season.Path;
                manageButton.Click += ManageButton_Click;
                seasonGrid.Children.Add(manageButton);

                InstalledSeasonsStack.Children.Add(seasonGrid);

                totalInstallSize += Convert.ToInt32(season.Size.Split(" ").First());
            }

            driveSize = GetDriveSize(App.settings["folder"].ToString().Substring(0, 1));
            FreeSpaceText.Text = $"{driveSize.Item1} GB of {driveSize.Item2} GB available";
            TotalSize.Text = " " + totalInstallSize + " GB taken up by downloader";
            CreateProgressBar();
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
            int colorIndex = 0;

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
                seasonSpace.Fill = new SolidColorBrush(colors.ElementAt(colorIndex));
                seasonSpace.Width = size;

                CustomProgressBar.Children.Add(seasonSpace);
                colorIndex++;
            }
        }
        public static Color GenerateRandomColor()
        {
            Random random = new Random();

            // Generate random values for red, green, and blue components
            byte red = Convert.ToByte(random.Next(0, 256));
            byte green = Convert.ToByte(random.Next(0, 256));
            byte blue = Convert.ToByte(random.Next(0, 256));

            // Create a random color using Color.FromArgb
            return Color.FromArgb(255, red, green, blue);
        }

        private void ManageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                //Do stuff
            }
        }
    }
}
