using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShimmysAdminTools.Components
{
    public static class UpdaterCore
    {
        /// <summary>
        /// GitHub url to global config file.
        /// This provides the latest version, to check for updates.
        /// Since the file is on GitHub, there is no way for me to see people/servers accessing it, and changes to it are publicly visible.
        /// </summary>
        public const string GlobalConfigURL = "https://raw.githubusercontent.com/ShimmyMySherbet/ShimmysAdminTools/master/ShimmysAdminTools/GlobalConfig.ini";

        public static INIFile GlobalConfig;

        public static readonly Version CurrentVersion = typeof(UpdaterCore).Assembly.GetName().Version;

        public static bool HasConfig = false;
        public static void Init()
        {
            try
            {
                var req = WebRequest.CreateHttp(GlobalConfigURL);
                req.Method = "GET";
                req.Timeout = 2500; // 2.5 sec timeout as to not hold up the server if there is no internet
                using (var resp = req.GetResponse())
                    using(var network = resp.GetResponseStream())
                using (var reader = new StreamReader(network))
                {
                    var ini = reader.ReadToEnd();
                    GlobalConfig = new INIFile(ini);
                }
                HasConfig = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static bool IsOutDated => LatestVersion > CurrentVersion;

        public static bool TryGetUpdateMessage(out string message)
        {
            message = null;
            if (HasConfig && GlobalConfig.KeySet("UpdateMessage"))
            {
                message = GlobalConfig["UpdateMessage"].Replace("\\n", "\n");
                return true;
            }
            return false;
        }

        public static Version LatestVersion
        {
            get
            {
                if (HasConfig && GlobalConfig.KeySet("LatestVersion") && Version.TryParse(GlobalConfig["LatestVersion"], out var latest))
                {
                    return latest;
                }
                return CurrentVersion;
            }
        }

    }
}
