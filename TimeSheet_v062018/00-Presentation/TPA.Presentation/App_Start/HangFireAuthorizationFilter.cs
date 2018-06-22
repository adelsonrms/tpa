using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TPA.Services.Seguranca;

namespace TPA.Presentation.App_Start
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return ControleAcesso.TemAcesso(SegurancaResources.JOBS);
        }
    }
}