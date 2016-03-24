using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Threading;

namespace MonitorService.GUI
{
    public static class Toast
    {

        private static string APP_ID = "My.Service.Monitor";
        public static bool TryCreateShortcut()
        {
            try
            {
                String shortcutPath = String.Format(@"C:\Users\Rostik\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\ServiceMonitor.lnk");
                if (!File.Exists(shortcutPath))
                {
                    InstallShortcut(shortcutPath);
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
                return false;
            }
        }

        private static void InstallShortcut(String shortcutPath)
        {
            try
            {
                String exePath = Process.GetCurrentProcess().MainModule.FileName;
                IShellLinkW newShortcut = (IShellLinkW)new CShellLink();
                ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
                ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));
                IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;
                using (PropVariant appId = new PropVariant(APP_ID))
                {
                    ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                    ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
                }

                IPersistFile newShortcutSave = (IPersistFile)newShortcut;

                ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
            }
        }
        
        public static void CreateToast(string name, string href,string img)
        {
            try
            {
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
                XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
                stringElements[0].AppendChild(toastXml.CreateTextNode(name));
                stringElements[1].AppendChild(toastXml.CreateTextNode(href));
                string ImgPath = String.Format(@"{0}\{1}.png", Configuration.Setup.IconsPath, img);
                if (File.Exists(ImgPath))
                {
                    XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
                    imageElements[0].Attributes.GetNamedItem("src").NodeValue = ImgPath;
                }
                ToastNotification toast = new ToastNotification(toastXml);
                toast.Activated += new Windows.Foundation.TypedEventHandler<ToastNotification, object>((q, w) => {
                    System.Diagnostics.Process.Start(href);                   
                });
                toast.Dismissed += new Windows.Foundation.TypedEventHandler<ToastNotification, ToastDismissedEventArgs>((q, w) => { System.Diagnostics.Process.Start(href); });
                ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
            }
            catch (Exception ex)
            {
                Service.WriteLog(ex.ToString());
            }

        }
    }
}
