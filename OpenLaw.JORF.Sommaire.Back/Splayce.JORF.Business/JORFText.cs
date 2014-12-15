using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    public class JORFText:IComparable
    {
        public int Ordre { get; set; }
        public DateTime DateJO { get; set; }
        public String Nature { get; set; }
        public String NumeroJO { get; set; }
        public String NOR { get; set; }
        public DateTime DateTexte { get; set; }
        public DateTime DatePublication { get; set; }
        public DateTime DateSignature { get; set; }
        public String Rubrique1 { get; set; }
        public String Rubrique2 { get; set; }
        public String Rubrique3 { get; set; }
        public String AutoriteEmettrice { get; set; }
        public String IdText { get; set; }
        public String TitreFull { get; set; }
        public String Titre { get; set; }
        public String TexteIntegral { get; set; }
        public String ShortContent { get; set; }
        public String Signataires { get; set; }
        public String UrlFondsOfficiel
        {
            get
            {
                return "http://www.legifrance.gouv.fr/affichTexte.do?cidTexte=" + IdText;
            }
        }
        public String UrlPdf
        {
            get
            {
                return "http://www.legifrance.gouv.fr/jo_pdf.do?cidTexte=" + IdText;
            }
        }
        public String UrlRTF
        {

            get
            {
                return "http://www.legifrance.gouv.fr/telecharger_rtf.do?idTexte=" + IdText + "&dateTexte=29990101";
            }
        }

        private List<Section> _Sections;
        public List<Section> Sections
        {
            get
            {
                if (_Sections == null)
                {
                    _Sections = new List<Section>();
                }
                return _Sections;
            }
            set
            {
                _Sections = value;
            }
        }
        private int _TotalArticles = -1;
        public int TotalArticles
        {
            get
            {
                if (_TotalArticles == -1)
                {
                    foreach (Section s in Sections)
                    {
                        CountArticles(s);
                    }
                    _TotalArticles += Articles.Count;
                }
                return _TotalArticles;
            }
        }

        private void CountArticles(Section sect)
        {
            _TotalArticles += sect.Articles.Count;
            foreach (Section s in sect.Sections)
            {
                _TotalArticles += s.Articles.Count;
                if (s.Sections.Count > 0)
                {
                    CountArticles(s);
                }
            }
        }
        private List<JORFArticle> _Articles;
        public List<JORFArticle> Articles
        {
            get
            {
                if (_Articles == null)
                {
                    _Articles = new List<JORFArticle>();
                }
                return _Articles;
            }
            set
            {
                _Articles = value;
            }
        }

        public String Type { get; set; }

        private List<String> _Tags;
        public List<String> Tags
        {
            get
            {
                if (_Tags == null)
                {
                    _Tags = new List<string>();
                }
                return _Tags;
            }
            set
            {
                _Tags = new List<string>();
            }
        }
        private List<LienAction> _LienActions;
        public List<LienAction> LienActions
        {
            get
            {
                if (_LienActions == null)
                {
                    _LienActions = new List<LienAction>();
                }
                return _LienActions;
            }
            set
            {
                _LienActions = value;
            }
        }
        public int CompareTo(object obj)
        {
            if (obj is JORFText)
            {
                return this.Ordre.CompareTo(((JORFText)obj).Ordre);
            }
            throw new InvalidCastException();
        }
    }
}
