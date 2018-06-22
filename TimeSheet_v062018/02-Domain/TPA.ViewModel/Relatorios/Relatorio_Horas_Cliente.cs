using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel.Relatorios
{

    /// <summary>
    /// relatório de horas por cliente
    /// </summary>
    public class Relatorio_Horas_Cliente
    {

        #region propriedades públicas 

        /// <summary>
        /// nome do cliente
        /// </summary>
        public virtual string Cliente { get; set; }

        /// <summary>
        /// segundos de atividade
        /// </summary>
        public virtual double Segundos { get; set; }

        /// <summary>
        /// horas convertidas para timespan
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
