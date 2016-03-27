using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorService.HelpFunctions
{
    #region Enumerable Extension
    public static class Extensions
    {
        /// <summary>
        /// A method which seeks to register the path to the browser
        /// </summary>
        public static string GetCleanPath(this Browsers browser, string path)
        {
            try
            {
                RegistryKey Key = Registry.LocalMachine;
                switch (browser)
                {
                    case Browsers.FireFox:
                        Key = Key.OpenSubKey(@"SOFTWARE\Classes\Applications\firefox.exe\shell\open\command");
                        string temp = Key.GetValue(null).ToString();
                        // String contains unnecessary characters, so we remove them
                        temp = temp.Substring(temp.IndexOf('"') + 1, temp.IndexOf(".exe") + 3);
                        return temp;
                    case Browsers.IE:
                        Key = Key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\IEXPLORE.EXE");
                        return Key.GetValue(null).ToString();
                    default:
                        return String.Empty;
                }
            }
            catch
            {
                return String.Empty;
            }
        }
    }
    #endregion

    public static class BrowserUtil
    {
        // Dictionary, which contains browsers paths in registers
        private static Dictionary<Browsers, string> DefaultBrowsersRegistry = new Dictionary<Browsers, string>() { 
            {
                Browsers.FireFox,
                @"SOFTWARE\Classes\Applications\firefox.exe\shell\open\command"
            },
            {
                Browsers.IE,@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\IEXPLORE.EXE"
            }
        };

        public static Browsers CurrentBrowser = Browsers.IE;

        public static string CurrentBrowserPath = String.Empty;

        /// <summary>
        /// The method , which selects the first found their browser and make it default for watching the news
        /// </summary>
        public static void GetPathToBrowser()
        {
            // BrowserName - BrowserPath
            Tuple<string, string> config = Configuration.Settings.GetBrowserSeting();
            if (config == null)
            {
                foreach (Browsers browser in DefaultBrowsersRegistry.Keys)
                {
                    string temp = browser.GetCleanPath(DefaultBrowsersRegistry[browser]);
                    if (temp != String.Empty)
                    {
                        CurrentBrowser = browser;
                        CurrentBrowserPath = temp;
                        Configuration.Settings.AddBrowserSettings(browser, temp);
                        break;
                    }
                }
            }
            else
            {
                CurrentBrowser = (Browsers)Enum.Parse(typeof(Browsers), config.Item1);
                CurrentBrowserPath = config.Item2;
            }
        }
        
        
    }
}
