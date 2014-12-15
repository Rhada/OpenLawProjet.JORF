using Splayce.JORF.Business;
using Splayce.JORF.Search.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Search
{
    /// <summary>
    /// Exposes a minimal set of search functionalities
    /// </summary>
    public class Search
    {

        private IEngine _SearchEngine;
        /// <summary>
        /// Get the SearchEngine. By Defaut, the search engine works with collection of JORTexts in memory
        /// </summary>
        public IEngine SearchEngine
        {
            get
            {
                if (_SearchEngine == null)
                {
                    _SearchEngine = (IEngine)new EngineObjects();
                }
                return _SearchEngine;
            }
            set
            {
                _SearchEngine = value;
            }
        }
        /// <summary>
        /// Get all JORFTexts
        /// </summary>
        /// <returns></returns>
        public ResultBox GetAllJORF()
        {
            return SearchEngine.GetAllJORF();
        }
        /// <summary>
        /// Get all JORText for a specific day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public ResultBox GetJORFOfDay(DateTime day)
        {
            return SearchEngine.GetJORFOfDay(day);
        }
        /// <summary>
        /// Search by keyword and some filters
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public ResultBox SearchByKeyword(String keyword, ClauseQuery filters)
        {
            return SearchEngine.SearchByKeyword(keyword, filters);
        }
        /// <summary>
        /// Get  a JORFText by its Id
        /// </summary>
        /// <param name="idText"></param>
        /// <returns></returns>
        public JORFText GetTextById(String idText)
        {
            return SearchEngine.GetTextById(idText);
        }
        /// <summary>
        /// Add a tag for a specific JORFText
        /// </summary>
        /// <param name="idText"></param>
        /// <param name="tag"></param>
        public void AddTag(String idText, String tag)
        {
            SearchEngine.AddTag(idText, tag);
        }
        /// <summary>
        /// Remove all tags associated with a specific JORFText
        /// </summary>
        /// <param name="idText"></param>
        public void RemoveAllTags(String idText)
        {
            SearchEngine.RemoveAllTags(idText);
        }

        /// <summary>
        /// Get all action links between articles and text
        /// </summary>
        /// <returns></returns>
        public List<LienAction> GetLienActions()
        {
            return SearchEngine.GetLienActions();
        }
        /// <summary>
        /// Get action links between articles and text for a specific JO 
        /// </summary>
        /// <param name="dt">Date of the JO</param>
        /// <returns></returns>
        public List<LienAction> GetLienActions(DateTime dt)
        {
            return SearchEngine.GetLienActions(dt);
        }
    }
}
