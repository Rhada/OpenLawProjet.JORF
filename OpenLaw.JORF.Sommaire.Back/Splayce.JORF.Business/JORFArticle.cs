using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// Article of JORFText
    /// </summary>
    public class JORFArticle
    {
        public String Id { get; set; }
        public String IdText { get; set; }
        public String Nature { get; set; }
        public String NumeroArticle { get; set; }
        public String Texte { get; set; }
        public String Type { get; set; }
    }
}
