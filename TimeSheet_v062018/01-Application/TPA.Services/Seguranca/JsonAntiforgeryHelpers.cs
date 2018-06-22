using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using System.Web.Helpers;

namespace TPA.Services.Seguranca
{
    /// <summary>
    /// helper estático para gerar/resgatar/gerenciar o token antiforgery em chamadas ajax
    /// </summary>
    public static class JsonAntiforgeryHelpers
    {

        #region métodos estáticos públicos

        /// <summary>
        /// cria um novo token ou resgata o existente se ainda for válido
        /// </summary>
        /// <returns>string - token a ser colocado em um cookie que vai ser comparado com outro em um hidden</returns>
        public static string GetFormToken()
        {
            string cookieToken, formToken;
            string oldCookieToken = HttpContext.Current.Request.Cookies[AntiForgeryConfig.CookieName] == null ? null : HttpContext.Current.Request.Cookies[AntiForgeryConfig.CookieName].Value;
            AntiForgery.GetTokens(oldCookieToken, out cookieToken, out formToken);

            if(!string.IsNullOrWhiteSpace(cookieToken))
            {
                HttpCookie cookie = new HttpCookie(AntiForgeryConfig.CookieName, cookieToken);
                cookie.HttpOnly = true;

                HttpContext.Current.Request.Cookies.Remove(AntiForgeryConfig.CookieName);
                HttpContext.Current.Response.Cookies.Remove(AntiForgeryConfig.CookieName);

                HttpContext.Current.Request.Cookies.Set(cookie);
                HttpContext.Current.Response.Cookies.Set(cookie);
            }

            return formToken;
        }


        #endregion

    }


}