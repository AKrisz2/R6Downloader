using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Windows.Storage;
using static PInvoke.User32;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI_3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public static Window Window => m_window;
        public static JObject settings;
        public static JObject imageHashes;
        public static string appFolder;
        public static string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\R6Downloader";

        public App()
        {
            App.Current.RequestedTheme = (ApplicationTheme)(int)1;
            ReadSettingsFile();
            appFolder = AppDomain.CurrentDomain.BaseDirectory;

            if (!Directory.Exists(appData))
            {
                Directory.CreateDirectory(appData);
            }
            if(!Directory.Exists(appData + "\\images"))
            {
                Directory.CreateDirectory(appData + "\\images");
            }

            this.InitializeComponent();
        }
        public async void ReadSettingsFile()
        {
            if (!File.Exists("config.json"))
            {
                File.WriteAllText("config.json", await new HttpClient().GetStringAsync("https://raw.githubusercontent.com/AKrisz2/r6cucc/main/config.json"));
            }
            settings = JObject.Parse(File.ReadAllText("config.json"));

            if (settings["userId"].ToString() == "")
            {
                settings["userId"] = GenerateRandomUserId();
                File.WriteAllText("config.json", App.settings.ToString());
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        static string GenerateRandomUserId()
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            // Generate 32 characters (8 + 4 + 4 + 4 + 12)
            for (int i = 0; i < 32; i++)
            {
                if (i == 8 || i == 13 || i == 18 || i == 23)
                {
                    sb.Append('-');
                }
                else
                {
                    int randomNumber = random.Next(36); // 26 letters + 10 numbers
                    char randomChar = (char)(randomNumber < 10 ? randomNumber + 48 : randomNumber + 87);
                    sb.Append(randomChar);
                }
            }

            return sb.ToString();
        }
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(m_window);

            SetWindowDetails(hwnd, 1436, 746);

            m_window.Activate();
        }

        public static Window m_window; private static void SetWindowDetails(IntPtr hwnd, int width, int height)
        {
            var dpi = GetDpiForWindow(hwnd);
            float scalingFactor = (float)dpi / 96;
            width = (int)(width * scalingFactor);
            height = (int)(height * scalingFactor);

            _ = SetWindowPos(hwnd, SpecialWindowHandles.HWND_TOP,
                                        0, 0, width, height,
                                        SetWindowPosFlags.SWP_NOMOVE);
            _ = SetWindowLong(hwnd,
                   WindowLongIndexFlags.GWL_STYLE,
                   (SetWindowLongFlags)(GetWindowLong(hwnd,
                      WindowLongIndexFlags.GWL_STYLE) &
                      ~(int)SetWindowLongFlags.WS_MINIMIZEBOX &
                      ~(int)SetWindowLongFlags.WS_MAXIMIZEBOX));
        }
    }
}
