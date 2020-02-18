using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class AmazonParser : Parser
    {
        private string m_DownloadDir;

        public string DownloadDir { get => m_DownloadDir; set => m_DownloadDir = value; }

        public AmazonParser(string downloadDir)
        {
            m_DownloadDir = downloadDir;
        }

        public enum ImageSize
        {
            Preview,
            Original,
            FullSize
        }

        public class ImageLink
        {
            //_SL1500_
            //_AC_UY218_ML3_

            private string m_Src;
            private string m_ServerPath;
            private string m_Name;
            private string m_Size;
            private string m_Ext;
            public ImageLink(string src)
            {
                m_Src = src;

                var pattern = @"([^/]+$)";
                var content = src;
                var imgName = "";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = rgx.Match(content);

                if (match.Success)
                {
                    imgName = match.Value.Trim(); // 71nCl5DivdL._AC_UY218_ML3_.jpg
                    m_Ext = Path.GetExtension(imgName);
                    m_Name = imgName.Split('.')[0];
                    m_Size = imgName.Split('.')[1];
                    m_ServerPath = src.Split(new string[] { imgName }, StringSplitOptions.None)[0];
                }
                else
                {
                    // error
                }

            }

            public string Src { get => m_Src; set => m_Src = value; }
            public string ServerPath { get => m_ServerPath; set => m_ServerPath = value; }
            public string Name { get => m_Name; set => m_Name = value; }
            public string FullName { get => m_Name + "." + m_Size + m_Ext; }
            public string Size { get => m_Size; set => m_Size = value; }
            public string Ext { get => m_Ext; set => m_Ext = value; }

            public string Build(string name, ImageSize size)
            {
                var s = "_AC_UY218_ML3_";
                if (size == ImageSize.FullSize)
                {
                    s = "_SL1500_";

                }
                else
                {

                }
                //https://m.media-amazon.com/images/I/71nCl5DivdL._AC_UY218_ML3_.jpg
                return string.Format("{0}{1}.{2}{3}", ServerPath, name, Size, Ext);
            }
        }

        public class AmazonAnounce
        {
            private string m_Url;
            private List<ImageLink> m_ImageLinks;
            public AmazonAnounce(string url)
            {
                m_Url = url;
                m_ImageLinks = new List<ImageLink>();
            }

            public AmazonAnounce(string url, List<ImageLink> images)
            {
                m_Url = url;
                m_ImageLinks = images;
            }

            public string Url { get => m_Url; set => m_Url = value; }
            public List<ImageLink> ImageLinks { get => m_ImageLinks; set => m_ImageLinks = value; }

            public void LoadAnounceData()
            {

            }
        }

        public void Parse(AmazonAnounce anounce)
        {

        }
    }
}
