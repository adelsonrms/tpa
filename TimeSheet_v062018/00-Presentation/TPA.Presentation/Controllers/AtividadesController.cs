using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using TPA.Domain.DomainModel;
using TFW.Domain;
using System.Data.Entity.Validation;
using TPA.Application;
using TPA.Services.Seguranca;
using TPA.ViewModel.Buscas;
using TPA.ViewModel;
using TPA.Infra.Services;
using AutoMapper;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// gerenciamento completo de atividades
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Gerenciar Atividades", "Permite que o usuário gerencie atividades dos funcionários")]
    public class AtividadesController : TPAController
    {


        #region public actions

        /// <summary>
        /// listar atividades dos usuários
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore  = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Listar Atividades", "Permite que o usuário listar atividades dos funcionários")]
        public async Task<ActionResult> Index()
        {
            var abvm = (Session["abvm"] as AtividadeBuscaStringViewModel) ?? new AtividadeBuscaStringViewModel();
            if ((abvm != null) && (!abvm.IsBlank()))
                return await Busca(abvm);

            CarregaUsuarios();

            var app = new AtividadeApplication(db);
            var ativs = await app.GetAtividadeIndexAsync(null, null, null);
            return View( ativs);
        }


        /// <summary>
        /// pesquisar atividades dos funcionários
        /// </summary>
        /// <param name="abvm"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Pesquisar Atividades", "Permite que o usuário pesquise atividades dos funcionários")]
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        public async Task<ActionResult> Busca(AtividadeBuscaStringViewModel abvm)
        {
            if (abvm == null || abvm.IsBlank())
                abvm = (Session["abvm"] as AtividadeBuscaStringViewModel) ?? new AtividadeBuscaStringViewModel();

            List<int> usuarioIds = new List<int>();

            if(abvm != null && abvm.IdUsuario != null && abvm.IdUsuario.Length > 0)
            {
                var ids = abvm.IdUsuario;
                foreach(string s in ids)
                {
                    usuarioIds.Add(TFW.TFWConvert.ToInteger(s));
                }
            }

            CarregaUsuarios(usuarioIds);

            DateTime? DataInicio = TFW.TFWConvert.ToNullableDateTime(abvm.DataInicio)?? new DateTime(1901, 1, 1);
            DateTime? DataFim = (TFW.TFWConvert.ToNullableDateTime(abvm.DataFim)?? new DateTime(2100, 12, 31)).AddDays(1).AddSeconds(-1);

            var app = new AtividadeApplication(db);
            var attIdx = await app.GetAtividadeIndexAsync(usuarioIds, DataInicio, DataFim);
            Session["abvm"] = abvm;
            return View("Index",  attIdx);

        }


        /// <summary>
        /// get da página de criação de atividades
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Página de Criação de Atividades", "Permite que o usuário criar atividades dos funcionários")]
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        public ActionResult Create()
        {
            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            return View();
        }


        /// <summary>
        /// post ta página de criação de atividades
        /// </summary>
        /// <param name="atividadeVm">AtividadeViewModel - dados da atividade</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Criar Atividades", "Permite que o usuário criar atividades dos funcionários")]
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        public async Task<ActionResult> Create([Bind(Include = "Id, Observacao, Data, Inicio, Fim, IdProjectNode, IdTipoAtividade, IdUsuario")] AtividadeViewModel atividadeVm)
        {
            if (atividadeVm.Data == DateTime.MinValue)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data", TempData, ModelState);
            }

            if ((atividadeVm.Inicio == null) || (atividadeVm.Inicio <= TimeSpan.Zero) || (atividadeVm.Inicio > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para o Início entre 00:00 e 23:59", TempData, ModelState);
            }


            if ((atividadeVm.Fim == null) || (atividadeVm.Fim <= TimeSpan.Zero) || (atividadeVm.Fim > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para o Fim entre 00:00 e 23:59", TempData, ModelState);
            }


            if (atividadeVm.Inicio > atividadeVm.Fim)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A hora de início não pode ser maior que a hora de término", TempData, ModelState);
            }



            if (ModelState.IsValid)
            {

                try
                {
                    var projeto = db.ProjectNodes.Find(atividadeVm.IdProjectNode);
                    var tipo = db.TiposAtividade.Find(atividadeVm.IdTipoAtividade);
                    var usuario = db.Usuarios.Find(atividadeVm.IdUsuario);


                    Atividade domainAtv = new Atividade
                    {
                        Observacao = atividadeVm.Observacao,
                        Inicio = atividadeVm.Data.AddHours(atividadeVm.Inicio.Hours).AddMinutes(atividadeVm.Inicio.Minutes),
                        Fim = atividadeVm.Data.AddHours(atividadeVm.Fim.Hours).AddMinutes(atividadeVm.Fim.Minutes),
                        ProjectNode = projeto,
                        TipoAtividade = tipo,
                        Usuario = usuario
                    };


                    AtividadeApplication app = new AtividadeApplication(this.db);
                    await app.SalvarAsync(domainAtv, true);
                    MensagemParaUsuarioViewModel.MensagemSucesso("Atividade salva.", TempData);
                    return RedirectToAction("Create");
                }
                catch (DbEntityValidationException ex)
                {
                    string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                    MensagemParaUsuarioViewModel.MensagemErro(exceptionMessage, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                }

            }

            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            return View(atividadeVm);
        }


       
        /// <summary>
        /// get - edição de atividades
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Editar Atividades", "Permite que o usuário altere atividades dos funcionários")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atividade atividade = await db.Atividades.FindAsync(id);

            if (atividade == null)
            {
                return HttpNotFound();
            }
            
            AtividadeViewModel atvVm = Mapper.Map<AtividadeViewModel>(atividade);

            CarregaTipos(atvVm.IdTipoAtividade);
            CarregaProjetos(atvVm.IdProjectNode);
            CarregaUsuarios(atvVm.IdUsuario);

            return View(atvVm);
        }




        /// <summary>
        /// post - edição de atividades
        /// </summary>
        /// <param name="atividadeVm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Editar Atividades", "Permite que o usuário altere atividades dos funcionários")]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Observacao, Data, Inicio, Fim, IdProjectNode, IdTipoAtividade, IdUsuario")] AtividadeViewModel atividadeVm)
        {
            if (atividadeVm.Data == DateTime.MinValue)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data", TempData, ModelState);
            }

            if ((atividadeVm.Inicio == null) || (atividadeVm.Inicio <= TimeSpan.Zero) || (atividadeVm.Inicio > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para o Início entre 00:00 e 23:59", TempData, ModelState);
            }


            if ((atividadeVm.Fim == null) || (atividadeVm.Fim <= TimeSpan.Zero) || (atividadeVm.Fim > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para o Fim entre 00:00 e 23:59", TempData, ModelState);
            }


            if (atividadeVm.Inicio > atividadeVm.Fim)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A hora de início não pode ser maior que a hora de término", TempData, ModelState);
            }




            if (ModelState.IsValid)
            {

                try
                {
                    var projeto = db.ProjectNodes.Find(atividadeVm.IdProjectNode);
                    var tipo = db.TiposAtividade.Find(atividadeVm.IdTipoAtividade);
                    var usuario = db.Usuarios.Find(atividadeVm.IdUsuario);

                    Atividade domainAtv = db.Atividades.Find(atividadeVm.Id);
                    domainAtv.Observacao = atividadeVm.Observacao;
                    domainAtv.Inicio = atividadeVm.Data.AddHours(atividadeVm.Inicio.Hours).AddMinutes(atividadeVm.Inicio.Minutes);
                    domainAtv.Fim = atividadeVm.Data.AddHours(atividadeVm.Fim.Hours).AddMinutes(atividadeVm.Fim.Minutes);
                    domainAtv.ProjectNode = projeto;
                    domainAtv.TipoAtividade = tipo;
                    domainAtv.Usuario = usuario;

                    AtividadeApplication app = new AtividadeApplication(this.db);
                    await app.SalvarAsync(domainAtv, true);

                    MensagemParaUsuarioViewModel.MensagemSucesso("Atividade salva.", TempData);
                    return RedirectToAction("Edit", atividadeVm);
                }
                catch (DbEntityValidationException ex)
                {
                    string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                    MensagemParaUsuarioViewModel.MensagemErro(exceptionMessage, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                }


            }

            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            return View(atividadeVm);
        }


        /// <summary>
        /// get - exclusão de atividades
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Excluir Atividades", "Permite que o usuário exclua atividades dos funcionários")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Atividade atividade = await db.Atividades.FindAsync(id);
            if (atividade == null)
            {
                return HttpNotFound();
            }
            return View(atividade);
        }


        


        /// <summary>
        /// post - exclusão de atividades
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Confirmar Exclusão Atividades", "Permite que o usuário exclua atividades dos funcionários")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AtividadeApplication app = new AtividadeApplication(this.db);
            await app.DeleteAsync(id, true);
            MensagemParaUsuarioViewModel.MensagemSucesso("Registro Excluido.", TempData);
            return RedirectToAction("Index");
        }

           
        
        /// <summary>
        /// get - exclusão múltipla de atividades
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Excluir Múltiplas Atividades", "Permite que o usuário exclua múltiplas atividades dos funcionários")]
        public async Task<ActionResult> DeleteMultiplo()
        {
            var abvm = (Session["abvm"] as AtividadeBuscaStringViewModel) ?? new AtividadeBuscaStringViewModel();
            if ((abvm != null) && (!abvm.IsBlank()))
                return await BuscaDeleteMultiplo(abvm);

            CarregaUsuarios();

            var app = new AtividadeApplication(db);
            var ativs = await app.GetAtividadeIndexAsync(null, null, null);
            return View(ativs);
        }



        
        /// <summary>
        /// get - busca e seleção para exclusão múltipla
        /// </summary>
        /// <param name="abvm"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Selecionar Múltiplas Atividades para Exclusão", "Permite que o usuário exclua múltiplas atividades dos funcionários")]
        public async Task<ActionResult> BuscaDeleteMultiplo(AtividadeBuscaStringViewModel abvm)
        {

            if (abvm == null || abvm.IsBlank()) 
                abvm = (Session["abvm"] as AtividadeBuscaStringViewModel) ?? new AtividadeBuscaStringViewModel();

            List<int> usuarioIds = new List<int>();

            if (abvm != null && abvm.IdUsuario != null && abvm.IdUsuario.Length > 0)
            {
                var ids = abvm.IdUsuario;
                foreach (string s in ids)
                {
                    usuarioIds.Add(TFW.TFWConvert.ToInteger(s));
                }
            }

            CarregaUsuarios(usuarioIds);

            DateTime? DataInicio = TFW.TFWConvert.ToNullableDateTime(abvm.DataInicio) ?? new DateTime(1901, 1, 1);
            DateTime? DataFim = (TFW.TFWConvert.ToNullableDateTime(abvm.DataFim) ?? new DateTime(2100, 12, 31)).AddDays(1).AddSeconds(-1);

            var app = new AtividadeApplication(db);
            var attIdx = await app.GetAtividadeIndexAsync(usuarioIds, DataInicio, DataFim);

            Session["abvm"] = abvm;

            return View("DeleteMultiplo", attIdx);

        }


        /// <summary>
        /// get - visualizar lista de atividades a serem excluidas
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [ValidateAntiForgeryToken]
        [HttpPost, ActionName("ConfirmaDeleteMultiplo")]
        [TPADescricaoAcaoController("Visualizar Múltiplas Atividades para Exclusão", "Permite que o usuário exclua múltiplas atividades dos funcionários")]
        public async Task<ActionResult> ConfirmaDeleteMultiplo(int[] ids)
        {

            if (ids == null || ids.Length == 0)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Escolha pelo menos um registro!", TempData, ModelState);
                return RedirectToAction("DeleteMultiplo");
            }

            List<Atividade> atividades = await db.Atividades.Where(a => ids.Contains(a.Id)).ToListAsync();

            if (atividades == null)
            {
                return HttpNotFound();
            }


            return View(atividades);
        }



        /// <summary>
        /// post - confirmar exclusão múltipla
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteMultiploConfirmado")]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Confirmar Exclusão de Múltiplas Atividades", "Permite que o usuário exclua múltiplas atividades dos funcionários")]
        public async Task<ActionResult> DeleteMultiploConfirmado(int[] ids)
        {

            AtividadeApplication app = new AtividadeApplication(this.db);
            

            foreach (int i in ids)
            {
                await app.DeleteAsync(i, true);
            }

            MensagemParaUsuarioViewModel.MensagemSucesso("Registros Excluidos.", TempData);
            return RedirectToAction("DeleteMultiplo");
        }


        #endregion










        #region métodos privados

        private void CarregaTipos(int? valor = null)
        {

            var tipo = db.TiposAtividade.ToList();
            SelectList TipoSelectList = new SelectList(tipo, "Id", "Nome", valor);
            ViewBag.Tipos = TipoSelectList;
        }

        private void CarregaProjetos(int? valor = null)
        {
            var nodes = db.ProjectNodes.ToList();
            TFWHierarchicalList lst = new TFWHierarchicalList();

            foreach (var n in nodes)
            {
                lst.Add(n.Id, n.Pai != null ? n.Pai.Id : new Nullable<int>(), n.Nome);
            }

            //SelectList NodeSelectList = new SelectList(nodes, "Id", "Nome", valor);
            ViewBag.Nodes = lst;
        }

        private void CarregaUsuarios(int? valor = null)
        {

            var usuario = db.Usuarios.ToList();
            SelectList UsuarioSelectList = new SelectList(usuario, "Id", "FuncionarioNomeOuLogin", valor);
            ViewBag.Usuarios = UsuarioSelectList;
        }


        private void CarregaUsuarios(List<int> valores)
        {

            var usuario = db.Usuarios.ToList();
            MultiSelectList UsuarioSelectList = new MultiSelectList(usuario, "Id", "FuncionarioNomeOuLogin", valores);
            ViewBag.Usuarios = UsuarioSelectList;
        }


        #endregion


    }
}
