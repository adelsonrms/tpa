using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TPA.Services.Seguranca;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// montagem de javascript dinâmico
    /// </summary>
    [AllowAnonymous]
    [TPADescricaoAcaoController("Javascript Dinâmico", "O acesso deve ser anônimo")]
    public class JsDinamicoController : Controller
    {

        #region métodos públicos

        /// <summary>
        /// view que contém javascript que é gerado automaticamente pelo razor com algumas variáveis vindas das regras de negócio
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Javascript Dinâmico", "O acesso deve ser anônimo")]
        public ActionResult Index()
        {
            return PartialView();
        }


        #endregion

    }
}