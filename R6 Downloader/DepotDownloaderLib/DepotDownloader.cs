using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SteamKit2;

namespace DepotDownloader
{
    public enum ArgType
    {
        [Description("-app")]
        AppId,
        [Description("-depot")]
        DepotId,
        [Description("-manifest")]
        ManifestId,
        [Description("-username")]
        Username,
        [Description("-password")]
        Password,
        [Description("-remember-password")]
        RememberPassword,
        [Description("-dir")]
        Directory,
        [Description("-max-servers")]
        MaxServers,
        [Description("-max-downloads")]
        MaxDownloads,
    }


    public class DownloadObj
    {
        public List<DownloaderArgument> argsList { get; set; }

        public DownloadObj(List<DownloaderArgument> _list)
        {
            this.argsList = _list;
        }

        public string[] getArgStringList()
        {
            List<string> argumentsStringList = new List<string>();
            foreach (var argument in this.argsList)
            {
                string[] argArr = argument.GetCommandArray();
                argumentsStringList.Add(argArr[0]);
                argumentsStringList.Add(argArr[1]);
            }
            return argumentsStringList.ToArray();
        }
    }

    public class DownloaderArgument
    {
        public ArgType argType { get; set; }
        public string argValue { get; set; }
        public DownloaderArgument(ArgType type, string value)
        {
            this.argType = type;
            this.argValue = value;
        }

        public string[] GetCommandArray()
        {
            return new[] { GetDescription(), this.argValue };
        }
        private string GetDescription()
        {
            var attribute =
                argType.GetType()
                        .GetTypeInfo()
                        .GetMember(argType.ToString())
                        .FirstOrDefault(member => member.MemberType == MemberTypes.Field)
                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .SingleOrDefault()
                    as DescriptionAttribute;

            return attribute?.Description ?? argType.ToString();
        }

    }


    public static class Program
    {
        /// <summary>
        /// Function gets called after a worker has finished the download list and throws the completed event
        /// </summary>
        public static Action onDownloadCompleted = null;
        /// <summary>
        /// Intercepts the Percentage Output from DepotDownloader, u can use this to for example show the current Download Progress as a ProgressBar in your GUI
        /// </summary>
        /// <param name="percentage (float)">contains the current download percentage as float</param>
        /// <param name="file (string)">contains the last written file</param>
        public static Action<float, string> onConsoleOutput = null;
        /// <summary>
        /// Overrides DepotDownloader's Default Console.ReadLine call when trying to get the 2FA code
        /// </summary>
        /// <param name="2fa code (string)"></param>
        public static Func<string> onConsoleInput = null;
        /// <summary>
        /// If calling this method from a GUI like WPF, use set the startAsWorker param to true
        /// </summary>
        /// <param name="downloaderArguments">provides the arguments for the downloads</param>
        /// <param name="startFromGui">starts the download process as a backgroundWorker (used for GUI use-cases)</param>



        public static async void RunDownload(List<DownloadObj> downloads, bool runAsWorker)
        {
            if (runAsWorker)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += async (o, args) =>
                {
                    foreach (var obj in downloads)
                    {
                        await MainAsync(obj.getArgStringList());
                    }
                };
                worker.RunWorkerCompleted += (o, args) =>
                {
                    Console.WriteLine("Download finished");
                    if (onDownloadCompleted != null)
                        onDownloadCompleted.Invoke();
                };
                worker.RunWorkerAsync();
            }
            else
            {
                foreach (var obj in downloads)
                {
                    await MainAsync(obj.getArgStringList());
                }
            }
        }

        internal static readonly char[] newLineCharacters = ['\n', '\r'];

        static async Task<int> MainAsync(string[] args)
        {
            if (args.Length == 0)
            {
                return 1;
            }

            DebugLog.Enabled = false;

            AccountSettingsStore.LoadFromFile("account.config");

            #region Common Options

            if (HasParameter(args, "-debug"))
            {
                DebugLog.Enabled = true;
                DebugLog.AddListener((category, message) =>
                {
                    Console.WriteLine("[{0}] {1}", category, message);
                });

                var httpEventListener = new HttpDiagnosticEventListener();

                DebugLog.WriteLine("DepotDownloader", "Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
                DebugLog.WriteLine("DepotDownloader", "Runtime: {0}", RuntimeInformation.FrameworkDescription);
            }

            var username = GetParameter<string>(args, "-username") ?? GetParameter<string>(args, "-user");
            var password = GetParameter<string>(args, "-password") ?? GetParameter<string>(args, "-pass");
            ContentDownloader.Config.RememberPassword = HasParameter(args, "-remember-password");
            ContentDownloader.Config.UseQrCode = HasParameter(args, "-qr");

            ContentDownloader.Config.DownloadManifestOnly = HasParameter(args, "-manifest-only");

            var cellId = GetParameter(args, "-cellid", -1);
            if (cellId == -1)
            {
                cellId = 0;
            }

            ContentDownloader.Config.CellID = cellId;

            var fileList = GetParameter<string>(args, "-filelist");

            if (fileList != null)
            {
                const string RegexPrefix = "regex:";

                try
                {
                    var fileListData = await File.ReadAllTextAsync(fileList);
                    var files = fileListData.Split(newLineCharacters, StringSplitOptions.RemoveEmptyEntries);

                    ContentDownloader.Config.UsingFileList = true;
                    ContentDownloader.Config.FilesToDownload = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    ContentDownloader.Config.FilesToDownloadRegex = [];

                    foreach (var fileEntry in files)
                    {
                        if (fileEntry.StartsWith(RegexPrefix))
                        {
                            var rgx = new Regex(fileEntry[RegexPrefix.Length..], RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            ContentDownloader.Config.FilesToDownloadRegex.Add(rgx);
                        }
                        else
                        {
                            ContentDownloader.Config.FilesToDownload.Add(fileEntry.Replace('\\', '/'));
                        }
                    }

                    Console.WriteLine("Using filelist: '{0}'.", fileList);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: Unable to load filelist: {0}", ex);
                }
            }

            ContentDownloader.Config.InstallDirectory = GetParameter<string>(args, "-dir");

            ContentDownloader.Config.VerifyAll = HasParameter(args, "-verify-all") || HasParameter(args, "-verify_all") || HasParameter(args, "-validate");
            ContentDownloader.Config.MaxServers = GetParameter(args, "-max-servers", 20);
            ContentDownloader.Config.MaxDownloads = GetParameter(args, "-max-downloads", 8);
            ContentDownloader.Config.MaxServers = Math.Max(ContentDownloader.Config.MaxServers, ContentDownloader.Config.MaxDownloads);
            ContentDownloader.Config.LoginID = HasParameter(args, "-loginid") ? GetParameter<uint>(args, "-loginid") : null;

            #endregion

            var appId = GetParameter(args, "-app", ContentDownloader.INVALID_APP_ID);
            if (appId == ContentDownloader.INVALID_APP_ID)
            {
                Console.WriteLine("Error: -app not specified!");
                return 1;
            }

            var pubFile = GetParameter(args, "-pubfile", ContentDownloader.INVALID_MANIFEST_ID);
            var ugcId = GetParameter(args, "-ugc", ContentDownloader.INVALID_MANIFEST_ID);
            if (pubFile != ContentDownloader.INVALID_MANIFEST_ID)
            {
                #region Pubfile Downloading

                if (InitializeSteam(username, password))
                {
                    try
                    {
                        await ContentDownloader.DownloadPubfileAsync(appId, pubFile).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (
                        ex is ContentDownloaderException
                        || ex is OperationCanceledException)
                    {
                        Console.WriteLine(ex.Message);
                        return 1;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Download failed to due to an unhandled exception: {0}", e.Message);
                        throw;
                    }
                    finally
                    {
                        ContentDownloader.ShutdownSteam3();
                    }
                }
                else
                {
                    Console.WriteLine("Error: InitializeSteam failed");
                    return 1;
                }

                #endregion
            }
            else if (ugcId != ContentDownloader.INVALID_MANIFEST_ID)
            {
                #region UGC Downloading

                if (InitializeSteam(username, password))
                {
                    try
                    {
                        await ContentDownloader.DownloadUGCAsync(appId, ugcId).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (
                        ex is ContentDownloaderException
                        || ex is OperationCanceledException)
                    {
                        Console.WriteLine(ex.Message);
                        return 1;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Download failed to due to an unhandled exception: {0}", e.Message);
                        throw;
                    }
                    finally
                    {
                        ContentDownloader.ShutdownSteam3();
                    }
                }
                else
                {
                    Console.WriteLine("Error: InitializeSteam failed");
                    return 1;
                }

                #endregion
            }
            else
            {
                #region App downloading

                var branch = GetParameter<string>(args, "-branch") ?? GetParameter<string>(args, "-beta") ?? ContentDownloader.DEFAULT_BRANCH;
                ContentDownloader.Config.BetaPassword = GetParameter<string>(args, "-betapassword");

                ContentDownloader.Config.DownloadAllPlatforms = HasParameter(args, "-all-platforms");
                var os = GetParameter<string>(args, "-os");

                if (ContentDownloader.Config.DownloadAllPlatforms && !string.IsNullOrEmpty(os))
                {
                    Console.WriteLine("Error: Cannot specify -os when -all-platforms is specified.");
                    return 1;
                }

                var arch = GetParameter<string>(args, "-osarch");

                ContentDownloader.Config.DownloadAllLanguages = HasParameter(args, "-all-languages");
                var language = GetParameter<string>(args, "-language");

                if (ContentDownloader.Config.DownloadAllLanguages && !string.IsNullOrEmpty(language))
                {
                    Console.WriteLine("Error: Cannot specify -language when -all-languages is specified.");
                    return 1;
                }

                var lv = HasParameter(args, "-lowviolence");

                var depotManifestIds = new List<(uint, ulong)>();
                var isUGC = false;

                var depotIdList = GetParameterList<uint>(args, "-depot");
                var manifestIdList = GetParameterList<ulong>(args, "-manifest");
                if (manifestIdList.Count > 0)
                {
                    if (depotIdList.Count != manifestIdList.Count)
                    {
                        Console.WriteLine("Error: -manifest requires one id for every -depot specified");
                        return 1;
                    }

                    var zippedDepotManifest = depotIdList.Zip(manifestIdList, (depotId, manifestId) => (depotId, manifestId));
                    depotManifestIds.AddRange(zippedDepotManifest);
                }
                else
                {
                    depotManifestIds.AddRange(depotIdList.Select(depotId => (depotId, ContentDownloader.INVALID_MANIFEST_ID)));
                }

                if (InitializeSteam(username, password))
                {
                    try
                    {
                        await ContentDownloader.DownloadAppAsync(appId, depotManifestIds, branch, os, arch, language, lv, isUGC).ConfigureAwait(false);
                    }
                    catch (Exception ex) when (
                        ex is ContentDownloaderException
                        || ex is OperationCanceledException)
                    {
                        Console.WriteLine(ex.Message);
                        return 1;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Download failed to due to an unhandled exception: {0}", e.Message);
                        throw;
                    }
                    finally
                    {
                        ContentDownloader.ShutdownSteam3();
                    }
                }
                else
                {
                    Console.WriteLine("Error: InitializeSteam failed");
                    return 1;
                }

                #endregion
            }

            return 0;
        }

        static bool InitializeSteam(string username, string password)
        {
            if (!ContentDownloader.Config.UseQrCode)
            {
                if (username != null && password == null && (!ContentDownloader.Config.RememberPassword || !AccountSettingsStore.Instance.LoginTokens.ContainsKey(username)))
                {
                    do
                    {
                        Console.Write("Enter account password for \"{0}\": ", username);
                        if (Console.IsInputRedirected)
                        {
                            password = Console.ReadLine();
                        }
                        else
                        {
                            // Avoid console echoing of password
                            password = Util.ReadPassword();
                        }

                        Console.WriteLine();
                    } while (string.Empty == password);
                }
                else if (username == null)
                {
                    Console.WriteLine("No username given. Using anonymous account with dedicated server subscription.");
                }
            }

            return ContentDownloader.InitializeSteam3(username, password);
        }

        static int IndexOfParam(string[] args, string param)
        {
            for (var x = 0; x < args.Length; ++x)
            {
                if (args[x].Equals(param, StringComparison.OrdinalIgnoreCase))
                    return x;
            }

            return -1;
        }

        static bool HasParameter(string[] args, string param)
        {
            return IndexOfParam(args, param) > -1;
        }

        static T GetParameter<T>(string[] args, string param, T defaultValue = default)
        {
            var index = IndexOfParam(args, param);

            if (index == -1 || index == (args.Length - 1))
                return defaultValue;

            var strParam = args[index + 1];

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFromString(strParam);
            }

            return default;
        }

        static List<T> GetParameterList<T>(string[] args, string param)
        {
            var list = new List<T>();
            var index = IndexOfParam(args, param);

            if (index == -1 || index == (args.Length - 1))
                return list;

            index++;

            while (index < args.Length)
            {
                var strParam = args[index];

                // If the argument starts with a "-", it's the start of the next parameter,
                // so we stop parsing this parameter's values.
                if (strParam.StartsWith("-"))
                {
                    break;
                }

                var parts = strParam.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        list.Add((T)converter.ConvertFromString(part));
                    }
                }

                index++;
            }

            return list;
        }

    }
    public class UpdateVar<T>
    {
        private T _value;

        public Action ValueChanged;

        public T Value
        {
            get => _value;

            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        protected virtual void OnValueChanged() => ValueChanged?.Invoke();
    }
}
