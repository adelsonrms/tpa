using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;

namespace TPA.Domain.DomainModel
{

    /// <summary>
    /// Tipo de atividade
    /// </summary>
    public class TipoAtividade : TPAEntity
    {

        #region propriedades públicas 

        /// <summary>
        /// id do tipo de atividade
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// nome do tipo de atividade
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        [Display(Name = "Nome do Tipo de Atividade")]
        public virtual string Nome { get; set; }

        /// <summary>
        /// flag para definir se o tipo de atividade é administrativa ou não
        /// </summary>
        [Display(Name = "Tipo de Atividade Administrativa")]
        public virtual bool? Administrativo { get; set; }

        #endregion

    }
}