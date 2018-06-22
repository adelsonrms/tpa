using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPA.Domain.DomainModel;

namespace TPA.ViewModel
{
    /// <summary>
    /// viewmodel para as listas e jobs de envio de alerta
    /// 
    /// </summary>
    public class UsuarioAlertaLancamentoViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// usuário com lançamentos de atividades em atraso
        /// </summary>
        public virtual Usuario Usuario { get; set; }

        /// <summary>
        /// último lançamento do usuário
        /// </summary>
        public virtual DateTime? UltimoLancamento { get; set; }

        /// <summary>
        /// quantidade de dias sem lançar
        /// </summary>
        public virtual int? DiasSemLancar { get; set; }

        /// <summary>
        /// quantidade de dias UTEIS sem lançar
        /// </summary>
        public virtual int DiasUteisSemLancar { get; set; }

        #endregion



    }
}
