using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace MonitorService.Configuration
{
    public static class Setup
    {
        public static readonly string DataPath = Path.Combine(@"C:\Users\Rostik\AppData\Roaming", @"WebNews\DataBases\DataStorage.sqlite");
        public static readonly string IconsPath = Path.Combine(@"C:\Users\Rostik\AppData\Roaming", @"WebNews\Images");
        public static readonly string MainPath = Path.Combine(@"C:\Users\Rostik\AppData\Roaming", @"WebNews");
        public static readonly string PluginsPath = Path.Combine(@"C:\Users\Rostik\AppData\Roaming\WebNews\Plugins");
        public static readonly string SettingsPath = Path.Combine(@"C:\Users\Rostik\AppData\Roaming", @"WebNews\settings.xml");
        public static bool SetupWorkPlace()
        {
            try
            {
                string path = MainPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(Path.Combine(path, @"DataBases"));
                    Directory.CreateDirectory(PluginsPath);
                    Directory.CreateDirectory(IconsPath);                    
                }
                if (!File.Exists(DataPath))
                    SQLiteConnection.CreateFile(DataPath);
                if (!File.Exists(SettingsPath))
                    Settings.CreateStartedSettingsDocument();
                return true;
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
                return false;
            }
        }
        
        public static void SetupDataBase(IEnumerable<IGrabber.IGrabber> list)
        {
            try
            {
                foreach (IGrabber.IGrabber module in list)
                {
                    Service.WriteLog("Started create " + module.Tag);
                    DataStorage.StorageManager.CreateTable(module.Tag);
                }
                
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
            }
        }

        public static List<IGrabber.IGrabber> AddPlugins()
        {
            List<IGrabber.IGrabber> result = new List<IGrabber.IGrabber>();
            PluginManager manager = new PluginManager(PluginsPath);
            if (manager.Plugins != null)
                Service.WriteLog("Plugins added. Count :  " + manager.Plugins.Count().ToString() );
            foreach (IGrabber.IGrabber plugin in manager.Plugins)
            {
                if (!Service.Plugins.ToList().Exists(x => x.Tag.Equals(plugin.Tag)))
                {
                    Service.WriteLog("New plugin : " + plugin.Tag);
                    result.Add(plugin);
                }
            }        
            return result;
        }
        public static string[] GetUsers()
        {
            List<string> users = Directory.GetDirectories(@"C:\Users").ToList();
            List<string> result = new List<string>();
            foreach (string user in users)
            {
                if (Directory.Exists(String.Format(@"{0}\AppData",user)))
                {
                    result.Add(user);
                }
            }
            return result.ToArray();
            
        }
    }
}
