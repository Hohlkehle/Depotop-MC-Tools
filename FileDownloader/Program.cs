using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloader
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length < 2) {
                Console.WriteLine("Wrong arguments length");
                return;
        }
            var imgurl = args[0];
            var downImage = args[1];

            Console.WriteLine("URL: " + imgurl);
            Console.WriteLine("FILE: " + downImage);

            try
            {
                using (WebClient client = new WebClient())
                    client.DownloadFile(new Uri(imgurl), downImage);
                //client.DownloadFileAsync(new Uri(imgurl), downImage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
