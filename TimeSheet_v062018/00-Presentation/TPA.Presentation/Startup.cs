using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using TPA.Infra.Services;
using TPA.Application;
using TPA.Infra.Data;
using TPA.Services.Seguranca;
using Hangfire.Dashboard;
using Hangfire.Annotations;
using System.Web;
using TPA.Presentation.App_Start;
using TPA.Services;

namespace TPA.Presentation
{
    public partial class Startup
    {


        public void Configuration(IAppBuilder app)
        {

            
            ConfigureAuth(app);
            HangfireBootstrapper.Instance.Start();

            app.UseHangfireServer();
            app.UseHangfireDashboard("/jobs", new DashboardOptions()
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });

            //testa o serviço de agendamento
            //RecurringJob.AddOrUpdate(() => UsuarioApplication.LogServicoDiario(), "0 20 * * *", TimeZoneInfo.Local);
            //manda mensagens de alerta aos usuarios com atraso no envio
            //RecurringJob.AddOrUpdate(() => UsuarioApplication.EnviaAlertasAgendados(), "0 22 * * *", TimeZoneInfo.Local);
            //sincroniza os novos controllers e actions com a tabela de ações para montagem de perfis
            //RecurringJob.AddOrUpdate(() => AcaoServices.AtualizaAcoesCopiandoDoAssembly(), Cron.Daily, TimeZoneInfo.Local);


            //faz uma requisição ao próprio site a cada 15 minutos para fazer o KeepAlive (não deixar ele dar shutdown por falta de requests)
            //RecurringJob.AddOrUpdate(() => KeepAliveService.Touch(), "*/15 * * * *", TimeZoneInfo.Local);
        }
    }
}
