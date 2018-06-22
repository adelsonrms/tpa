using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;
using TPA.Presentation.Util;

namespace TPA.ViewModel.Buscas
{

    /// <summary>
    /// classe para encapsular os parâmetros de busca de atividade da queryString
    /// </summary>
    public class AtividadeBuscaStringViewModel 
    {

        #region propriedades públicas 

        /// <summary>
        /// vetor com os ids dos usuários a serem pesquisados. Se for null traz todos
        /// </summary>
        public virtual string[] IdUsuario { get; set; }

        /// <summary>
        /// data de início do intervalo de busca em forma de string
        /// </summary>
        public virtual string DataInicio { get; set; }

        /// <summary>
        /// data de final do intervalo de busca em forma de string
        /// </summary>
        public virtual string DataFim { get; set; }

        #endregion


        #region métodos públicos

        /// <summary>
        /// verifica se os parâmetros não estão preenchidos
        /// </summary>
        /// <returns></returns>
        public bool IsBlank()
        {
            return ((this.DataFim == null) && (this.DataInicio == null) && (this.IdUsuario == null || this.IdUsuario.Length == 0));
        }

        #endregion


    }
}