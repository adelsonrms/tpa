using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TPA.Services.Seguranca;

namespace TPA.Presentation.Controllers
{
    /// <summary>
    /// testes de páginas de erros
    /// </summary>
    [AllowAnonymous]
    [TPADescricaoAcaoController("Testes", "Páginas de testes")]
    public class TestesController : Controller
    {


        #region métodos públicos

        /// <summary>
        /// get - lista de testes de páginas de erro
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar Testes", "Páginas de testes")]
        public ActionResult Index()
        {
            return View();
        }





        /// <summary>
        /// get - divisão por zero
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Divisão por Zero", "Páginas de testes")]
        public ActionResult DivZero()
        {
            int x = 1;
            int y = 0;
            int z = x / y;

            return View("About");
        }



        /// <summary>
        /// get - página proibida com statuscoderesult
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Proibido 1", "Páginas de testes")]
        public ActionResult Proibido1()
        {
            return new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden, "Você é persona não grata");
        }



        /// <summary>
        /// get - página proibida com http exception
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Proibido 2", "Páginas de testes")]
        public ActionResult Proibido2()
        {
            throw new HttpException((int)System.Net.HttpStatusCode.Forbidden, "Você é persona não grata");
        }


        /// <summary>
        /// get - teste de erro 404 com notfoundresult
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Not Found 1", "Páginas de testes")]
        public ActionResult NotFound1()
        {
            return HttpNotFound("not found 1");
        }



        /// <summary>
        /// get - teste de erro 404 com notfoundresult
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Not Found 2", "Páginas de testes")]
        public ActionResult NotFound2()
        {
            return new HttpNotFoundResult("not found 2");
        }



        /// <summary>
        /// get - teste de erro 404 com exception
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Not Found 3", "Páginas de testes")]
        public ActionResult NotFound3()
        {
            throw new HttpException((int)System.Net.HttpStatusCode.NotFound, "not found 3");
        }



        /// <summary>
        /// get - teste de bad request
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Bad Request 1", "Páginas de testes")]
        public ActionResult BadRequest1()
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "no cookie for you");
        }



        /// <summary>
        /// get - teste de bad request com exception
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Bad Request 2", "Páginas de testes")]
        public ActionResult BadRequest2()
        {
            throw new HttpException((int)System.Net.HttpStatusCode.BadRequest, "no cookie for you");
        }

        #endregion



    }
}