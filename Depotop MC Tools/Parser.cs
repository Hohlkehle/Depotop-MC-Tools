using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class Parser
    {
        public class ImageLink : IComparer, IComparable
        {
            public virtual string Url { get; }
            public virtual string BigImageUrl { get; }

            public virtual int Compare(object x, object y)
            {
                if (((ImageLink)x).Url == ((ImageLink)y).Url) return 1;
                return 0;
            }

            public virtual int CompareTo(object obj)
            {
                ImageLink c = (ImageLink)obj;
                return String.Compare(this.Url, c.Url);
            }
        }
        public class ParsedImage
        {
            private string m_Sku;
            private string m_Url;
            private string m_Oe;
            public ParsedImage(string sku, string url, string oe)
            {
                m_Sku = sku;
                m_Url = url;
                m_Oe = oe;
            }

            public string Oe { get => m_Oe; set => m_Oe = value; }
            public string Url { get => m_Url; set => m_Url = value; }
            public string Sku { get => m_Sku; set => m_Sku = value; }
        }
        protected HtmlWeb m_HtmlWeb;
        protected string m_SearchStr = "";
        protected string m_UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win32; x86) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.116 ";
        protected List<string> m_ImagesUrl;
        protected Dictionary<string, List<Anounce>> m_searchResults;
        public Parser() { m_ImagesUrl = new List<string>(); }
        public string SearchStr { get => m_SearchStr; set => m_SearchStr = value; }
        public string UserAgent { get => m_UserAgent; set => m_UserAgent = value; }
        public List<string> ImagesUrl { get => m_ImagesUrl; set => m_ImagesUrl = value; }
        public Dictionary<string, List<Anounce>> SearchResults { get => m_searchResults; set => m_searchResults = value; }
        public HtmlWeb HtmlWebInstance { get => m_HtmlWeb; set => m_HtmlWeb = value; }

        public virtual void Parse()
        {
            foreach (KeyValuePair<string, List<Anounce>> kvp in m_searchResults)
            {
                foreach (var a in kvp.Value)
                {
                    a.LoadAnounceData(this);
                }
            }
        }
        public virtual void Search(string id, string searchStr) { }
        public virtual void Initialize()
        {
            m_searchResults = new Dictionary<string, List<Anounce>>();
            if (m_HtmlWeb == null)
            {
                m_HtmlWeb = new HtmlWeb()
                {
                    AutoDetectEncoding = false,
                    OverrideEncoding = Encoding.UTF8
                };
                m_HtmlWeb.BrowserDelay = TimeSpan.FromSeconds(1);
                m_HtmlWeb.UseCookies = true;
                m_HtmlWeb.UserAgent = UserAgent;
            }
        }

        public void DumpResultToCsv(string path)
        {
            var csv = new StringBuilder();

            foreach (KeyValuePair<string, List<Anounce>> kvp in m_searchResults)
            {
                foreach (var a in kvp.Value)
                {
                    if (a.ImageLinks == null)
                        continue;
                    foreach (var i in a.ImageLinks)
                    {
                        var newLine = string.Format("{0};{1}", kvp.Key, i.BigImageUrl);
                        csv.AppendLine(newLine);
                    }
                }
            }

            File.WriteAllText(path, csv.ToString());
        }

        public static string GetFileExtensionFromUrl(string url)
        {
            url = url.Split('?')[0];
            url = url.Split('/').Last();
            return url.Contains('.') ? url.Substring(url.LastIndexOf('.')) : "";
        }
    }
}
