using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TPA.Framework
{

    /// <summary>
    /// extension methods para TimeSpan
    /// </summary>
    public static class TPATimeSpanExtensions
    {

        #region métodos estáticos públicos de extensão

        /// <summary>
        /// retorna um timespan > que 24 horas eliminando a parte dos dias e convertendo tudo para horas no formato hhh:mm
        /// com zero a esquerda opcional
        /// com possibilidade de timespan negativo 
        /// </summary>
        /// <param name="timespan">TimeSpan - TimeSpan a ser convertido para string</param>
        /// <returns>string - representação do timespan em string</returns>
        public static string BigTimeSpanToString(this TimeSpan timespan)
        {
            return timespan.Ticks >= 0 ? string.Format("{0:##0}:{1:00}", Math.Truncate(timespan.TotalHours), timespan.Minutes) : string.Format("-{0:##0}:{1:00}", Math.Abs(Math.Truncate(timespan.TotalHours)), Math.Abs(timespan.Minutes));
        }

        /// <summary>
        /// converte um timespan para o formato hh:mm
        /// com zero a esquerda opcional
        /// com possibilidade de timespan negativo 
        /// </summary>
        /// <param name="timespan">TimeSpan - TimeSpan a ser convertido para string</param>
        /// <returns>string - representação do timespan em string</returns>
        public static string TimeSpanToString(this TimeSpan timespan)
        {
            return timespan.Ticks >= 0 ? string.Format("{0:#0}:{1:00}", Math.Truncate(timespan.TotalHours), timespan.Minutes) : string.Format("-{0:#0}:{1:00}", Math.Abs(Math.Truncate(timespan.TotalHours)), Math.Abs(timespan.Minutes));
        }

        #endregion

    }
}