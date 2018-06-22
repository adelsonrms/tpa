using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using TPA.Presentation.App_Start;
using TPA.Infra.Data;
using TPA.Domain.DomainModel;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.MicrosoftAccount;
using System.IdentityModel.Claims;

namespace TPA.Presentation
{

    public partial class Startup
    {
        // The Client ID is used by the application to uniquely identify itself to Azure AD.
        string clientId = System.Configuration.ConfigurationManager.AppSettings["ClientId"];

        // o clientSecret
        string clientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];

        // RedirectUri is the URL where the user will be redirected to after they sign in.
        string redirectUri = System.Configuration.ConfigurationManager.AppSettings["RedirectUri"];
        private static string postLogoutRedirectUri = System.Configuration.ConfigurationManager.AppSettings["PostLogoutRedirectUri"];

        // Tenant is the tenant ID (e.g. contoso.onmicrosoft.com, or 'common' for multi-tenant)
        static string tenant = System.Configuration.ConfigurationManager.AppSettings["Tenant"];

        // Authority is the URL for authority, composed by Azure Active Directory v2 endpoint and the tenant name (e.g. https://login.microsoftonline.com/contoso.onmicrosoft.com/v2.0)
        string authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["Authority"], tenant);

        /// <summary>
        /// Handle failed authentication requests by redirecting the user to the home page with an error in the query string
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }

        // Para obter mais informações sobre a autenticação de configuração, visite https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            //app.UseKentorOwinCookieSaver();

            // Configure o contexto db, gerenciador de usuários e gerenciador de login para usar uma única instância por solicitação
            app.CreatePerOwinContext(TPAContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);


            // Habilitar o aplicativo a usar um cookie para armazenar informações do usuário logado
            // e para usar um cookie para armazenar temporariamente informações sobre um usuário fazendo logon com um provedor de logon de terceiros
            // Configurar o cookie de logon
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {

               // CookieSecure = CookieSecureOption.Always,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new Microsoft.Owin.PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Permite que o aplicativo valide o carimbo de segurança quando o usuário efetuar login.
                    // Este é um recurso de segurança que é usado quando você altera uma senha ou adiciona um login externo à sua conta.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseMicrosoftAccountAuthentication(new MicrosoftAccountAuthenticationOptions()
            {
                ClientId = "b4c120e5-0c75-4e51-9cb9-b87d0e8b093c",
                ClientSecret = "kkwcaHFV481!#kjXQVA42~)",
                Provider = new MicrosoftAccountAuthenticationProvider()
                {
                    OnAuthenticated = context =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:microsoftaccount:access_token", context.AccessToken, "Microsoft"));
                        context.Identity.AddClaim(new System.Security.Claims.Claim("urn:microsoft:email", context.Email));
                        return Task.FromResult(true);
                    }
                }
            });


            //segundo:
            //https://stackoverflow.com/questions/38517518/how-to-use-both-internal-forms-authenticationas-well-as-azure-ad-authentication
            //https://stackoverflow.com/questions/26517925/redirect-user-to-custom-login-page-when-using-azure-ad

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Authority = authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,
                    RedirectUri = redirectUri,
                    AuthenticationMode = AuthenticationMode.Passive,
                    
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        RedirectToIdentityProvider =  (context) =>
                        {
                            context.ProtocolMessage.DomainHint = "tecnun.com.br";
                            return  Task.FromResult(0);
                        },

                        AuthenticationFailed = (context) =>
                        {
                            if (context.Exception.Message.StartsWith("OICE_20004") || context.Exception.Message.Contains("IDX10311"))
                            {
                                context.SkipToNextMiddleware();
                                return Task.FromResult(0);
                            }

                            context.HandleResponse();
                            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
                            return Task.FromResult(0);
                        }
                    }
                });

        }
    }
}
