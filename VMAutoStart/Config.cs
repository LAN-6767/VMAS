using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace VMAutoStart
{
    internal class UserConfig
    {
        public double WindowsizeWidth { get; set; }
        public double WindowsizeHeight { get; set; }
        public UserConfig(Window window)
        {
            String userconfigPath = Path.GetDirectoryName(Environment.ProcessPath) + @"\userconfig.json";
            try
            {
                File.ReadAllText(userconfigPath);
            }
            catch (FileNotFoundException)
            {
                File.Create(userconfigPath).Close();
                this.WindowsizeWidth = window.Width;
                this.WindowsizeHeight = window.Height;
                File.WriteAllText(userconfigPath, JsonSerializer.Serialize(this));
            }
        }

        public UserConfig()
        {
        }
        public UserConfig(double windowsizeWidth, double windowsizeHeight)
        {
            WindowsizeWidth = windowsizeWidth;
            WindowsizeHeight = windowsizeHeight;
        }

        public void UpdataWindowSize(Window window)
        {
            String userconfigPath = Path.GetDirectoryName(Environment.ProcessPath) + @"\userconfig.json";
            String str2 = File.ReadAllText(userconfigPath);
            UserConfig userConfigTmp = JsonSerializer.Deserialize<UserConfig>(str2);
            this.WindowsizeWidth = userConfigTmp.WindowsizeWidth;
            this.WindowsizeHeight = userConfigTmp.WindowsizeHeight;
            window.Width = userConfigTmp.WindowsizeWidth;
            window.Height = userConfigTmp.WindowsizeHeight;
        }
        public void ChangeWindowSizeInfo(Window window)
        {
            this.WindowsizeWidth = window.Width;
            this.WindowsizeHeight = window.Height;
        }
        public void SaveWindowSize(Window window)
        {
            String userconfigPath = Path.GetDirectoryName(Environment.ProcessPath) + @"\userconfig.json";
            File.WriteAllText(userconfigPath, JsonSerializer.Serialize(this));
        }
    }
    internal class Config
    {
        public ConfigBase[] Vmxs { get; set; }
        public static Config operator +(Config runconfig, Config fileconfig)
        {
            if (fileconfig != null)
            {
                for (int i = 0; i < fileconfig.Vmxs.Length; i++)
                {
                    fileconfig.Vmxs[i].Isrun = false;
                }
                Config config = new Config();
                List<ConfigBase> list1 = new List<ConfigBase>(runconfig.Vmxs);
                list1.AddRange(fileconfig.Vmxs);
                list1 = list1.GroupBy(x => x.Path)
                    .Select(group => group.OrderBy(x => x.Isrun ? 0 : 1).First())
                    .ToList();
                config.Vmxs = list1.ToArray();
                return config;
            }
            else
            {
                return runconfig;
            }

        }

    }
    internal class ConfigBase
    {
        public bool Isrun { get; set; }
        public string Path { get; set; }
        public bool IsAutorun { get; set; }
        public static implicit operator ListitenC(ConfigBase cbase)
        {
            ListitenC listitenC = new ListitenC();
            listitenC.Path = cbase.Path;
            switch (cbase.Isrun)
            {
                case true:
                    listitenC.Color = "green";
                    break;
                case false:
                    listitenC.Color = "red";
                    break;
            }
            listitenC.Autorun = cbase.IsAutorun;
            return listitenC;
        }
    }
    internal class ConfigC
    {
        public static String ConfigPath;
        public static Config config;
        public static void Init()
        {
            ConfigPath = Path.GetDirectoryName(Environment.ProcessPath) + @"\VMconfig.json";
            String configjson;
            try
            {
                configjson = File.ReadAllText(ConfigPath);
                config = JsonSerializer.Deserialize<Config>(configjson);
            }
            catch (FileNotFoundException)
            {
                File.Create(ConfigPath).Close();
                File.WriteAllText(ConfigPath, "{\"Vmxs\":[]}");
            }
        }
        public static void Writefile(Config config)
        {
            config = config;
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(config));
        }
        public static Config? Readfile()
        {
            List<ConfigBase> list = JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigPath)).Vmxs.ToList<ConfigBase>();
            list = list.GroupBy(x => x.Path)
                       .Select(group => group.First())
                       .ToList();
            if (list.Count == 0) return null;
            if (config == null) config = new Config();
            config.Vmxs = list.ToArray();
            return config;
        }
    }
}
