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
    /// viewmodel para as páginas que associam um usuário a uma role
    /// </summary>
    public class UsuarioRoleViewModel
    {
        #region propriedades públicas


        /// <summary>
        /// objeto do usuário (para as exibições de sucesso/erro)
        /// </summary>
        public virtual Usuario Usuario { get; set; }

        /// <summary>
        /// objeto do perfil/role (para as exibições de sucesso/erro)
        /// </summary>
        public virtual Perfil Perfil { get; set; }

        /// <summary>
        /// id do usuário
        /// </summary>
        [Required]
        [Display(Name = "Usuário")]
        public virtual int IdUsuario { get; set; }

        /// <summary>
        /// id do perfil
        /// </summary>
        [Required]
        [Display(Name = "Perfil")]
        public virtual int IdPerfil { get; set; }

        #endregion
    }
}