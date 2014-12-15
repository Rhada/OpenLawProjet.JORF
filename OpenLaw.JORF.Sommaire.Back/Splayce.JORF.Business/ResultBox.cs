using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// Represent the result of the search
    /// </summary>
    public class ResultBox
    {
        public String Keyword { get; set; }
        private ClauseQuery _SelectedFacets;
        public ClauseQuery SelectedFacets
        {
            get
            {
                if (_SelectedFacets == null)
                {
                    _SelectedFacets = new ClauseQuery();
                }
                return _SelectedFacets;
            }
            set
            {
                _SelectedFacets = value;
            }
        }

        private List<Facet> _Facets;
        public List<Facet> Facets
        {
            get
            {
                if (_Facets == null)
                {
                    _Facets = new List<Facet>();
                }
                return _Facets;
            }
            set
            {
                _Facets = value;
            }
        }

        private List<JORFText> _JorfTexts;
        public List<JORFText> JorfTexts
        {
            get
            {
                if (_JorfTexts == null)
                {
                    _JorfTexts = new List<JORFText>();
                }
                return _JorfTexts;
            }
            set
            {
                _JorfTexts = value;
            }
        }
        public int CurrentPage { get; set; }

    }
}
