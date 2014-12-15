using Splayce.JORF.Business;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splayce.JORF.Search.Engine
{
    /// <summary>
    /// Engine based on JORTexts in memory.
    /// This implementation has been made for demo only, so it need optimisation and factorisation
    /// </summary>
    class EngineObjects : IEngine
    {
        private static String _pathTags = System.Configuration.ConfigurationManager.AppSettings["TagsPath"];
        private static String _JOPath = System.Configuration.ConfigurationManager.AppSettings["JOPath"];
        private static List<KeyValuePair<String, String>> _Tags;
        private static List<LienAction> _CacheLienActions;
        private static List<JORF.Business.JORFText> _CacheObject;
        /// <summary>
        /// List of JORFText im memory. This list is shared for all users. Don't use it as is in production !!
        /// </summary>
        public static List<JORF.Business.JORFText> CacheObject
        {
            get
            {
                if (_CacheObject == null)
                {
                    _CacheObject = new List<Business.JORFText>();
                    _CacheLienActions = new List<LienAction>();
                    RetrieveTagsFromFile(_pathTags);

                    Splayce.JORF.Parser.Manager mgt = new Parser.Manager();
                    var updates = System.IO.File.ReadLines(_JOPath);
                    if (updates.Count() > 0)
                    {
                        foreach (var updateFile in updates)
                        {
                            _CacheObject.AddRange(mgt.GetJORFTextFromUpdate(updateFile));
                        }
                    }
                }
                foreach (JORFText txt in _CacheObject)
                {
                    List<KeyValuePair<String, String>> tags = GetTagsById(txt.IdText);
                    foreach (var tag in tags)
                    {
                        if (!txt.Tags.Contains(tag.Value))
                        {
                            txt.Tags.Add(tag.Value);
                        }
                    }
                    _CacheLienActions.AddRange(txt.LienActions);
                }
                return _CacheObject;
            }
        }
        /// <summary>
        /// Get Tags for a specific JORFText
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static List<KeyValuePair<string, string>> GetTagsById(string id)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            if (_Tags != null)
            {
                foreach (var tag in _Tags)
                {
                    if (tag.Key == id)
                    {
                        result.Add(tag);
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Get tags from a file. The file stores all user tags 
        /// </summary>
        /// <param name="pathfile"></param>
        private static void RetrieveTagsFromFile(String pathfile)
        {

            String[] lines = System.IO.File.ReadAllLines(pathfile);
            _Tags = new List<KeyValuePair<string, string>>();
            foreach (String line in lines)
            {
                String[] parts = line.Split(Char.Parse(";"));
                if (parts.Length == 2)
                {
                    KeyValuePair<String, String> tag = new KeyValuePair<string, string>(parts[0], parts[1]);
                    _Tags.Add(tag);
                }
            }
        }
        /// <summary>
        /// Add tag for a specific JORFText
        /// </summary>
        /// <param name="idText"></param>
        /// <param name="tag"></param>
        public void AddTag(String idText, String tag)
        {
            JORFText text = GetTextById(idText);
            if (text != null)
            {
                if (!text.Tags.Contains(tag))
                {
                    text.Tags.Add(tag);
                    String[] newline = new String[1] { idText + ";" + tag + Environment.NewLine };
                    System.IO.File.AppendAllLines(_pathTags, newline);
                }
            }
        }
        /// <summary>
        /// Search by keyword and some filters
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public Business.ResultBox SearchByKeyword(string keyword, ClauseQuery filters)
        {
            try
            {
                String lower_keyword = keyword.ToLower().ToString();
                var jorftexts = from p in CacheObject
                                where p.Titre.ToLower().IndexOf(lower_keyword) != -1 ||
                                                            p.TitreFull.ToLower().IndexOf(lower_keyword) != -1 ||
                                                            p.TexteIntegral.ToLower().IndexOf(lower_keyword) != -1 ||
                                                            p.Nature.ToLower().IndexOf(lower_keyword) != -1 || p.Tags.Where(x => x.ToLower().IndexOf(lower_keyword) != -1).Count() > 0
                                select p;


                jorftexts = Filter(jorftexts, filters);

                List<Facet> facets = ConstructFacets(jorftexts);
                facets.Sort();
                Facet facetNature = facets.Find(x => x.Name == "Nature");
                if (facetNature != null && facetNature.Values.Count > 0)
                {
                    foreach (FacetValue fc in facetNature.Values)
                    {
                        if (fc.Value == "LOI")
                        {
                            fc.Ordre = 1;
                        }
                        if (fc.Value == "DECRET")
                        {
                            fc.Ordre = 2;
                        }
                        if (fc.Value == "ARRETE")
                        {
                            fc.Ordre = 3;
                        }
                    }
                    facetNature.Values.Sort();
                }
                ResultBox rBox = new ResultBox();
                rBox.Facets = facets;

                rBox.JorfTexts = jorftexts.ToList();
                rBox.SelectedFacets = filters;
                rBox.Keyword = keyword;
                rBox.JorfTexts.Sort();
                return rBox;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// Filter result with some criterias
        /// </summary>
        /// <param name="jorftexts">result to be filtered</param>
        /// <param name="filters">criterias</param>
        /// <returns></returns>
        private IEnumerable<JORFText> Filter(IEnumerable<JORFText> jorftexts, ClauseQuery filters)
        {
            foreach (JORFTermQuery term in filters.Terms)
            {
                switch (term.Field)
                {
                    case "DateJO":
                        jorftexts = from p in jorftexts
                                    where p.DateJO == DateTime.ParseExact(term.Value, "dd/MM/yyyy", CultureInfo.GetCultureInfo("fr-FR"))
                                    select p;
                        break;
                    case "Authority":
                        jorftexts = from p in jorftexts
                                    where p.AutoriteEmettrice == term.Value
                                    select p;
                        break;
                    case "Nature":
                        jorftexts = from p in jorftexts
                                    where p.Nature == term.Value
                                    select p;
                        break;
                    case "Type":
                        jorftexts = from p in jorftexts
                                    where p.Type == term.Value
                                    select p;

                        break;
                    case "Rubrique":
                        jorftexts = from p in jorftexts
                                    where p.Rubrique3 == term.Value
                                    select p;

                        break;
                    case "Tag":
                        jorftexts = from p in jorftexts
                                    where p.Tags.Contains(term.Value)
                                    select p;

                        break;
                    //case "Thematic": break;
                }
            }

            return jorftexts;
        }
        /// <summary>
        /// Construct facets
        /// </summary>
        /// <param name="jorftexts"></param>
        /// <returns></returns>
        private static List<Facet> ConstructFacets(IEnumerable<JORFText> jorftexts)
        {
            //Emulate facets
            Hashtable hfacet = new Hashtable();
            hfacet.Add("DateJO", new Facet() { Name = "Date du JO", TechnicalName = "DateJO", Ordre = 1 });
            hfacet.Add("Nature", new Facet() { Name = "Nature", TechnicalName = "Nature", Ordre = 2 });
            hfacet.Add("Authority", new Facet() { Name = "Autorité", TechnicalName = "Authority", Ordre = 3 });
            hfacet.Add("Rubrique", new Facet() { Name = "Rubrique", TechnicalName = "Rubrique", Ordre = 4 });
            hfacet.Add("Type", new Facet() { Name = "Type", TechnicalName = "Type", Ordre = 5 });
            hfacet.Add("Tag", new Facet() { Name = "Tag", TechnicalName = "Tag", Ordre = 6 });
            //List<Facet> facets = new List<Facet>();
            foreach (JORFText jorftext in jorftexts)
            {
                if (!String.IsNullOrEmpty(jorftext.AutoriteEmettrice))
                {
                    Facet facet = (Facet)hfacet["Authority"];
                    facet.AddValue(jorftext.AutoriteEmettrice);
                }
                if (!String.IsNullOrEmpty(jorftext.Nature))
                {
                    Facet facet = (Facet)hfacet["Nature"];
                    facet.AddValue(jorftext.Nature);
                }
                if (!String.IsNullOrEmpty(jorftext.Type))
                {
                    Facet facet = (Facet)hfacet["Type"];
                    facet.AddValue(jorftext.Type);
                }
                if (!String.IsNullOrEmpty(jorftext.Rubrique3))
                {
                    Facet facet = (Facet)hfacet["Rubrique"];
                    facet.AddValue(jorftext.Rubrique3);
                }
                if (!jorftext.DateJO.Equals(DateTime.MinValue))
                {
                    Facet facet = (Facet)hfacet["DateJO"];
                    facet.AddValue(jorftext.DateJO.ToString("dd/MM/yyyy"));
                }

                if (jorftext.Tags.Count > 0)
                {
                    Facet facet = (Facet)hfacet["Tag"];
                    foreach (var tag in jorftext.Tags)
                    {
                        facet.AddValue(tag);
                    }
                }

            }


            List<Facet> facets = new List<Facet>();
            foreach (String key in hfacet.Keys)
            {
                facets.Add((Facet)hfacet[key]);
            }

            return facets;
        }
        /// <summary>
        /// Get JORFText by its Id
        /// </summary>
        /// <param name="idText"></param>
        /// <returns></returns>
        public Business.JORFText GetTextById(string idText)
        {
            var jorftexts = from p in CacheObject
                            where p.IdText == idText
                            select p;
            return jorftexts.FirstOrDefault();
        }
        /// <summary>
        /// Get All JORFText for a specific day
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public Business.ResultBox GetJORFOfDay(DateTime day)
        {
            var jorftexts = from p in CacheObject
                            where p.DateJO == day
                            select p;

            List<Facet> facets = ConstructFacets(jorftexts);
            facets.Sort();

            ResultBox rBox = new ResultBox();
            rBox.Facets = facets;

            rBox.JorfTexts = jorftexts.ToList();
            rBox.JorfTexts.Sort();
            return rBox;

        }
        /// <summary>
        /// Get all JORFTexts
        /// </summary>
        /// <returns></returns>
        public ResultBox GetAllJORF()
        {
            var jorftexts = Filter(CacheObject, new ClauseQuery());

            List<Facet> facets = ConstructFacets(jorftexts);
            facets.Sort();
            ResultBox rBox = new ResultBox();
            rBox.Facets = facets;

            Facet facetNature = facets.Find(x => x.Name == "Nature");
            if (facetNature != null && facetNature.Values.Count > 0)
            {
                foreach (FacetValue fc in facetNature.Values)
                {
                    if (fc.Value == "LOI")
                    {
                        fc.Ordre = 1;
                    }
                    if (fc.Value == "DECRET")
                    {
                        fc.Ordre = 2;
                    }
                    if (fc.Value == "ARRETE")
                    {
                        fc.Ordre = 3;
                    }
                }
                facetNature.Values.Sort();
            }

            rBox.JorfTexts = jorftexts.ToList();
            rBox.JorfTexts.Sort();
            return rBox;
        }
        /// <summary>
        /// Remove all tags associated with a specific JORFText
        /// </summary>
        /// <param name="idText"></param>
        public void RemoveAllTags(string idText)
        {
            JORFText text = GetTextById(idText);
            if (text != null)
            {
                text.Tags.Clear();
            }
        }
        /// <summary>
        /// Get all action links between articles and text
        /// </summary>
        /// <returns></returns>
        public List<LienAction> GetLienActions()
        {
            return _CacheLienActions;
        }
        /// <summary>
        /// Get action links between articles and text for a specific JO 
        /// </summary>
        /// <param name="dt">Date of the JO</param>
        /// <returns></returns>
        public List<LienAction> GetLienActions(DateTime dateJo)
        {
            var jorftexts = from p in CacheObject
                            where p.DateJO == dateJo.Date
                            select p.LienActions;
            return jorftexts.SelectMany(d => d.ToList()).ToList();
        }

    }

}
