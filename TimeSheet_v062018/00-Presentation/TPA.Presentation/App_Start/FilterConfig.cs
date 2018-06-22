using System.Web;
using System.Web.Mvc;
using TPA.Services.Seguranca;

namespace TPA.Presentation
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {

            filters.Add(new CustomHandleErrorAttribute());
            filters.Add(new TPAAuthorizeAttribute());
        }
    }
}
