using Newtonsoft.Json;
using RustIDE.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustIDE
{
    public static class Globals
    {
        public static Config Config = new Config();
        public static string CurrentProject { get; set; } = "";
        public static bool TreeViewLoaded { get; set; } = false;

        public static JsonSerializerSettings JSS { get; set; } = new JsonSerializerSettings() {  };

        public static void Load()
        {
            if (File.Exists("settings.config"))
            {
                Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("settings.config"), JSS);
            }
        }

        public static void Save()
        {
            File.WriteAllText("settings.config",JsonConvert.SerializeObject(Config));
        }
    }
}
