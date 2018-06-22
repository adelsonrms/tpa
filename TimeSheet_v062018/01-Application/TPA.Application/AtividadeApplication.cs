using System;
using System.Collections.Generic;
using TPA.Domain.DomainModel;
using System.Threading.Tasks;
using TPA.Infra.Data;
using TPA.ViewModel;
using TPA.Infra.Data.Repository;
using TPA.Services.Seguranca;

namespace TPA.Application
{
    /// <summary>
    /// classe de controle / regras de negócio / application para lidar com atividades
    /// classe para lidar com atividades e os cálculos e validações internos que as atividades necessitam
    /// </summary>
    public class AtividadeApplication
    {
        #region private fields
        /// <summary>
        /// a instância do DBContext para esse application
        /// </summary>
        private TPAContext _db;

        /// <summary>
        /// A ionstância do Repository para essa application
        /// </summary>
        private AtividadeRepository _rep;
        #endregion


        #region contructors 


        /// <summary>
        /// constructor que obtém como parâmetros apenas o context e cria o repository
        /// </summary>
        /// <param name="ctx"></param>
        public AtividadeApplication(TPAContext ctx)
        {
            _db = ctx;
            _rep =  new AtividadeRepository(_db);
        }

        #endregion


        #region public methods

        /// <summary>
        /// salva uma atividade
        /// </summary>
        /// <param name="att"></param>
        /// <param name="podeSalvarReferenciaFechada"></param>
        public virtual void Salvar(Atividade att, bool podeSalvarReferenciaFechada = false)
        {
            _rep.Save(att, podeSalvarReferenciaFechada);
        }


        /// <summary>
        /// salva uma atividade de modo assíncrono
        /// </summary>
        /// <param name="att"></param>
        /// <param name="podeSalvarReferenciaFechada"></param>
        /// <returns></returns>
        public virtual async Task SalvarAsync(Atividade att, bool podeSalvarReferenciaFechada = false)
        {
            await _rep.SaveAsync(att, podeSalvarReferenciaFechada);
        }

        /// <summary>
        /// exclui uma atividade
        /// </summary>
        /// <param name="att"></param>
        /// <param name="podeSalvarReferenciaFechada"></param>
        public virtual void Delete(Atividade att, bool podeSalvarReferenciaFechada = false)
        {
            _rep.Delete(att, podeSalvarReferenciaFechada);
        }

        /// <summary>
        /// exclui uma atividade de modo assíncrono
        /// </summary>
        /// <param name="att"></param>
        /// <param name="podeSalvarReferenciaFechada"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Atividade att, bool podeSalvarReferenciaFechada = false)
        {
            await _rep.DeleteAsync(att, podeSalvarReferenciaFechada);
        }

        /// <summary>
        /// exclui uma atividade pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="podeSalvarReferenciaFechada"></param>
        public virtual void Delete(int id, bool podeSalvarReferenciaFechada = false)
        {
            _rep.Delete(id, podeSalvarReferenciaFechada);
        }


        /// <summary>
        /// exclui uma atividade pelo seu id de modo assíncrono
        /// </summary>
        /// <param name="id"></param>
        /// <param name="podeSalvarReferenciaFechada"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(int id, bool podeSalvarReferenciaFechada = false)
        {
            await _rep.DeleteAsync(id, podeSalvarReferenciaFechada);
        }

        /// <summary>
        /// traz todas as atividades brutas e consolidadas de uma determinada referência de forma assíncrona
        /// </summary>
        /// <param name="referenciaId"></param>
        /// <returns></returns>
        public virtual async Task<AtividadeIndexViewModel> GetAtividadeIndexAsync(int referenciaId)
        {
            var referencia = _db.Referencias.Find(referenciaId);

            if(referencia != null)
            {
                return await GetAtividadeIndexAsync(referencia.Usuario.Id, referencia.Mes, referencia.Ano);
            }


            throw new ArgumentException("Referência não encontrada");
        }


        /// <summary>
        /// obtém os dados de atividades brutas e consolidadas de apenas um usuário, com suas referencias, mediante mês e ano
        /// de forma assíncrona
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <param name="mes"></param>
        /// <param name="ano"></param>
        /// <returns></returns>
        public virtual async Task<AtividadeIndexViewModel> GetAtividadeIndexAsync(int usuarioId, int? mes, int? ano)
        {
            if (((mes ?? 0) == 0) || (mes > 12))
            {
                mes = DateTime.Today.Month;
            }

            if ((ano ?? 0) == 0)
            {
                ano = DateTime.Today.Year;
            }

            DateTime dataInicial = new DateTime(ano.Value, mes.Value, 1);
            DateTime dataFinal = dataInicial.AddMonths(1).AddSeconds(-1);

            return await GetAtividadeIndexAsync(usuarioId, dataInicial, dataFinal);
        }



        /// <summary>
        /// traz os dados de atividades brutas e consolidadas de apenas um usuário, com suas referencias
        /// de forma assíncrona
        /// </summary>
        /// <param name="usuarioId"></param>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        public virtual async Task<AtividadeIndexViewModel> GetAtividadeIndexAsync(int usuarioId, DateTime? inicio, DateTime? fim)
        {
            AtividadeIndexViewModel result = await GetAtividadeIndexAsync(new List<int> { usuarioId }, inicio, fim);
            ReferenciaRepository refRep = new ReferenciaRepository(_db);

            var referencias = await refRep.GetReferenciasAsync(usuarioId, inicio, fim);
            var referenciaAtual = await refRep.GetReferenciaAsync(usuarioId);

            result.Referencias.Clear();
            result.Referencias.AddRange(referencias);
            result.ReferenciaAtual = referenciaAtual;

            if(ControleAcesso.TemAcesso(SegurancaResources.GERENCIAR_ATIVIDADES_FUNCIONARIOS))
            {
                UsuarioApplication usuapp = new UsuarioApplication(_db);
                var modelo = await usuapp.GetUsuariosComAtrasoNoEnvioAsync();
                result.UsuariosComAtrasoNoLancamento = modelo.Count;
            }


            return result;
        }


        /// <summary>
        /// traz os dados brutos e consolidados de todos os usuarios listaods, mas sem as referências/fechamentos
        /// de forma assíncrona
        /// </summary>
        /// <param name="usuarioIds"></param>
        /// <param name="inicio"></param>
        /// <param name="fim"></param>
        /// <returns></returns>
        public virtual async Task<AtividadeIndexViewModel> GetAtividadeIndexAsync(List<int> usuarioIds, DateTime? inicio, DateTime? fim)
        {
            AtividadeIndexViewModel result = new AtividadeIndexViewModel();

            var atividades = await _rep.GetAtividadesAsync(usuarioIds, inicio, fim);
            var consolidadoDiario = await _rep.GetAtividadesConsolidadoDiarioAsync(usuarioIds, inicio, fim);

            result.Atividades =  atividades;
            result.ConsolidadoDiario =  consolidadoDiario;

            return result;
        }





        /// <summary>
        /// atalho para GetReferenciasUltimoAnoAsync do repositório. Aqui podemos interceptar e manipular a lista,
        /// podemos chamar esse método com a certeza de estar usando o mesmo context e sem criar um novo repositório
        /// Assíncrona
        /// </summary>
        /// <param name="idUsu"></param>
        /// <returns></returns>
        public virtual async Task<List<ReferenciaViewModel>> GetReferenciasUltimoAnoAsync(int idUsu)
        {

            ReferenciaRepository rep = new ReferenciaRepository(_db);
            return await rep.GetReferenciasUltimoAnoAsync(idUsu);

        }

        #endregion

    }
}