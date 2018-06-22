using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace TPA.ViewModel
{
    /// <summary>
    /// viewmodel para a página de usuários
    /// </summary>
    public class UsuarioViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// id do usuário
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Login do usuário, no caso da Tecnun é o e-mail
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(800, ErrorMessage = "O login deve ter no máximo 80 caracteres")]
        public virtual string Login { get; set; }

        /// <summary>
        /// define se o usuário aparece nas listas para envio de alerta de atividades atrasadas ou não
        /// </summary>
        [Display(Name = "Enviar e-mail de alerta de lançamentos atrasados")]
        public virtual bool EnviarAlertaLancamento { get; set; }

        /// <summary>
        /// define se o usuário está ativo
        /// se estiver inativo não deve se logar e nem aparecer em certas listas
        /// </summary>
        [Display(Name = "Usuário Ativo")]
        public virtual bool Ativo { get; set; }

        /// <summary>
        /// nome do usuário
        /// </summary>
        [Required]
        public virtual string Nome {get; set;}

        /// <summary>
        /// telefone celular do usuário
        /// </summary>
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\([0-9]{2}\))\s([0-9]{4})-([0-9]{4,5})$", ErrorMessage = "Telefone Inválido")]
        public virtual string Celular { get; set; }

        /// <summary>
        /// e-mail secundário do usuário no ambiente onde ele presta consultoria
        /// </summary>
        [EmailAddress]
        [Display(Name = "E-mail no ambiente do cliente")]
        public virtual string EmailProfissional { get; set; }

        #endregion

    }
}