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
    public class FebestParser : Parser
    {
        public class FebestImageLink : ImageLink
        {
            private string m_Src;
            private string m_ServerPath;
            private string m_Name;
            private string m_Size;
            private string m_Ext;
            public FebestImageLink(string src)
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
            public FebestImageLink(FebestImageLink link)
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
                //https://shop.febest.eu/media/catalog/product/cache/all/image/450x450/9df78eab33525d08d6e5fb8d27136e95/base/p/BZAB-016.jpg
                return BigImageUrl;
            }

            public override int Compare(object x, object y)
            {
                if (((FebestImageLink)x).Name == ((FebestImageLink)y).Name) return 1;
                return 0;
            }

            public override int CompareTo(object obj)
            {
                FebestImageLink c = (FebestImageLink)obj;
                if (this.Src == c.Src)
                    return 1;
                return 0;
            }
        }

        public class FebestAnounce : Anounce
        {
            private string m_HomeUrl = "https://shop.febest.eu";
            private string m_Url;
            public FebestAnounce(string url, FebestImageLink prewievImage)
            {
                m_Url = url;
                ImageLinks = new List<ImageLink>();
                ImageLinks.Add(prewievImage);
            }

            public FebestAnounce(string url, List<ImageLink> images)
            {
                m_Url = url;
                ImageLinks = images;
            }

            public string Url { get => m_Url; set => m_Url = value; }
            public string FullUrl { get => m_Url; }
            public override string PrewievUrl { get { if (ImageLinks.Count > 0) return ImageLinks[0].PreviewImageUrl; return ""; } }
            //public List<AmazonImageLink> ImageLinks { get => m_ImageLinks; set => m_ImageLinks = value; }
            public FebestImageLink PrewievImage
            {
                get
                {
                    if (ImageLinks.Count > 0) return (FebestImageLink)ImageLinks[0]; return null;
                }
            }

            public override void LoadAnounceData(Parser parser)
            {
                var htmlDoc = parser.HtmlWebInstance.LoadFromBrowser(FullUrl);
                var nodes = htmlDoc.DocumentNode.SelectNodes(".//li[contains(@class, 'thumbnail-item')]");//thumbnail-item bx-clone
                if (nodes != null)
                {
                    foreach (HtmlNode item in nodes)
                    {
                        var imgUrl = "";
                        var postNode = item.SelectSingleNode(".//a[contains(@class, 'cloud-zoom-gallery')]");
                        if (postNode != null)
                        {
                            var pn = postNode.Attributes.FirstOrDefault(u => u.Name == "href");
                            imgUrl = pn.Value;
                            var link = new FebestImageLink(imgUrl);
                            var contains = false;

                            foreach (var im in ImageLinks)
                                if (link.CompareTo(im) == 1)
                                    contains = true;

                            if (!contains)
                            {
                                link.ReBuild(ImageSize.FullSize);
                                ImageLinks.Add(link);
                            }
                        }
                    }
                }
                ImageLinks = ImageLinks.Distinct().ToList();
            }
        }

        private string m_SearchUrl = "https://shop.febest.eu/catalogsearch/result/?q=";

        public string SearchUrl { get => m_SearchUrl; set => m_SearchUrl = value; }

        public FebestParser() : base()
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            m_HtmlWeb.BrowserTimeout = TimeSpan.FromSeconds(40);
        }

        public override void Search(string id, string searchStr)
        {
            var htmlDoc = m_HtmlWeb.LoadFromBrowser(SearchUrl + searchStr);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'ma-box-content')]");
            var m_ImageLink = new List<FebestImageLink>();
            var anounces = new List<Anounce>();

            if (nodes != null)
            {
                foreach (HtmlNode item in nodes)
                {
                    var postUrl = "";
                    var postNode = item.SelectSingleNode(".//a[contains(@class, 'product-image')]");
                    if (postNode != null)
                    {
                        var pn = postNode.Attributes.FirstOrDefault(u => u.Name == "href");
                        postUrl = pn.Value;
                    }

                    var iurlNode = item.SelectSingleNode(".//img/@src");
                    if (iurlNode == null)
                        continue;

                    var src = iurlNode.Attributes.FirstOrDefault(u => u.Name == "src");

                    var anounce = new FebestAnounce(postUrl, new FebestImageLink(src.Value));

                    anounces.Add(anounce);

                    if (anounces.Count >= 4)
                        break;
                }
            }
            m_searchResults.Add(id, anounces);
        }


    }
}
