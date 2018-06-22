using System;
using System.IdentityModel.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TPA.Presentation.App_Start;
using TPA.Presentation;
using TPA.Infra.Services;
using System.Web.SessionState;

namespace TPA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Session_Start() { }

        protected void Application_Start()
        {
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            AutoMapBindings.Config();

            HangfireBootstrapper.Instance.Start();
        }


        protected void Application_End(object sender, EventArgs e)
        {
            HangfireBootstrapper.Instance.Stop();
        }






        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false))
            {

                //redirecionar automárico para https
                //http://stackoverflow.com/questions/47089/best-way-in-asp-net-to-force-https-for-an-entire-site
                //https://docs.microsoft.com/pt-br/azure/app-service-web/web-sites-configure-ssl-certificate
                Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
           
            }
        }

        /// <summary>
        /// não deveria ser usado esse método caso você esteja tentando usar custom errors ou http errors, porque sempre vai cair aqui
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception err = Server.GetLastError();
            if (err != null)
            {
                LogServices.LogarException(err);
                if ((HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    HttpContext.Current.Session.Add("UltimoErro", err); 
                }
            }
        }
    }
}
