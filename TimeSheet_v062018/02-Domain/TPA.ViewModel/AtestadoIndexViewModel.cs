using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel
{
    /// <summary>
    /// ViewModel para formatar a lista de atestados na index
    /// </summary>
    public class AtestadoIndexViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// id do atestado
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// nome do funcionário
        /// </summary>
        [Display(Name = "Funcionário")]
        public virtual string NomeUsuario { get; set; }

        /// <summary>
        /// quantidade de atividades associadas
        /// </summary>
        [Display(Name = "Quantidade de atividades")]
        public virtual int QuantidadeDeAtividades { get; set; }

        /// <summary>
        /// observação adicionada
        /// </summary>
        [Display(Name = "Observação")]
        public virtual string Observacao { get; set; }

        /// <summary>
        /// data inicial do primeiro abono
        /// </summary>
        [Display(Name = "Data Inicial")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime? DataInicial { get; set; }

        /// <summary>
        /// data final do último abono
        /// </summary>
        [Display(Name = "Data Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime? DataFinal { get; set; }

        /// <summary>
        /// horas abonadas totais (deve ser calculado na camada application)
        /// </summary>
        [Display(Name = "Horas Abonadas")]
        public virtual Double? HorasAbonadas { get; set; }

        #endregion


    }
}
