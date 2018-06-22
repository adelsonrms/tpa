using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TPA.ViewModel
{

    /// <summary>
    /// ViewModel para lançamento em lote (múltiplas datas e possivelmente múltiplos usuários)
    /// </summary>
    public class LancamentoEmLoteModel
    {


        #region propriedades públicas 
        /// <summary>
        /// data inicial
        /// </summary>
        [Display(Name = "Data Inicial")]
        public virtual DateTime DataInicial { get; set; }

        /// <summary>
        /// data final
        /// </summary>
        [Display(Name = "Data Final")]
        public virtual DateTime DataFinal { get; set; }

        /// <summary>
        /// id do projeto
        /// </summary>
        [Display(Name = "Projeto")]
        public virtual int IdProjeto { get; set; }

        /// <summary>
        /// id do tipo de atividade
        /// </summary>
        [Display(Name = "Tipo de Atividade")]
        public virtual int IdTipoAtividade { get; set; }

        /// <summary>
        /// id do usuário (ou ids dos usuários)
        /// </summary>
        [Display(Name = "Usuários")]
        [Required]
        public virtual int[] IdsUsuarios { get; set; }

        /// <summary>
        /// descrição da atividade/abono/lançamento
        /// </summary>
        [Display(Name = "Descrição")]
        public virtual string Descricao { get; set; }

        /// <summary>
        /// entrada da manhã
        /// </summary>
        [Display(Name = "Entrada Manhã")]
        public virtual TimeSpan EntradaManha { get; set; }

        /// <summary>
        /// saída da manhã
        /// </summary>
        [Display(Name = "Saida Manhã")]
        public virtual TimeSpan SaidaManha { get; set; }

        /// <summary>
        /// entrada da tarde
        /// </summary>
        [Display(Name = "Entrada Tarde")]
        public virtual TimeSpan EntradaTarde { get; set; }

        /// <summary>
        /// saída da tarde
        /// </summary>
        [Display(Name = "Saida Tarde")]
        public virtual TimeSpan SaidaTarde { get; set; }

        #endregion


    }
}