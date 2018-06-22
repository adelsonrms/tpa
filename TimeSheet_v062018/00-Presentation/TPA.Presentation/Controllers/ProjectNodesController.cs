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
using TPA.Application;
using TPA.Services.Seguranca;
using TPA.ViewModel;
using TPA.Infra.Services;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller de gerenciamento de ProjectNodes visão tabular
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Gerenciar ProjectNodes (Tabular)", "Permitir que o usuário gerencie ProjectNodes na visão tabular")]
    public class ProjectNodesController : TPAController
    {


        #region public methods

        /// <summary>
        /// get - listar ProjectNodes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar ProjectNodes", "Permitir que o usuário visualize ProjectNodes na visão tabular")]
        public async Task<ActionResult> Index(int? id)
        {
            if ((id == null) || (id == 0))
            {
                ProjectNode raiz = new ProjectNode
                {
                    Id = 0,
                    Nome = "Raiz",
                    Filhos = new List<ProjectNode>(await db.ProjectNodes.Where(n => n.Pai == null).ToListAsync())
                };

                return View(raiz);
            }
            else
            {
                return View(await db.ProjectNodes.Where(n => n.Id == id).FirstOrDefaultAsync());
            }
        }



        /// <summary>
        /// get - criar nós de projeto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Criar ProjectNodes", "Permitir que o usuário crie ProjectNodes na visão tabular")]
        public ActionResult Create(int? id)
        {
            ProjectNodeViewModel pn = new ProjectNodeViewModel();
            if (id != null && id > 0)
            {
                pn.IdPai = id;
                pn.Pai = db.ProjectNodes.Find(id);
            }

            CarregaNodeLabels();
            return View(pn);
        }



        /// <summary>
        /// post - criar nós de projeto
        /// </summary>
        /// <param name="projectNodeVm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Criar ProjectNodes", "Permitir que o usuário crie ProjectNodes na visão tabular")]
        public async Task<ActionResult> Create([Bind(Include = "Id, Nome, HorasEstimadas, IdPai, IdNodeLabel")] ProjectNodeViewModel projectNodeVm)
        {
            if (ModelState.IsValid)
            {

                try
                {

                    var existente = db.ProjectNodes.Any(x => (x.Pai_Id ?? 0) == (projectNodeVm.IdPai ?? 0) && x.Nome == projectNodeVm.Nome);
                    if (existente)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Já existe um item com esse mesmo nome debaixo deste nó de projeto. \r\n  Itens de projetos com nomes repetidos podem existir dentro de nós de projeto diferentes, mas não debaixo do mesmo nó.", TempData, ModelState);
                        CarregaNodeLabels();
                        return View(projectNodeVm);
                    }

                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Não foi possível pesquisar no banco de dados por este item.\r\n " + LogServices.ConcatenaExceptions(err), TempData, ModelState);
                    CarregaNodeLabels();
                    return View(projectNodeVm);
                }

                ProjectNode pn = new ProjectNode();
                pn.HorasEstimadas = projectNodeVm.HorasEstimadas;
                pn.Nome = projectNodeVm.Nome;
                
                if((projectNodeVm.IdPai??0)>0)
                {
                    ProjectNode pai = db.ProjectNodes.Where(x => x.Id == projectNodeVm.IdPai).FirstOrDefault();
                    if(pai!=null)
                    {
                        pn.Pai = pai;
                    }
                }

                if((projectNodeVm.IdNodeLabel??0) > 0 )
                {
                    NodeLabel nl = db.NodeLabels.Find(projectNodeVm.IdNodeLabel);
                    if(nl != null)
                    {
                        pn.NodeLabel = nl;
                    }
                }

                db.ProjectNodes.Add(pn);

                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { Id = projectNodeVm.IdPai??0});
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. \r\n " + LogServices.ConcatenaExceptions(duce), TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. \r\n " + LogServices.ConcatenaExceptions(err), TempData, ModelState);
                }

            }

            CarregaNodeLabels();
            return View(projectNodeVm);
        }



        /// <summary>
        /// get - editar projectnodes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar ProjectNodes", "Permitir que o usuário altere ProjectNodes na visão tabular")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para editar", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                ProjectNode projectNode = await db.ProjectNodes.FindAsync(id);
                if (projectNode == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }

                ProjectNodeViewModel projectVm = new ProjectNodeViewModel
                {
                    HorasEstimadas = projectNode.HorasEstimadas,
                    Id = projectNode.Id,
                    IdNodeLabel = projectNode.NodeLabel != null ? projectNode.NodeLabel.Id : new Nullable<int>(),
                    IdPai = projectNode.Pai != null ? projectNode.Pai.Id : new Nullable<int>(),
                    Pai = projectNode.Pai,
                    Nome = projectNode.Nome
                };

                CarregaNodeLabels(projectNode.NodeLabel != null ? projectNode.NodeLabel.Id : 0);
                return View(projectVm);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Erro ao procurar nó de projeto: < br />\r\n " + LogServices.ConcatenaExceptions(err), TempData, ModelState);
                return RedirectToAction("index");
            }
        }



        
        /// <summary>
        /// post - editar projectnodes
        /// </summary>
        /// <param name="projectNodeVm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Editar ProjectNodes", "Permitir que o usuário altere ProjectNodes na visão tabular")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nome,HorasEstimadas, IdNodeLabel")] ProjectNodeViewModel projectNodeVm)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var existente = db.ProjectNodes.Any(x => (x.Pai_Id ?? 0) == (projectNodeVm.IdPai ?? 0) && x.Nome == projectNodeVm.Nome);
                    if (existente)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Já existe um item com esse mesmo nome debaixo deste nó de projeto. \r\n  Itens de projetos com nomes repetidos podem existir dentro de nós de projeto diferentes, mas não debaixo do mesmo nó.", TempData, ModelState);
                        CarregaNodeLabels();
                        return View(projectNodeVm);
                    }

                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Não foi possível pesquisar no banco de dados por este item.\r\n " + LogServices.ConcatenaExceptions(err), TempData, ModelState);
                    CarregaNodeLabels();
                    return View(projectNodeVm);
                }

                try
                {
                    ProjectNode pn = db.ProjectNodes.Find(projectNodeVm.Id);
                    NodeLabel nl = db.NodeLabels.Find(projectNodeVm.IdNodeLabel);

                    int? idpai = pn.Pai!=null?pn.Pai.Id:new Nullable<int>();

                    pn.Nome = projectNodeVm.Nome;
                    pn.HorasEstimadas = projectNodeVm.HorasEstimadas;
                    pn.NodeLabel = nl;

                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { Id = idpai });
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. \r\n " + LogServices.ConcatenaExceptions(duce), TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. \r\n " + LogServices.ConcatenaExceptions(err), TempData, ModelState);
                }

            }

            CarregaNodeLabels();
            return View(projectNodeVm);
        }



        /// <summary>
        /// get - exclusão de projectnodes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Excluir ProjectNodes", "Permitir que o usuário altere ProjectNodes na visão tabular")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para excluir", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                ProjectNode projectNode = await db.ProjectNodes.FindAsync(id);
                if (projectNode == null)
                {
                    return HttpNotFound();
                }
                return View(projectNode);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(LogServices.ConcatenaExceptions(err), TempData, ModelState);
                return RedirectToAction("index");
            }

        }

        


        /// <summary>
        /// post - exclusão de projectnodes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Confirmar Exclusão ProjectNodes", "Permitir que o usuário altere ProjectNodes na visão tabular")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProjectNode projectNode = await db.ProjectNodes.FindAsync(id);

            if(projectNode == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Registro não encontrado", TempData, ModelState);
                return RedirectToAction("Index");
            }

            if(projectNode.Filhos.Count > 0)
            {
                MensagemParaUsuarioViewModel.MensagemAlerta("Este projeto não pode ser excluído porque tem projetos filhos associados a ele.", TempData);
                return RedirectToAction("Index");
            }

            if (projectNode.Atividades.Count > 0)
            {
                MensagemParaUsuarioViewModel.MensagemAlerta("Este projeto não pode ser excluído porque já tem atividades lançadas para ele.", TempData);
                return RedirectToAction("Index");
            }


            if (projectNode.UsuariosDesteNode.Count > 0)
            {
                MensagemParaUsuarioViewModel.MensagemAlerta("Este projeto não pode ser excluído porque está designado a usuários nas configurações do usuário.", TempData);
                return RedirectToAction("Index");
            }

            try
            {
                db.Entry(projectNode).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                MensagemParaUsuarioViewModel.MensagemSucesso("Registro Excluido.", TempData);
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa.  \r\n " + LogServices.ConcatenaExceptions(duce), TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído.  \r\n " + LogServices.ConcatenaExceptions(err), TempData, ModelState);
            }

            return View(projectNode);

        }


        #endregion


        #region private methods

        private void CarregaNodeLabels(int? valor = null)
        {
            var lbls = db.NodeLabels.ToList();
            SelectList lblsSelectList = new SelectList(lbls, "Id", "Nome", valor);
            ViewBag.Labels = lblsSelectList;
        }

        #endregion


    }
}
