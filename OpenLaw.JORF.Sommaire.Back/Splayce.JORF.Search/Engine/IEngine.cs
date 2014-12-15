using Splayce.JORF.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Search.Engine
{
    public interface IEngine
    {
        ResultBox GetJORFOfDay(DateTime day);
        /// <summary>
        /// Get all JORFTexts
        /// </summary>
        /// <returns></returns>
        ResultBox GetAllJORF();
        /// <summary>
        /// Search by keyword and some filters
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        ResultBox SearchByKeyword(String keyword, ClauseQuery filters);
        /// <summary>
        /// Get JORFText by its Id
        /// </summary>
        /// <param name="idText"></param>
        /// <returns></returns>
        JORFText GetTextById(String idText);
        /// <summary>
        /// Add tag for a specific JORFText
        /// </summary>
        /// <param name="idText"></param>
        /// <param name="tag"></param>
        void AddTag(String idText, String tag);
        /// <summary>
        /// Remove all tags associated with a specific JORFText
        /// </summary>
        /// <param name="idText"></param>
        void RemoveAllTags(String idText);
        /// <summary>
        /// Get all action links between articles and text
        /// </summary>
        /// <returns></returns>
        List<LienAction> GetLienActions();
        /// <summary>
        /// Get action links between articles and text for a specific JO 
        /// </summary>
        /// <param name="dt">Date of the JO</param>
        /// <returns></returns>
        List<LienAction> GetLienActions(DateTime dateJo);
    }
}
