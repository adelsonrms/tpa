 using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web.Helpers;

namespace TPA.Services.Seguranca
{
    /// <summary>
    /// versão do AntiForgeryTokenAttribute que valida o AntiForgeryToken para requisições ajax
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ValidateJsonAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {


        #region métodos públicos

        /// <summary>
        /// método executado durante o evento de autorização
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            var httpContext = filterContext.HttpContext;

            //obtendo o cookie do request
            var cookieReq = httpContext.Request.Cookies[AntiForgeryConfig.CookieName];
            var cookie = cookieReq;

            //obtendo o token da formdata
            var req = httpContext.Request[AntiForgeryConfig.CookieName];
            AntiForgery.Validate(cookie != null ? cookie.Value : null, req);

        }

        #endregion


    }
}