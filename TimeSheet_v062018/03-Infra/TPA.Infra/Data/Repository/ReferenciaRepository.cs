using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPA.Domain.DomainModel;
using System.Data.Entity;
using TPA.ViewModel;
using TPA.Infra.Services;

namespace TPA.Infra.Data.Repository
{
    /// <summary>
    /// classe repositório simplificada fake (não implementa as interfaces ITPARepository e ITPARepositoryAsync)
    /// métodos que apenas consultam os períodos de referência
    /// 
    /// esse repositório é diferenciado porque um período é criado pelas suas interações com o usuário/atividade. 
    /// Ele não pode ser criado, excluído ou replicado
    /// </summary>
    public class ReferenciaRepository
    {
        #region propriedades privadas

        /// <summary>
        /// contexto para consultas pelo EF
        /// </summary>
        private TPAContext _db;


        #endregion


        #region constructors

        /// <summary>
        /// constructor padrão que injeta o context do EF
        /// </summary>
        /// <param name="db"></param>
        public ReferenciaRepository(TPAContext db)
        {
            _db = db;
        }

        #endregion



        #region métodos públicos

        /// <summary>
        /// obtém todas as referências do usuário dado no intervalo de datas
        /// se ambas as datas forem nulas, serão substituídas pela data inicial e final do mês corrente
        /// </summary>
        /// <param name="idUsu">int - id do usuário sendo pesquisado</param>
        /// <param name="dtIni">DateTime - data inicial, se for null vai pegar a data mínima</param>
        /// <param name="dtFim">DateTime - data final, se for null vai pegar a data máxima</param>
        /// <returns></returns>
        public virtual async Task<List<ReferenciaViewModel>> GetReferenciasAsync(int idUsu, DateTime? dtIni, DateTime? dtFim)
        {
            if (dtIni == null && dtFim == null)
            {
                dtIni = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                dtFim = dtIni.Value.AddMonths(1).AddSeconds(-1);
            }
            else if (dtIni == null)
            {
                dtIni = DateTime.MinValue;
            }
            else if (dtFim == null)
            {
                dtFim = DateTime.MaxValue;
            }

            var dados = await _db.Referencias
                .Include(x => x.Usuario)
                .AsNoTracking()
                .Where(x => (x.Usuario.Id == idUsu) && ((x.Ano >= dtIni.Value.Year && x.Mes >= dtIni.Value.Month) && (x.Ano <= dtFim.Value.Year && x.Mes <= dtFim.Value.Month)))
                .ToListAsync();

            var referencias = dados.Select(x => new ReferenciaViewModel
            {
                Id = x.Id,
                PrevistoDoMes = x.Previsto,
                PrevistoCorrente = x.PrevistoCorrente,
                RealizadoDoMes = x.Realizado,

                SaldoDoMes = x.SaldoDoMes,
                Saldo = x.Saldo,

                BancoDeHoras = x.BancoDeHoras,

                Ano = x.Ano,
                Mes = x.Mes
            }).ToList();

            return referencias;


        }




        /// <summary>
        /// dado o id de um usuário, obtém as referências desde janeiro doano anterior
        /// </summary>
        /// <param name="idUsu">int - id do usuário sendo pesquisado</param>
        /// <returns></returns>
        public virtual async Task<List<ReferenciaViewModel>> GetReferenciasUltimoAnoAsync(int idUsu)
        {
            DateTime dtini = new DateTime(DateTime.Today.Year-1, 1, 1);
            DateTime dtfin = DateTime.Today;
            var referencias = await GetReferenciasAsync(idUsu, dtini, dtfin);

            return referencias;


        }




        /// <summary>
        /// obtém a referência atual do usuário, criando caso ainda não exista e sincronizando as atividades
        /// </summary>
        /// <param name="idUsu">int - id do usuário sendo pesquisado</param>
        /// <returns></returns>
        public virtual async Task<ReferenciaViewModel> GetReferenciaAsync(int idUsu)
        {
            //exemplo de include, para desabilitar o lazy load tirando o virtual
            Usuario usuLogado = await _db.Usuarios
                .AsNoTracking()
                .Include(x => x.Atividades)
                .Include(x => x.Referencias)
                .Where(x => x.Id == idUsu)
                .FirstOrDefaultAsync();
                //.FindAsync(idUsu);

            if (usuLogado != null)
            {
                var referencia = usuLogado.GetReferencia();
                referencia.SincronizaAtividades(new CalendarioServices());
                await _db.SaveChangesAsync();
                ReferenciaViewModel rvm = new ReferenciaViewModel
                {
                    Id = referencia.Id,
                    PrevistoDoMes = referencia.Previsto,
                    PrevistoCorrente = referencia.PrevistoCorrente,
                    RealizadoDoMes = referencia.Realizado,

                    SaldoDoMes = referencia.SaldoDoMes,
                    Saldo = referencia.Saldo,

                    BancoDeHoras = referencia.BancoDeHoras,

                    Ano = referencia.Ano,
                    Mes = referencia.Mes
                };

                return rvm;
            }

            return new ReferenciaViewModel();

        }


        #endregion


    }
}