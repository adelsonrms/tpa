using System.Linq;
using System.Web;
using System.Data.Entity;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace TPA.Services.Seguranca
{


    #region classe ControleAcesso

    /// <summary>
    /// Classe de serviço que consulta e compara perfis e ações e diz quem tem acesso a o que
    /// </summary>
    public class ControleAcesso
    {
        #region campos privados
        /// <summary>
        /// contexto
        /// </summary>
        private TPAContext _db;
        #endregion

        #region Construtores

        /// <summary>
        /// constructor onde você passa o context
        /// </summary>
        /// <param name="ctx"></param>
        public ControleAcesso(TPAContext ctx)
        {
            this._db = ctx;
        }

        #endregion

        #region métodos públicos

        /// <summary>
        /// verifica se o usuário no http context atual está logado
        /// </summary>
        /// <returns>bool - true se existir um usuário no contexto, false caso contrário</returns>
        public bool EstaLogado()
        {
            return
                    ((HttpContext.Current != null)
                    && (HttpContext.Current.User != null)
                    && (HttpContext.Current.User.Identity != null)
                    && (HttpContext.Current.User.Identity.IsAuthenticated));
        }


        /// <summary>
        /// verifica se o usuário logado pode acessar o recurso composto por nome do controller e nome da ação
        /// </summary>
        /// <param name="controllerName">string - nome do controller</param>
        /// <param name="actionName">string - nome da ação</param>
        /// <returns></returns>
        public  bool PodeAcessar(string controllerName, string actionName)
        {
            string nomeAcao = controllerName + "Controller/" + actionName;
            return PodeAcessar(nomeAcao);
        }


        /// <summary>
        /// verifica se o recurso pode ser acessado
        /// </summary>
        /// <param name="nomePermissao"></param>
        /// <returns>string - nome da permissao</returns>
        public bool PodeAcessar(string nomePermissao)
        {
            bool temAcesso = false;

            if(!EstaLogado() )
            {
                return false;
            }

            if((HttpContext.Current.User == null) ||(HttpContext.Current.User.Identity == null) ||(string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name)))
            {
                return false;
            }

            Usuario usu = this._db.Usuarios
                .Include(x => x.Perfis)
                .Where(u => u.Ativo && u.Login == HttpContext.Current.User.Identity.Name).FirstOrDefault();
            if (usu != null)
            {
                if (!usu.Ativo)
                    return false;

                if (usu.Perfis.Where(p => p.Nome.ToLower() == "Admin".ToLower()).Any() )
                {
                    temAcesso = true;
                }
                else
                {

                    temAcesso = (from p in usu.Perfis
                                 from a in p.Acoes
                                 where a.Nome.ToLower() == nomePermissao.ToLower()
                                 select a).Any();


                }
            }

            return EstaLogado() && temAcesso;
        }

        #endregion




        #region métodos estáticos públicos

        /// <summary>
        /// helper estático para verificar nas páginas cshtml se um usuário tem acesso ao controller / action especificados
        /// </summary>
        /// <param name="controllerName">string - nome do controller</param>
        /// <param name="actionName">string - nome da ação</param>
        /// <returns>bool - true se o usuário no contexto tem acesso, false caso contrário</returns>
        public static bool TemAcesso(string controllerName, string actionName)
        {
            using (TPAContext db = new TPAContext())
            {
                ControleAcesso ctrl = new ControleAcesso(db);
                return ctrl.PodeAcessar(controllerName, actionName);
            }
        }

        /// <summary>
        /// helper estático para verificar nas páginas cshtml se um usuário tem acesso ao recurso especificado
        /// </summary>
        /// <param name="nomePermissao">string - nome do recurso/permissão</param>
        /// <returns>bool - true se o usuário no contexto tem acesso, false caso contrário</returns>
        public static bool TemAcesso(string nomePermissao)
        {
            using (TPAContext db = new TPAContext())
            {
                ControleAcesso ctrl = new ControleAcesso(db);
                return ctrl.PodeAcessar(nomePermissao);
            }
        }



        #endregion
    }

    #endregion



    #region classe Menuextensions

    /// <summary>
    /// define o html helper para gerar um menu apenas para usuários autorizados
    /// </summary>
    public static class Menuextensions
    {

        #region métodos estáticos públicos

        /// <summary>
        /// se o usuário logado tem acesso ao recurso, gera um item de menu usando um list item html. Retorna string vazia caso contrário.
        /// </summary>
        /// <param name="html">HtmlHelper - o html helper do Razor</param>
        /// <param name="texto">string - o texto do menu</param>
        /// <param name="acao">string - a ação a ser verificada</param>
        /// <param name="controller">string - o controller a ser verificado</param>
        /// <returns>HtmlString - Um item de menu em html</returns>
        public static HtmlString MenuItemLiSeguro(this HtmlHelper html, string texto, string acao, string controller)
        {
            if (ControleAcesso.TemAcesso(controller, acao))
            {
                return new HtmlString(string.Format("<li>{0}</li>", LinkExtensions.ActionLink(html, texto, acao, controller)));
            }

            return new HtmlString("");
        }

        #endregion

    }

    #endregion

}