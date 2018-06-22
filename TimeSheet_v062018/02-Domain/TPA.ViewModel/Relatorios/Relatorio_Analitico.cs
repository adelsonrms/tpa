using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel.Relatorios
{
    /// <summary>
    /// relatório analítico com os dados granulares, sem consolidação
    /// </summary>
    public class Relatorio_Analitico
    {

        #region propriedades públicas 

        /// <summary>
        /// nome do funcionário
        /// </summary>
        public virtual string Funcionario { get; set; }

        /// <summary>
        /// nome do tipo de atividade
        /// </summary>
        public virtual string Tipo_Atividade { get; set; }

        /// <summary>
        /// observação
        /// </summary>
        public virtual string Observacao { get; set; }

        /// <summary>
        /// data de início
        /// </summary>
        public virtual DateTime Inicio { get; set; }

        /// <summary>
        /// horário de fim
        /// </summary>
        public virtual DateTime Fim { get; set; }

        /// <summary>
        /// quantidade de horas
        /// </summary>
        public virtual TimeSpan Horas { get; set; }

        /// <summary>
        /// lançamento admionistrativo
        /// </summary>
        public virtual string Administrativo { get; set; }

        /// <summary>
        /// cliente raiz da hierarquia
        /// </summary>
        public virtual string Cliente_Raiz { get; set; }

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
        /// entregáveis
        /// </summary>
        public virtual string Entregaveis { get; set; }

        /// <summary>
        /// etápas
        /// </summary>
        public virtual string Etapas { get; set; }

        #endregion

    }
}
