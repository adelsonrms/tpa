using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TPA.Infra.Services;

namespace TPA.Services
{
    /// <summary>
    /// Serviço criado para fazer requisições com uma certa frequência ao site 
    /// e não deixar o opp pool morrer caso o servidor seja gerenciado por terceiros (hospedagem)
    /// </summary>
    public static class KeepAliveService
    {
        /// <summary>
        /// faz uma requisição ao próprio site
        /// </summary>
        public static void Touch()
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(new Uri("https://timesheet.tecnun.com.br/"));

                //LogServices.Logar("touch");
            }
        }
    }
}
