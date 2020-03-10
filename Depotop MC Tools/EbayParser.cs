using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class EbayParser : Parser
    {
        public class LoadEbayAnounceDataResult : Anounce.LoadAnounceDataResult
        {
            public String Key;
            public String Url;
       
            public LoadEbayAnounceDataResult(int index, string url, string imgUrl, string key) : base(index, imgUrl)
            {
                Key = key;
                Url = url;
            }
        }

        public class EbayImageLink : ImageLink
        {
            private string m_Src;
            private string m_ServerPath;
            private string m_Name;
            private string m_Size;
            private string m_Ext;

            public EbayImageLink(string src)
            {
                m_Src = src;

                var pattern = @"([^/]+$)";
                var content = src;
                var imgName = "";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = rgx.Match(content);

                if (match.Success)
                {
                    imgName = match.Value.Trim(); // BZAB-016.jpg
                    m_Ext = Path.GetExtension(imgName);
                    m_Name = imgName.Split('.')[0];
                    m_Size = "450x450"; // imgName.Split('.')[1];
                    m_ServerPath = src.Split(new string[] { imgName }, StringSplitOptions.None)[0];
                }
                else
                {
                    // error
                }
            }
            public EbayImageLink(EbayImageLink link)
            {
                if (link == null)
                    return;
                m_Src = link.Src;
                m_Ext = link.Ext;
                m_Name = link.Name;
                m_Size = link.Size;
                m_ServerPath = link.ServerPath;
            }
            public string Src { get => m_Src; set => m_Src = value; }
            public string ServerPath { get => m_ServerPath; set => m_ServerPath = value; }
            public string Name { get => m_Name; set => m_Name = value; }
            public string FullName { get => m_Name + m_Ext; }
            public string Size { get => m_Size; set => m_Size = value; }
            public string Ext { get => m_Ext; set => m_Ext = value; }
            public override string Url { get { return ServerPath + FullName; } }
            public override string PreviewImageUrl { get { return ServerPath + FullName; } }
            public override string BigImageUrl { get { return PreviewImageUrl; } }
            public string ReBuild(ImageSize size)
            {
                Size = "450x450";
                //https://i.ebayimg.com/images/g/emMAAOSwroZapYf7/s-l300.jpg
                return BigImageUrl;
            }

            public override int Compare(object x, object y)
            {
                if (((EbayImageLink)x).Src == ((EbayImageLink)y).Src) return 1;
                return 0;
            }

            public override int CompareTo(object obj)
            {
                EbayImageLink c = (EbayImageLink)obj;
                if (this.Src == c.Src)
                    return 1;
                return 0;
            }
        }

        public class EbayAnounce : Anounce
        {
            private string m_HomeUrl = "https://ebay.com";
            
            public EbayAnounce(string url, EbayImageLink prewievImage)
            {
                m_Url = url;
                ImageLinks = new List<ImageLink>();
                ImageLinks.Add(prewievImage);

                Uri myUri = new Uri(url);
                m_HomeUrl = myUri.Host;
            }

            public EbayAnounce(string url, List<ImageLink> images)
            {
                m_Url = url;
                ImageLinks = images;

                Uri myUri = new Uri(url);
                m_HomeUrl = myUri.Host;
            }

            
            public string FullUrl { get => m_Url; }
            public override string PrewievUrl { get { if (ImageLinks.Count > 0) return ImageLinks[0].PreviewImageUrl; return ""; } }
            public override string HomeUrl { get => m_HomeUrl; set => m_HomeUrl = value; }
            //public List<AmazonImageLink> ImageLinks { get => m_ImageLinks; set => m_ImageLinks = value; }
            public EbayImageLink PrewievImage
            {
                get
                {
                    if (ImageLinks.Count > 0) return (EbayImageLink)ImageLinks[0]; return null;
                }
            }

            public override void LoadAnounceData(Parser parser)
            {
                try
                {
                    var htmlDoc = parser.HtmlWebInstance.LoadFromBrowser(FullUrl);
                    var img = htmlDoc.DocumentNode.SelectSingleNode(".//img[contains(@id, 'icImg')]/@src");
                    if (img != null)
                    {
                        var src = img.Attributes.FirstOrDefault(u => u.Name == "src");
                        var link = new EbayImageLink(src.Value);
                        ImageLinks.Add(link);
                    }
                }
                catch (Exception) { }
            }
        }

        private string m_SearchUrl = "https://www.ebay.co.uk/sch/i.html?_nkw=";
        private string m_DownloadDir;
        private string m_CurrentParsedKey;
        
        public EbayParser(string downloadDir) : base()
        {
            m_DownloadDir = downloadDir;
        }

        public string SearchUrl { get => m_SearchUrl; set => m_SearchUrl = value; }
        public string CurrentParsedKey { get => m_CurrentParsedKey; set => m_CurrentParsedKey = value; }

        public EbayParser() : base()
        {

        }

        public override IEnumerable<Anounce.LoadAnounceDataResult> Parse()
        {
            int progress = 1;
            foreach (KeyValuePair<string, List<Anounce>> kvp in m_searchResults)
            {
                foreach (var a in kvp.Value)
                {
                    m_CurrentParsedKey = kvp.Key;
                    a.LoadAnounceData(this);
                    yield return new LoadEbayAnounceDataResult(progress, a.Url, a.PrewievUrl, kvp.Key);
                    progress++;
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            m_HtmlWeb.BrowserTimeout = TimeSpan.FromSeconds(40);
        }

        public override void Search(string id, string searchStr)
        {
            var htmlDoc = m_HtmlWeb.LoadFromBrowser(SearchUrl + searchStr);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//ul[contains(@id, 'ListViewInner')]");
            var m_ImageLink = new List<EbayImageLink>();
            var anounces = new List<Anounce>();

            if (nodes != null)
            {
                foreach (HtmlNode item in nodes)
                {
                    var postUrl = "";
                    var postNode = item.SelectSingleNode(".//a[contains(@class, 'img')]");
                    if (postNode != null)
                    {
                        var pn = postNode.Attributes.FirstOrDefault(u => u.Name == "href");
                        postUrl = pn.Value;
                    }

                    var iurlNode = item.SelectSingleNode(".//img/@src");
                    if (iurlNode == null)
                        continue;

                    var src = iurlNode.Attributes.FirstOrDefault(u => u.Name == "src");

                    var anounce = new EbayAnounce(postUrl, new EbayImageLink(src.Value));

                    anounces.Add(anounce);

                    if (anounces.Count >= 4)
                        break;
                }
            }
            m_searchResults.Add(id, anounces);
        }
    }
}
