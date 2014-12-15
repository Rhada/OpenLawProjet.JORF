using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// Facet or dimension associated with a result of a search
    /// </summary>
    public class Facet:IComparable
    {
        public int Ordre { get; set; }
        public String TechnicalName { get; set; }
        public String Name { get; set; }
        private List<FacetValue> _Values;
        public List<FacetValue> Values 
        {
            get
            {
                if (_Values == null)
                {
                    _Values = new List<FacetValue>();
                }
                return _Values;
            }
            set
            {
                _Values = value;
            }
        }
        public void AddValue(String val)
        {
            bool found = false;
            foreach (FacetValue fv in Values)
            {
                if (fv.Value == val)
                {
                    fv.Count++;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                Values.Add(new FacetValue() { Value = val, Count = 1 });
            }
        }

        public int CompareTo(object obj)
        {
            if (obj is Facet)
            {
                return this.Ordre.CompareTo(((Facet)obj).Ordre);
            }
            throw new InvalidCastException();
        }
    }
}
