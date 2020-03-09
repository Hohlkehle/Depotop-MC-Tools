using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class DeepAIComparerExecutor
    {
        string app = "daic.exe";
        string key = "983cc50b-cd6f-48ad-85c9-b3aae4f7f26f";
        string output = "";

        public DeepAIComparerExecutor()
        {

        }

        public string Output { get => output; set => output = value; }

        public int Compare(string img1, string img2, bool localFiles = false)
        {
            int result = -1;
            Output = "";
            ExecuteFileDownloader(img1, img2, localFiles);

            if (!String.IsNullOrEmpty(Output) && int.TryParse(Output, out result))
                return result;

            return result;
        }

        public void ExecuteFileDownloader(string img1, string img2, bool localFiles)
        {
            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = app;
            startInfo.Arguments = string.Format(@"""-k={0}"" ""-is={1}""  ""-is={2}"" {3}", key, img1, img2, localFiles ? "" : "-r");
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            //startInfo.RedirectStandardInput
            startInfo.CreateNoWindow = true;
            p.StartInfo = startInfo;
            p.Start();

            output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
        }
    }
}
