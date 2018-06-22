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
    /// representa um nome ou etiqueta para um label
    /// </summary>
    public class NodeLabel : TPAEntity
    {

        #region propriedades públicas

        /// <summary>
        /// id do nodelabel
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// o nome / título do label
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        public virtual string Nome { get; set; }

        #endregion


    }
}