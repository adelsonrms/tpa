using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TFW.Domain;
using TPA.Application;
using TPA.Infra.Services;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{


    /// <summary>
    /// controller para importação de planilha de atividades no formato disponível para download
    /// </summary>
    [TPADescricaoAcaoController("Importação de Planilha de Atividades de Múltiplos Usuários", "Permitir a importação de planilha administrativa com atividades de múltiplos usuários")]
    public class ImportacaoPlanilhaAdminController : TPAController
    {



        #region métodos públicos

        /// <summary>
        /// get da página de importação
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Acessar página de importação de múltiplos usuários", "Permitir o acesso a página de importação de planilha administrativa com atividades de múltiplos usuários")]
        public ActionResult Index()
        {

            return View();
        }



        /// <summary>
        /// post com o upload da planilha
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Upload de planilha de múltiplos usuários", "Permitir o upload da planilha")]
        public ActionResult Index(FormCollection formCollection)
        {
            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName) && (Path.GetExtension(file.FileName).ToUpper() == ".XLSX"))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;


                    using (var package = new ExcelPackage(file.InputStream))
                    {

                        try
                        {
                            ImportaPlanilhaApplication app = new ImportaPlanilhaApplication(this.db);
                            var planilha = app.TransformarPlanilhaAdmin(package);
                            return View("LancaPlanilha", planilha);
                        }
                        catch (Exception err)
                        {
                            MensagemParaUsuarioViewModel.MensagemErro(err.ToString(), TempData, ModelState);
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
                MensagemParaUsuarioViewModel.MensagemErro("Ocorreu algum erro e o request está nulo", TempData, ModelState);
            }



            return View("Index");
        }


        /// <summary>
        /// get da página com os dados da planilha para edição
        /// </summary>
        /// <param name="planmodel"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Lançar Planilha", "Permitir Edição dos dados da planilha Administrativa")]
        public ActionResult LancaPlanilha(ImportacaoPlanilhaAtividadesViewModel planmodel)
        {
            CarregaTipos();
            CarregaProjetos();
            CarregaUsuarios();

            return View(planmodel);
        }



        /// <summary>
        /// post do lançamento dos dados alterados
        /// </summary>
        /// <param name="planmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("LancaPlanilha")]
        [TPADescricaoAcaoController("Enviar Atividades Editadas", "Permitir a postagem dos dados da planilha Administrativa")]
        public async Task< ActionResult> LancaPlanilhaPost(ImportacaoPlanilhaAtividadesViewModel planmodel)
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

            ViewBag.Usuarios = db.Usuarios.AsNoTracking().ToList();
        }

        #endregion
    }
}