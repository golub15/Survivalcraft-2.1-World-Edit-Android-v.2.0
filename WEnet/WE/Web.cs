using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using Game;

namespace API_WE_Mod
{
    public static class WEB_manager
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);



        public static void Dowland(string path, string url, Action<string> error, Action<object, AsyncCompletedEventArgs> succses)
        {

            try
            {
                using (WebClient web = new WebClient())
                {
                    byte[] arr = web.DownloadData(new Uri(url));
                    var now = DateTime.Now;
                    Image img = Image.FromStream(new System.IO.MemoryStream(arr));
                    string s = string.Format("Image {0:D4}-{1:D2}-{2:D2} {3:D2}-{4:D2}-{5:D2}.jpg", (object)now.Year,
                        (object)now.Month, (object)now.Day, (object)now.Hour, (object)now.Minute,
                        (object)now.Second);
                    img.Save(path + s + ".png", System.Drawing.Imaging.ImageFormat.Png);

                    AnalyticsManager.LogEvent("WE image down suc", new AnalyticsParameter("url", url));


                }
            }
            catch (Exception ex)
            {
                AnalyticsManager.LogError("WE image down fail", ex);
                error(ex.Message);
            }



        }


        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }



    }
}
