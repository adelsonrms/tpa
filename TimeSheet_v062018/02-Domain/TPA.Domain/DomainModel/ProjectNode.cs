using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;
using System.Web.Mvc;

namespace TPA.Domain.DomainModel
{

    /// <summary>
    /// representa um node em  uma estrutura hierárquica
    /// os projetos são representados por hierarquias por causa
    /// da natureza em que são divididos/monitorados: cada departamento necessita de um
    /// nível de profundidade diferente, então em vez de criarmos múltiplas entidades 
    /// "tipo" e "tipoDeTipo" relacionadas, criamos uma entidade hierárquica
    /// Atualmente estamos usando a hierarquia Cliente - Area - Projetos - Entregáveis - Etapas
    /// </summary>
    public class ProjectNode : TPAEntity
    {

        #region propriedades públicas

        /// <summary>
        /// id do node de projeto
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// nome do node de projeto
        /// </summary>
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [Required]
        [Index("UQ_SubNosUnicosPorNo", 1, IsUnique = true)]
        [System.Web.Mvc.AllowHtml]
        public virtual string Nome { get; set; }

        /// <summary>
        /// descrição do node de projeto
        /// </summary>
        [Display(Name = "Descrição")]
        [MaxLength(500, ErrorMessage = "A Descrição deve ter no máximo 500 caracteres")]
        [System.Web.Mvc.AllowHtml]
        public virtual string Descricao { get; set; }

        /// <summary>
        /// label do node
        /// </summary>
        public virtual NodeLabel NodeLabel { get; set; }

        /// <summary>
        /// node pai
        /// </summary>
        public virtual ProjectNode Pai { get; set; }

        /// <summary>
        /// id do pai
        /// </summary>
        [ForeignKey("Pai")]
        [Index("UQ_SubNosUnicosPorNo", 2, IsUnique = true)]
        [Index]
        public virtual int? Pai_Id { get; set; }

        /// <summary>
        /// lista de nodes filhos
        /// </summary>
        public virtual ICollection<ProjectNode> Filhos { get; set; }

        /// <summary>
        /// lista de atividades desse node
        /// todo: essa propriedade pode ser tirada, não tem sentido listar as atividades pelo projeto
        /// </summary>
        public virtual ICollection<Atividade> Atividades { get; set; }

        /// <summary>
        /// horas estimadas
        /// todo: fazer a codificação para somar/diluir esse total hierarquia acima e/ou abaixo
        /// </summary>
        [Display(Name = "Horas Estimadas")]
        public virtual int HorasEstimadas { get; set; }

        /// <summary>
        /// usuarios que podem ver esse node
        /// usuários para os quais esse node foi atribuído
        /// </summary>
        [Display(Name = "Usuários que podem ver este node")]
        public virtual ICollection<Usuario> UsuariosDesteNode { get; set; }


        /// <summary>
        /// Caminho completo das strings desde a raiz separados por / 
        /// </summary>
        [Display(Name = "Caminho completo")]
        [NotMapped]
        public virtual string NomePath { get { return GetNomePath(); } }

        /// <summary>
        /// Caminho completo dos id`s desde a raiz separados por /
        /// </summary>
        [Display(Name = "Caminho completo de id's")]
        [NotMapped]
        public virtual string IdPath { get { return GetIdPath(); } }

        #endregion









        #region métodos públicos

        /// <summary>
        /// obtém a raiz da estrutura
        /// </summary>
        /// <returns></returns>
        public virtual ProjectNode GetCliente()
        {
            if (this.Pai == null)
                return this;

            else
                return this.Pai.GetCliente();
        }

        #endregion




        #region métodos privados

        /// <summary>
        /// pega o caminho de nomes
        /// </summary>
        /// <returns></returns>
        private string GetNomePath()
        {
            return ((this.Pai == null) ? "" : (this.Pai.GetNomePath() + "/")) + this.Nome;
        }

        /// <summary>
        /// pega o caminho de ids
        /// </summary>
        /// <returns></returns>
        private string GetIdPath()
        {
            return ((this.Pai == null) ? "" : (this.Pai.GetIdPath() + "/")) + this.Id.ToString();
        }

        #endregion


    }
}