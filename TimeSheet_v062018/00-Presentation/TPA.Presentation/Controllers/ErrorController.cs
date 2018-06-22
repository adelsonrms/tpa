using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{
    /// <summary>
    /// classe para tratar todos os erros
    /// 
    /// http://stackoverflow.com/questions/25849167/custom-403-error-page-in-asp-net-mvc-2
    /// http://stackoverflow.com/questions/28429272/mvc-error-handle-with-custom-error-messages
    /// https://www.codeproject.com/Articles/850062/Exception-handling-in-ASP-NET-MVC-methods-explaine
    /// http://benfoster.io/blog/aspnet-mvc-custom-error-pages
    /// https://dusted.codes/demystifying-aspnet-mvc-5-error-pages-and-error-logging
    /// 
    /// componentes httpmodule para captura de erros
    /// http://elmah.github.io/
    /// https://elmah.io/
    /// 
    /// conclusões:
    /// 1) O uso do Application_Error do Global.asax sempre é executado, então é interessante que lá ocorra apenas log e nunca redirecionamento. É importante manter os status dos erros para tratar no ajax (as vezes queremos obter o número do statuscode no javascript para tomar ações diferentes, e o pior erro é quando não traz o que queremos em vez de dar erro, por exemplo o conteúdo de uma página de erro 404 bonitinha com 200 ok)
    /// 1.1) Quando o erro não gera uma exception no .net, por exemplo um httperror (e não http exception) ou um erro do servidor devido a recurso estático não encontrado (html, css, imgg, js) então ele não é chamado. 
    /// 2) HandleErrorAttribute só funciona com custom errors ligado, e redireciona para views/shared/error.cshtml (mesmo havendo action de erro genérica)
    /// 2.1) Se existir páginas para todos os erros no customerrors elas respondem http 200 ok, bypassam o erro e em cada página você não pega a exception, apenas na views/shared/error.cshtml
    /// 2.2) se existir http erros no system.webserver para cada página, vai cair na página de erro para cada erro separado (sem poder logar exception) e se houver um erro genérico ele redireciona para essa e nunca para views/shared/error.cshtml 
    /// 2.3) erros de forbidden 403 podem ou não ser encaminhados para outra página
    /// 2.3.1) não usar HttpUnauthorizedResult (401) quando é autenticado por serviço externo/terceiro, pois dá loop infinito
    /// 2.3.2) se você tem um AuthorizeAtribute customizado melhor retornar um httperror ou httpexception de 403 (não 401 por causa do loop infinito) em vez de retornar para uma action específica
    /// 2.4) pode ser feito um HandleErrorAttribute customizado para tratar o erro, redirecionar para outra página etc, mas talvez se houverem configurações de httperros no no system.webserver o erro não poderá ser capturado
    /// 2.4.1)system.webserver httperros só funcionam em IIS 7 ou posterior
    /// 2.4.2)system.web custom errors deveriam ser usados apenas se você precisa usar o IIS 6.0
    /// 2.4.3)Usar custom errors ou http erros vai depender se a página é uma página para ser vista por um usuário ou um webservice ou api para ser consumido por um programa
    /// 2.5) Com custom errors uma exception de divisão por zero dentro da aplicação não cai no Application_Error (independente de ter http erros ou não), mas sem custom errors cai no Application_Error
    /// 2.5.1) sem custom errors HandleErrorAttribute não funciona, a não ser que você faça um customizado
    /// 2.5.2) com customerrors HandleErrorAttribute para exceptions são interceptadas não caindo no Application_Error mas vão para o views/shared/error.cshtml
    /// 2.5.3) se somar http errors aos custom errors HandleErrorAttribute para de funcionar para exceptions padrão, mas continua funcionando para erros internos da aplicação, porém redireciona para outra página
    /// 3) Erros 404 e 403 (com httpexception e não http error) SEMPRE caem no  Application_Error
    /// 4) system.webserver http errors pegam erros http que não são http exceptions, e caso não existam eles vão para a página de http error padrão do .net bonitinha, mas não cai em uma página customizada sua, nem propaga o erro  
    /// </summary>
    [AllowAnonymous]
    [TPADescricaoAcaoController("Ver Mensagens de erros personalizadas", "O acesso é anônimo")]
    public class ErrorController : TPAController
    {

        #region métodos públicos

        /// <summary>
        /// mensagem de erro 400
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Erro 400", "O acesso é anônimo")]
        public ActionResult BadRequest()
        {
            ErroViewModel errorInfo = ErroViewModel.Create(400, GetLasException());
            this.Response.StatusCode = errorInfo.StatusCode;
            this.Response.TrySkipIisCustomErrors = true;
            return PartialViewIfAjax("Error", errorInfo);
        }

        /// <summary>
        /// mensagem de erro 403
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Erro 403", "O acesso é anônimo")]
        public ActionResult Forbidden()
        {
            ErroViewModel errorInfo = ErroViewModel.Create(403, GetLasException());

            if (User != null && User.Identity != null)
                errorInfo.Usuario = User.Identity.Name;

            if (!string.IsNullOrEmpty(errorInfo.Usuario))
            {
                var usu = db.Usuarios.Where(u => u.Login == errorInfo.Usuario).FirstOrDefault();
                if ((usu == null) || (usu.Perfis.Count == 0))
                {
                    errorInfo.Descricao += string.Format("\r\n O usuário {0} não está cadastrado em nenhum perfil.", errorInfo.Usuario);
                }
            }

            this.Response.StatusCode = errorInfo.StatusCode;
            this.Response.TrySkipIisCustomErrors = true;
            return PartialViewIfAjax("Forbidden", errorInfo);
        }

        /// <summary>
        /// erro 404
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Erro 404", "O acesso é anônimo")]
        public ActionResult NotFound()
        {


            ErroViewModel errorInfo = ErroViewModel.Create(404, GetLasException());

            this.Response.StatusCode = errorInfo.StatusCode;
            this.Response.TrySkipIisCustomErrors = true;
            return PartialViewIfAjax("Error", errorInfo);
        }


        /// <summary>
        /// erro 414
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Erro 414", "O acesso é anônimo")]
        public ActionResult URLTooLong()
        {
            ErroViewModel errorInfo = ErroViewModel.Create(414, GetLasException());

            this.Response.StatusCode = errorInfo.StatusCode;
            this.Response.TrySkipIisCustomErrors = true;
            return PartialViewIfAjax("Error", errorInfo);
        }


        /// <summary>
        /// erro 500 e outros
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Erro 500", "O acesso é anônimo")]
        public ActionResult Error()
        {
            ErroViewModel errorInfo = ErroViewModel.Create(500, GetLasException());
            if ((errorInfo.UltimoErro != null) && (errorInfo.UltimoErro is System.Web.Mvc.HttpAntiForgeryException))
            {
                this.Response.StatusCode = errorInfo.StatusCode;
                this.Response.TrySkipIisCustomErrors = true;
                errorInfo.Mensagem = "Sua sessão expirou";
                errorInfo.Descricao = "Clique em Login para efetuar login novamente";
                return PartialViewIfAjax("AntiforgeryError", errorInfo);
            }
            else if ((errorInfo.UltimoErro != null) && (errorInfo.UltimoErro is HttpRequestValidationException))
            {
                this.Response.StatusCode = errorInfo.StatusCode;
                this.Response.TrySkipIisCustomErrors = true;
                errorInfo.Mensagem = "Caracteres inválidos no preenchimento";
                errorInfo.Descricao = errorInfo.UltimoErro.Message;
                return PartialViewIfAjax("Error", errorInfo);
            }
            else
            {
                this.Response.StatusCode = errorInfo.StatusCode;
                this.Response.TrySkipIisCustomErrors = true;
                return PartialViewIfAjax(errorInfo);
            }
        }



        /// <summary>
        /// erro 503
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Erro 503", "O acesso é anônimo")]
        public ActionResult ServiceUnavailable()
        {
            ErroViewModel errorInfo = ErroViewModel.Create(503, GetLasException());

            this.Response.StatusCode = errorInfo.StatusCode;
            this.Response.TrySkipIisCustomErrors = true;
            return PartialViewIfAjax("Error", errorInfo);
        }


        #endregion


        #region métodos privados

        private Exception GetLasException()
        {
            var result = (this.TempData["UltimoErro"] as Exception) ?? (Session["UltimoErro"] as Exception) ?? Server.GetLastError();
            if (result != null)
            {
                this.TempData["UltimoErro"] = null;
                Session["UltimoErro"] = null;
            }
            return result;
        }

        #endregion

    }
}