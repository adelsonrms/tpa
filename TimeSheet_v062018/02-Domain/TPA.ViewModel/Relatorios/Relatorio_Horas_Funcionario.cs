using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel.Relatorios
{
    /// <summary>
    /// relatório de horas por funcionário
    /// </summary>
    public class Relatorio_Horas_Funcionario
    {

        #region propriedades públicas 

        /// <summary>
        /// nome do funcionário
        /// </summary>
        public virtual string Funcionario { get; set; }

        /// <summary>
        /// segundos
        /// </summary>
        public virtual double Segundos { get; set; }

        /// <summary>
        /// horas
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
