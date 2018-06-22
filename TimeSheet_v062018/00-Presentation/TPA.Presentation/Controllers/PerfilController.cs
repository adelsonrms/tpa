using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TPA.Domain.DomainModel;
using System.Data.Entity.Infrastructure;
using TPA.Services.Seguranca;
using TPA.ViewModel;
using System.Reflection;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller de gerenciamento de perfis
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Gerenciar Perfis", "Permitir o usuário gerenciar perfis de acesso")]
    public class PerfilController : TPAController
    {


        #region métodos/actions públicos


        /// <summary>
        /// get - lista de perfis
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar Perfis", "Permitir o usuário liste perfis de acesso")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Perfis.ToListAsync());
        }



        /// <summary>
        /// get - criar perfis
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Criar Perfis", "Permitir o usuário criar perfis de acesso")]
        public ActionResult Create()
        {
            return View();
        }



        /// <summary>
        /// post - criar perfis
        /// </summary>
        /// <param name="perfil"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Criar Perfis", "Permitir o usuário criar perfis de acesso")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nome")] Perfil perfil)
        {
            if (ModelState.IsValid)
            {
                db.Perfis.Add(perfil);
                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }
            }

            return View(perfil);
        }



        /// <summary>
        /// get - editar perfis
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar Perfis", "Permitir o usuário alterar perfis de acesso")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para editar", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Perfil perfil = await db.Perfis.FindAsync(id);
                if (perfil == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(perfil);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }



        /// <summary>
        /// post - editar perfis
        /// </summary>
        /// <param name="perfil"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Editar Perfis", "Permitir o usuário alterar perfis de acesso")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nome")] Perfil perfil)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(perfil).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }
            }
            return View(perfil);
        }



        /// <summary>
        /// get - excluir perfis
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Excluir Perfis", "Permitir o usuário exclua perfis de acesso")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para excluir", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Perfil perfil = await db.Perfis.FindAsync(id);
                if (perfil == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(perfil);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }

        

        /// <summary>
        /// post - exclusão de perfis
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Confirmar Exclusão de Perfis", "Permitir o usuário exclua perfis de acesso")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Perfil perfil = new Perfil { Id = id };


            try
            {
                db.Entry(perfil).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                MensagemParaUsuarioViewModel.MensagemSucesso("Registro Excluido.", TempData);
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído. " + err.Message, TempData, ModelState);
            }

            return View(perfil);
        }


        #endregion



        #region métodos públicos para gerenciar usuários

        /// <summary>
        /// get - adicionar usuário ao perfi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Adicionar Usuário ao Perfil", "Permitir que o usuário adicione outros usuários aos perfis")]
        public async Task<ActionResult> AdicionarUsuario(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Escolha um perfil para adicionar usuários", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Perfil perfil = await db.Perfis.FindAsync(id);
                if (perfil == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O perfil não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }

                UsuarioRoleViewModel urvm = new UsuarioRoleViewModel
                {
                    Perfil = perfil,
                    IdPerfil = perfil.Id
                };
                CarregaUsuarios(perfil);
                return View(urvm);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }


        /// <summary>
        /// post - adicionar usuário ao perfil
        /// </summary>
        /// <param name="urvm"></param>
        /// <returns></returns>
        [HttpPost, ActionName("AdicionarUsuario")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Adicionar Usuário ao Perfil", "Permitir que o usuário adicione outros usuários aos perfis")]
        public async Task<ActionResult> AdicionarUsuario(UsuarioRoleViewModel urvm)
        {
            if (urvm == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("O usuário e o perfil não podem ser nulos", TempData, ModelState);
                return RedirectToAction("index");
            }

            if (ModelState.IsValid)
            {


                try
                {

                    var usuario = db.Usuarios.Find(urvm.IdUsuario);
                    var perfil = db.Perfis.Find(urvm.IdPerfil);

                    if (usuario == null)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Usuário não encontrado", TempData, ModelState);
                        return RedirectToAction("index");
                    }

                    if (perfil == null)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Perfil não encontrado", TempData, ModelState);
                        return RedirectToAction("index");
                    }

                    urvm.Usuario = usuario;
                    urvm.Perfil = perfil;
                    urvm.IdPerfil = perfil.Id;
                    urvm.IdUsuario = usuario.Id;

                    bool jaPossui = perfil.Usuarios.Contains(usuario);
                    if (!jaPossui)
                    {
                        perfil.Usuarios.Add(usuario);
                    }

                    await db.SaveChangesAsync();

                    if (jaPossui)
                    {
                        MensagemParaUsuarioViewModel.MensagemInfo("Esse perfil já tinha esse usuário", TempData);
                    }
                    else
                    {
                        MensagemParaUsuarioViewModel.MensagemSucesso("Usuário Adicionado ao perfil", TempData);
                    }

                    CarregaUsuarios(perfil);

                    return View(urvm);
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }

            }

            return View(urvm);
        }


        /// <summary>
        /// post - remover usuário do perfil
        /// </summary>
        /// <param name="IdUsuarioExclusao"></param>
        /// <param name="IdPerfilExclusao"></param>
        /// <returns></returns>
        [HttpPost, ActionName("RemoverUsuario")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Remover Usuário do Perfil", "Permitir que o usuário remova outros usuários dos perfis")]
        public async Task<ActionResult> RemoverUsuario(int IdUsuarioExclusao, int IdPerfilExclusao)
        {

            try
            {
                Usuario usuario = db.Usuarios.Find(IdUsuarioExclusao);
                if (usuario != null)
                {
                    Perfil perfil = usuario.Perfis.Where(x => x.Id == IdPerfilExclusao).FirstOrDefault();
                    if (perfil != null)
                    {
                        usuario.Perfis.Remove(perfil);
                        await db.SaveChangesAsync();
                    }
                }

                return RedirectToAction("AdicionarUsuario", new { Id = IdPerfilExclusao });
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído. " + err.Message, TempData, ModelState);
            }

            return RedirectToAction("AdicionarUsuario", new { Id = IdPerfilExclusao });
        }

        #endregion








        #region métodos públicos para gerenciar ações

        /// <summary>
        /// get - adicionar ação ao perfil
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Adicionar Ação ao Perfil", "Permitir que o usuário adicione ações aos perfis")]
        public async Task<ActionResult> AdicionarAcao(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Escolha um perfil para adicionar ações", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Perfil perfil = await db.Perfis.FindAsync(id);
                if (perfil == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O perfil não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }

                PerfilAcaoViewModel pavm = new PerfilAcaoViewModel
                {
                    Perfil = perfil,
                    IdPerfil = perfil.Id
                };
                CarregaAcoes(perfil);
                return View(pavm);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }



        /// <summary>
        /// post - adicionar ação ao perfil
        /// </summary>
        /// <param name="pavm">PerfilAcaoViewModel - dados da ação e do perfil</param>
        /// <returns></returns>
        [HttpPost, ActionName("AdicionarAcao")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Adicionar Ação ao Perfil", "Permitir que o usuário adicione ações aos perfis")]
        public async Task<ActionResult> AdicionarAcao(PerfilAcaoViewModel pavm)
        {
            if (pavm == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A ação e o perfil não podem ser nulos", TempData, ModelState);
                return RedirectToAction("index");
            }

            if (ModelState.IsValid)
            {


                try
                {

                    var acao = db.Acoes.Find(pavm.IdAcao);
                    var perfil = db.Perfis.Find(pavm.IdPerfil);

                    if (acao == null)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Ação não encontrada", TempData, ModelState);
                        return RedirectToAction("index");
                    }

                    if (perfil == null)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Perfil não encontrado", TempData, ModelState);
                        return RedirectToAction("index");
                    }

                    pavm.Acao = acao;
                    pavm.Perfil = perfil;
                    pavm.IdPerfil = perfil.Id;
                    pavm.IdAcao = acao.Id;

                    bool jaPossui = perfil.Acoes.Contains(acao);
                    if (!jaPossui)
                    {
                        perfil.Acoes.Add(acao);
                    }

                    await db.SaveChangesAsync();

                    if (jaPossui)
                    {
                        MensagemParaUsuarioViewModel.MensagemInfo("Esse perfil já tinha essa ação", TempData);
                    }
                    else
                    {
                        MensagemParaUsuarioViewModel.MensagemSucesso("Ação Adicionada ao perfil", TempData);
                    }

                    CarregaAcoes(perfil);

                    return View(pavm);
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }

            }

            return View(pavm);
        }


        /// <summary>
        /// post - remover ação do perfil
        /// </summary>
        /// <param name="IdAcaoExclusao"></param>
        /// <param name="IdPerfilExclusao"></param>
        /// <returns></returns>
        [HttpPost, ActionName("RemoverAcao")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Remover Ação do Perfil", "Permitir que o usuário remova ações dos perfis")]
        public async Task<ActionResult> RemoverAcao(int IdAcaoExclusao, int IdPerfilExclusao)
        {

            try
            {
                Acao acao = db.Acoes.Find(IdAcaoExclusao);
                Perfil perfil = db.Perfis.Find(IdPerfilExclusao);
                if ((acao != null)&&(perfil!=null))
                {
                    perfil.Acoes.Remove(acao);
                    await db.SaveChangesAsync();
                }

                return RedirectToAction("AdicionarAcao", new { Id = IdPerfilExclusao });
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído. " + err.Message, TempData, ModelState);
            }

            return RedirectToAction("AdicionarAcao", new { Id = IdPerfilExclusao });
        }

        #endregion












        #region métodos públicos para o novo gerenciamento de ações




        /// <summary>
        /// get - gerenciar ações dos perfis
        /// Essa página fará requisições ajax aos métodos GetActions, AddActionToRole e RemoveAtionFromRole do controller SegurancaController
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Gerenciar Ações", "Permitir que o usuário gerencie ações dos perfis perfis, para funcionar o usuário deve ter acesso também a métodos GetActions, AddActionToRole e RemoveAtionFromRole do controller SegurancaController")]
        public async Task<ActionResult> GerenciarAcoes(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Escolha um perfil para adicionar ações", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Perfil perfil = await db.Perfis.FindAsync(id);
                if (perfil == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O perfil não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }




                Type tipo = typeof(TPAController);
                Assembly asm = Assembly.GetAssembly(tipo);
                GerenciarAcoesViewModel gvm = new GerenciarAcoesViewModel();
                gvm.IdPerfil = perfil.Id;
                gvm.NomePerfil = perfil.Nome;
                gvm.Controllers = new List<PerfilGerenciarControllerViewModel>();
                gvm.Controllers.AddRange(
                    (
                        from t in asm.GetTypes()
                        where tipo.IsAssignableFrom(t)
                        orderby t.Name
                        select new PerfilGerenciarControllerViewModel
                        {
                            IdPerfil = perfil.Id,
                            NomePerfil = perfil.Nome,
                            NomeClasse = t.Name,
                            NomeClasseCompleto = t.FullName,
                            Nome = ((t.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute(t.Name, t.FullName)).Nome,
                            Descricao = ((t.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute(t.Name, t.FullName)).Descricao,

                        }
                    ).ToList<PerfilGerenciarControllerViewModel>()
                );

                ResourceSet resourceSet = SegurancaResources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
                foreach (DictionaryEntry entry in resourceSet)
                {
                    PerfilGerenciarControllerViewModel segvm = new PerfilGerenciarControllerViewModel
                    {
                        IdPerfil = perfil.Id,
                        Descricao = entry.Value.ToString(),
                        Nome = entry.Value.ToString(),
                        NomeClasse = entry.Value.ToString(),
                        NomeClasseCompleto = entry.Value.ToString()
                    };

                    gvm.Controllers.Add(segvm);
                }



                return View(gvm);

            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
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
        public JsonResult GetActions(int idPerfil, string controllerName)
        {


            try
            {
                List<PerfilGerenciarActionViewModel> acoesTipo = new List<PerfilGerenciarActionViewModel>();

                Type tipo = typeof(TPAController);
                Assembly asm = Assembly.GetAssembly(tipo);
                acoesTipo.AddRange(
                    (
                        from t in asm.GetTypes().SelectMany(tp => tp.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public), (parent, child) => new { TipoController = parent, MetodoAction = child })
                        where tipo.IsAssignableFrom(t.TipoController) && t.TipoController.FullName == controllerName
                        orderby t.MetodoAction.Name
                        select new PerfilGerenciarActionViewModel
                        {
                            NomeAction = t.TipoController.Name + "/" + t.MetodoAction.Name

                        }
                    ).ToList<PerfilGerenciarActionViewModel>());


                ResourceSet resourceSet = SegurancaResources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
                if (resourceSet.GetString(controllerName.ToLower(), true) == controllerName.ToLower())
                {
                    acoesTipo.Add(new PerfilGerenciarActionViewModel
                    {
                        NomeAction = controllerName
                    });
                };

                List<string> nomes = acoesTipo.Select(x => x.NomeAction).Distinct().ToList<string>();

                List<PerfilGerenciarActionViewModel> acoesbanco =
                    (from acao in db.Acoes.Include(a => a.Perfis)
                     where  nomes.Contains(acao.Nome)
                     select new PerfilGerenciarActionViewModel
                     {
                         Id = acao.Id,
                         IdPerfil = idPerfil,
                         NomeAction = acao.Nome,
                         Nome = acao.NomeAmigavel,
                         Descricao = acao.DescricaoAmigavel,
                         TemAcesso = acao.Perfis.Any(p => p.Id == idPerfil)
                     }).ToList();


                var dados = acoesbanco
                    .Select(x => new string[]
                    {
                        x.TemAcesso?"true":"false",
                        x.NomeAction,
                        x.Nome,
                        x.Descricao,
                        ""
                    })
                    .ToList();

                return Json(new
                {
                    Sucesso = true,
                    Mensagem = "",
                    Dados = dados
                });

            }
            catch (Exception err)
            {
                return Json(new
                {
                    Sucesso = false,
                    Mensagem = err.Message
                });
            }
        }



        #endregion












        #region métodos privados

        private void CarregaUsuarios(Perfil perf, int? valor = null)
        {

            var tipo = db.Usuarios.ToList().Except(perf.Usuarios);
            SelectList UsuariosSelectList = new SelectList(tipo, "Id", "FuncionarioNomeOuLogin", valor);
            ViewBag.Usuarios = UsuariosSelectList;
        }


        private void CarregaAcoes(Perfil perf, int? valor = null)
        {

            var tipo = db.Acoes.ToList().Except(perf.Acoes);
            SelectList AcoesSelectList = new SelectList(tipo, "Id", "Nome", valor);
            ViewBag.Acoes = AcoesSelectList;
        }


        #endregion

    }
}
