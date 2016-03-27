using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MonitorService.HelpFunctions
{
    class Connection
    {
        /// <summary>
        /// The method , which check internet connection
        /// </summary>
        public static bool InternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
