using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TPA.Domain.DomainModel;
using System.Threading.Tasks;
using TPA.Presentation.App_Start;
using TPA.Presentation.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using TPA.ViewModel;
using System.IdentityModel.Claims;
using System.Security.Claims;
using TPA.Application;
using AutoMapper;
using TPA.Services.Seguranca;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controller de account vindo do pacote nuget de Identity da microsoft
    /// </summary>
    [TPADescricaoAcaoController("Controle de contas de usuário", "Acessar o controle de contas de usuário")]
    public class AccountController : TPAController
    {

        #region propriedades privadas para o novo esquema de login
        /// <summary>
        /// gerenciador de login
        /// </summary>
        private ApplicationSignInManager _signInManager;
        /// <summary>
        /// gerenciador de usuários
        /// </summary>
        private ApplicationUserManager _userManager;

        #endregion



        #region constructors para  o novo esquema de login

        /// <summary>
        /// constructor parameterless
        /// </summary>
        public AccountController()
        {
        }

        /// <summary>
        /// constructor padrão com a injeção do UserManager e AccountManager
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #endregion



        #region propriedades públicas para o novo esquema de login

        /// <summary>
        /// gerenciador de login
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
        /// gerenciador de usuários
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



        #region métodos públicos para o novo sistema de login

        /// <summary>
        /// formulário de login
        /// </summary>
        /// <param name="returnUrl">string - url para retornar após o login</param>
        /// <returns></returns>
        [AllowAnonymous]
        [TPADescricaoAcaoController("Fazer login", "Permitir que o usuário faça login (essa ação deve ser pública, obviamente)")]
        public ActionResult Login(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || (returnUrl.ToLower().Contains("account/externallogin")))
                returnUrl = "/Home/Index/";

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">LoginViewModel - login e senha</param>
        /// <param name="returnUrl">string - url para retorno pós login</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [TPADescricaoAcaoController("Fazer login", "Permitir que o usuário faça login (essa ação deve ser pública, obviamente)")]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Isso não conta falhas de login em relação ao bloqueio de conta
            // Para permitir que falhas de senha acionem o bloqueio da conta, altere para shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Tentativa de login inválida.");
                    return View(model);
            }
        }



        /// <summary>
        /// login externo
        /// </summary>
        /// <param name="provider">tecnologia provedora do login</param>
        /// <param name="returnUrl">url para ser redirecionado depois do login</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [TPADescricaoAcaoController("Fazer login com serviços externos", "Permitir que o usuário faça login com serviços do azure ou da microsoft (essa ação deve ser pública, obviamente)")]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || (returnUrl.ToLower().Contains("account/externallogin")))
                returnUrl = "/Home/Index/";

            // Solicitar um redirecionamento para o provedor de logon externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        /// <summary>
        /// login externo
        /// </summary>
        /// <param name="returnUrl">url para redirecionar após login</param>
        /// <returns></returns>
        [HttpGet, ActionName("ExternalLogin")]
        [AllowAnonymous]
        [TPADescricaoAcaoController("Fazer login com serviços externos", "Permitir que o usuário faça login com serviços do azure ou da microsoft (essa ação deve ser pública, obviamente)")]
        public ActionResult ExternalLogin(string returnUrl)
        {
            if(User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrWhiteSpace(returnUrl) || (returnUrl.ToLower().Contains("account/externallogin")))
                    return RedirectToAction("Index", "Home");

                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        /// <summary>
        /// action de callback de login externo, para onde o serviço de login externo deve redirecionar
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [TPADescricaoAcaoController("Callback de login externo", "Permitir que o usuário faça login com serviços do azure ou da microsoft (essa ação deve ser pública, obviamente)")]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            bool autenticado = User.Identity.IsAuthenticated;

            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            string email = "";
            if (loginInfo.Login.LoginProvider == "Microsoft")
            {
                var identity = await AuthenticationManager.AuthenticateAsync(
                                    DefaultAuthenticationTypes.ExternalCookie);

                var emailClaim = identity.Identity.FindFirst(System.Security.Claims.ClaimTypes.Email);
                if ((emailClaim != null) && (!string.IsNullOrWhiteSpace(emailClaim.Value)))
                {
                    loginInfo.DefaultUserName = emailClaim.Value;
                    email = emailClaim.Value;
                    System.Diagnostics.Trace.Assert(email == loginInfo.Email, "O e-mail não é igual ao UserName");
                    System.Diagnostics.Debug.Assert(email == loginInfo.Email, "O e-mail não é igual ao UserName");
                }


            }
            else
            {
                var identity = await AuthenticationManager.AuthenticateAsync(
                    DefaultAuthenticationTypes.ExternalCookie);

                var emailClaim = identity.Identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");
                if ((emailClaim != null) && (!string.IsNullOrWhiteSpace(emailClaim.Value)))
                {
                    loginInfo.Email = emailClaim.Value;
                    email = emailClaim.Value;
                    System.Diagnostics.Trace.Assert(email == loginInfo.DefaultUserName, "O e-mail não é igual ao UserName");
                    System.Diagnostics.Debug.Assert(email == loginInfo.DefaultUserName, "O e-mail não é igual ao UserName");
                }


            }

            // Faça logon do usuário com este provedor de logon externo se o usuário já tiver um logon
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    // Se o usuário não tiver uma conta, solicite que o usuário crie uma conta
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return await ExternalLoginConfirmation(new ExternalLoginConfirmationViewModel { Email = email }, returnUrl);
            }
        }


        /// <summary>
        /// confirmação do login externo
        /// grava os dados do login externo no perfil do usuário, se for de um domínio / serviço que pode entrar
        /// </summary>
        /// <param name="model">ExternalLoginConfirmationViewModel - dados de login, e-mail</param>
        /// <param name="returnUrl">string - url de retorno pós login</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [TPADescricaoAcaoController("Confirmação de login externo", "Associa os dados de login externo com o perfil do usuário(essa ação deve ser pública, obviamente)")]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obter as informações sobre o usuário do provedor de logon externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                if ((string.IsNullOrWhiteSpace(model.Email)) || (model.Email.Split('@').Length < 2) || (model.Email.Split('@')[1].ToLower() != "tecnun.com.br"))
                {
                    TempData["Detalhes"] = "O login deve ser um e-mail @tecnun.com.br válido!";
                    return View("ExternalLoginFailure");
                }

                //procura pelo usuário
                ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    //caso não exista, primeiro criamos e...
                    user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        //depois adicionamos informação de login externo
                        result = await UserManager.AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded)
                        {
                            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                            return RedirectToLocal(returnUrl);
                        }
                    }
                    AddErrors(result);
                }
                else
                {
                    //caso já exista apenas adicionamos informação de login externo, acumulando
                    var result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                    AddErrors(result);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View("ExternalLoginConfirmation", model);
        }





        /// <summary>
        /// ação de logoff
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [TPADescricaoAcaoController("Desconectar-se", "permite o usuário fazer logoff (essa ação deve ser pública, obviamente)")]
        public ActionResult SignOut()
        {           
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }



        /// <summary>
        /// ação de logoff de serviço externo
        /// </summary>
        [TPADescricaoAcaoController("Desconectar-se de serviço externo", "permite o usuário fazer logoff de serviço externo (essa ação deve ser pública, obviamente)")]
        [AllowAnonymous]
        public void SignOutMicrosoft()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);
            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);

        }

        /// <summary>
        /// callback do logoff
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [TPADescricaoAcaoController("Callback do logoff", "permite o usuário fazer logoff (essa ação deve ser pública, obviamente)")]
        public ActionResult SignOutCallback()
        {
            if (!Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        /// <summary>
        /// action de falha de login
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [TPADescricaoAcaoController("Orientações de falha de login", "permite o usuário ver uma página com instruções sobre o que fazer caso não consiga logar-se (essa ação deve ser pública, obviamente)")]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }




        #endregion



        #region métodos protegisdos


        /// <summary>
        /// dispose padrão
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion



        #region métodos privados  Auxiliares para o novo esquema de login

        /// <summary>
        /// Usado para proteção XSRF ao adicionar logons externos
        /// </summary>
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }


                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion















        #region métodos públicos de negócio

        /// <summary>
        /// visualizar meus dados cadastrais
        /// </summary>
        /// <param name="id">int - id do usuário nas tabelas de negócio</param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Meus Dados", "Permitir que o usuário visualize seus prórpios dados cadastrais")]
        public ActionResult MeusDados(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Id deve ser informado");
            }

            Usuario usu =  db.Usuarios.Find(id);
            if (usu == null)
            {
                return HttpNotFound();
            }


            if(usu.Funcionario == null)
            {
                usu.Funcionario = new Funcionario() { Id = usu.Id};
                db.SaveChanges();                
            }

            return View(Mapper.Map<Usuario, MeusDadosViewModel>(usu));
        }


        /// <summary>
        /// alterar meus dados cadastrais
        /// </summary>
        /// <param name="usuVm">MeusDadosViewModel - dados que o usuário pode alterar (não pode haver Ativo ou outros dados gerenciais)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Meus Dados", "Permitir que o usuário altere seus prórpios dados cadastrais")]
        public async Task< ActionResult> MeusDados([Bind(Include = "Id, Nome, Celular, EmailProfissional")] MeusDadosViewModel usuVm)
        {
            if ((usuVm == null)||(usuVm.Id == 0))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Id deve ser informado");
            }

            if (ModelState.IsValid)
            {
                UsuarioApplication usuApp = new UsuarioApplication(db);
                Usuario usu = usuApp.GetById(usuVm.Id);

                if (usu == null)
                {
                    return HttpNotFound();
                }


                if (usuVm.EmailProfissional == usu.Login)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Este e-mail não pode ser igual ao seu login.", TempData, ModelState, "EmailProfissional");
                    return View(usuVm);
                }

                if (!string.IsNullOrWhiteSpace(usuVm.EmailProfissional))
                {
                    if (await usuApp.VerificaExistenciaEmailAsync(usuVm.EmailProfissional, usuVm.Id))
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Esse e-mail profissional já está em uso.", TempData, ModelState, "EmailProfissional");
                        return View(usuVm);
                    }
                }

                usuVm.Login = usu.Login;
                var teste = Mapper.Map<MeusDadosViewModel, Usuario>(usuVm, usu);

                await usuApp.SaveAsync(usu);
                MensagemParaUsuarioViewModel.MensagemSucesso("Registro Atualizado.", TempData);
            }

            return View(usuVm);
        }

        #endregion


    }
}