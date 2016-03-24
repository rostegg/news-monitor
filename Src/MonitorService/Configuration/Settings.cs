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
            catch(Exception ex)
            {
                Service.WriteLog(ex.ToString());
            }
           
        }
        
        public static void AddPluginsSettings(IEnumerable<IGrabber.IGrabber> items)
        {
            try
            {
                Service.WriteLog("Write settings " +  items.Count());
                XDocument doc = XDocument.Load(Setup.SettingsPath);
                foreach (IGrabber.IGrabber item in items)
                {
                    XElement element = new XElement("plugin");
                    element.Add(new XElement("tag", item.Tag));
                    element.Add(new XElement("url", item.Url));
                    element.Add(new XElement("interval", item.Interval));
                    doc.Element("plugins").Add(element);
                    Service.WriteLog(element.ToString());
                }
                Service.WriteLog(doc.ToString());
                doc.Save(Setup.SettingsPath);
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
            }
        }
    }
}
