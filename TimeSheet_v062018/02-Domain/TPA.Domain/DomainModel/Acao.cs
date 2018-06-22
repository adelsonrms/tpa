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
    /// simboliza uma ação de usuário, que pode ser uma action do controller ou uma permissão personalizada baseada em alguma resource string de segurança
    /// </summary>
    public class Acao : TPAEntity
    {

        #region propriedades públicas

        /// <summary>
        /// Id da ação
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]        
        public override int Id { get; set; }

        /// <summary>
        /// Nome da Ação
        /// </summary>
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(250, ErrorMessage = "O nome deve ter no máximo 250 caracteres")]
        [Display(Name = "Nome")]
        public virtual string Nome { get; set; }

        /// <summary>
        /// nome amigável ao usuário da ação
        /// </summary>
        [MaxLength(250, ErrorMessage = "O nome amigável deve ter no máximo 250 caracteres")]
        [Display(Name = "Nome Amigável")]
        public virtual string NomeAmigavel { get; set; }

        /// <summary>
        /// descrição amigável da ação
        /// </summary>
        [MaxLength(1000, ErrorMessage = "A Descrição amigável deve ter no máximo 1000 caracteres")]
        [Display(Name = "Descrição Amigável")]
        public virtual string DescricaoAmigavel { get; set; }

        /// <summary>
        /// Perfis onde essa ação aparece
        /// </summary>
        public virtual ICollection<Perfil> Perfis { get; set; }

        #endregion


    }
}