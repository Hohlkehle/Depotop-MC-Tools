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
            DwnList = new List<string[]>();
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
                        if (!Uri.IsWellFormedUriString(values[1], UriKind.Absolute))
                            continue;
                        m_DwnList.Add(values);
                    }
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
            return m_DwnList.Count;
        }

        public int LoadDataWithVerify(System.Data.SQLite.SQLiteConnection sQLiteConnection, dynamic opt)
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
                        if (!Uri.IsWellFormedUriString(values[1], UriKind.Absolute))
                            continue;

                        //SELECT * FROM anounces WHERE url='' depotop=1 AND img IS NOT NULL AND img != "";
                        //SELECT COUNT(*) FROM anounces WHERE url='' depotop=1 AND img IS NOT NULL AND img != "";
                        var cmd = new System.Data.SQLite.SQLiteCommand("SELECT COUNT(*) FROM anounces WHERE url=@url AND depotop=@depotop AND img IS NOT NULL AND img != ''", sQLiteConnection);

                        cmd.Parameters.AddWithValue("@url", values[1]);
                        cmd.Parameters.AddWithValue("@depotop", opt.depotop);
                        var count = (long)cmd.ExecuteScalar();

                        if (count != 0)
                            continue;

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
                try { m_EbayParser.SearchResults.Add(url, anounces); }
                catch (Exception) { }
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
