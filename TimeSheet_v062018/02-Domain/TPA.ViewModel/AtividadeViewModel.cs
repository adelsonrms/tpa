using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;

namespace TPA.ViewModel
{

    /// <summary>
    /// ViewModel para actions do controller de atividade
    /// </summary>
    public class AtividadeViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// id da atividade
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// observação da atividade
        /// </summary>
        [Display(Name = "Descrição")]
        public  string Observacao { get; set; }

        /// <summary>
        /// id do projeto
        /// </summary>
        [Display(Name = "Projeto / Fase")]
        [Required]
        public virtual int IdProjectNode { get; set; }

        /// <summary>
        /// id do tipo de atividade
        /// </summary>
        [Display(Name = "Tipo de Atividade")]
        [Required]
        public virtual int IdTipoAtividade { get; set; }

        /// <summary>
        /// id do usuário
        /// </summary>
        [Display(Name = "Usuário")]
        [Required]
        public virtual int IdUsuario { get; set; }

        /// <summary>
        /// data da atividade
        /// </summary>
        [Required]
        [Display(Name = "Data")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [DataType(DataType.Date, ErrorMessage = "Data em formato inválido")]
        public DateTime Data { get; set; }

        /// <summary>
        /// horário de início
        /// </summary>
        [Required]
        [Display(Name = "Hora de Início")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Inicio { get; set; }


        /// <summary>
        /// horário de fim
        /// </summary>
        [Required]
        [Display(Name = "Hora de Término")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Fim { get; set; }

        #endregion


    }


}