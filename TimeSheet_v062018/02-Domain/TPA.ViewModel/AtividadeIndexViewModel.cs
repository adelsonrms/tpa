using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TPA.Domain.DomainModel;

namespace TPA.ViewModel
{

    /// <summary>
    /// ViewModel completo para as actions Home/Index ou Atividade/Index
    /// possui classes internas e coleções dessas classes
    /// quebra varias coisas do modelo de negócio/Domain model e reintegra em uma exibição de um objeto só
    /// </summary>
    public class AtividadeIndexViewModel
    {

        #region classe AtividadeGridViewModel

        /// <summary>
        /// viewmodel para entrar na lista de atividades para mostrar no grid
        /// </summary>
        public class AtividadeGridViewModel
        {

            #region propriedades públicas

            /// <summary>
            /// id da atividade
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// nome do projeto
            /// </summary>
            public string ProjectNodeNome { get; set; }

            /// <summary>
            /// tipo da atividade
            /// </summary>
            public string TipoAtividadeNome { get; set; }

            /// <summary>
            /// observação da atividade
            /// </summary>
            public string Observacao { get; set; }

            /// <summary>
            /// data/hora de início
            /// </summary>
            public DateTime Inicio { get; set; }

            /// <summary>
            /// data/hora de fim
            /// </summary>
            public DateTime Fim { get; set; }

            /// <summary>
            /// duração
            /// </summary>
            public TimeSpan Duracao { get { return Fim - Inicio; } }

            /// <summary>
            /// bool - indicação se o período de referência está fechado
            /// </summary>
            public bool ReferenciaFechado { get; set; }

            /// <summary>
            /// id do usuário
            /// </summary>
            public int UsuarioId { get; set; }

            /// <summary>
            /// login do usuário
            /// </summary>
            public string UsuarioLogin { get; set; }

            /// <summary>
            /// nome do funcionário
            /// </summary>
            public string UsuarioFuncionarioNome { get; set; }

            #endregion

        }

        #endregion


        #region classe ConsolidadoDiarioGridViewModel

        /// <summary>
        /// ViewModel para Iens para exibição na grid do consolidado diário
        /// </summary>
        public class ConsolidadoDiarioGridViewModel
        {

            #region propriedades públicas

            /// <summary>
            /// dia de atividade
            /// </summary>
            public DateTime Data { get; set; }

            /// <summary>
            /// id do usuário
            /// </summary>
            public int UsuarioId { get; set; }

            /// <summary>
            /// login do usuário
            /// </summary>
            public string UsuarioLogin { get; set; }

            /// <summary>
            /// nome do funcionário
            /// </summary>
            public string UsuarioFuncionarioNome { get; set; }

            /// <summary>
            /// timespan com quantidade de horas reconstruidas a partir da quantidade de segundos no banco de dados
            /// </summary>
            public TimeSpan Horas
            {
                get
                {
                    return TimeSpan.FromSeconds(this.Segundos);
                }
                set
                {
                    if (value != null)
                    {
                        this.Segundos = (int)Math.Truncate(value.TotalSeconds);
                    }
                    else
                    {
                        this.Segundos = 0;
                    }
                }
            }

            /// <summary>
            /// quantidade de segundos do tempo de atividade consolidado
            /// </summary>
            public int Segundos { get; set; }

            /// <summary>
            /// nome do cliente
            /// </summary>
            public string ClienteNome { get; set; }

            #endregion
        }

        #endregion


        #region construtores

        /// <summary>
        /// constructor padrão, inicializa a lista de referências
        /// </summary>
        public AtividadeIndexViewModel()
        {
            Referencias = new List<ReferenciaViewModel>();
        }

        #endregion



        #region propriedades públicas

        /// <summary>
        /// lista de atividades
        /// </summary>
        public virtual List<AtividadeGridViewModel> Atividades { get; set; } = new List<AtividadeGridViewModel>();

        /// <summary>
        /// lista de horas consolidadas por dia
        /// </summary>
        public virtual List<ConsolidadoDiarioGridViewModel> ConsolidadoDiario { get; set; } = new List<ConsolidadoDiarioGridViewModel>();

        /// <summary>
        /// mês de referência atual
        /// </summary>
        public virtual ReferenciaViewModel ReferenciaAtual { get; set; }

        /// <summary>
        /// lista de referências
        /// </summary>
        public virtual List<ReferenciaViewModel> Referencias { get; set; }

        /// <summary>
        /// mostra a quantidade de usuários com atraso no lançamento, para mostrar para o admin
        /// </summary>
        public virtual int? UsuariosComAtrasoNoLancamento { get; set; }

        #endregion


    }
}