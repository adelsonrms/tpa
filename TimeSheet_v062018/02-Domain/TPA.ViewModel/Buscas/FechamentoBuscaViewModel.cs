using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TPA.ViewModel.Buscas
{
    /// <summary>
    /// classe que encapsula os parâmetros de busca da página de fechamento
    /// </summary>
    public class FechamentoBuscaViewModel
    {

        #region construtores

        /// <summary>
        /// construtor padrão que inicializa o ano e mês atual
        /// </summary>
        public FechamentoBuscaViewModel()
        {
            this.AnoInicial = this.AnoFinal = DateTime.Today.Year;
            this.MesInicial = this.MesFinal = DateTime.Today.Month;
        }

        #endregion


        #region propriedades públicas 

        /// <summary>
        /// vetor com  os ids dos usuários a serem pesquisados
        /// </summary>
        [Display(Name = "Funcionários")]
        public virtual int[] IdsUsuarios { get; set; }

        /// <summary>
        /// ano inicial da busca
        /// </summary>
        [Display(Name = "Ano Inicial")]
        public virtual int? AnoInicial { get; set; }

        /// <summary>
        /// mês inicial da busca
        /// </summary>
        [Display(Name = "Mês Inicial")]
        public virtual int? MesInicial { get; set; }

        /// <summary>
        /// ano final da busca
        /// </summary>
        [Display(Name = "Ano Final")]
        public virtual int? AnoFinal { get; set; }

        /// <summary>
        /// mês final da busca
        /// </summary>
        [Display(Name = "Mês Final")]
        public virtual int? MesFinal { get; set; }

        #endregion


    }
}