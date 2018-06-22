using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using TPA.Domain.DomainModel;
using System.Data.Entity.Infrastructure;
using TPA.Application;
using TPA.Services.Seguranca;
using TPA.ViewModel;
using TPA.Services;

namespace TPA.Presentation.Controllers
{


    /// <summary>
    /// controller para lidar com ações de usuários e implementar o esquema de segurança
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Ações de Usuário", "Permitir o controle e edição de ações de usuário e montagem de perfis")]
    public class AcaoController : TPAController
    {


        #region métodos públicos

        /// <summary>
        /// ação get, index da página de ações, com lista de ações
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Acessar página de ações", "Permitir o acesso a página de ações de usuário")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Acoes.ToListAsync());
        }



        /// <summary>
        /// ação get para a página de cadastro de ação
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Criar Ação", "Permitir a criação de ações de usuário")]
        public ActionResult Create()
        {
            return View();
        }



        /// <summary>
        /// post da página de criação de ações
        /// </summary>
        /// <param name="acao">Acao - Acao sendo criada</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Criar Ação", "Permitir a criação de ações de usuário")]
        public async Task<ActionResult> Create([Bind(Include = "Id, Nome, NomeAmigavel, DescricaoAmigavel")] Acao acao)
        {
            if (ModelState.IsValid)
            {
                db.Acoes.Add(acao);
                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch(DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch(Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }
                
            }

            return View(acao);
        }



        /// <summary>
        /// get da página de adição/alteração de ações
        /// </summary>
        /// <param name="id">int - id da ação sendo editada</param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar Ação", "Permitir a alteração de ações de usuário")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para editar", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Acao acao = await db.Acoes.FindAsync(id);
                if (acao == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(acao);
            }
            catch(Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }


        }



        /// <summary>
        /// post da página de edição de ações
        /// </summary>
        /// <param name="acao">Acao - Ação (Domain Model) sendo editada</param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar Ação", "Permitir a alteração de ações de usuário")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Nome, NomeAmigavel, DescricaoAmigavel")] Acao acao)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(acao).State = EntityState.Modified;
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
            return View(acao);
        }



        /// <summary>
        /// get da página de exclusão de ação 
        /// </summary>
        /// <param name="id">int - id da Acao a ser excluída</param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Excluir Ação", "Permitir a exclusão de ações de usuário")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para excluir", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Acao acao = await db.Acoes.FindAsync(id);
                if (acao == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(acao);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }



        /// <summary>
        /// post pós confirmação de exclusão
        /// </summary>
        /// <param name="id">int - id da Acao a ser excluída</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Confirmar Exclusão Ação", "Permitir a exclusão de ações de usuário")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Acao acao = await db.Acoes.FindAsync(id);
            if (acao != null)
            {
                if(acao.Perfis.Count > 0)
                {
                    MensagemParaUsuarioViewModel.MensagemAlerta("Essa ação não pode ser excluída porque já tem perfis associados a ela.", TempData);
                    return RedirectToAction("Index");
                }

                try
                {

                    db.Acoes.Remove(acao);
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
                MensagemParaUsuarioViewModel.MensagemErro("Ação não encontrada.", TempData, ModelState);
            }

            return View(acao);
        }



        /// <summary>
        /// Varre o assembly buscando controllers e actions, juntando com resources SegurancaResources , para cadastrar no banco de dados e atribuir ao Admin 
        /// </summary>
        /// <param name="atualizarDescricoes">bool - se true, sobrescreve os nomes e descrições amigáveis do banco de dados com os dados vindos do assembly</param>
        /// <returns></returns>
        [HttpPost, ActionName("ImportarDoAssembly")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Importar ações do assembly", "Sincroniza as ações do banco de dados com os métodos e resources criados no assembly desse sistema")]
        public ActionResult ImportarDoAssembly(bool atualizarDescricoes = false)
        {
            int alterados = 0;

            Type tipo =  Type.GetType("TPA.Presentation.Controllers.TPAController, TPA.Presentation");

            AcaoServices g = new AcaoServices(db);
            alterados = g.ImportarDoAssembly(tipo, atualizarDescricoes);
            g.AtualizaAdmin();

            MensagemParaUsuarioViewModel.MensagemInfo(string.Format( "Foram importadas/atualizadas {0} ações do sistema", alterados), TempData);

            return RedirectToAction("Index");
        }


        #endregion


    }
}
