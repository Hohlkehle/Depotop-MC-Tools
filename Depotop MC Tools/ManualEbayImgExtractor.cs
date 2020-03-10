using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class ManualEbayImgExtractor
    {
        private List<string[]> m_DepotopRecords;
        private List<string[]> m_OtherRecords;
        public SQLiteConnection m_SQLiteConnection;

        public string DepotopCsvFile;
        public string OtherCsvFile;

        public int AllItemsCount { get { return DepotopRecords.Count + OtherRecords.Count; } }

        public List<string[]> DepotopRecords { get => m_DepotopRecords; set => m_DepotopRecords = value; }
        public List<string[]> OtherRecords { get => m_OtherRecords; set => m_OtherRecords = value; }

       

        public ManualEbayImgExtractor(SQLiteConnection sQLiteConnection)
        {
            m_SQLiteConnection = sQLiteConnection;
            m_DepotopRecords = new List<string[]>();
            m_OtherRecords = new List<string[]>();
        }



        public void Initialize()
        {
            LoadCsvFor(ref m_DepotopRecords, DepotopCsvFile);
            LoadCsvFor(ref m_OtherRecords, OtherCsvFile);
        }

        int LoadCsvFor(ref List<string[]> list, string file)
        {
            list = new List<string[]>();
            try
            {
                using (var reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        if (!Uri.IsWellFormedUriString(values[1], UriKind.Absolute))
                            continue;
                        list.Add(values);
                    }
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
            return list.Count;
        }

        internal ICItem SelectByIndex(int dIndex)
        {
            if (dIndex < DepotopRecords.Count)
            {
                return new ICItem(-1, DepotopRecords[0][0], DepotopRecords[0][1], "", true, -1);
            }
            return new ICItem();
        }

     

        internal List<ICItem> SelectAllByIndex(int dIndex)
        {
            var result = new List<ICItem>();
            if (dIndex < DepotopRecords.Count)
            {
                var dItem = DepotopRecords[dIndex];
                result.Add(new ICItem(-1, DepotopRecords[dIndex][0], DepotopRecords[dIndex][1], "", true, -1));
                foreach (var item in from i in OtherRecords where i[0] == dItem[0] select i)
                {
                    result.Add(new ICItem(-1, item[0], item[1], "", false, -1));
                }
            }
            return result;
        }

        internal int ActualizeWhithDB(ref List<ICItem> items)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var sql = "SELECT id, sku, url, img, depotop, similarity FROM anounces WHERE sku=@sku AND url=@url";
                var cmd = new SQLiteCommand(sql, m_SQLiteConnection);
                cmd.Parameters.AddWithValue("@sku", items[i].Sku);
                cmd.Parameters.AddWithValue("@url", items[i].Url);

                try
                {
                    SQLiteDataReader r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        items[i].Id = int.Parse(r["id"].ToString());
                        items[i].Sku = (string)r["sku"];
                        items[i].Url = (string)r["url"];
                        items[i].Img = (string)r["img"];
                        items[i].Depotop = bool.Parse(r["depotop"].ToString());
                        items[i].Similarity = int.Parse(r["similarity"].ToString());
                        break;
                    }
                    r.Close();
                }
                catch (SQLiteException) { return 1; }

            }

            return 0;
        }

        public void ExportToCsv()
        {
            // Export to csv

            var csv = new StringBuilder();
            csv.AppendLine("Url;Model;Similarity");

            string sql = "SELECT id, sku, url, img, depotop, similarity FROM anounces WHERE sku=@sku AND url=@url AND depotop=@depotop";
            var cmd = new SQLiteCommand(sql, m_SQLiteConnection);

            foreach (var item in OtherRecords)
            {
               
                    cmd.Parameters.AddWithValue("@sku", item[0]);
                    cmd.Parameters.AddWithValue("@url", item[1]);
                    cmd.Parameters.AddWithValue("@depotop", 1);

                    try
                    {
                        SQLiteDataReader r = cmd.ExecuteReader();

                        while (r.Read())
                        {
                            var iCItem = new ICItem(r);
                            var newLine = string.Format("{0};{1};{2};{3}", iCItem.Url, iCItem.Sku, iCItem.Similarity, iCItem.Img);
                            csv.AppendLine(newLine);
                    }
                        r.Close();
                    }
                    catch (SQLiteException ex)
                    {
                        
                    }
            }

            var path = "icoutput.csv";
            try
            {
                File.WriteAllText(path, csv.ToString());
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }
        }
    }
}
