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
using System.IO;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// gerenciador de atestados
    /// </summary>
    [TPADescricaoAcaoController("Gerenciar Atestados", "Permite que o usuário adicione atestados e abonos")]
    public class AtestadoController : TPAController
    {


        #region métodos públicos

        /// <summary>
        /// index do controle de atestados
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Acessar página de atestados", "Permite que o usuário adicione atestados e abonos")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public async Task< ActionResult> Index()
        {
            var query = (from a in db.Atestados
                         from att in a.Atividades

                         group new { Atestado = a, Atividade = att } by new { Usuario = att.Usuario, Atestado = a } into g
                         select new AtestadoIndexViewModel
                         {
                             Id = g.Key.Atestado.Id,
                             NomeUsuario = g.Key.Usuario.Funcionario != null ? g.Key.Usuario.Funcionario.Nome : g.Key.Usuario.Login,
                             Observacao = g.Key.Atestado.Observacao,
                             QuantidadeDeAtividades = g.Count(),
                             DataInicial = g.Min(x => x.Atividade.Inicio),
                             DataFinal = g.Max(x => x.Atividade.Fim),

                         });


            return View(await query.ToListAsync<AtestadoIndexViewModel>());
        }


        /// <summary>
        /// get da ação adicionar um atestado
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Adicionar Atestados", "Permite que o usuário adicione atestados e abonos")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Adicionar()
        {
            return View(new AtestadoAnexoViewModel
            {
                DataInicial = DateTime.Today,
                Horas = TimeSpan.FromHours(8)
            });
        }


        /// <summary>
        /// post / salvar da ágina de gerenciamento de atestados
        /// </summary>
        /// <param name="avm"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Adicionar Atestados", "Permite que o usuário adicione atestados e abonos")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Adicionar(AtestadoAnexoViewModel avm)
        {
            if (avm.IdUsuario == 0)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data inicial", TempData, ModelState, "IdUsuario");
            }

            if (avm.Horas == null || avm.Horas == TimeSpan.Zero || avm.Horas.Value.TotalHours > 8)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Por favor preencha corretamente a quantidade de horas abonadas. Ela não pode ser maior que 8. ", TempData, ModelState, "Horas");
            }

            if (avm.DataInicial == DateTime.MinValue)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Favor preencher a data inicial", TempData, ModelState, "DataInicial");
            }

            if (avm.DataFinal < avm.DataInicial)
            {
                MensagemParaUsuarioViewModel.MensagemErro("A data final deve ser maior ou igual a data inicial", TempData, ModelState, "DataFinal");
            }

            if (avm.DataFinal == null || avm.DataFinal.Value == DateTime.MinValue)
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
                int IDATESTADO = TFW.TFWConvert.ToInteger(ApplicationResources.TIPOATIVIDADE_ATESTADO_ID);

                var projeto = db.ProjectNodes.Include(X => X.Pai).Where(x => x.Id == IDADMIN).SingleOrDefault();
                var tipo = db.TiposAtividade.Find(IDATESTADO);
                var usuario = await db.Usuarios.FindAsync(avm.IdUsuario);
                avm.NomeUsuario = usuario.FuncionarioNomeOuLogin;



                AtestadoAnexo atestado = new AtestadoAnexo();
                atestado.Atividades = new List<Atividade>();
                atestado.Observacao = avm.Observacao;
                byte[] fileData = null;
                using (var binaryReader = new BinaryReader(avm.ArquivoAnexo.InputStream))
                {
                    fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                }
                atestado.Arquivo = fileData;
                atestado.NomeArquivoOriginal = avm.ArquivoAnexo.FileName;
                db.Atestados.Add(atestado);


                while (data <= avm.DataFinal)
                {
                    try
                    {
                        if (cal.IsDiaUtil(data))
                        {

                            Atividade atv = new Atividade
                            {
                                Observacao = avm.Observacao,
                                Inicio = data,
                                Fim = data.Add(avm.Horas.Value),
                                ProjectNode = projeto,
                                TipoAtividade = tipo,
                                Usuario = usuario
                            };

                            await app.SalvarAsync(atv, true);
                            atestado.Atividades.Add(atv);

                        }
                    }
                    catch (DbEntityValidationException ex)
                    {
                        string exceptionMessage = LogServices.ConcatenaErrosDbEntityValidation(ex);
                        MensagemParaUsuarioViewModel.MensagemErro(exceptionMessage, TempData, ModelState);
                        return View(avm);
                    }
                    catch (Exception err)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                        return View( avm);
                    }
                    finally
                    {
                        data = data.AddDays(1);
                    }

                    await db.SaveChangesAsync();

                }

                MensagemParaUsuarioViewModel.MensagemSucesso("Atestado salvo e abonos lançados.", TempData);
                return View( avm);
            }



            return View( avm);
        }



        /// <summary>
        /// método get que leva a página  de exclusão de atestados
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Excluir Atestados", "Permite que o usuário exclua atestados e abonos")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para excluir", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                AtestadoAnexo atestado = await db.Atestados.FindAsync(id);
                if (atestado == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }

                var model = new AtestadoIndexViewModel
                {
                    Id = atestado.Id,
                    NomeUsuario = atestado.Atividades.FirstOrDefault().Usuario.FuncionarioNomeOuLogin,
                    Observacao = atestado.Observacao,
                    QuantidadeDeAtividades = atestado.Atividades.Count,
                    DataInicial = atestado.Atividades.Min(x => x.Inicio),
                    DataFinal = atestado.Atividades.Max(x => x.Fim),
                };

                return View(model);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }



        /// <summary>
        /// confirmação da exclusão de atestados
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Confirmar Exclusão de Atestados", "Permite que o usuário exclua atestados e abonos")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var deletando = await db.Atestados.FindAsync(id);
                if (deletando != null)
                {
                    List<Atividade> atts = new List<Atividade>();
                    atts.AddRange(deletando.Atividades);
                    foreach(Atividade att in atts)
                    {
                        deletando.Atividades.Remove(att);
                        db.Atividades.Remove(att);
                    }

                    deletando.Atividades.Clear();
                    db.Atestados.Remove(deletando);
                    await db.SaveChangesAsync();
                    MensagemParaUsuarioViewModel.MensagemSucesso("Registro Excluido.", TempData);
                }
                else
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Registro não encontrado.", TempData);
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

            return RedirectToAction("Index");
        }



        /// <summary>
        /// download de atestados
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Fazer download de um atestado", "Permite que o usuário faça download de um atestado")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Download(int id)
        {
            var arquivo = db.Atestados.Find(id);
            if (arquivo != null)
            {
                byte[] fileBytes = arquivo.Arquivo;
                string fileName = arquivo.NomeArquivoOriginal;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                MensagemParaUsuarioViewModel.MensagemErro("Registro não encontrado.", TempData);
                return RedirectToAction("Index");
            }
        }

        #endregion


    }
}