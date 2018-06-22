using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TPA.Domain;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;

namespace TPA.Infra.Services
{

    /// <summary>
    /// serviços de calendário
    /// precisa ficar junto da camada de infra por causa do alto acoplamento com a entidade feriado
    /// </summary>
    public class CalendarioServices : ICalendarioServices
    {

        #region construtores

        /// <summary>
        /// constructor padrão
        /// </summary>
        public CalendarioServices()
        {

        }


        #endregion


        #region métodos públicos

        /// <summary>
        /// retorna true se a data for dia útil, false se for sábado, domingo e feriado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool IsDiaUtil(DateTime data)
        {
            using (TPAContext db = new TPAContext())
            {
                if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                    return false;

                Feriado feriado = db.Feriados.Where(f => f.Data == data).FirstOrDefault();
                if (feriado != null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// traz um dicionário de datas/bool onde o lado bool será true apenas se a data for dia útil
        /// </summary>
        /// <param name="ano"></param>
        /// <param name="mes"></param>
        /// <returns></returns>
        public virtual Dictionary<DateTime, bool>GetCalendarioMes(int ano, int mes)
        {
            DateTime dt = new DateTime(ano, mes, 1);
            DateTime dtFim = dt.AddMonths(1).AddDays(-1);

            return GetCalendarioMes(dt, dtFim);

        }

        /// <summary>
        /// traz um dicionário de datas/bool onde o lado bool será true apenas se a data for dia útil
        /// usa um intervalo de datas para trazer um calendário maior
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtFim"></param>
        /// <returns></returns>
        public virtual Dictionary<DateTime, bool>GetCalendarioMes(DateTime dt, DateTime dtFim)
        {

            Dictionary<DateTime, bool> result = new Dictionary<DateTime, bool>();
            List<DateTime> feriados = new List<DateTime>();

            using (TPAContext db = new TPAContext())
            {
                feriados.AddRange(db.Feriados.Where(f => f.Data >= dt && f.Data <= dtFim).Select(f => f.Data).ToList<DateTime>());
            }

            while (dt <= dtFim)
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                {
                    result.Add(dt, false);
                }
                else if (feriados.Contains(dt))
                {
                    result.Add(dt, false);
                }
                else
                {
                    result.Add(dt, true);
                }

                dt = dt.AddDays(1);
            }

            return result;
        }


        /// <summary>
        /// conta quantos dias úteis tem entre duas datas
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dtFim"></param>
        /// <returns></returns>
        public virtual int DiasUteisEntreDatas(DateTime dt, DateTime dtFim)
        {
            var calendario = GetCalendarioMes(dt, dtFim);

            return calendario.Where(x => x.Value && x.Key > dt).Count();
        }


        #endregion


    }
}