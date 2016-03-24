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
        static string[] Users;
        public Service()
        {
            InitializeComponent();
        }
        
        public static void WriteLog(string msg)
        {
            semaphore.WaitOne();
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"D:\Log\Loggs.txt", true))
            {
                file.WriteLine(DateTime.Now + " " + msg);
            }
            semaphore.Release();
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                Users = Configuration.Setup.GetUsers();

                GUI.Toast.TryCreateShortcut();

                Configuration.Setup.SetupWorkPlace();
               

                Plugins.AddRange(Configuration.Setup.AddPlugins());
                Configuration.PluginManager.SetupPlugins(Plugins);

                Configuration.TimersManager.CreatePluginsMonitoringTimer();

            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }
            

        public static void TimeUp(object sender, System.Timers.ElapsedEventArgs e, IGrabber.IGrabber module)
        {
            WriteLog("Started " + module.Tag);
            try
            {
                if (HelpFunctions.Connection.InternetConnection())
                {
                    Task.Run(() => {
                        List<Tuple<string, string>> info = module.GetInfo();
                        WriteLog("Count " + info.Count + " for " + module.Tag ) ;
                        info.ForEach(q => {
                            if (DataStorage.StorageManager.AddItem(q.Item1, q.Item2, module.Tag))
                            {
                                WriteLog("New item! " + q.Item1 + " - " + q.Item2);
                                GUI.Toast.CreateToast(q.Item1, q.Item2,  module.Tag);
                                
                            }
                            Thread.Sleep(3000);                               
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }            
        }
        

        protected override void OnStop()
        {
        }

    }
}
