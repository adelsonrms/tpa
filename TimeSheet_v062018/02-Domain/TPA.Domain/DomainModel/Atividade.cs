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
    /// entidade core do negócio: simboliza uma atividade feita por um usuário, 
    /// com um tipo, início, fim em um projeto
    /// </summary>
    public class Atividade : TPAEntity
    {

        #region propriedades públicas

        /// <summary>
        /// id da atividade
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// observação ou descrição da atividade, definida pelo usuário
        /// o usuário descreve as atividades e ações que performou
        /// </summary>
        [Display(Name = "Descrição")]
        public virtual string Observacao { get; set; }


        /// <summary>
        /// hora de início
        /// </summary>
        [Required]
        [Display(Name = "Hora de Início")]
        [DataType(DataType.DateTime, ErrorMessage = "Data em formato inválido")]
        public virtual DateTime Inicio { get; set; }

        /// <summary>
        /// hora de término
        /// </summary>
        [Required]
        [Display(Name = "Hora de Término")]
        [DataType(DataType.DateTime, ErrorMessage = "Data em formato inválido")]
        public virtual DateTime Fim { get; set; }


        /// <summary>
        /// tipo da atividade (Desenvolvimento, Testes, Levantamento etc)
        /// </summary>
        [Required]
        [Display(Name = "Tipo de Atividade")]
        public virtual TipoAtividade TipoAtividade { get; set; }

        /// <summary>
        /// usuário que executou a atividade
        /// </summary>
        [Required]
        [Display(Name = "Usuário")]
        public virtual Usuario Usuario { get; set; }




        /// <summary>
        /// id do nó de projeto, para navegação two ways
        /// configuração necessária para o funcionamento correto do mapeamento no Entity Framework
        /// quando você tem referências múltiplas à mesma tabela
        /// </summary>
        [Column("ProjectNode_Id")]        
        public int ProjectNodeId { get; set; }

        /// <summary>
        /// Nó do projeto
        /// </summary>
        [Required]
        [ForeignKey("ProjectNodeId")]
        [InverseProperty("Atividades")]
        [Display(Name = "Projeto / Fase")]
        public virtual ProjectNode ProjectNode { get; set; }

        /// <summary>
        /// Id do nó de projeto raiz do selecionado
        /// </summary>
        [Column("Cliente_Id")]
        public int? ClienteId { get; set; }

        /// <summary>
        /// nó raiz do nó selecionado
        /// </summary>
        [ForeignKey("ClienteId")]
        public virtual ProjectNode Cliente { get; set; }


        #endregion


    }
}