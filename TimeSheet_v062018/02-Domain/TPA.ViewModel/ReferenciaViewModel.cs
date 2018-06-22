using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TPA.ViewModel
{
    /// <summary>
    /// DTO ou viewmodel para exibição do mês de referência de um usuário/mês
    /// </summary>
    public class ReferenciaViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// id do mês de referência
        /// </summary>
        public virtual int? Id { get; set; }

        /// <summary>
        /// horas previstas do mês
        /// </summary>
        [Display(Name = "HoraS Previstas do Mês")]
        public virtual TimeSpan PrevistoDoMes { get; set; }

        /// <summary>
        /// horas previstas até agora
        /// </summary>
        [Display(Name = "Horas Previstas até Hoje")]
        public virtual TimeSpan PrevistoCorrente { get; set; }

        /// <summary>
        /// horas realizadas até agora
        /// </summary>
        [Display(Name = "Realizado")]
        public virtual TimeSpan RealizadoDoMes { get; set; }

        /// <summary>
        /// horas previstas até hoje - horas realizadas
        /// horas remanescente do mês
        /// diferença entre previsto e realizado
        /// </summary>
        [Display(Name = "Saldo do Mês")]
        public virtual TimeSpan SaldoDoMes { get; set; }

        /// <summary>
        /// saldo anterior - saldo do mês
        /// </summary>
        [Display(Name = "Saldo Total / Remanescente")]
        public virtual TimeSpan Saldo { get; set; }

        /// <summary>
        /// banco de horas
        /// diferença entre realizado e previsto dos mêses anteriores
        /// </summary>
        [Display(Name = "Banco de Horas")]
        public virtual TimeSpan BancoDeHoras { get; set; }

        /// <summary>
        /// ano
        /// </summary>
        [Display(Name = "Ano")]
        public virtual int Ano { get; set; }

        /// <summary>
        /// mes
        /// </summary>
        [Display(Name = "Mês")]
        public virtual int Mes { get; set; }

        #endregion


    }
}