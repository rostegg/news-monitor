using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;

namespace MonitorService.Configuration
{
    public class PluginManager
    {
        private CompositionContainer Container;

        [ImportMany(typeof(IGrabber.IGrabber))]
        public IEnumerable<IGrabber.IGrabber> Plugins
        {
            get;
            set;
        }



        public PluginManager(string path)
        {
            DirectoryCatalog directoryCatalog = new DirectoryCatalog(path);
            var catalog = new AggregateCatalog(directoryCatalog);
            Container = new CompositionContainer(catalog);
            Container.ComposeParts(this);
        }

        /// <summary>
        /// Method prepare plugins (create workspace,timers,settings)
        /// </summary>
        public static void SetupPlugins(IEnumerable<IGrabber.IGrabber> plugins)
        {
            if (plugins.Count() > 0)
            {
                Setup.SetupDataBase(plugins);
                TimersManager.CreatePluginsTimer(plugins);
                Settings.AddPluginsSettings(plugins);
            }
        }
    }
}
