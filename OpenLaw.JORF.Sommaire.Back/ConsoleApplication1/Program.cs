using Splayce.JORF.Business;
using Splayce.JORF.Parser;
using Splayce.JORF.Search;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            Search search = new Search();
            ResultBox rBox2 = search.GetAllJORF();
            JORFText text = search.GetTextById("JORFTEXT000029573022");
            search.AddTag(text.IdText, "POLYSOK");
            ResultBox rBox3  = search.SearchByKeyword("polysok", new ClauseQuery());

            var liens = search.GetLienActions(new DateTime(2014, 10, 2));
            String facetName = rBox2.Facets[0].TechnicalName;
            String facetValue = rBox2.Facets[0].Values[0].Value;
            ClauseQuery cQ = new ClauseQuery();
            cQ.Terms.Add(new JORFTermQuery() { Field = facetName, Value = facetValue });
            ResultBox rBox = search.SearchByKeyword("Loi", cQ);
            facetName = rBox2.Facets[2].TechnicalName;
            facetValue = rBox2.Facets[2].Values[0].Value;
            cQ.Terms.Add(new JORFTermQuery() { Field = facetName, Value = facetValue });

            rBox = search.SearchByKeyword("Loi", cQ);

            search.AddTag(rBox.JorfTexts[0].IdText, "test2");
            search.AddTag(rBox.JorfTexts[0].IdText, "test2");

        }
    }
}
