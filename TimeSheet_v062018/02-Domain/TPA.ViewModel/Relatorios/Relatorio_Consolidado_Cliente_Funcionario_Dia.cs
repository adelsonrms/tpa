using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel.Relatorios
{

    /// <summary>
    /// relatório consolidado de cliente por funcionário por dia
    /// </summary>
    public class Relatorio_Consolidado_Cliente_Funcionario_Dia
    {

        #region propriedades públicas 

        /// <summary>
        /// dia
        /// </summary>
        public virtual DateTime Dia { get; set; }

        /// <summary>
        /// funcionário
        /// </summary>
        public virtual string Funcionario { get; set; }

        /// <summary>
        /// cliente
        /// </summary>
        public virtual string Cliente { get; set; }

        /// <summary>
        /// área
        /// </summary>
        public virtual string Area { get; set; }

        /// <summary>
        /// projeto
        /// </summary>
        public virtual string Projeto { get; set; }

        /// <summary>
        /// horas
        /// </summary>
        public virtual string Horas { get; set; }

        /// <summary>
        /// horas como timespan
        /// </summary>
        public virtual TimeSpan HorasTimeSpan
        {
            get
            {
                double horas = Convert.ToInt32(this.Horas.Split(':')[0]);
                double minutos = Convert.ToInt32(this.Horas.Split(':')[1]) / 60;
                TimeSpan result = TimeSpan.FromHours(horas + minutos);
                return result;
            }
        }

        #endregion



    }
}
