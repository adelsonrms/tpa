using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TPA.Domain.DomainModel;
using System.Data.Entity.Infrastructure;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller para gerenciamento de labels dos nós de projeto
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Gerenciar NodeLabels", "Permitir o gerenciamento de Labels para Nós de Projetos / Etapas")]
    public class NodeLabelsController : TPAController
    {


        #region métodos públicos

        /// <summary>
        /// get - visualizar página e listar
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar NodeLabels", "Acessar a página inicial e listar os nodelabels")]
        public async Task<ActionResult> Index()
        {
            return View(await db.NodeLabels.ToListAsync());
        }



        /// <summary>
        /// get - criar nodelabel
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Criar NodeLabels", "Permitir a criação de Labels para Nós de Projetos / Etapas")]
        public ActionResult Create()
        {
            return View();
        }



        /// <summary>
        /// post - criar nodelabel
        /// </summary>
        /// <param name="nodeLabel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Criar NodeLabels", "Permitir a criação de Labels para Nós de Projetos / Etapas")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Nome")] NodeLabel nodeLabel)
        {
            if (ModelState.IsValid)
            {
                db.NodeLabels.Add(nodeLabel);
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

            return View(nodeLabel);
        }



        /// <summary>
        /// get - editar nodelabel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar NodeLabels", "Permitir a alteração de Labels para Nós de Projetos / Etapas")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para editar", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                NodeLabel nodeLabel = await db.NodeLabels.FindAsync(id);
                if (nodeLabel == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(nodeLabel);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }



        /// <summary>
        /// post - edição de nodelabel
        /// </summary>
        /// <param name="nodeLabel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Editar NodeLabels", "Permitir a alteração de Labels para Nós de Projetos / Etapas")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Nome")] NodeLabel nodeLabel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(nodeLabel).State = EntityState.Modified;
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
            return View(nodeLabel);
        }




        /// <summary>
        /// get - exclusão de nodelabel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Excluir NodeLabels", "Permitir a exclusão de Labels para Nós de Projetos / Etapas")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para excluir", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                NodeLabel nodeLabel = await db.NodeLabels.FindAsync(id);
                if (nodeLabel == null)
                {
                    return HttpNotFound();
                }
                return View(nodeLabel);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }

        


        /// <summary>
        /// post - exclusão de nodelabels
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Confirmar Exclusão de NodeLabels", "Permitir a exclusão de Labels para Nós de Projetos / Etapas")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            NodeLabel nodeLabel = new NodeLabel { Id = id};

            var count = (from p in db.ProjectNodes
                         where p.NodeLabel.Id == nodeLabel.Id
                         select p).Count();

            if(count > 0)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse rótulo não pode ser excluído porque está associado a projetos.", TempData, ModelState);
                return View(nodeLabel);
            }

            try
            {
                db.Entry(nodeLabel).State = EntityState.Deleted;

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

            return View(nodeLabel);
        }

        #endregion


    }
}
