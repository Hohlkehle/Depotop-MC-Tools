using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class ManualImageComparer
    {
        MainWindow m_MainWindow;
        public List<ICItem> AllItems;

        public List<ICItem> DepotopItems;
        public List<ICItem> OtherItems;


        private int indexF = 0;
        private int indexS = 0;

        public ICItem CurrentDepotopItem;
        public ICItem CurrentOtherItem;
        public bool IsInitialized { set; get; }
        public ManualImageComparer()
        {
            IsInitialized = false;
        }

        internal void Initialize(MainWindow instace)
        {
            m_MainWindow = instace;
            
            DepotopItems = (from i in AllItems where i.Depotop select i).ToList();
            OtherItems = (from i in AllItems where !i.Depotop select i).ToList();

            IsInitialized = true;
        }

        internal bool Next()
        {
            var isFinished = false;
            var dItem = DepotopItems[indexF];
            var otherItems = (from i in AllItems where !i.Depotop && i.Sku == dItem.Sku select i).ToArray();
            m_MainWindow.ICUpdateFirstImagePreview(dItem.Img);
            ICItem oItem = null;
            if (otherItems.Length > 0)
            {
                oItem = otherItems[indexS];
                m_MainWindow.ICUpdateSeconImagePreview(oItem.Img);
            }

            CurrentOtherItem = oItem;
            CurrentDepotopItem = dItem;

            indexS++;
            if (indexS >= otherItems.Length)
            {
                indexF = (indexF + 1) % DepotopItems.Count;
                indexS = 0;
                if (indexF == 0)
                    isFinished = true;
            }

            return isFinished;
        }

        public void ExportToCsv()
        {
            // Export to csv
           
            var csv = new StringBuilder();
            csv.AppendLine("Url;Model;Similarity");
            foreach (var dItem in DepotopItems)
            {
                var otherItems = (from i in AllItems where !i.Depotop && i.Sku == dItem.Sku select i);
                foreach (var oItem in otherItems)
                {
                    var newLine = string.Format("{0};{1};{2}", oItem.Url, oItem.Sku, oItem.Similarity);
                    csv.AppendLine(newLine);
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
