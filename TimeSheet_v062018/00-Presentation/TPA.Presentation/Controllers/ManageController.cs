using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TPA.Presentation.App_Start;
using TPA.Presentation.Models;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller para gerenciamento de usuários
    /// </summary>
    //[TPAAuthorize]
    [TPADescricaoAcaoController("Gerenciar Usuários", "Permitir o gerenciamento de usuários")]
    public class ManageController : TPAController
    {


        #region campos privados

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        #endregion



        #region construtores

        /// <summary>
        /// constructor padrão
        /// </summary>
        public ManageController()
        {
        }

        /// <summary>
        /// constructor injetando UserManager e SignInManager
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #endregion



        #region propriedades públicas

        /// <summary>
        /// Gerenciador de Login
        /// </summary>
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        /// <summary>
        /// Gerenciador de Accounts
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion




        #region métodos públicos


        /// <summary>
        /// index da página de gerenciamento de logins
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [TPADescricaoAcaoController("Página de Gerenciamento de Logins", "Acessar página de gerenciamento de logins")]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Sua senha foi alterada."
                : message == ManageMessageId.SetPasswordSuccess ? "Sua senha foi definida."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Seu provedor de autenticação de dois fatores foi definido."
                : message == ManageMessageId.Error ? "Ocorreu um erro."
                : message == ManageMessageId.AddPhoneSuccess ? "Seu número de telefone foi adicionado."
                : message == ManageMessageId.RemovePhoneSuccess ? "Seu número de telefone foi removido."
                : "";

            //TODO: Meu UserID
            var userId = User.Identity.GetUserId(); //"c0bbc2f2-2659-49c5-89bc-9f82857427f2"; //User.Identity.GetUserId();
            var model = new ManageViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }


        /// <summary>
        /// remoção das informações de login externo
        /// desvincular o login externo
        /// </summary>
        /// <param name="loginProvider"></param>
        /// <param name="providerKey"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Remover Login", "Permitir ao usuário remover informações de login externo")]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }


        /// <summary>
        /// get da página de alterar a senha
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Alterar Senha", "Permitir ao usuário alterar a senha")]
        public ActionResult SetPassword()
        {
            return View();
        }




        /// <summary>
        /// post da página de alterar a senha
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Alterar Senha", "Permitir ao usuário alterar a senha")]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {

            var usrID = User.Identity.GetUserId();// "c0bbc2f2-2659-49c5-89bc-9f82857427f2";

            if (ModelState.IsValid)
            {
                if (await UserManager.HasPasswordAsync(usrID))
                {
                    var reset = await UserManager.RemovePasswordAsync(usrID);
                    if(!reset.Succeeded)
                    {
                        AddErrors(reset);
                    }
                }

                var result = await UserManager.AddPasswordAsync(usrID, model.NewPassword);
                
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(usrID);
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    MensagemParaUsuarioViewModel.MensagemSucesso("Registro Atualizado.", TempData);
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // Se chegamos até aqui, algo falhou, mostrar formulário novamente
            return View(model);
        }



        /// <summary>
        /// get da página de gerenciamento de logins
        /// vincular/desvincular logins externos
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Gerenciar Logins", "Permitir ao usuário gerenciar seus logins")]
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "O login externo foi removido."
                : message == ManageMessageId.Error ? "Ocorreu um erro."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();

            var tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"].ToString() ?? "";
            if (userLogins.Where(
                        x => x.LoginProvider.Contains(tenant)
             ).Any())
            {
                otherLogins.RemoveAll(x => x.AuthenticationType == "OpenIdConnect");
            }

            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }


        /// <summary>
        /// post da página de vincular logins
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Vincular Logins", "Permitir ao usuário vincular seus logins externos")]
        public ActionResult LinkLogin(string provider)
        {
            // Solicitar um redirecionamento para o provedor de login externo para vincular um login para o usuário atual
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }




        /// <summary>
        /// get da página pós vinculação de logins
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [TPADescricaoAcaoController("Mensagem pós vinculação Logins", "Página exibida ao usuário após vincular logins")]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (!result.Succeeded)
                TempData["Detalhes"] = string.Join(" ", result.Errors.ToArray());
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        #endregion



        #region métodos protegidos

        /// <summary>
        /// dispose padrão do controller
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }


        #endregion











        #region Auxiliadores
        // Usado para proteção XSRF ao adicionar logins externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion

    }
}