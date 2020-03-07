using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepAI; // Add this line to the top of your file
using System.IO;
using NDesk.Options;

namespace DeepAIComparer
{
    class DeepAiOptions
    {
        public object image1 { set; get; }
        public object image2 { set; get; }
    }

    class Program
    {
        static int verbosity;
        static bool useLocalFiles = true;
        static void Main(string[] args)
        {
            args = new string[] { "-k=983cc50b-cd6f-48ad-85c9-b3aae4f7f26f ", "-i=SM-0016.www.ebay.co.uk.123752402471.jpg", "-i1=SM-0016_.www.ebay.co.uk.183994826038.jpg"};
            bool show_help = false;
            string apiKey = "";

            List<string> images = new List<string>();
   
            DeepAI_API m_DeepAI_API = null;

            var p = new OptionSet() {
                { "k|key=", "DeepAI api key", v => apiKey = v },
                { "r", "Use remote files with url", v => useLocalFiles = v == null },
                { "i|img=", "First image to compare", (string v) => images.Add(v) },
                { "i1|img1=", "1 image to compare", (string v) => images.Add(v) },
                { "i2|img2=", "2 image to compare", (string v) => images.Add(v) },
                { "i3|img3=", "3 image to compare", (string v) => images.Add(v) },
                { "i4|img4=", "4 image to compare", (string v) => images.Add(v) },
                { "i5|img5=", "5 image to compare", (string v) => images.Add(v) },
                { "v", "increase debug message verbosity", v => { if (v != null) ++verbosity; } },
                { "h|help",  "show this message and exit", v => show_help = v != null }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("daic: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `daic --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            if (extra.Count > 0)
            {

            }
            else
            {

            }

            if (images.Count < 2)
            {
                Console.WriteLine("Images count must be lager that 2.");
                Console.WriteLine("Try `daic --help' for more information.");
                return;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Deep Api key not specified!");
                Console.WriteLine("Try `daic --help' for more information.");
                return;
            }

            var results = new List<StandardApiResponse>();
            m_DeepAI_API = new DeepAI_API(apiKey: apiKey);

            var comparsionCount = images.Count - 1;
            var firstImage = images[0];

            for (int i = 1; i <= comparsionCount; i++)
            {
                var opt = BuildInputOptions(firstImage, images[i]);
                StandardApiResponse resp = m_DeepAI_API.callStandardApi("image-similarity", opt);
                results.Add(resp);
            }

            foreach (var result in results)
            {
                var distance = result.output.GetType().GetProperty("distance");
                Console.WriteLine(distance);
            }

            Console.ReadKey();

            Debug("Using new message: {0}", "");






            /*
            // Ensure your DeepAI.Client NuGet package is up to date: https://www.nuget.org/packages/DeepAI.Client
            // Example posting a image URL:

            DeepAI_API api = new DeepAI_API(apiKey: "983cc50b-cd6f-48ad-85c9-b3aae4f7f26f");

            StandardApiResponse resp = api.callStandardApi("image-similarity", new
            {
                image1 = "YOUR_IMAGE_URL",
                image2 = "YOUR_IMAGE_URL",
            });
            Console.Write(api.objectAsJsonString(resp));
            */

            // Example posting a local image file:


        }

        static DeepAiOptions BuildInputOptions(string img, string img1)
        {
            var opt = new DeepAiOptions();
            if (useLocalFiles)
            {
                opt = new DeepAiOptions()
                {
                    image1 = File.OpenRead(img),
                    image2 = File.OpenRead(img1),
                };

            }
            else
            {
                opt = new DeepAiOptions()
                {
                    image1 = img,
                    image2 = img1,
                };
            }
            return opt;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: daic [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
        static void Debug(string format, params object[] args)
        {
            if (verbosity > 0)
            {
                Console.Write("# ");
                Console.WriteLine(format, args);
            }
        }
    }
}
