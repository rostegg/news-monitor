using System;
using System.Collections.Generic;
using System.Timers;
namespace MonitorService.Configuration
{
    public static class TimersManager
    {
        private static Timer PluginsMonitoringTimer = null;
        public static void CreatePluginsTimer(IEnumerable<IGrabber.IGrabber> plugins)
        {
            foreach (IGrabber.IGrabber module in plugins)
            {
                Service.WriteLog("Create timer for " + module.Tag);
                Timer aTimer = new Timer();
                aTimer.Interval = 60000 * module.Interval;
                aTimer.Elapsed += (sender, e) => Service.TimeUp(sender, e, module);
                aTimer.Start();
            }
        }

        public static void CreatePluginsMonitoringTimer()
        {
            if (PluginsMonitoringTimer == null)
            {
                Service.WriteLog("Created PluginsMonitoringTime");
                PluginsMonitoringTimer = new Timer();
                PluginsMonitoringTimer.Interval = 60000 * 15;
                PluginsMonitoringTimer.Elapsed += (sender, e) =>
                {
                    List<IGrabber.IGrabber> result = Setup.AddPlugins();
                    Service.WriteLog("Finded " + result + " plugins");
                    if (result.Count != 0)
                    {
                        Service.Plugins.AddRange(result);
                        PluginManager.SetupPlugins(result);
                    }
                };
                PluginsMonitoringTimer.Start();
            }
        }
    }
}
