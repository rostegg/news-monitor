using System;
using System.Collections.Generic;
using System.Timers;
namespace MonitorService.Configuration
{
    public static class TimersManager
    {
        private static Timer PluginsMonitoringTimer = null;

        /// <summary>
        /// Method create a timerы, which will check for updates for a specified interval
        /// </summary>
        public static void CreatePluginsTimer(IEnumerable<IGrabber.IGrabber> plugins)
        {
            foreach (IGrabber.IGrabber module in plugins)
            {
                Timer aTimer = new Timer();
                aTimer.Interval = 60000 * module.Interval;
                aTimer.Elapsed += (sender, e) => Service.TimeUp(sender, e, module);
                aTimer.Start();
            }
        }

        /// <summary>
        /// Method create timer, which monitors the appearance of new plug-ins
        /// </summary>
        public static void CreatePluginsMonitoringTimer()
        {
            if (PluginsMonitoringTimer == null)
            {
                PluginsMonitoringTimer = new Timer();
                PluginsMonitoringTimer.Interval = 60000 * 2;
                PluginsMonitoringTimer.Elapsed += (sender, e) =>
                {
                    List<IGrabber.IGrabber> result = Setup.AddPlugins();
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
