using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Splayce.JORF.Business;
using Splayce.JORF.Search;
using Newtonsoft.Json;
using System.Collections;

namespace OpenLaw.JORF.Sommaire.FrontWeb.Controllers
{
    public class SearchController : Controller
    {
        private static Hashtable _LastResults;
        public static Hashtable LastResults
        {
            get
            {
                if (_LastResults == null)
                {
                    _LastResults = new Hashtable();
                }
                return _LastResults;

            }
        }

        public ActionResult List()
        {
            Search srch = new Search();
            ResultBox Rbox = srch.GetAllJORF();
            return View(Rbox);
        }
        public ActionResult Index()
        {

            Search srch = new Search();
            ResultBox Rbox = srch.GetAllJORF();
            GetSarde();
            //var pages = Request.QueryInt32("page");
            //var pagination = new Pagination(Rbox.JorfTexts.Count(), pages);
            //var skipped = Pagination.GetSkipped(pages);
            //ViewData["pagination"] = pagination;
            //ViewData["data"] = Rbox.Skip(skipped).Take(10).ToArray();
            List<String> SearchedTerms = new List<String>();

            ViewBag.SearchedTerms = SearchedTerms;
            ViewBag.KeyWord = Rbox.Keyword;
            if (!LastResults.Contains(""))
            {
                LastResults.Add("", Rbox);
                return View(Rbox);
            }
            else
            {
                LastResults[""] = Rbox;
            }

            return View((ResultBox)LastResults[""]);
        }
        [HttpGet]
        public ActionResult Search(string search)
        {
            Search srch = new Search();
            if (!LastResults.Contains(search))
            {
                LastResults.Add(search, srch.SearchByKeyword(search, new ClauseQuery()));

            }
            ViewBag.KeyWord = search;
            GetSarde();
            List<String> SearchedTerms = new List<String>();
            SearchedTerms.Add(search);
            ViewBag.SearchedTerms = SearchedTerms;

            return View("Index", (ResultBox)LastResults[search]);
        }

        private void GetSarde()
        {
            string[] sardes = System.IO.File.ReadAllLines(System.Configuration.ConfigurationManager.AppSettings["SARDEPath"]);
            string json = JsonConvert.SerializeObject(sardes);
            ViewBag.Sarde = json;
        }
        public ActionResult SearchFacet(string keyword, string facetname, string facetvalue)
        {
            if (LastResults.Contains(keyword))
            {
                List<String> SearchedTerms = new List<String>();
                SearchedTerms.Add(keyword);
                SearchedTerms.Add(facetvalue);
                ViewBag.SearchedTerms = SearchedTerms;
                Search search = new Search();
                ResultBox rbox = (ResultBox)LastResults[keyword];
                rbox.SelectedFacets.Terms.Add(new JORFTermQuery() { Field = facetname, Value = facetvalue });
                rbox = search.SearchByKeyword(keyword, rbox.SelectedFacets);
                ViewBag.SelectedFacets = rbox.SelectedFacets;
                LastResults[keyword] = rbox;
                GetSarde();
                ViewBag.KeyWord = rbox.Keyword;
                return View("Index", (ResultBox)LastResults[keyword]);
            }
            else
            {
                List<String> SearchedTerms = new List<String>();
                SearchedTerms.Add(keyword);
                SearchedTerms.Add(facetvalue);
                ViewBag.SearchedTerms = SearchedTerms;
                Search search = new Search();
                ResultBox rbox = new ResultBox();
                rbox.SelectedFacets.Terms.Add(new JORFTermQuery() { Field = facetname, Value = facetvalue });
                rbox = search.SearchByKeyword(keyword, rbox.SelectedFacets);
                ViewBag.SelectedFacets = rbox.SelectedFacets;
                LastResults.Add(keyword, rbox);
                GetSarde();
                ViewBag.KeyWord = rbox.Keyword;
                return View("Index", (ResultBox)LastResults[keyword]);
            }

        }

        public ActionResult Details(string jorfID)
        {
            Search srch = new Search();
            JORFText text = srch.GetTextById(jorfID);

            //srch.AddTag(jorfID, "tag");
            return View(srch.GetTextById(jorfID));
        }

        public ActionResult JoModification()
        {
            Search srh = new Search();
           List<LienAction> actList = srh.GetLienActions();
            return View("JO", actList);
        }
        public ActionResult AddTags(string IdText, string[] tags)
        {
            Search srch = new Search();
            var text = srch.GetTextById(IdText);
            text.Tags.Clear();
            foreach (string item in tags.ToList())
            {
                string[] splitted = item.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string split in splitted.ToList())
                {
                    srch.AddTag(IdText, split);
                }
            }
            return View("Details", text);
        }
    }
}