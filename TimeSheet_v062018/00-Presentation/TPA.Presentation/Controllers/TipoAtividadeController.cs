using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using TPA.Domain.DomainModel;
using System.Data.Entity.Infrastructure;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controlador de tipo de atividade
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Tipos de Atividade", "Permitir usuário gerenciar tipos de atividades")]
    public class TipoAtividadeController : TPAController
    {


        #region métodos públicos

        /// <summary>
        /// get - listar tipos de atividades
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar Tipos de Atividade", "Permitir usuário listar tipos de atividades")]
        public async Task<ActionResult> Index()
        {
            return View(await db.TiposAtividade.ToListAsync());
        }



        /// <summary>
        /// get - criar tipos de atividades
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Criar Tipos de Atividade", "Permitir usuário criar tipos de atividades")]
        public ActionResult Create()
        {
            return View();
        }



        /// <summary>
        /// post - criar tipos de atividades
        /// </summary>
        /// <param name="tipoAtividade"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Criar Tipos de Atividade", "Permitir usuário criar tipos de atividades")]
        public async Task<ActionResult> Create([Bind(Include = "Id, Nome, Administrativo")] TipoAtividade tipoAtividade)
        {
            if (ModelState.IsValid)
            {
                db.TiposAtividade.Add(tipoAtividade);

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

            return View(tipoAtividade);
        }




        /// <summary>
        /// get - criar tipos de atividades
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar Tipos de Atividade", "Permitir usuário alterar tipos de atividades")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para editar", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                TipoAtividade tipoAtividade = await db.TiposAtividade.FindAsync(id);
                if (tipoAtividade == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(tipoAtividade);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }




        /// <summary>
        /// post - editar tipos de atividades
        /// </summary>
        /// <param name="tipoAtividade"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Editar Tipos de Atividade", "Permitir usuário alterar tipos de atividades")]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Nome, Administrativo")] TipoAtividade tipoAtividade)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(tipoAtividade).State = EntityState.Modified;
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
            return View(tipoAtividade);
        }



        /// <summary>
        /// get - excluir tipos de atividades
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Excluir Tipos de Atividade", "Permitir usuário exclua tipos de atividades")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para excluir", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                TipoAtividade tipoAtividade = await db.TiposAtividade.FindAsync(id);
                if (tipoAtividade == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(tipoAtividade);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }

        


        /// <summary>
        /// post - excluir tipos de atividades
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Confirmar Exclusão de Tipos de Atividade", "Permitir usuário exclua tipos de atividades")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TipoAtividade tipoAtividade = await db.TiposAtividade.FindAsync(id);
            if (tipoAtividade != null)
            {
                if (await db.Atividades.AnyAsync(x=>x.TipoAtividade.Id == tipoAtividade.Id))
                {
                    MensagemParaUsuarioViewModel.MensagemAlerta("Essa ação não pode ser excluída porque já tem perfis associados a ela.", TempData);
                    return RedirectToAction("Index");
                }

                try
                {
                    db.Entry(tipoAtividade).State = EntityState.Deleted;
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
            }
            else
            {
                MensagemParaUsuarioViewModel.MensagemErro("Tipo de Atividade não econtrado.", TempData, ModelState);
            }

            return View(tipoAtividade);
        }

        #endregion



    }
}
