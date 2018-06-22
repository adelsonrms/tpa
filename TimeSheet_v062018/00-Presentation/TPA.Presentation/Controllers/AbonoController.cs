using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using TPA.Services.Seguranca;
using TPA.Application;
using TPA.Infra.Services;
using TPA.ViewModel;
using System.Threading.Tasks;
using TPA.Infra;
using TPA.Domain.DomainModel;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller para lançamentos especiais de abonos
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Controlar Abonos", "Permitir o controle de abonos e justificativas de ausências")]
    public class AbonoController : TPAController
    {

        #region métodos públicos

        /// <summary>
        /// página de abonos
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Acessar página de Abono", "Permitir o usuário acese a página de abonos, ausências e folgas remuneradas")]
        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// adicionar uma linha ao painel de abonos
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Adicionar Abono", "Permitir que o usuário abone ausências de outros funcionários")]
        public ActionResult AdicionarLinhaDeAbono()
        {
            return PartialView("~/views/shared/EditorTemplates/AbonoViewModel.cshtml", new AbonoViewModel {DataInicial = DateTime.Today, Horas = new TimeSpan(8,0,0)});
        }



        /// <summary>
        /// salvar uma linha de abono, para múltiplos usuários e em múltiplas datas
        /// </summary>
        /// <param name="avm"></param>
        /// <returns>AbonoViewModel - ViewModel com os dados de abono - lista de usuários, intervalo de datas, tipo de atividade e horas abonadas</returns>
        [HttpPost]
        [ValidateJsonAntiForgeryToken]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Salvar Abono", "Permitir que o usuário salve abonos para outros funcionários")]
        public async Task<ActionResult> Salvar(AbonoViewModel avm)
        {

            if(avm.IdsUsuarios == null || avm.IdsUsuarios.Count() == 0)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data inicial", TempData, ModelState, "IdsUsuarios");
            }



            if (avm.DataInicial == DateTime.MinValue)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data inicial", TempData, ModelState, "DataInicial");
            }

            if (avm.DataFinal < avm.DataInicial)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A data final deve ser maior ou igual a data inicial", TempData, ModelState, "DataFinal");
            }

            if(avm.DataFinal == null || avm.DataFinal.Value == DateTime.MinValue)
            {
                avm.DataFinal = avm.DataInicial;
            }

            if ((avm.DataFinal.Value - avm.DataInicial).TotalDays > 365)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não é permitido um lançamento em lote maior que um ano", TempData, ModelState, "DataFinal");
            }





            if (ModelState.IsValid)
            {



                CalendarioServices cal = new CalendarioServices();
                AtividadeApplication app = new AtividadeApplication(this.db);

                DateTime data = avm.DataInicial.Date;
                int IDADMIN = TFW.TFWConvert.ToInteger(ApplicationResources.PROJECTNODE_ADMINISTRATIVO_ID);

                while (data <= avm.DataFinal)
                {
                    try
                    {
                        if (cal.IsDiaUtil(data))
                        {
                            var projeto = db.ProjectNodes.Include(X=> X.Pai).Where(x => x.Id == IDADMIN).SingleOrDefault();
                            var tipo = db.TiposAtividade.Where(x => x.Id == avm.IdTipoAtividade).SingleOrDefault();


                            var usuarios = await db.Usuarios
                                .Include(x => x.Funcionario)                              
                                .Where(u => avm.IdsUsuarios.Contains(u.Id))
                                .ToListAsync();

                            string nomesDosUsuarios = string.Join(", ", usuarios.Select(u => u.FuncionarioNomeOuLogin).ToList<string>().ToArray());

                            foreach (var u in usuarios)
                            {

                                if (tipo.Nome == ApplicationResources.TIPOATIVIDADE_FERIAS_NOME)
                                {

                                    await app.SalvarAsync(new Atividade
                                    {
                                        Observacao = avm.Descricao,
                                        Inicio = data.AddHours(8),
                                        Fim = data.AddHours(12),
                                        ProjectNode = projeto,
                                        TipoAtividade = tipo,
                                        Usuario = u
                                    }, true);

                                    await app.SalvarAsync(new Atividade
                                    {
                                        Observacao = avm.Descricao,
                                        Inicio = data.AddHours(13),
                                        Fim = data.AddHours(17),
                                        ProjectNode = projeto,
                                        TipoAtividade = tipo,
                                        Usuario = u
                                    }, true);

                                }
                                else 
                                {
                                    await app.SalvarAsync(new Atividade
                                    {
                                        Observacao = avm.Descricao,
                                        Inicio = data,
                                        Fim = data.Add(avm.Horas.Value),
                                        ProjectNode = projeto,
                                        TipoAtividade = tipo,
                                        Usuario = u
                                    }, true);
                                }
                            }

                            avm.NomeTipoAtividade = tipo.Nome;
                            avm.NomeUsuario = nomesDosUsuarios;

                            
                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                        MensagemParaUsuarioViewModel.MensagemErro(exceptionMessage, TempData, ModelState);
                        return PartialView("~/views/shared/EditorTemplates/AbonoViewModel.cshtml", avm);
                    }
                    catch (Exception err)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                        return PartialView("~/views/shared/EditorTemplates/AbonoViewModel.cshtml", avm);
                    }
                    finally
                    {
                        data = data.AddDays(1);
                    }

                }

                MensagemParaUsuarioViewModel.MensagemSucesso("Atividades salvas.", TempData);
                return PartialView("~/views/shared/DisplayTemplates/AbonoViewModel.cshtml", avm);
            }



            return PartialView("~/views/shared/EditorTemplates/AbonoViewModel.cshtml", avm);
        }

        #endregion

    }
}