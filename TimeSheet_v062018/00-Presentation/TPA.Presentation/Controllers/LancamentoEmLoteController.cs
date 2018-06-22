using System;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using TFW.Domain;
using TPA.Application;
using TPA.Domain.DomainModel;
using TPA.Infra.Services;
using TPA.ViewModel;
using TPA.Services.Seguranca;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controle de lançamento de atividades em lote
    /// </summary>
    [TPADescricaoAcaoController("Lançamento em Lote", "Permitir ao usuário fazer lançamentos em lote para vários funcionários")]
    public class LancamentoEmLoteController : TPAController
    {

        #region métodos públicos

        /// <summary>
        /// get da index de lançamentos em lote
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Página Lançamento em Lote", "Permitir ao usuário acessar a página de lançamento em lote")]
        public ActionResult Index()
        {
            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            return View("Lancar");
        }


        /// <summary>
        /// salvar múltiplas atividades com vários usuários e intervalo de datas
        /// </summary>
        /// <param name="lctos"></param>
        /// <returns>LancamentoEmLoteModel - dados dos múltiplos lançamentos (intervalo de datas)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Lançar em Lote", "Permitir ao usuário salvar lançamentos em lote com múltiplos funcionários e intervalo de datas")]
        public async Task<ActionResult> Lancar(LancamentoEmLoteModel lctos)
        {
            if(lctos.IdsUsuarios == null || lctos.IdsUsuarios.Count() == 0)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Escolha pelo menos um usuário", TempData, ModelState, "IdsUsuarios");
            }

            if (lctos.DataInicial == DateTime.MinValue)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data inicial", TempData, ModelState, "DataInicial");
            }

            if (lctos.DataFinal == DateTime.MinValue)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data final", TempData, ModelState, "DataFinal");
            }

            if (lctos.DataFinal < lctos.DataInicial)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A data final deve ser maior ou igual a data inicial", TempData, ModelState, "DataFinal");
            }


            if ((lctos.DataFinal - lctos.DataInicial).TotalDays > 365)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não é permitido um lançamento em lote maior que um ano", TempData, ModelState, "DataFinal");
            }


            if ((lctos.EntradaManha == null) || (lctos.EntradaManha <= TimeSpan.Zero) || (lctos.EntradaManha > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para a entrada da manhã entre 00:00 e 23:59", TempData, ModelState, "EntradaManha");
            }


            if ((lctos.SaidaManha == null) || (lctos.SaidaManha <= TimeSpan.Zero) || (lctos.SaidaManha > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para a saída da manhã entre 00:00 e 23:59", TempData, ModelState, "SaidaManha");
            }




            if ((lctos.EntradaTarde == null) || (lctos.EntradaTarde <= TimeSpan.Zero) || (lctos.EntradaTarde > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para a entrada da tarde entre 00:00 e 23:59", TempData, ModelState, "EntradaTarde");
            }


            if ((lctos.SaidaTarde == null) || (lctos.SaidaTarde <= TimeSpan.Zero) || (lctos.SaidaTarde > new TimeSpan(23, 59, 59)))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher uma hora válida para a saída da tarde entre 00:00 e 23:59", TempData, ModelState, "SaidaTarde");
            }


            if (lctos.EntradaManha > lctos.SaidaManha)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A hora de início não pode ser maior que a hora de término no primeiro horário", TempData, ModelState, "EntradaManha");
            }

            if (lctos.EntradaTarde > lctos.SaidaTarde)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A hora de início não pode ser maior que a hora de término no segundo horário", TempData, ModelState, "EntradaTarde");
            }


            if (lctos.EntradaManha > lctos.SaidaTarde)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A primeira hora de início não pode ser maior que última hora de término", TempData, ModelState, "EntradaManha");
            }

            if (lctos.SaidaManha > lctos.EntradaTarde)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A saída do primeiro horário não pode ser maior que a entrada do segundo horário", TempData, ModelState, "SaidaManha");
            }

            if (ModelState.IsValid)
            {



                CalendarioServices cal = new CalendarioServices();
                AtividadeApplication app = new AtividadeApplication(this.db);

                DateTime data = lctos.DataInicial;

                var projeto = await db.ProjectNodes.FindAsync(lctos.IdProjeto);
                var tipo = await db.TiposAtividade.FindAsync(lctos.IdTipoAtividade);
                var usuarios = await db.Usuarios.Where(u => lctos.IdsUsuarios.Contains(u.Id)).ToListAsync<Usuario>();

                while (data <= lctos.DataFinal)
                {
                    try
                    {
                        if (cal.IsDiaUtil(data))
                        {


                            foreach (Usuario usu in usuarios)
                            {

                                await app.SalvarAsync(new Atividade
                                {
                                    Observacao = lctos.Descricao,
                                    Inicio = data.AddHours(lctos.EntradaManha.Hours).AddMinutes(lctos.EntradaManha.Minutes),
                                    Fim = data.AddHours(lctos.SaidaManha.Hours).AddMinutes(lctos.SaidaManha.Minutes),
                                    ProjectNode = projeto,
                                    TipoAtividade = tipo,
                                    Usuario = usu
                                }, true);

                                await app.SalvarAsync(new Atividade
                                {
                                    Observacao = lctos.Descricao,
                                    Inicio = data.AddHours(lctos.EntradaTarde.Hours).AddMinutes(lctos.EntradaTarde.Minutes),
                                    Fim = data.AddHours(lctos.SaidaTarde.Hours).AddMinutes(lctos.SaidaTarde.Minutes),
                                    ProjectNode = projeto,
                                    TipoAtividade = tipo,
                                    Usuario = usu
                                }, true);

                            }

                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                        MensagemParaUsuarioViewModel.MensagemErro(exceptionMessage, TempData, ModelState);
                        return RedirectToAction("Index");
                    }
                    catch (Exception err)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                        return RedirectToAction("Index");
                    }
                    finally
                    {
                        data = data.AddDays(1);
                    }

                }

                MensagemParaUsuarioViewModel.MensagemSucesso("Atividades salvas.", TempData);
            }

            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            
            return View(lctos);
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

        #endregion

    }
}