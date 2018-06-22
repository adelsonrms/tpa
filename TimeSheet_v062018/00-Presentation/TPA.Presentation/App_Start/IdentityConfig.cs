using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Security.Claims;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;

namespace TPA.Presentation.App_Start
{


    // Configure o gerenciador de usuários do aplicativo usado nesse aplicativo. O UserManager está definido no ASP.NET Identity e é usado pelo aplicativo.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<TPAContext>()));
            // Configurar a lógica de validação para nomes de usuário
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configurar a lógica de validação para senhas
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configurar padrões de bloqueio de usuário
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Registre dois provedores de autenticação de fator. Este aplicativo usa Telefone e Emails como um passo para receber um código para verificar o usuário
            // Você pode gravar seu próprio provedor e conectá-lo aqui.
            manager.RegisterTwoFactorProvider("Código de telefone", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Seu código de segurança é {0}"
            });
            manager.RegisterTwoFactorProvider("Código de e-mail", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Código de segurança",
                BodyFormat = "Seu código de segurança é {0}"
            });

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure o gerenciador de login do aplicativo que é usado neste aplicativo.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }



        public override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool rememberMe, bool shouldLockout)
        {
            using (TPAContext ctx = new TPAContext())
            {
                var user = ctx.Usuarios.Where(u => u.Login == userName).SingleOrDefault();
                if(!user.Ativo)
                {
                    return Task.FromResult<SignInStatus>(SignInStatus.Failure);
                }

                return base.PasswordSignInAsync(userName, password, rememberMe, shouldLockout);
            }

        }
    }
}