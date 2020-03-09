using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class ICItem
    {
        int m_Id;
        string m_Sku;
        string m_Url;
        string m_Img;
        bool m_Depotop;
        int m_Similarity;
        public int Id { get => m_Id; set => m_Id = value; }
        public string Sku { get => m_Sku; set => m_Sku = value; }
        public string Url { get => m_Url; set => m_Url = value; }
        public string Img { get => m_Img; set => m_Img = value; }
        public bool Depotop { get => m_Depotop; set => m_Depotop = value; }
        public int Similarity { get => m_Similarity; set => m_Similarity = value; }

        public ICItem()
        {
        }

        /*
         * id, sku, url, img, depotop, similarity
         */
        public ICItem(SQLiteDataReader reader)
        {
            m_Id = int.Parse(reader["id"].ToString());
            m_Sku = (string)reader["sku"];
            m_Url = (string)reader["url"];
            m_Img = (string)reader["img"];
            m_Depotop = bool.Parse(reader["depotop"].ToString());
            m_Similarity = int.Parse(reader["similarity"].ToString());
        }

        public ICItem(int id, string sku, string url, string img, bool depotop, int similarity)
        {
            m_Id = id;
            m_Sku = sku;
            m_Url = url;
            m_Img = img;
            m_Depotop = depotop;
            m_Similarity = similarity;
        }

       

    }
}
