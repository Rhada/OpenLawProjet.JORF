using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// Values of a facet
    /// </summary>
    public class FacetValue:IComparable
    {
        public String Value { get; set; }
        public int Count { get; set; }
        private int _Ordre = 100;
        public int Ordre
        {
            get
            {
                return _Ordre;
            }
            set
            {
                _Ordre = value;
            }
        }
        public int CompareTo(object obj)
        {
            if (obj is FacetValue)
            {
                return this.Ordre.CompareTo(((FacetValue)obj).Ordre);
            }
            throw new InvalidCastException();
        }
    }
}
