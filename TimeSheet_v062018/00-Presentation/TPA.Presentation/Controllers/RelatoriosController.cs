using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TPA.Application;
using TPA.Infra.Data;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{


    /// <summary>
    /// controller de relatórios
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Relatórios", "Permitir que o usuário acese os relatórios")]
    public class RelatoriosController : TPAController
    {


        #region métodos públicos

        /// <summary>
        /// get - acessar a página de relatórios
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Baixar Relatórios", "Permitir que o usuário baixe os relatórios")]
        public ActionResult Index()
        {
            return View();
        }



        /// <summary>
        /// post - escolher datas e baixar relatórios
        /// </summary>
        /// <param name="dtIni">DateTime - data inicial</param>
        /// <param name="dtFin">DateTime - data final</param>
        /// <returns>Planilha Excel com todos os relatórios</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Baixar Relatórios", "Permitir que o usuário baixe os relatórios")]
        public FileResult Index(DateTime dtIni, DateTime dtFin)
        {
            dtFin = dtFin.AddDays(1).AddSeconds(-1);

            string caminho = Server.MapPath("~/doc/MODELO_RELATORIO.xlsx");

            using (var db = new TPAContext())
            {
                RelatorioApplication app = new RelatorioApplication(db);
                string saida = app.GeraRelatorioPadrao(dtIni, dtFin, caminho);
                var bytes = System.IO.File.ReadAllBytes(saida);

                try
                {
                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Relatorio.xlsx");
                }
                finally
                {
                    if(!string.IsNullOrWhiteSpace(saida) && (System.IO.File.Exists(saida)))
                    {

                            System.IO.File.Delete(saida);

                    }
                }

            }
        }

        #endregion



    }
}