using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class EbayImgExtractor
    {
        public class EbayImgExtractorResult
        {
            public string Url;
            public string Key;
            public int Index;

            public EbayImgExtractorResult(int index, string url, string key = "")
            {
                this.Url = url;
                Index = index;
                Key = key;
            }
        }

        private string m_CsvFile;
        private List<string[]> m_DwnList;
        private EbayParser m_EbayParser;

        public List<string[]> DwnList { get => m_DwnList; set => m_DwnList = value; }

        public EbayImgExtractor(string csvFile)
        {
            m_CsvFile = csvFile;
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

        public Parser InitializeParser()
        {
            if (m_DwnList.Count == 0)
                return null;

            m_EbayParser = new EbayParser();
            m_EbayParser.Initialize();

            foreach (var line in m_DwnList)
            {
                var model = line[0];
                var url = line[1];

                var anounce = new EbayParser.EbayAnounce(url, new List<Parser.ImageLink>());
                var anounces = new List<Anounce>
                {
                    anounce
                };
                m_EbayParser.SearchResults.Add(model, anounces);
            }

            return m_EbayParser;
        }

        public IEnumerable<EbayParser.LoadEbayAnounceDataResult> ExtractNext()
        {
            if (m_DwnList.Count == 0)
                yield break;

            int progress = 1;

            foreach (var result in m_EbayParser.Parse())
            {
                yield return (EbayParser.LoadEbayAnounceDataResult)result;
                progress++;
            }
        }
    }
}
