using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TPA.Infra.Data;
using TPA.Infra.Services;

namespace TPA.Services.Seguranca
{
    /// <summary>
    /// Custom Authorize Attribute d Tecnun
    /// Usa o ControleAcesso para implementar um AuthorizeAttribute verificando se o usuário pode acessar a ação debaixo do contexto, e redirecionando para uma página de acesso negado caso ocorra
    /// Diferentemente do AuthorizeAttribute da microsoft, não entra em loop infinito caso o usuário seja autenticado mas não autorizado
    /// Funciona com ajax, mandando um erro 403 ou 401 para o client, conforme a necessidade
    /// </summary>
    public class TPAAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {


        #region métodos protegidos

        /// <summary>
        /// sobrecarrega AuthorizeCore e diz se o usuário pode acessar ou não
        /// </summary>
        /// <param name="httpContext">HttpContextBase - contexto da chamada</param>
        /// <returns>bool - true se o usuário estiver autorizado</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if ((httpContext.Handler != null) && (httpContext.Handler is MvcHandler))
            {
                bool podeAcessar = false;
                using (TPAContext db = new TPAContext())
                {
                    ControleAcesso c = new ControleAcesso(db);

                    try
                    {
                        var routeData = ((MvcHandler)httpContext.Handler).RequestContext.RouteData;
                        string actionName = routeData.Values["action"].ToString();

                        string controllerName = routeData.Values["controller"].ToString();

                        podeAcessar = c.PodeAcessar(controllerName, actionName);
                    }
                    catch (Exception err)
                    {
                        LogServices.LogarException(err);
                    }
                }
                return podeAcessar;
            }
            else
            {
                return base.AuthorizeCore(httpContext);
            }
        }


        /// <summary>
        /// define o que vai fazer se o usuário não está autorizado
        /// verifica se é problema de autorização ou autenticação. 
        /// Se o usuário está autenticado mas não autorizado vai para uma página que diga isso.
        /// Se o usuário não está autenticado verifica se é uma requisição ajax
        ///     se for ajax, retorna um erro 401
        ///     se não for ajax vai para a tela de login
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                /*
                 * EM HIPÓTESE ALGUMA CHAMAR HandleUnauthorizedRequest SE O USUÁRIO JÁ ESTIVER AUTENTICADO
                 * DEVIDO A UMA MUDANÇA NAS ESPECIFICAÇÕES DO PROTOCOLO HTTP E UMA IMPLEMENTAÇÃO ERRÔNEA DA MICROSOFT
                 * ISSO CAUSA UM ERRO DE LOOP INFINITO
                 * */

                //para pegar o nome da action dentro do HandleUnauthorizedRequest
                string action = filterContext.ActionDescriptor.ActionName;
                string controller = filterContext.RequestContext.RouteData.GetRequiredString("controller");

                string msg = string.Format("Você precisa ter autorização para a ação {0}Controller/{1}", controller, action);

                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);


            }
            else
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.HttpContext.Response.Clear();

                    filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Unauthorized, "Não Autorizado. Talvez a sessão tenha caído.");
                    filterContext.HttpContext.Response.Write("Não Autorizado. Talvez a sessão tenha caído.");
                    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                    filterContext.HttpContext.Response.End();


                }
                else
                {
                    //em vez de fazer o que está definido na base de AuthorizeAttribute, redirecionamos para a página de login
                    //segundo:
                    //https://stackoverflow.com/questions/38517518/how-to-use-both-internal-forms-authenticationas-well-as-azure-ad-authentication
                    //https://stackoverflow.com/questions/26517925/redirect-user-to-custom-login-page-when-using-azure-ad

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        {"action", "Login"},
                        { "controller", "Account"}
                    });

                }



            }
        }


        #endregion

    }
}