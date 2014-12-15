using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// Represents a criteria or filter
    /// </summary>
    public class JORFTermQuery
    {
        public String Field { get; set; }
        public String Value { get; set; }
    }
}
