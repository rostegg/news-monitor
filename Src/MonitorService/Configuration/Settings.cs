using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MonitorService.Configuration
{
    public static class Settings
    {

        /// <summary>
        /// Method generate initial xml-settings
        /// </summary>
        public static void CreateStartedSettingsDocument()
        {
            try
            {
                XElement plugins = new XElement("plugins");
                XElement main = new XElement("Project");
                main.Add(new XAttribute("Version", "1.0"));
                main.Add(plugins);
                XDocument doc = new XDocument();    
                doc.Add(main);
                doc.Save(Setup.SettingsPath);
            }                
            catch
            {
            }
           
        }

        /// <summary>
        /// Method add informations about plugins to settings
        /// </summary>
        public static void AddPluginsSettings(IEnumerable<IGrabber.IGrabber> items)
        {
            try
            {
                XDocument doc = XDocument.Load(Setup.SettingsPath);
                var str = XElement.Parse(doc.Root.Element("plugins").ToString());
                foreach (IGrabber.IGrabber item in items)
                {

                    var result = str.Elements("plugin").Where(x => x.Element("name").Value.Equals(item.Tag)).ToList();
                    if (result.Count == 0)
                    {
                        XElement element = new XElement("plugin");
                        element.Add(new XElement("tag", item.Tag));
                        element.Add(new XElement("url", item.Url));
                        element.Add(new XElement("interval", item.Interval));

                        doc.Root.Element("plugins").Add(element);
                    }
                }
                doc.Save(Setup.SettingsPath);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Method add informations about browser to settings
        /// </summary>
        public static void AddBrowserSettings(HelpFunctions.Browsers browser,string path)
        {
            try
            {
                XDocument doc = XDocument.Load(Setup.SettingsPath);
                if (doc.Root.Element("browser") == null)
                {
                    XElement element = new XElement("browser");
                    element.Add(new XElement("name", browser.ToString()));
                    element.Add(new XElement("path", path));
                    doc.Root.Add(element);
                    doc.Save(Setup.SettingsPath);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Method return pair BrowserName - BrowserPath 
        /// </summary>
        public static Tuple<string,string> GetBrowserSeting()
        {
            try
            {
                XDocument doc = XDocument.Load(Setup.SettingsPath);
                if (doc.Root.Element("browser") != null)
                {
                    XElement element = doc.Root.Element("browser");
                    return new Tuple<string,string>(element.Element("name").Value, element.Element("path").Value);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
