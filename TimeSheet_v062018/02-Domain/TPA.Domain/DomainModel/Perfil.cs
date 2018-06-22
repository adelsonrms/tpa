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
    /// um perfil ou role da aplicação
    /// </summary>
    public class Perfil : TPAEntity
    {


        #region propriedades públicas

        /// <summary>
        /// id do perfil
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// nome do perfil
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres")]
        public virtual string Nome { get; set; }

        /// <summary>
        /// ações que esse perfil permite
        /// </summary>
        public virtual ICollection<Acao> Acoes { get; set; }

        /// <summary>
        /// usuários que fazem parte desse perfil
        /// </summary>
        public virtual ICollection<Usuario> Usuarios { get; set; }

        #endregion



    }
}