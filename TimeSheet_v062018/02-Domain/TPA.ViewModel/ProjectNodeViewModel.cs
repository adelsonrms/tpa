using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TPA.Domain.DomainModel;
using Microsoft.Extensions.Localization;
using System.Web.Mvc;

namespace TPA.ViewModel
{
    /// <summary>
    /// viewmodel para o projectnode, para a página da treeview de projetos
    /// </summary>
    public class ProjectNodeViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// Id do projectnode
        /// </summary>
        [Key]
        public virtual int Id { get; set; }

        /// <summary>
        /// nome do node
        /// </summary>
        [System.Web.Mvc.AllowHtml]
        [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        [Required]
        public virtual string Nome { get; set; }

        /// <summary>
        /// descrição do node
        /// </summary>
        [System.Web.Mvc.AllowHtml]
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]        
        public virtual string Descricao { get; set; }

        /// <summary>
        /// horas estimadas desse galho do projeto
        /// </summary>
        [Display(Name = "Horas Estimadas")]
        public virtual int HorasEstimadas { get; set; }

        /// <summary>
        /// id do pai, se houver
        /// </summary>
        [Display(Name = "Estrutura Pai ou Agrupador")]
        public virtual int? IdPai { get; set; }

        /// <summary>
        /// ProjectNode Pai
        /// </summary>
        [Display(Name = "Estrutura Pai ou Agrupador")]
        public virtual ProjectNode Pai { get; set; }



        /// <summary>
        /// id do nodelabel
        /// </summary>
        [Required]
        [Display(Name = "Rótulo de estrutura de projeto")]
        public virtual int? IdNodeLabel { get; set; }

        /// <summary>
        /// nome do nodelabel
        /// </summary>
        [Display(Name = "Nome do Rótulo de estrutura de projeto")]
        public virtual string NomeNodeLabel { get; set; }

        #endregion



    }
}