using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Depotop_MC_Tools.Parser;

namespace Depotop_MC_Tools
{
   public class Anounce
    {
        public class LoadAnounceDataResult 
        {
            public String ImgUrl;
            public int Index;

            public LoadAnounceDataResult(int index, string url = "")
            {
                this.ImgUrl = url;
                Index = index;
            }

          
        }

        private List<ImageLink> m_ImageLinks;

        public virtual string PrewievUrl { get { return ""; } }

        public List<ImageLink> ImageLinks { get => m_ImageLinks; set => m_ImageLinks = value; }

        public virtual void LoadAnounceData(Parser parser)
        {

        }
    }
}
