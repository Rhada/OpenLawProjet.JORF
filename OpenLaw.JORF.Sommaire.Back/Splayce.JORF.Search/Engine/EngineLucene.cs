using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Search.Engine
{
    public class EngineLucene:IEngine
    {
        public Business.ResultBox GetJORFOfDay(DateTime day)
        {
            throw new NotImplementedException();
        }

        public Business.ResultBox GetAllJORF()
        {
            throw new NotImplementedException();
        }

        public Business.ResultBox SearchByKeyword(string keyword, Business.ClauseQuery filters)
        {
            throw new NotImplementedException();
        }

        public Business.JORFText GetTextById(string idText)
        {
            throw new NotImplementedException();
        }

        public void AddTag(string idText, string tag)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllTags(string idText)
        {
            throw new NotImplementedException();
        }

        public List<Business.LienAction> GetLienActions()
        {
            throw new NotImplementedException();
        }

        public List<Business.LienAction> GetLienActions(DateTime dateJo)
        {
            throw new NotImplementedException();
        }
    }
}
