using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TPA.Framework;


namespace TPA.ViewModel
{

    /// <summary>
    /// ViewModel para fechamento do mês
    /// </summary>
    public class FechamentoViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// horas previstas 
        /// </summary>
        [Display(Name = "Horas Previstas")]
        [Required]
        public virtual TimeSpan HorasPrevistasTimeSpan { get; set; }


        /// <summary>
        /// conversão das horas previstas para double, para cálculos e operações
        /// </summary>
        [Display(Name = "Horas Previstas")]
        [Required]
        public virtual double HorasPrevistasDouble {
            get
            {
                return GetHorasPrevistasDouble();
            }
            set
            {
                SetHorasPrevistasDouble(value);
            }
        }

        /// <summary>
        /// conversão das horas previstas de/para string para aceitar da view ou formatar a saída
        /// </summary>
        [StringLength(6)]
        [Display(Name = "Horas Previstas")]
        [Required]
        [RegularExpression("^[0-2][0-9][0-9]:[0-5][0-9]$", ErrorMessage = "Horas Inválidas")]
        public virtual string HorasPrevistasString
        {
            get
            {
                return GetHorasPrevistasString();
            }
            set
            {
                SetHorasPrevistasString(value);
            }
        }

        /// <summary>
        /// id do fechamento
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// id do usuário
        /// </summary>
        public virtual int IdUsuario { get; set; }

        /// <summary>
        /// ano
        /// </summary>
        public virtual int? Ano { get; set; }

        /// <summary>
        /// mês
        /// </summary>
        [Display(Name ="Mês")]
        public virtual int? Mes { get; set; }




        /// <summary>
        /// define se o período está fechado ou não
        /// </summary>
        public virtual bool Fechado { get; set; }

        #endregion


        #region métodos privados

        /// <summary>
        /// converte de timespan para double
        /// </summary>
        /// <returns></returns>
        private double GetHorasPrevistasDouble()
        {
            return this.HorasPrevistasTimeSpan.TotalHours;
        }

        /// <summary>
        /// converte de double para timespan
        /// </summary>
        /// <param name="horas"></param>
        private void SetHorasPrevistasDouble(double horas)
        {
            this.HorasPrevistasTimeSpan = TimeSpan.FromHours(horas);
        }

        /// <summary>
        /// converte de timespan para string
        /// </summary>
        /// <returns></returns>
        private string GetHorasPrevistasString()
        {
            return this.HorasPrevistasTimeSpan.BigTimeSpanToString();
        }

        /// <summary>
        /// converte de string para timespan
        /// </summary>
        /// <param name="horas"></param>
        private void SetHorasPrevistasString(string horas)
        {
            int[] weights = { 60 * 60 * 1000, 60 * 1000, 1000, 1 };

            this.HorasPrevistasTimeSpan = TimeSpan.FromMilliseconds(horas.Split('.', ':')
                .Zip(weights, (d, w) => Convert.ToInt64(d) * w).Sum());
        }


        #endregion


    }
}