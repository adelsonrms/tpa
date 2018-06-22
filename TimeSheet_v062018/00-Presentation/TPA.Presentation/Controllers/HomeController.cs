using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IdentityModel.Claims;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TFW.Domain;
using TPA.Application;
using TPA.Domain.DomainModel;
using TPA.ViewModel;
using TPA.ViewModel.Buscas;
using TPA.Services.Seguranca;
using TPA.Presentation.Util;
using TPA.Framework;
using TPA.Infra.Services;
using AutoMapper;


namespace TPA.Presentation.Controllers
{
    /// <summary>
    /// controller para as páginas e ações que tratam das atividades do usuário logado
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Home", "Página inicial onde os usuários podem ver e lançar suas atividades")]
    public class HomeController : TPAController
    {


        #region métodos públicos / actions


        #region index e busca

        /// <summary>
        /// Início da página onde o usuário bate o ponto
        /// mostra a lista de atividades do último mês
        /// se houver uma busca na sessão, refaz a busca
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore  = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Entrar na home", "permite o usuário apenas entrar na home e visualizar a lista de atividades já lançadas e os cálculos das horas previstas e trabalhadas (se ele tiver permissão nas ações que preencham esses dados)")]
        public async Task<ActionResult> Index()
        {
            var dtini = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var dtfin = dtini.AddMonths(1).AddSeconds(-1);

            var abvmUsu = (Session != null ? Session["abvmUsu"] as AtividadeBuscaStringViewModel : new AtividadeBuscaStringViewModel()) ?? new AtividadeBuscaStringViewModel();
            if ((abvmUsu != null) && (!abvmUsu.IsBlank()))
                return await Busca(abvmUsu);

            if (this.User != null && this.User.Identity != null)
            {
                var usu = this.User.Identity.Name;
                var usuLogado = db.Usuarios.Where(u => u.Login == usu).FirstOrDefault();

                if (usuLogado != null)
                {
                    var app = new AtividadeApplication(db);
                    var attIdx = await app.GetAtividadeIndexAsync(usuLogado.Id, new Nullable<DateTime>(), new Nullable<DateTime>());

                    //as referencias mostradas no combo não devem refletir as trazidas na busca
                    var referencias = await app.GetReferenciasUltimoAnoAsync(usuLogado.Id);
                    CarregaReferencias(referencias, attIdx.Referencias.Count > 0 ? attIdx.Referencias.First().Id : new Nullable<int>());
                    return View(attIdx);
                }
            }

            MensagemParaUsuarioViewModel.MensagemErro("Usuário não autorizado.");
            return View(new AtividadeIndexViewModel());
        }

        /// <summary>
        /// busca avançada por vérios termos
        /// os termos de busca são agrupados em um viewmodel AtividadeBuscaStringViewModel
        /// </summary>
        /// <param name="abvmUsu"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, Location = System.Web.UI.OutputCacheLocation.None, VaryByParam = "*")]
        [TPADescricaoAcaoController("Buscas na home", "Permite que o usuário faça buscas")]
        public async Task<ActionResult> Busca(AtividadeBuscaStringViewModel abvmUsu)
        {
            if (abvmUsu == null || abvmUsu.IsBlank())
                abvmUsu = (Session["abvmUsu"] as AtividadeBuscaStringViewModel) ?? new AtividadeBuscaStringViewModel();


            var usu = this.User.Identity.Name;
            var usuLogado = db.Usuarios.Where(u => u.Login == usu).FirstOrDefault();



            if (usuLogado != null)
            {

                DateTime? DataInicio = TFW.TFWConvert.ToNullableDateTime(abvmUsu.DataInicio) ?? new DateTime(1901, 1, 1);
                DateTime? DataFim = (TFW.TFWConvert.ToNullableDateTime(abvmUsu.DataFim) ?? new DateTime(2100, 12, 31)).AddDays(1).AddSeconds(-1);

                var app = new AtividadeApplication(db);
                var attIdx = await app.GetAtividadeIndexAsync(usuLogado.Id, DataInicio, DataFim);


                //as referencias mostradas no combo não devem refletir as trazidas na busca
                var referencias = await app.GetReferenciasUltimoAnoAsync(usuLogado.Id);
                CarregaReferencias(referencias, attIdx.Referencias.Count > 0 ? attIdx.Referencias.First().Id : new Nullable<int>());

                Session["abvmUsu"] = abvmUsu;
                return View("Index", attIdx);
            }

            MensagemParaUsuarioViewModel.MensagemErro("Usuário não autorizado.");
            return View("Index", new AtividadeIndexViewModel());

        }



        #endregion



        #region métodos crud para um registro

        /// <summary>
        /// exclusão de atividade pelo método delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Delete), ActionName("Delete")]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Excluir atividade", "Permitir que o usuário exclua suas atividades, caso o período esteja aberto")]
        public async Task<ActionResult> Delete(int id)
        {
            Atividade atividade = await db.Atividades.FindAsync(id);
            if (atividade != null)
            {
                Referencia refe = atividade.Usuario.GetReferencia(atividade.Inicio.Year, atividade.Inicio.Month);
                if (!refe.Fechado)
                {
                    AtividadeApplication app = new AtividadeApplication(this.db);
                    await app.DeleteAsync(atividade, true);
                    return Json(true);
                }
            }

            return Json(false);
        }





        #endregion

        /// <summary>
        /// get da edição de atividades
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar atividade", "Permitir que o usuário altere a atividade, caso o período esteja aberto")]
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

            Referencia refe = atividade.Usuario.GetReferencia(atividade.Inicio.Year, atividade.Inicio.Month);
            if (refe.Fechado)
            {
                MensagemParaUsuarioViewModel.MensagemAlerta("Você não pode alterar uma atividade de um mês que já foi fechado.", TempData);
                return RedirectToAction("Index");
            }


            AtividadeViewModel atvVm = Mapper.Map<AtividadeViewModel>(atividade);

            CarregaTipos(atvVm.IdTipoAtividade);
            CarregaProjetos(atvVm.IdProjectNode);

            return View(atvVm);
        }



        /// <summary>
        /// post da edição de atividades
        /// </summary>
        /// <param name="atividadeVm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Editar atividade", "Permitir que o usuário altere a atividade, caso o período esteja aberto")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Observacao,Data,Inicio,Fim, IdProjectNode, IdTipoAtividade")] AtividadeViewModel atividadeVm)
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
                var usuario = db.Usuarios.Where<Usuario>(x => x.Login == User.Identity.Name).FirstOrDefault();

                if (usuario != null)
                {
                    try
                    {
                        var projeto = await db.ProjectNodes.FindAsync(atividadeVm.IdProjectNode);
                        var tipo = await db.TiposAtividade.FindAsync(atividadeVm.IdTipoAtividade);

                        Atividade domainAtv = db.Atividades.Find(atividadeVm.Id);
                        domainAtv.Observacao = atividadeVm.Observacao;
                        domainAtv.Inicio = atividadeVm.Data.AddHours(atividadeVm.Inicio.Hours).AddMinutes(atividadeVm.Inicio.Minutes);
                        domainAtv.Fim = atividadeVm.Data.AddHours(atividadeVm.Fim.Hours).AddMinutes(atividadeVm.Fim.Minutes);
                        domainAtv.ProjectNode = projeto;
                        domainAtv.TipoAtividade = tipo;
                        domainAtv.Usuario = usuario;


                        Referencia refe = usuario.GetReferencia(domainAtv.Inicio.Year, domainAtv.Inicio.Month);
                        if (!refe.Fechado)
                        {

                            AtividadeApplication app = new AtividadeApplication(this.db);
                            await app.SalvarAsync(domainAtv, false);
                            MensagemParaUsuarioViewModel.MensagemSucesso("Atividade salva.", TempData);
                            return RedirectToAction("Edit", atividadeVm);
                        }
                        else
                        {
                            MensagemParaUsuarioViewModel.MensagemAlerta("Você não pode alterar uma atividade de um mês que já foi fechado.", TempData);
                        }

                        
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
                else
                {
                    ModelState.AddModelError("Usuario", "Usuário logado não encontrado!.");
                    MensagemParaUsuarioViewModel.MensagemErro("Usuário logado não encontrado!.", TempData);
                }

            }

            CarregaTipos();
            CarregaProjetos();

            return View(atividadeVm);
        }


        /// <summary>
        /// get da view ajax onde se cria a lista de atividades para postar
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Criar atividades múltiplas", "Permitir que o usuário acesse a página de criação de múltiplas atividades (em ajax)")]
        public ActionResult CreateAjax()
        {
            return View();
        }


        /// <summary>
        /// get da partial view que adiciona uma linha de atividade (formulário em branco)
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Adicionar linha de atividade", "Permitir que o usuário adicione uma atividade na página de atividades múltiplas")]
        public ActionResult AdicionarLinhaDeAtividade()
        {
            CarregaTipos();
            CarregaProjetos();
            return PartialView("~/views/shared/EditorTemplates/AtividadeAjaxViewModel.cshtml", new AtividadeAjaxViewModel { Data = DateTime.Today });
        }


        /// <summary>
        /// post do método para criar atividade via ajax
        /// </summary>
        /// <param name="atividadeVm"></param>
        /// <returns>AtividadeAjaxViewModel - dados da atividade</returns>
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Salvar Atividade", "Permitir que o usuário salve as atividades criadas na página de atividades múltiplas")]
        public async Task<ActionResult> CreateAjax([Bind(Include = "Observacao,Data,Inicio,Fim, IdProjectNode, IdTipoAtividade")] AtividadeAjaxViewModel atividadeVm)
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
                var usuario = db.Usuarios.Where<Usuario>(x => x.Login == User.Identity.Name).FirstOrDefault();

                if (usuario != null)
                {
                    try
                    {
                        var projeto = await db.ProjectNodes.FindAsync(atividadeVm.IdProjectNode);
                        var tipo = await db.TiposAtividade.FindAsync(atividadeVm.IdTipoAtividade);


                        Atividade domainAtv = new Atividade
                        {
                            Observacao = atividadeVm.Observacao,
                            Inicio = atividadeVm.Data.AddHours(atividadeVm.Inicio.Hours).AddMinutes(atividadeVm.Inicio.Minutes),
                            Fim = atividadeVm.Data.AddHours(atividadeVm.Fim.Hours).AddMinutes(atividadeVm.Fim.Minutes),
                            ProjectNode = projeto,
                            TipoAtividade = tipo,
                            Usuario = usuario
                        };

                        Referencia refe = domainAtv.Usuario.GetReferencia(domainAtv.Inicio.Year, domainAtv.Inicio.Month);
                        if (!refe.Fechado)
                        {
                            AtividadeApplication app = new AtividadeApplication(this.db);
                            await app.SalvarAsync(domainAtv, false);
                            MensagemParaUsuarioViewModel.MensagemSucesso("Atividade salva.", TempData);
                            return PartialView("~/views/shared/DisplayTemplates/AtividadeAjaxViewModel.cshtml", domainAtv);
                        }
                        else
                        {
                            MensagemParaUsuarioViewModel.MensagemAlerta("Você não pode criar uma atividade para um mês que já foi fechado.", TempData);
                        }

                        
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
                else
                {
                    ModelState.AddModelError("Usuario", "Usuário logado não encontrado!.");
                    MensagemParaUsuarioViewModel.MensagemErro("Usuário logado não encontrado!.", TempData);
                }

            }

            CarregaTipos();
            CarregaProjetos();

            return PartialView("~/views/shared/EditorTemplates/AtividadeAjaxViewModel.cshtml", atividadeVm);
        }



        /// <summary>
        /// baseado em um id, carrega os dados do mês de referência no painel da home
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post), ActionName("CarregaReferencia")]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true,  Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Carregar Referência", "Permitir o carregamento e cálculo das horas previstas/trabalhadas no painel da home")]
        public async Task<JsonResult> CarregaReferencia(int id)
        {

            var referencia =  await db.Referencias.FindAsync(id);
            referencia.SincronizaAtividades(new CalendarioServices());
            var app = new AtividadeApplication(db);


            var atvidx =  await app.GetAtividadeIndexAsync(referencia.Id);

            var refvm = new
            {
                Id = referencia.Id,
                PrevistoDoMes = referencia.Previsto.BigTimeSpanToString(),
                PrevistoCorrente = referencia.PrevistoCorrente.BigTimeSpanToString(),
                RealizadoDoMes = referencia.Realizado.BigTimeSpanToString(),

                SaldoDoMes = referencia.SaldoDoMes.BigTimeSpanToString(),
                Saldo = referencia.Saldo.BigTimeSpanToString(),

                BancoDeHoras = referencia.BancoDeHoras.BigTimeSpanToString(),

                Ano = referencia.Ano.ToString(),
                Mes = referencia.Mes.ToString("D2"),

                Atividades = ControleAcesso.TemAcesso(SegurancaResources.EDITAR_ATIVIDADE) ? 
                
                    atvidx.Atividades.Select(x =>
                        new string[] {
                            x.ProjectNodeNome,
                            x.TipoAtividadeNome,
                            x.Observacao,
                            x.Inicio.ToString("dd/MM/yyyy"),
                            x.Inicio.ToString("HH:mm"),
                            x.Fim.ToString("HH:mm"),
                            ((TimeSpan)(x.Fim - x.Inicio)).TimeSpanToString(),
                            x.Inicio.ToString("yyyy-MM-dd"),
                            x.ReferenciaFechado?"":"<a href='"+ Url.Action("Edit", "Home", new { id = x.Id.ToString() }) +"'><span class='glyphicon glyphicon-edit'></span></a>",
                            x.ReferenciaFechado?"":"<span data-id='"+x.Id.ToString()+"' class='glyphicon glyphicon-remove delete-button' id='spdeletar"+x.Id.ToString()+"'></span>",

                        }
                    ).ToArray() : atvidx.Atividades.Select(x =>
                        new string[] {
                            x.ProjectNodeNome,
                            x.TipoAtividadeNome,
                            x.Observacao,
                            x.Inicio.ToString("dd/MM/yyyy"),
                            x.Inicio.ToString("HH:mm"),
                            x.Fim.ToString("HH:mm"),
                            ((TimeSpan)(x.Fim - x.Inicio)).TimeSpanToString(),
                            x.Inicio.ToString("yyyy-MM-dd")

                        }
                    ).ToArray(),

                ConsolidadoDiario = atvidx.ConsolidadoDiario.Select(x => 
                
                    new string[] {
                        x.Data.ToString("dd/MM/yyyy"),
                        x.Horas.TimeSpanToString(),
                        x.ClienteNome,
                        x.Data.ToString("yyyy-MM-dd") }

                ).ToArray()
            };

            return Json(refvm);
        }


        #endregion




        #region métodos privados

        private void CarregaReferencias(List<ReferenciaViewModel> referencias, int? idSelecionada)
        {
            
            var dados = referencias
                .OrderByDescending(x=>x.Ano)
                .ThenByDescending(x=>x.Mes)
                .Select(x => new {Id = x.Id, Text = x.Mes.ToString("D2") + "/"+x.Ano});
            SelectList ReferenciaSelectList = new SelectList(dados, "Id", "Text", idSelecionada);
            ViewBag.Referencias = ReferenciaSelectList;


        }

        private void CarregaTipos(int? valor = null)
        {

            ViewBag.Tipos = Colecoes.GetTiposPublicos();
        }

        private void CarregaProjetos(int? valor = null)
        {
            TFWHierarchicalList lstUsuario = new TFWHierarchicalList();
            TFWHierarchicalList lst= new TFWHierarchicalList();
            List<int> idsNodesUsuario = new List<int>();

            var usu = this.User.Identity.Name;
            var usuLogado = db.Usuarios.Where(u => u.Login == usu).FirstOrDefault();
            if (usuLogado != null)
            {
                var nodes = usuLogado.NosDoUsuario.ToList();
                if (nodes != null && nodes.Any())
                {
                    foreach (var n in nodes)
                    {
                        lstUsuario.Add(n.Id, n.Pai != null ? n.Pai.Id : new Nullable<int>(), n.Nome);
                    }
                }
            }

            idsNodesUsuario.AddRange(lstUsuario.Select(s => s.Id).ToList());

            foreach (var node in db.ProjectNodes.ToList())
            {
                if(idsNodesUsuario.Contains(node.Id))
                {
                    lst.Add(node.Id, node.Pai_Id != null ? node.Pai_Id : new Nullable<int>(), node.Nome);
                }
                else
                {
                    lst.Add(node.Id, node.Pai != null ? node.Pai.Id : new Nullable<int>(), node.Nome, false);
                }
            }

            ViewBag.Nodes = lst;


        }



        #endregion



    }
}