using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Extensions.Localization;

namespace TPA.ViewModel
{

    /// <summary>
    /// viewmodel para passar mensagens para o usuário na página
    /// </summary>
    public class MensagemParaUsuarioViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// título da mensagem
        /// </summary>
        public virtual string Titulo { get; set; }

        /// <summary>
        /// mensagem
        /// </summary>
        public virtual string Mensagem { get; set; }

        /// <summary>
        /// usado para retorno de objetos json
        /// true se houve sucesso no processamento,
        /// false caso contrário
        /// </summary>
        public virtual bool? Sucesso { get; set; }

        /// <summary>
        /// classe css para formatar a mensagem
        /// </summary>
        public virtual string Classe { get; set; }

        /// <summary>
        /// objeto json serializado para o retorno
        /// </summary>
        public virtual object Objeto { get; set; }

        /// <summary>
        /// string html para mostrar no retorno
        /// </summary>
        public virtual string Html { get; set; }

        #endregion




        #region métodos públicos estáticos

        /// <summary>
        /// usa o bootstrap para formatar uma mensagem de sucesso no _layout.chtml 
        /// </summary>
        /// <param name="mensagem">string - mensagem a ser exibida</param>
        /// <param name="tempData">TempDataDictionary - o tempdata da action ou da view</param>
        /// <returns>MensagemParaUsuarioViewModel - retorna esse objeto formatado, além de colocar a mensagem no contexto</returns>
        public static MensagemParaUsuarioViewModel MensagemSucesso(string mensagem, TempDataDictionary tempData = null)
        {
            var result = new MensagemParaUsuarioViewModel
            {
                Titulo = "Sucesso",
                Mensagem = mensagem,
                Classe = "alert alert-success",
                Sucesso = true
            };

            if (tempData != null)
            {
                tempData["MensagemUsuario"] = result;
            }


            return result;
        }


        /// <summary>
        /// usa o bootstrap para formatar uma mensagem de erro no _layout.chtml 
        /// </summary>
        /// <param name="mensagem">string - mensagem a ser exibida</param>
        /// <param name="tempData">TempDataDictionary - o tempdata da action ou da view</param>
        /// <param name="modelState">ModelStateDictionary - modelState vindo da action, para marcar onde estão os erros</param>
        /// <param name="ErrorKey">string - nome de uma propriedade para marcar em caso de erro</param>
        /// <returns>MensagemParaUsuarioViewModel - retorna esse objeto formatado, além de colocar a mensagem no contexto</returns>
        public static MensagemParaUsuarioViewModel MensagemErro(string mensagem, TempDataDictionary tempData = null, ModelStateDictionary modelState = null, string ErrorKey="Erro")
        {
            var result = new MensagemParaUsuarioViewModel
            {
                Titulo = "Erro",
                Mensagem = mensagem,
                Classe = "alert alert-danger",
                Sucesso = false
            };

            if (tempData != null)
            {
                tempData["MensagemUsuario"] = result;
            }

            if(modelState != null)
            {
                modelState.AddModelError( string.IsNullOrWhiteSpace(ErrorKey)? "Erro": ErrorKey, mensagem);
            }

            return result;
        }


        /// <summary>
        /// usa o bootstrap para formatar uma mensagem de alerta no _layout.chtml 
        /// </summary>
        /// <param name="mensagem">string - mensagem a ser exibida</param>
        /// <param name="tempData">TempDataDictionary - o tempdata da action ou da view</param>
        /// <returns>MensagemParaUsuarioViewModel - retorna esse objeto formatado, além de colocar a mensagem no contexto</returns>
        public static MensagemParaUsuarioViewModel MensagemAlerta(string mensagem, TempDataDictionary tempData = null)
        {
            var result = new MensagemParaUsuarioViewModel
            {
                Titulo = "Alerta",
                Mensagem = mensagem,
                Classe = "alert alert-warning",
                Sucesso = null
            };

            if (tempData != null)
            {
                tempData["MensagemUsuario"] = result;
            }

            return result;
        }


        /// <summary>
        /// usa o bootstrap para formatar uma mensagem de info no _layout.chtml 
        /// </summary>
        /// <param name="mensagem">string - mensagem a ser exibida</param>
        /// <param name="tempData">TempDataDictionary - o tempdata da action ou da view</param>
        /// <returns>MensagemParaUsuarioViewModel - retorna esse objeto formatado, além de colocar a mensagem no contexto</returns>
        public static MensagemParaUsuarioViewModel MensagemInfo(string mensagem, TempDataDictionary tempData = null)
        {
            var result = new MensagemParaUsuarioViewModel
            {
                Titulo = "Informação",
                Mensagem = mensagem,
                Classe = "alert alert-info",
                Sucesso = null
            };

            if (tempData != null)
            {
                tempData["MensagemUsuario"] = result;
            }

            return result;
        }

        #endregion



    }
}