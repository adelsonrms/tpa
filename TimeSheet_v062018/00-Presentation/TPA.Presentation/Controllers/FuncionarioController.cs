using AutoMapper;
using Microsoft.AspNet.Identity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TPA.Domain.DomainModel;
using TPA.Presentation.Models;
using TPA.Services.Seguranca;
using TPA.ViewModel;
using static TPA.Presentation.Controllers.ManageController;

namespace TPA.Presentation.Controllers
{
    [TPAAuthorize]
    [TPADescricaoAcaoController("Funcionarios", "Gerencia informações sobre os funcionarios")]
    public class FuncionarioController : TPAController
    {
        // GET: Funcionario
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// index da página de gerenciamento de logins
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Página de Gerenciamento de Logins", "Acessar página de gerenciamento de logins")]
        public ActionResult FichaCadastral(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Id deve ser informado");
            }
            Funcionario funcionario = db.Funcionarios.Find(id);

            if (funcionario == null)
            {
                return HttpNotFound();
            }
            return View(Mapper.Map<Funcionario, FichaCadastralViewModel>(funcionario));
        }

    }
}