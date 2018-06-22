using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel
{

    /// <summary>
    /// viewmodel para a página de abono
    /// lançamento de abono ou ferias
    /// </summary>
    public class AbonoViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// data inicial do abono, obrigatório
        /// </summary>
        [Required(ErrorMessage = "Preencha a data inicial")]
        [Display(Name = "Data Inicial")]
        public virtual DateTime DataInicial { get; set; }

        /// <summary>
        /// data final, se for null será igual a inicial
        /// </summary>
        [Display(Name = "Data Final")]
        public virtual DateTime? DataFinal {get; set;}

        /// <summary>
        /// array com os ids dos usuários
        /// </summary>
        [Required(ErrorMessage = "Preencha os usuários")]
        [Display(Name = "Usuário")]
        public virtual int[] IdsUsuarios { get; set; }

        /// <summary>
        /// id do tipo de atividade (Ferias/Atestado)
        /// </summary>
        [Required(ErrorMessage = "Preencha o tipo de atividade")]
        [Display(Name = "Tipo de Atividade")]
        public virtual int IdTipoAtividade { get; set; }

        /// <summary>
        /// quantidade de horas
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [Display(Name = "Horas")]
        public virtual TimeSpan? Horas { get; set; }

        /// <summary>
        /// descrição/observação das atividades
        /// </summary>
        [Display(Name = "Observação")]
        public virtual string Descricao { get; set; }

        /// <summary>
        /// Nome do usuário para o retorno do display template
        /// </summary>
        [Display(Name = "Usuário")]
        public virtual string NomeUsuario { get; set; }

        /// <summary>
        /// nome do tipo de atividade para o retorno no display template
        /// </summary>
        [Display(Name = "Tipo de Atividade")]
        public virtual string NomeTipoAtividade { get; set; }

        #endregion


    }
}
