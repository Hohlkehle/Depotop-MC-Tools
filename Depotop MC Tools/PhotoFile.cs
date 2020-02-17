using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depotop_MC_Tools
{
    public class PhotoFile
    {
        private string m_Path;
        private string m_Name;
        private string m_Ext;

        public PhotoFile(string path)
        {
            m_Path = System.IO.Path.GetDirectoryName(path);
            m_Name = System.IO.Path.GetFileNameWithoutExtension(path);
            m_Ext = System.IO.Path.GetExtension(path);
        }

        public string Path { get => m_Path; set => m_Path = value; }
        public string Name { get => m_Name; set => m_Name = value; }
        public string Ext { get => m_Ext; set => m_Ext = value; }
    }
}
