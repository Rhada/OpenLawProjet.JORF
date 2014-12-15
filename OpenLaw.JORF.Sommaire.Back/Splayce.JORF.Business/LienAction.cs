using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Business
{
    /// <summary>
    /// Represents links between article and text (modification link, creation link, etc)
    /// </summary>
    public class LienAction
    {
        /// <summary>
        /// Type of Action (modification, creation, deletion, etc)
        /// </summary>
        public String TypeAction { get; set; }

        /// <summary>
        /// Text that is modified
        /// </summary>
        public String TexteSujet { get; set; }
        /// <summary>
        /// Id of text that is modified
        /// </summary>
        public String IdTexteSujet { get; set; }
        /// <summary>
        /// article of text that is modified
        /// </summary>
        public String ArticleSujet { get; set; }
        /// <summary>
        /// Id of Text that makes the action
        /// </summary>
        public String IdTextModificateur { get; set; }
        /// <summary>
        /// Text that makes the action
        /// </summary>
        public String TextModificateur { get; set; }
        /// <summary>
        ///Nature of text that is modified
        /// </summary>
        public String Nature { get; set; }
    }
}
