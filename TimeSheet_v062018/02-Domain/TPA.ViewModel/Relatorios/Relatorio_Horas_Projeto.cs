using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel.Relatorios
{
    /// <summary>
    /// relatório de horas por projeto
    /// </summary>
    public class Relatorio_Horas_Projeto
    {

        #region propriedades públicas 

        /// <summary>
        /// projeto
        /// </summary>
        public virtual string Projeto { get; set; }

        /// <summary>
        /// quantidade de segundos para conversão
        /// </summary>
        public virtual double Segundos { get; set; }

        /// <summary>
        /// horas/minutos como timespan
        /// </summary>
        public virtual TimeSpan Horas
        {
            get
            {
                return TimeSpan.FromSeconds(this.Segundos);
            }
        }

        #endregion


    }
}
