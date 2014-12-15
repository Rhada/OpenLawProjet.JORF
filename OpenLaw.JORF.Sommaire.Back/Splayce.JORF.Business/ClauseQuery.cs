using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// List of criterias or filters associated with a search query
    /// </summary>
    public class ClauseQuery
    {
        public BooleanQuery Operator { get; set; }

        public List<JORFTermQuery> _Terms;
        public List<JORFTermQuery> Terms
        {
            get
            {
                if (_Terms == null)
                {
                    _Terms = new List<JORFTermQuery>();
                }
                return _Terms;
            }
            set
            {
                _Terms = value;
            }
        }

    }
}
