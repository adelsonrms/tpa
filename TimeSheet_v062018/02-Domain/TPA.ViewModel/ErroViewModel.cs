using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Localization;

namespace TPA.ViewModel
{
    public class ErroViewModel
    {

        #region constantes

        public const string ERRO_400_MENSAGEM = "Erro 400 - Requisição inválida";
        public const string ERRO_403_MENSAGEM = "Erro 403 - Acesso Negado";
        public const string ERRO_404_MENSAGEM = "Erro 404 - A página não pode ser encontrada";
        public const string ERRO_414_MENSAGEM = "Erro 414 - Url Muito Longa";
        public const string ERRO_500_MENSAGEM = "Erro 500 - Erro interno do Servidor";
        public const string ERRO_503_MENSAGEM = "Erro 503 - Serviço temporariamente indisponível";


        public const string ERRO_400_DESCRICAO = "A sua requisição não pode ser atendida devido a um erro de sintaxe. Verifique se a URL está completa e não tem caracteres especiais.";
        public const string ERRO_403_DESCRICAO = "O seu perfil não lhe dá acesso a essa página";
        //public const string ERRO_404_DESCRICAO = "Existe um lugar mágico para onde vão os guarda-chuvas, palhetas e pés de meia rebeldes. A página que você procurava está lá, olhando por nós.";
        public const string ERRO_404_DESCRICAO = "A página que você tentou acessar não foi encontrada. Ela pode ter sido removida ou o link que você acessou pode estar errado. Se você acessou pelo favoritos, tente acessar timesheet.tecnun.com.br diretamente";
        public const string ERRO_414_DESCRICAO = "A URL é muito longa. Talvez o link esteja quebrado, ou é um erro de autenticação e redirecionamento. Limpe o cache com ctrl + shif + del e tente novamente.";
        public const string ERRO_500_DESCRICAO = "Um erro inesperado e não tratado ocorreu. Isso não é comun. O administrador do sistema já foi notificado e está trabalhando para corrigir o problema.";
        public const string ERRO_503_DESCRICAO = "Desculpem o inconveniente, o serviço está temporariamente indisponível devido a manutenção.";

        #endregion


        #region dicionários privados estáticos de mensagens de erro

        private static Dictionary<int, string> _mensagensErro = new Dictionary<int, string>()
        {
            {400, ERRO_400_MENSAGEM},
            {403, ERRO_403_MENSAGEM},
            {404, ERRO_404_MENSAGEM},
            {414, ERRO_414_MENSAGEM},
            {500, ERRO_500_MENSAGEM},
            {503, ERRO_503_MENSAGEM}
        };


        private static Dictionary<int, string> _descricoesErro = new Dictionary<int, string>()
        {
            {400, ERRO_400_DESCRICAO},
            {403, ERRO_403_DESCRICAO},
            {404, ERRO_404_DESCRICAO},
            {414, ERRO_414_DESCRICAO},
            {500, ERRO_500_DESCRICAO},
            {503, ERRO_503_DESCRICAO}
        };


        #endregion


        #region métodos estáticos privados

        private static string GetMessage(int statusCode)
        {
            if (ErroViewModel._mensagensErro.ContainsKey(statusCode))
                return ErroViewModel._mensagensErro[statusCode];
            else
                return ErroViewModel.ERRO_500_MENSAGEM;
        }

        private static string GetDescription(int statusCode)
        {
            if (ErroViewModel._descricoesErro.ContainsKey(statusCode))
                return ErroViewModel._descricoesErro[statusCode];
            else
                return ErroViewModel.ERRO_500_DESCRICAO;
        }

        #endregion







        #region factories


        /// <summary>
        /// cria um objeto ErroViewModel já formatado com o statuscode e exception, se houver
        /// </summary>
        /// <param name="statuscode">int - statuscode http</param>
        /// <param name="ex">Exception - erro causado na action, no processamento</param>
        /// <returns>ErroViewModel criado por esse método</returns>
        public static ErroViewModel Create(int statuscode, Exception ex = null)
        {

            return new ErroViewModel(statuscode, ex);

        }

        #endregion






        #region constructors


        /// <summary>
        /// constructor padrão que já inicializa o statuscode e exception
        /// </summary>
        /// <param name="statuscode">int - statuscode http</param>
        /// <param name="ex">Exception - erro causado na action, no processamento</param>
        public ErroViewModel(int statusCode, Exception ex = null)
        {
            this.Mensagem = ErroViewModel.GetMessage(statusCode);
            this.Descricao = ErroViewModel.GetDescription(statusCode);
            this.StatusCode = statusCode;

            if (ex != null)
            {
                this.UltimoErro = ex;
                if (statusCode == 500)
                {
                    this.Descricao = ex.Message;
                }
            }
        }



        #endregion



        #region public properties

        /// <summary>
        /// statuscode http que foi interceptado ou será exibido
        /// </summary>
        public virtual int StatusCode { get; set; }

        /// <summary>
        /// mensagem de erro
        /// </summary>
        public virtual string Mensagem { get; set; }

        /// <summary>
        /// descrição do erro e possível causa ou solução
        /// </summary>
        public virtual string Descricao { get; set; }

        /// <summary>
        /// usuário na action no momento do erro
        /// </summary>
        public virtual string Usuario { get; set; }

        /// <summary>
        /// perfil do usuário logado
        /// </summary>
        public virtual string Perfil { get; set; }

        /// <summary>
        /// último erro interceptado
        /// </summary>
        public virtual Exception UltimoErro { get; set; }

        /// <summary>
        /// nome da action
        /// </summary>
        public virtual string ActionName { get; set; }

        /// <summary>
        /// nome do controller
        /// </summary>
        public virtual string ControllerName { get; set; }

        #endregion


    }
}