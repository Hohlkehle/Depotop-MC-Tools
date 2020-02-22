using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class ImageDownloader
    {
        public class ImageDownloaderResult
        {
            public string File;
            public string Key;
            public int Index;

            public ImageDownloaderResult(int index, string file, string key = "")
            {
                this.File = file;
                Index = index;
                Key = key;
            }
        }

        private string m_CsvFile;
        private string m_OutDirectory;
        private List<string[]> m_DwnList;
        private bool m_SplitByFolder = true;
        public string CsvFile { get => m_CsvFile; set => m_CsvFile = value; }
        public bool SplitByFolder { get => m_SplitByFolder; set => m_SplitByFolder = value; }
        public string OutDirectory { get => m_OutDirectory; set => m_OutDirectory = value; }

        public ImageDownloader(string csvFile, string outDirectory)
        {
            m_CsvFile = csvFile;
            m_OutDirectory = outDirectory;
        }

        public int LoadData()
        {
            m_DwnList = new List<string[]>();
            try
            {
                using (var reader = new StreamReader(m_CsvFile))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        m_DwnList.Add(values);
                    }
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
            return m_DwnList.Count;
        }

        public IEnumerable<ImageDownloaderResult> DownloadNext()
        {
            if (m_DwnList.Count == 0)
                yield break;

            int progress = 1;
            var imgIndex = 0;
            var lastSku = m_DwnList[0][0];

            foreach (var line in m_DwnList)
            {
                var dir = line[0];
                var imgurl = line[1];
                var ext = Parser.GetFileExtensionFromUrl(imgurl);

                if (lastSku == dir)
                    imgIndex++;
                else
                {
                    lastSku = dir;
                    imgIndex = 1;
                }

                var fileName = string.Format("{0}_{1}{2}", dir, imgIndex, ext);
                var dwnDir = SplitByFolder ? System.IO.Path.Combine(OutDirectory, dir) : OutDirectory;
                var downImage = System.IO.Path.Combine(dwnDir, fileName);

                if (SplitByFolder && !Directory.Exists(dwnDir))
                {
                    Directory.CreateDirectory(dwnDir);
                }

                // Task.Factory.StartNew(() => { });

                ExecuteFileDownloader(imgurl, downImage);

                Thread.Sleep(1000);
                /*
                try
                {
                    using (WebClient client = new WebClient())
                        client.DownloadFile(new Uri(imgurl), downImage);
                    //client.DownloadFileAsync(new Uri(imgurl), downImage);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }*/

                yield return new ImageDownloaderResult(progress, downImage, dir);
                progress++;

            }
        }

        public void ExecuteFileDownloader(string imgurl, string file)
        {
            //var exp = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            //var fd = System.IO.Path.Combine(exp, "fd.exe");
            //var hh = string.Format(@"""{0}"" ""{1}"" ", imgurl, file);
            //Process.Start(fd, hh);

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "fd.exe";
            startInfo.Arguments = string.Format(@"""{0}"" ""{1}"" ", imgurl, file);
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            //startInfo.RedirectStandardInput
            startInfo.CreateNoWindow = true;
            p.StartInfo = startInfo;
            p.Start();
        }
    }
}
