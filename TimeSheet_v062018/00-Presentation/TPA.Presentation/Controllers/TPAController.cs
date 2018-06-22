using System.Web.Mvc;
using TPA.Infra.Data;

namespace TPA.Presentation.Controllers
{


    /// <summary>
    /// classe ancestral de todos os controllers dessa aplicação
    /// </summary>
    public class TPAController : Controller
    {

        #region campos privados

        private readonly TPAContext _db = new TPAContext();

        #endregion



        #region propriedades protegidas

        /// <summary>
        /// context do EF 
        /// </summary>
        protected virtual TPAContext db {get {return _db;} }

        #endregion



        #region métodos públicos

        /// <summary>
        /// verifica se o request é ajax, e se for retorna condicionalmente uma partial view em vez da view
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ActionResult PartialViewIfAjax(string viewName, object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewName: viewName, model: model);
            }
            else
            {
                return View(viewName: viewName, model: model);
            }
        }


        /// <summary>
        /// verifica se o request é ajax, e se for retorna condicionalmente uma partial view em vez da view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual ActionResult PartialViewIfAjax(object model)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView( model: model);
            }
            else
            {
                return View(model: model);
            }
        }


        #endregion



        #region métodos protegidos

        /// <summary>
        /// dispose padrão
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        #endregion


    }
}