using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TPA.ViewModel;

namespace TPA.Services.Seguranca
{
    /// <summary>
    /// Filter attribute customzado para gerenciar, logar e redirecionar erros
    /// Todo: colocar aqui a chamada aos logs, e colocar dentro da classe de log o log em banco de dados exception safe
    /// </summary>
    public class CustomHandleErrorAttribute :  FilterAttribute, IExceptionFilter
    {

        #region métodos públicos

        /// <summary>
        /// evento executado quando uma exception em uma action acontece
        /// </summary>
        /// <param name="filterContext">ExceptionContext - dados sobre o erro</param>
        public virtual void OnException(ExceptionContext filterContext)
        {

            //if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
            //    return;

            if (filterContext.ExceptionHandled)
                return;

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var ex = filterContext.Exception;
            filterContext.ExceptionHandled = true;

            if (filterContext.Exception is HttpException)
            {
                statusCode = (ex as HttpException).GetHttpCode();
            }
            else if (ex is UnauthorizedAccessException)
            {
                //para evitar o redirect infinito de login por confundir o unauthorized com forbidden
                statusCode = (int)HttpStatusCode.Forbidden;
            }


            //código inserido aqui
            var ctx = new ControllerContext(filterContext.RequestContext, filterContext.Controller);
            ctx.Controller.TempData["UltimoErro"] = ex;
            ctx.HttpContext.Session["UltimoErro"] = ex;

            
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            if (statusCode == (int)HttpStatusCode.InternalServerError) //500
            {
                var result = CreateActionResult(filterContext, statusCode);
                filterContext.Result = result;
            }

            filterContext.HttpContext.Response.StatusCode = statusCode;

        }


        #endregion




        #region métodos protegidos

        /// <summary>
        /// cria um ActionResult para onde / qual página de erro redirecionar
        /// </summary>
        /// <param name="filterContext">ExceptionContext - contexto da action</param>
        /// <param name="statusCode">int - código de status http, por exemplo 404</param>
        /// <returns>ActionResult - o resultado para onde redirecionar o fluxo</returns>
        protected virtual ActionResult CreateActionResult(ExceptionContext filterContext, int statusCode)
        {
            var statusCodeName = ((HttpStatusCode)statusCode).ToString();
            var viewName = "~/Views/Error/Error.cshtml";
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var ex = filterContext.Exception;

            var model = ErroViewModel.Create(statusCode, ex);
            model.ControllerName = controllerName;
            model.ActionName = actionName;

            var result = new ViewResult()
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<ErroViewModel>(model),
            };
            result.ViewData.Model = model;
            result.TempData["UltimoErro"] = ex;

            return result;
        }

        #endregion

    }
}