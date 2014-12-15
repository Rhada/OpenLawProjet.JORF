using System.Web;
using System.Web.Mvc;

namespace OpenLaw.JORF.Sommaire.FrontWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}