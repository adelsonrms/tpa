using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;

namespace TPA.Domain.DomainModel
{

    /// <summary>
    /// representa um determinado mês na rotina de um funcionário. É o mês / ano de referência, onde são calculados o total de horas trabalhadas pelo funcionário, banco de horas etc
    /// </summary>
    public class Referencia : TPAEntity
    {



        #region public properties

        /// <summary>
        /// id da referência
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// usuário / funcionário
        /// </summary>
        [Display(Name = "Usuário")]
        public virtual Usuario Usuario { get; set; }

        /// <summary>
        /// ano
        /// </summary>
        [Display(Name = "Ano")]
        public virtual int Ano { get; set; }

        /// <summary>
        /// mês
        /// </summary>
        [Display(Name = "Mês")]
        public virtual int Mes { get; set; }


        /// <summary>
        /// horas previstas do mês convertidas para timespan
        /// </summary>
        [NotMapped]
        [Display(Name = "Horas Previstas do Mês")]
        public virtual TimeSpan Previsto
        {
            get
            {
                return TimeSpan.FromSeconds(this.PrevistoSegundos);
            }
            set
            {
                if(value != null)
                {
                    this.PrevistoSegundos = (int)Math.Truncate(value.TotalSeconds);
                }
                else
                {
                    this.PrevistoSegundos = 0;
                }
            }
        }

        /// <summary>
        /// horas previstas do mês em segundos
        /// </summary>
        public virtual int PrevistoSegundos { get; set; }


        /// <summary>
        /// horas previstas até o dia atual convertidas para timespan
        /// </summary>
        [NotMapped]
        [Display(Name = "Horas Previstas até Hoje")]
        public virtual TimeSpan PrevistoCorrente
        {
            get
            {
                return TimeSpan.FromSeconds(this.PrevistoCorrenteSegundos);
            }
            set
            {
                if (value != null)
                {
                    this.PrevistoCorrenteSegundos = (int)Math.Truncate(value.TotalSeconds);
                }
                else
                {
                    this.PrevistoCorrenteSegundos = 0;
                }
            }
        }

        /// <summary>
        /// horas previstas até o dia atual em segundos
        /// </summary>
        public virtual int PrevistoCorrenteSegundos { get; set; }


        /// <summary>
        /// realizado até o momento convertido para timespan
        /// </summary>
        [NotMapped]
        [Display(Name = "Realizado")]
        public virtual TimeSpan Realizado
        {
            get
            {
                return TimeSpan.FromSeconds(this.RealizadoSegundos);
            }
            set
            {
                if (value != null)
                {
                    this.RealizadoSegundos = (int)Math.Truncate(value.TotalSeconds);
                }
                else
                {
                    this.RealizadoSegundos = 0;
                }
            }
        }

        /// <summary>
        /// realizado até o momento em segundos
        /// </summary>
        public virtual int RealizadoSegundos { get; set; }





        /// <summary>
        /// saldo atual do mês (previsto - realizado) convertido para timespan
        /// </summary>
        [NotMapped]
        [Display(Name = "Saldo do Mês")]
        public virtual TimeSpan SaldoDoMes
        {
            get
            {
                return TimeSpan.FromSeconds(this.SaldoDoMesSegundos);
            }
            set
            {
                if (value != null)
                {
                    this.SaldoDoMesSegundos = (int)Math.Truncate(value.TotalSeconds);
                }
                else
                {
                    this.SaldoDoMesSegundos = 0;
                }
            }
        }

        /// <summary>
        /// saldo atual do mês (previsto - realizado) em segundos
        /// </summary>
        public virtual int SaldoDoMesSegundos { get; set; }






        /// <summary>
        /// saldo total (mês anterior - saldo do mês) convertido para timespan
        /// </summary>
        [NotMapped]
        [Display(Name = "Saldo Total / Remanescente")]
        public virtual TimeSpan Saldo
        {
            get
            {
                return TimeSpan.FromSeconds(this.SaldoSegundos);
            }
            set
            {
                if (value != null)
                {
                    this.SaldoSegundos = (int)Math.Truncate(value.TotalSeconds);
                }
                else
                {
                    this.SaldoSegundos = 0;
                }
            }
        }

        /// <summary>
        /// saldo total (mês anterior - saldo do mês) em segundos
        /// </summary>
        public virtual int SaldoSegundos { get; set; }





        /// <summary>
        /// banco de horas calculado por 
        /// saldoAnterior - saldo do mês ou 
        /// saldoAnterior - (Previsto - Realizado)
        /// convertido para timespan
        /// </summary>
        [NotMapped]
        [Display(Name = "Banco de Horas")]
        public virtual TimeSpan BancoDeHoras
        {
            get
            {
                return TimeSpan.FromSeconds(this.BancoDeHorasSegundos);
            }
            set
            {
                if (value != null)
                {
                    this.BancoDeHorasSegundos = (int)Math.Truncate(value.TotalSeconds);
                }
                else
                {
                    this.BancoDeHorasSegundos = 0;
                }
            }
        }


        /// <summary>
        /// banco de horas calculado por 
        /// saldoAnterior - saldo do mês ou 
        /// saldoAnterior - (Previsto - Realizado)
        ///em segundos
        /// </summary>
        public virtual int BancoDeHorasSegundos { get; set; }







        /// <summary>
        /// define se o período está fechado ou não
        /// se estiver fechado o funcionário não pode adicionar atividades
        /// </summary>
        [Display(Name = "Fechado")]
        public virtual bool Fechado { get; set; }



        #endregion








        #region public methods


        /// <summary>
        /// baseado no primeio dia do mês, calcula as horas previstas do mês, horas realizadas
        /// captura o saldo do mês anterior
        /// se não estiver fechado calcula o previsto corrente
        /// efeta todos os cálculos de negócio para todas as propriedades desta classe
        /// calcular 
        /// </summary>
        /// <param name="dataBase">DateTime - data inicial do mês de referência para sincronizar atividades</param>
        /// <param name="calSvc">ICalendarioServices - instância de um serviço de calendário para calcular se o dia é útil ou não</param>
        public void SincronizaAtividades(DateTime dataBase, ICalendarioServices calSvc)
        {
            DateTime dtFim = dataBase.AddMonths(1).AddSeconds(-1);

            DateTime mesAnterior = dataBase.AddDays(-1);

            Referencia anterior = (from r in this.Usuario.Referencias
                                   where r.Ano == mesAnterior.Year && r.Mes == mesAnterior.Month
                                   select r).FirstOrDefault();


            TimeSpan saldoAnterior = anterior != null ? anterior.Saldo : new TimeSpan(0);

            if ((this.Previsto == null || this.Previsto == TimeSpan.Zero) && !this.Fechado)
            {
                this.Previsto = this.GetPrevisto(dataBase.Year, dataBase.Month, calSvc);
            }

            if (!this.Fechado)
            {
                this.PrevistoCorrente = this.GetPrevistoCorrente(dataBase.Year, dataBase.Month, calSvc);
            }

            if (this.PrevistoCorrente > this.Previsto)
            {
                this.PrevistoCorrente = this.Previsto;
            }

            if ((dtFim <= DateTime.Today) && (this.Previsto != this.PrevistoCorrente))
            {
                this.PrevistoCorrente = this.Previsto;
            }

            this.Realizado = this.GetRealizado(dataBase.Year, dataBase.Month);
            this.SaldoDoMes = this.Previsto - this.Realizado;
            this.Saldo = saldoAnterior - this.SaldoDoMes;
            if (this.Fechado)
            {
                this.BancoDeHoras = saldoAnterior - (this.Previsto - this.Realizado);
            }
            else
            {
                this.BancoDeHoras = saldoAnterior - (this.PrevistoCorrente - this.Realizado);
            }
        }

        /// <summary>
        /// Sincroniza as atividades construindo a data base a partir do Ano e Mes declarados na classe
        /// </summary>
        /// <param name="calSvc">ICalendarioServices - instância de um serviço de calendário para calcular se o dia é útil ou não</param>
        public void SincronizaAtividades(ICalendarioServices calSvc)
        {
            this.SincronizaAtividades(new DateTime(this.Ano, this.Mes, 1), calSvc);
        }






        /// <summary>
        /// obtém a lista de atividades do usuário para esse mês de referência
        /// </summary>
        /// <returns>List[Atividades] - atividades do usuário</returns>
        public virtual List<Atividade> GetAtividades()
        {
            return this.Usuario.GetAtividades(this);
        }


        /// <summary>
        /// obtém o total de horas trabalhadas no tia pelo usuário declarado na referência
        /// </summary>
        /// <param name="dia"></param>
        /// <returns>timespan - total de horas trabalhadas no dia</returns>
        public virtual TimeSpan GetHorasTrabalhadasNoDia(DateTime dia)
        {
            DateTime inicioDia = new DateTime(dia.Year, dia.Month, dia.Day);
            DateTime fimDia = inicioDia.AddDays(1).AddSeconds(-1);

            var totalTrabalhadoAtividades = (from t in this.Usuario.Atividades
                                             where t.Inicio >= inicioDia && t.Fim < fimDia
                                             select (t.Fim - t.Inicio).Ticks).Sum();


            TimeSpan result = new TimeSpan(totalTrabalhadoAtividades);

            return result;
        }

        /// <summary>
        /// baseado no ano, mês e um serviço de calendário, calcula as horas previstas do mês
        /// </summary>
        /// <param name="ano">int - ano de referência</param>
        /// <param name="mes">it - mês de referência</param>
        /// <param name="calendariosvc">ICalendarioServices - instância de um serviço de calendário para calcular se o dia é útil ou não</param>
        /// <returns>TimeSpan - horas previstas até o dia atual</returns>
        public virtual TimeSpan GetPrevistoCorrente(int ano, int mes, ICalendarioServices calendariosvc)
        {

            var calendario = calendariosvc.GetCalendarioMes(ano, mes);

            TimeSpan result = new TimeSpan(8 * calendario.Where(d => d.Value && d.Key < DateTime.Today.AddDays(1)).Count(), 0, 0);

            return result;

        }

        /// <summary>
        /// baseado no ano e mês CORRENTES declarado na classe, obtém  total de horas até o dia atual
        /// </summary>
        /// <param name="calendariosvc">ICalendarioServices - instância de um serviço de calendário para calcular se o dia é útil ou não</param>
        /// <returns>TimeSpan - total de horas previstas até o dia de hoje</returns>
        public virtual TimeSpan GetPrevistoAteHoje(ICalendarioServices calendariosvc)
        {

            return this.GetPrevistoCorrente(this.Ano, this.Mes, calendariosvc);

        }

        /// <summary>
        /// calcula, baseado na quantidade de horas por dia e quantidade de dias úteis do mês, a quantidade de horas previstas nesse mês
        /// </summary>
        /// <param name="ano">int - ano</param>
        /// <param name="mes">int - mes</param>
        /// <param name="calendariosvc">ICalendarioServices - instância de um serviço de calendário para calcular se o dia é útil ou não</param>
        /// <returns>TimeSpan - horas previstas para o mês de referência dado</returns>
        public virtual TimeSpan GetPrevisto(int ano, int mes, ICalendarioServices calendariosvc)
        {

            var calendario = calendariosvc.GetCalendarioMes(ano, mes);

            TimeSpan result = new TimeSpan(8 * calendario.Where(d => d.Value).Count(), 0, 0);

            return result;
        }

        /// <summary>
        /// calcula, baseado na quantidade de horas por dia e quantidade de dias úteis do mês corrente (declarado na classe), a quantidade de horas previstas nesse mês
        /// </summary>
        /// <param name="calendariosvc">ICalendarioServices - instância de um serviço de calendário para calcular se o dia é útil ou não</param>
        /// <returns>TimeSpan - horas previstas para o mês corrente</returns>
        public virtual TimeSpan GetPrevisto(ICalendarioServices calendariosvc)
        {
            return this.GetPrevisto(this.Ano, this.Mes, calendariosvc);
        }

        /// <summary>
        /// baseado em um ano/mes de referência, calcula o total de horas trabalhadas
        /// </summary>
        /// <param name="ano">int - ano</param>
        /// <param name="mes">int - mes</param>
        /// <returns>TimeSpan - quantidade de horas trabalhadas</returns>
        public virtual TimeSpan GetRealizado(int ano, int mes)
        {
            DateTime dt = new DateTime(ano, mes, 1);
            DateTime dtFim = dt.AddMonths(1);

            var totalTrabalhadoAtividades = (from t in this.Usuario.Atividades
                                             where t.Inicio >= dt && t.Fim < dtFim
                                             select (t.Fim - t.Inicio).Ticks).Sum();


            TimeSpan result = new TimeSpan(totalTrabalhadoAtividades);

            return result;
        }

        /// <summary>
        /// calcula o total de horas trabalhadas no mes corrente
        /// </summary>
        /// <returns>TimeSpan - horas trabalhadas no mês corrente</returns>
        public virtual TimeSpan GetRealizado()
        {
            return this.GetRealizado(this.Ano, this.Mes);
        }

        #endregion


    }
}