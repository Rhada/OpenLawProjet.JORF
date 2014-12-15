using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// Container for articles
    /// </summary>
    public class Section
    {
        public String Title { get; set; }

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
    }
}
