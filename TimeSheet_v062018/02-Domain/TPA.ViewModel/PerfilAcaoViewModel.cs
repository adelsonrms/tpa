using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TPA.Domain.DomainModel;
using Microsoft.Extensions.Localization;

namespace TPA.ViewModel
{
    /// <summary>
    /// ViewModel para associar perfil com ação
    /// </summary>
    public class PerfilAcaoViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// Ação (para display no retorno)
        /// </summary>
        public virtual Acao Acao { get; set; }

        /// <summary>
        /// Perfil (para display no retorno)
        /// </summary>
        public virtual Perfil Perfil { get; set; }

        /// <summary>
        /// Id da ação
        /// </summary>
        [Required]
        [Display(Name = "Ação")]
        public virtual int IdAcao { get; set; }

        /// <summary>
        /// id do perfil
        /// </summary>
        [Required]
        [Display(Name = "Perfil")]
        public virtual int IdPerfil { get; set; }

        #endregion


    }
}