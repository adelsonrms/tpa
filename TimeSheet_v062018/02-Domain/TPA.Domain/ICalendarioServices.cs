using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.Domain
{
    /// <summary>
    /// interface para serviços de calendário para a aplicação poder fazer cálculos baseados em dias úteis
    /// </summary>
    public interface ICalendarioServices
    {
        /// <summary>
        /// veifica se uma data é dia util
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool IsDiaUtil(DateTime data);

        /// <summary>
        /// traz um calendário no formato de data/bool, onde o bool é true se for dia útil e false caso contrário
        /// </summary>
        /// <param name="ano"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        Dictionary<DateTime, bool> GetCalendarioMes(int ano, int mes);
    }
}
