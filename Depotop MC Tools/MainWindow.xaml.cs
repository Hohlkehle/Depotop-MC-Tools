using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Depotop_MC_Tools
{
    public class TitleData
    {
        private string m_Sku;
        private string m_Auto;
        private string m_Sides;
        private string m_Oe;

        public TitleData(string sku, string auto, string sides, string oe)
        {
            m_Sku = sku;
            m_Auto = auto;
            m_Sides = sides;
            m_Oe = oe;
        }

        public string Sku { get => m_Sku; set => m_Sku = value; }
        public string Sides { get => m_Sides; set => m_Sides = value; }
        public string Auto { get => m_Auto; set => m_Auto = value; }
        public string Oe { get => m_Oe; set => m_Oe = value; }
    }

    public enum Langs
    {
        FR, EN, RU, DE, IT
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> m_WordToKey = new List<string>() { "FR", "EN", "RU", "DE", "IT" };
        private List<string> m_AutoList = new List<string>() { "TOYOTA", "FORD", "CHEVROLET", "NISSAN", "KIA", "MERCEDES", "BMW", "OPEL", "MAZDA", "VOLKSWAGEN", "CITROEN", "VOLVO", "SKODA", "LAND", "RENAULT", "HONDA", "MITSUBISHI", "AUDI", "JEEP", "PEUGEOT", "LEXUS", "SUZUKI", "INFINITI", "FIAT", "MINI", "ALFA", "VW", "HYUNDAI", "CHRYSLER", "SAAB", "HUMMER", "JAGUAR", "CADILLAC", "SUBARU", "DAIHATSU", "DACIA", "DODGE", "SSANGYONG", "LANCIA", "ISUZU", "SSANG", "SMART", "PORSCHE", "IVECO", "SEAT", "DAEWOO", "MCLAREN" };

        private Dictionary<string, string[]> m_SidesLang = new Dictionary<string, string[]>()
        {
            { "avant", new string[] {"avant", "front", "передний", "vorne", "anteriore"} },
            { "arriére", new string[] {"arriére", "rear", "задний", "hinten", "posteriore"} },
            { "arriere", new string[] {"arriére", "rear", "задний", "hinten", "posteriore"} },
            { "supérieur", new string[] {"supérieur", "upper", "верхний", "oben", "superiore"} },
            { "superieur", new string[] {"supérieur", "upper", "верхний", "oben", "superiore"} },
            { "inferieur", new string[] {"inferieur", "lower", "нижний", "unten", "inferiore"} },
            { "droite", new string[] {"droite", "right", "правый", "rechts", "destro"} },
            { "gauche", new string[] {"gauche", "left", "левый", "links", "sinistro"} },
            { "longitudinal", new string[] {"longitudinal", "longitudinal", "продольный", "entlang", "longitudinale"} },
            { "transversal", new string[] {"transversal", "transverse", "поперечный", "quer", "trasversale"} },
            { "pour", new string[] {"pour", "for", "для", "für", "per"} }
        };

        private Dictionary<string, string[]> m_Records;

        public string Autos { get { return String.Join(" ", m_AutoList.ToArray()); } }

        public List<string> Titles { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            m_Records = new Dictionary<string, string[]>();
            Titles = new List<string>();
        }

        private void ReadInputData()
        {
            TbOutputData.Text = "";
            m_Records.Clear();
            Titles = TbInputTitles.Text.Split(';').ToList<string>();

            var text = TbInputData.Text;
            string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            var autos = String.Join("|", m_AutoList.ToArray());
            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i] == string.Empty)
                    continue;

                var marque = "";
                var sides = "";
                var items = lines[i].Split(new[] { ';' });

                var pattern = @"(" + autos + ")";
                var content = items[1];
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                Match match = rgx.Match(content);


                if (match.Success)
                {
                    marque = match.Value.ToUpper().Trim();
                }
                else
                {
                    pattern = @"(^\w+)";
                    rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    match = rgx.Match(content);
                    if (match.Success)
                    {
                        marque = match.Value.ToUpper().Trim();
                    }
                    else
                        continue;
                }
                pattern = "(avant|arriére|arriere|supérieur|superieur|inferieur|droite|gauche|longitudinal|transversal)";
                content = items[1];
                MatchCollection matchList = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
                var sidesList = matchList.Cast<Match>().Select(m => m.Value.ToLower().Trim()).ToList();
                var uniquesidesList = new HashSet<string>(sidesList);
                sides = String.Join(" ", uniquesidesList.ToArray());

                var title = new TitleData(items[0], marque, sides, items[2]);


                m_Records.Add(items[0], new[] { items[2], marque, sides });

                var output = String.Format("{0}\t{1}\t{2}\t{3}\t{4}",
                    FormatTitle(title, Langs.FR),
                    FormatTitle(title, Langs.EN),
                    FormatTitle(title, Langs.RU),
                    FormatTitle(title, Langs.DE),
                    FormatTitle(title, Langs.IT));

                TbOutputData.Text += output;
                TbOutputData.Text += "\r\n";
            }
        }

        private string FormatTitle(TitleData titleData, Langs lang)
        {
            var title = Titles[(int)lang].Trim();
            var auto = MarqueAuto(titleData.Auto);
            var sides = TranslatePhrase(titleData.Sides, lang);
            var pour = Translate("pour", lang);
            var refr = titleData.Oe;
            var mainTitle = String.Format("{0} {1} {2} {3} | {4}", title, sides, pour, auto, refr);
            return mainTitle;
        }

        private string Translate(string word, Langs lang)
        {
            if (!m_SidesLang.ContainsKey(word))
                return "UNKNOWN";
            return m_SidesLang[word][(int)lang];
        }

        private string TranslatePhrase(string phrase, Langs lang)
        {
            var words = new List<string>();
            foreach (var ph in phrase.Split(' '))
            {
                if (ph == string.Empty)
                    continue;
                words.Add(Translate(ph, lang));
            }

            return String.Join(" ", words.ToArray());
        }

        private string MarqueAuto(string auto)
        {
            var res = auto.ToUpper();
            if (res == "LAND")
                return "LAND ROVER";
            else if (res == "ALFA")
                return "ALFA ROMEO";
            else if (res == "SSANG")
                return "SSANG YONG";
            else
                return res;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReadInputData();

        }
    }
}
