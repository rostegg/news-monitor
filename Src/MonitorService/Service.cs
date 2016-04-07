using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using IGrabber;

namespace MonitorService
{
    public partial class Service : ServiceBase
    {

        public static List<IGrabber.IGrabber> Plugins = new List<IGrabber.IGrabber>();
        
        public static List<Timer> Timers = new List<Timer>();
        static Semaphore semaphore = new Semaphore(1,1);
        public Service()
        {
            InitializeComponent();
        }
        
        protected override void OnStart(string[] args)
        {
            try
            {

                GUI.Toast.TryCreateShortcut();

                Configuration.Setup.SetupWorkPlace();
               


                Plugins.AddRange(Configuration.Setup.AddPlugins());
                Configuration.PluginManager.SetupPlugins(Plugins);

                Configuration.TimersManager.CreatePluginsMonitoringTimer();
               // HelpFunctions.BrowserUtil.GetPathToBrowser();

            }
            catch 
            {
            }
        }
            

        public static void TimeUp(object sender, System.Timers.ElapsedEventArgs e, IGrabber.IGrabber module)
        {
            try
            {
                if (HelpFunctions.Connection.InternetConnection())
                {
                    Task.Run(() => {
                        List<Tuple<string, string>> info = module.GetInfo();
                        info.ForEach(q => {
                            if (DataStorage.StorageManager.AddItem(q.Item1, q.Item2, module.Tag))
                            {
                                GUI.Toast.CreateToast(q.Item1, q.Item2,  module.Tag);
                            }
                            Thread.Sleep(3000);                               
                        });
                    });
                }
            }
            catch
            {
            }            
        }
        

        protected override void OnStop()
        {
        }

    }
}
