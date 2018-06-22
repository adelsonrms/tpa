using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPA.Domain.DomainModel;
using TPA.Infra.Data.Repository;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller de segurança
    /// </summary>
    [TPADescricaoAcaoController("Segurança", "Permitir que o usuário gerencie Segurança e Perfis")]
    public class SegurancaController : TPAController
    {


        #region métodos públicos

        /// <summary>
        /// get - index com a lista de controladores do gerenciamento de segurança
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Ver Controlers", "Permitir que o usuário veja os controladores")]
        public  ActionResult Index()
        {
            CarregaPerfis();



            Type tipo = typeof(TPAController);
            Assembly asm = Assembly.GetAssembly(tipo);
            List<SegurancaControllerViewModel> controllerList = 
                (
                    from t in asm.GetTypes()
                    where tipo.IsAssignableFrom(t)
                    orderby t.Name
                    select new SegurancaControllerViewModel
                    {
                        NomeClasse = t.Name,
                        NomeClasseCompleto = t.FullName,
                        Nome = ((t.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute(t.Name, t.FullName)).Nome,
                        Descricao = ((t.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute(t.Name, t.FullName)).Descricao,

                    }
                ).ToList<SegurancaControllerViewModel>();


            ResourceSet resourceSet = SegurancaResources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                SegurancaControllerViewModel segvm = new SegurancaControllerViewModel
                {
                    Descricao = entry.Value.ToString(),
                    Nome = entry.Value.ToString(),
                    NomeClasse = entry.Value.ToString(),
                    NomeClasseCompleto = entry.Value.ToString()
                };

                controllerList.Add(segvm);
            }



            return View(controllerList);
        }




        /// <summary>
        /// post - listar ações
        /// </summary>
        /// <param name="controllerName">string - nome do controller</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Ver Ações", "Permitir que o usuário veja as ações de um controlador")]
        public JsonResult GetActions(string controllerName)
        {
            
            try
            {
                ResourceSet resourceSet = SegurancaResources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
                string resStr = resourceSet.GetString(controllerName.ToLower(), true);
                if ((!string.IsNullOrWhiteSpace(resStr)) && (resStr.ToLower() == controllerName.ToLower()))
                {
                    return Json(new 
                    {
                        Sucesso = true,
                        Mensagem = "",
                        Dados = new List<string[]> { new string[] { controllerName, controllerName, controllerName, controllerName } }
                    });
                }


                Type tipo = typeof(TPAController);
                Assembly asm = Assembly.GetAssembly(tipo);
                List<SegurancaActionViewModel> actionList =
                    (
                        from t in asm.GetTypes().SelectMany(tp => tp.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public), (parent, child) => new { TipoController = parent, MetodoAction = child })
                        where tipo.IsAssignableFrom(t.TipoController) && t.TipoController.FullName == controllerName
                        orderby t.MetodoAction.Name
                        select new SegurancaActionViewModel
                        {
                            NomeAction = t.TipoController.Name + "/" + t.MetodoAction.Name,
                            Nome = ((t.MetodoAction.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute(t.MetodoAction.Name, "")).Nome,
                            Descricao = ((t.MetodoAction.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute("", t.TipoController.Name + "/" + t.MetodoAction.Name)).Descricao,

                        }
                    ).Distinct().ToList<SegurancaActionViewModel>();


                var dados = actionList
                    .Select(x => new string[]
                    {
                        "",
                        x.NomeAction,
                        x.Nome,
                        x.Descricao
                    })
                    .ToList();

                return Json(new
                {
                    Sucesso = true,
                    Mensagem = "",
                    Dados = dados
                });

            }
            catch(Exception err)
            {
                return Json(new
                {
                    Sucesso = false,
                    Mensagem = err.Message
                });
            }
        }



        /// <summary>
        /// post - dado o nome completo da ação, lista os perfis que contém essa ação
        /// </summary>
        /// <param name="completeActionName"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Ver Perfis", "Permitir que o usuário veja os perfis de uma ação")]
        public JsonResult GetRoles(string completeActionName)
        {
            AcaoRepository acaoRep = new AcaoRepository(db);
            Acao existente = acaoRep.GetByName(completeActionName);
            if(existente == null)
            {
                existente = new Acao();
                existente.Nome = completeActionName;

                try
                {
                    Type tipo = typeof(TPAController);
                    Assembly asm = Assembly.GetAssembly(tipo);
                    SegurancaActionViewModel action =
                        (
                            from t in asm.GetTypes().SelectMany(tp => tp.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public), (parent, child) => new { TipoController = parent, MetodoAction = child })
                            where tipo.IsAssignableFrom(t.TipoController) && ((t.TipoController.Name + "/" + t.MetodoAction.Name) == completeActionName)
                            orderby t.MetodoAction.Name
                            select new SegurancaActionViewModel
                            {
                                NomeAction = t.TipoController.Name + "/" + t.MetodoAction.Name,
                                Nome = ((t.MetodoAction.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute(t.MetodoAction.Name, "")).Nome,
                                Descricao = ((t.MetodoAction.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute("", t.TipoController.Name + "/" + t.MetodoAction.Name)).Descricao,

                            }
                        ).FirstOrDefault<SegurancaActionViewModel>();

                    if(action != null)
                    {
                        existente.NomeAmigavel = action.Nome;
                        existente.DescricaoAmigavel = action.Descricao;
                    }
                }
                catch
                {

                }

                acaoRep.Save(existente);
            }

            if (existente.Perfis != null && existente.Perfis.Count > 0)
            {
                return Json(existente.Perfis.Select(x => x.Id).ToArray<int>());
            }
            else
            {
                return Json(null);
            }
        }





        /// <summary>
        /// post - adicionar ação ao perfil
        /// </summary>
        /// <param name="actionName">string - nome completo da ação</param>
        /// <param name="roleId">int - id do perfil</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Adicionar Ação ao Perfil", "Permitir que o usuário adicione ações a um perfil")]
        public JsonResult AddActionToRole(string actionName, int roleId)
        {
            Acao acao = db.Acoes.Where(x => x.Nome == actionName).FirstOrDefault();
            Perfil perfil = db.Perfis.Find(roleId);

            if(acao == null)
            {
                return Json(new { Sucesso = false, Mensagem = "Ação não encontrada" });
            }

            if(perfil == null)
            {
                return Json(new { Sucesso = false, Mensagem = "Perfil não encontrado" });
            }

            try
            {
                if (!perfil.Acoes.Contains(acao))
                {
                    perfil.Acoes.Add(acao);
                    db.SaveChanges();
                }

                return Json(new { Sucesso = true, Mensagem = "Ação associada com sucesso" });
            }
            catch(Exception err)
            {
                return Json(new { Sucesso = false, Mensagem = err.Message });
            }
        }



        /// <summary>
        /// post - remover ação do perfil
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Remover Ação do Perfil", "Permitir que o usuário remova ações de um perfil")]
        public JsonResult RemoveActionFromRole(string actionName, int roleId)
        {
            Acao acao = db.Acoes.Where(x => x.Nome == actionName).FirstOrDefault();
            Perfil perfil = db.Perfis.Find(roleId);

            if (acao == null)
            {
                return Json(new { Sucesso = false, Mensagem = "Ação não encontrada" });
            }

            if (perfil == null)
            {
                return Json(new { Sucesso = false, Mensagem = "Perfil não encontrado" });
            }

            try
            {
                if (perfil.Acoes.Contains(acao))
                {
                    perfil.Acoes.Remove(acao);
                    db.SaveChanges();
                }

                return Json(new { Sucesso = true, Mensagem = "Ação associada com sucesso" });
            }
            catch (Exception err)
            {
                return Json(new { Sucesso = false, Mensagem = err.Message });
            }
        }


        #endregion







        #region métodos privados


        private void CarregaPerfis()
        {
            List<Perfil> results = db.Perfis.AsNoTracking().ToList();
            MultiSelectList select = new MultiSelectList(results, "Id", "Nome");
            ViewBag.Perfis = select;
        }

        #endregion



    }
}