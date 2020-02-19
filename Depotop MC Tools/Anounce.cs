using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Depotop_MC_Tools.Parser;

namespace Depotop_MC_Tools
{
   public class Anounce
    {
        private List<ImageLink> m_ImageLinks;

        public List<ImageLink> ImageLinks { get => m_ImageLinks; set => m_ImageLinks = value; }

        public virtual void LoadAnounceData(HtmlAgilityPack.HtmlWeb htmlWeb)
        {

        }
    }
}
