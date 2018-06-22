using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TFW;
using TFW.Domain;
using TPA.Application;
using TPA.Infra.Services;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller para importação da palnilha do usuário
    /// </summary>
    [TPADescricaoAcaoController("Importação da Planilha do Usuário", "Permitir que o usuário importe uma planilha com seu timesheet")]
    public class ImportacaoPlanilhaController : TPAController
    {



        #region métodos públicos

        /// <summary>
        /// get da página da importação de timesheet com as atividades do usuário em excel
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Página de Importação", "Acessar a página de upload do timesheet em excel")]
        public ActionResult Index()
        {
            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            return View();
        }



        /// <summary>
        /// post da página da importação de timesheet com as atividades do usuário em excel
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Página de Importação", "Acessar a página de upload do timesheet em excel")]
        public ActionResult Index(FormCollection formCollection)
        {

            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();


            if (Request != null)
            {
                if (formCollection["IdUsuario"] != null)
                {
                    int IdUsuario = TFWConvert.ToInteger(formCollection["IdUsuario"]);
                    if (IdUsuario > 0)
                    {
                        HttpPostedFileBase file = Request.Files["UploadedFile"];
                        if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName) && (Path.GetExtension(file.FileName).ToUpper()==".XLSX"))
                        {
                            string fileName = file.FileName;
                            string fileContentType = file.ContentType;

                            //byte[] fileBytes = new byte[file.ContentLength];
                            //var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                            using (var package = new ExcelPackage(file.InputStream))
                            {

                                try
                                {
                                    ImportaPlanilhaApplication app = new ImportaPlanilhaApplication(this.db);
                                    var planilha = app.TransformarPlanilhaUsuario(IdUsuario, package);
                                    return View("LancaPlanilha", planilha);
                                }
                                catch(Exception err)
                                {
                                    MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                                    LogServices.LogarException(err);
                                }
                            }
                        }
                        else
                        {
                            MensagemParaUsuarioViewModel.MensagemErro("Faça o upload de uma planilha de horários em excel .xlsx no padrão da Tecnun.", TempData, ModelState);
                        }
                    }
                    else
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Selecione um usuário válido", TempData, ModelState);
                    }
                }
                else
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Não foi encontrado o Id do Usuário", TempData, ModelState);
                }
            }
            else
            {
                MensagemParaUsuarioViewModel.MensagemErro("Ocorreu algum erro e o request está nulo", TempData, ModelState);
            }



            return View("Index");
        }


        /// <summary>
        /// visualizar os dados da planilha para alterar antes de salvar
        /// </summary>
        /// <param name="planmodel"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Visualizar dados da Planilha", "Visualizar os dados da planilha para alterar antes de salvar")]
        public ActionResult LancaPlanilha(ImportacaoPlanilhaAtividadesUsuarioViewModel planmodel)
        {
            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            return View(planmodel);
        }



        /// <summary>
        /// salvar os dados da planilha depois de editar
        /// </summary>
        /// <param name="planmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("LancaPlanilha")]
        [TPADescricaoAcaoController("Salvar dados da Planilha", "Salvar os dados da planilha")]
        public async Task< ActionResult> LancaPlanilhaPost(ImportacaoPlanilhaAtividadesUsuarioViewModel planmodel)
        {
            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            if (ModelState.IsValid && planmodel != null && planmodel.Itens.Count > 0)
            {
                try
                {
                    ImportaPlanilhaApplication app = new ImportaPlanilhaApplication(this.db);
                    int linhas = await app.LancarAsync(planmodel);
                    MensagemParaUsuarioViewModel.MensagemSucesso(string.Format("Planilha importada com sucesso! {0} linhas da planilha importadas.", linhas), TempData);
                    return View("Index");
                }
                catch(Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                    LogServices.LogarException(err);
                }
            }




            return View(planmodel);
        }


        #endregion








        #region métodos privados

        private void CarregaTipos(int? valor = null)
        {

            var tipo = db.TiposAtividade.AsNoTracking().ToList();
            SelectList TipoSelectList = new SelectList(tipo, "Id", "Nome");
            ViewBag.Tipos = TipoSelectList;
        }

        private void CarregaProjetos(int? valor = null)
        {
            var nodes = db.ProjectNodes.AsNoTracking().ToList();
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

            var usuario = db.Usuarios.AsNoTracking().ToList();
            SelectList UsuarioSelectList = new SelectList(usuario, "Id", "FuncionarioNomeOuLogin", valor);
            ViewBag.Usuarios = UsuarioSelectList;
        }

        #endregion

    }
}