using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TPA.Domain.DomainModel;
using System.Data.Entity;
using System.Collections.Concurrent;
using TFW.Domain;
using TPA.ViewModel;
using TPA.Infra.Services;

namespace TPA.Infra.Data.Repository
{

    /// <summary>
    /// classe repositório para atividade
    /// </summary>
    public class AtividadeRepository : ITPARepository<Atividade>, ITPARepositoryAsync<Atividade>
    {


        #region fields estáticos

        /// <summary>
        /// cache organizado em árvore dos projectnodes
        /// </summary>
        private static readonly TFWHierarchicalList _projectNodes = new TFWHierarchicalList();

        #endregion



        #region propriedades privadas

        private TPAContext _db;


        #endregion



        #region constructors

        /// <summary>
        /// constructor padrão, obtém o context 
        /// </summary>
        /// <param name="db"></param>
        public AtividadeRepository(TPAContext db)
        {
            _db = db;
        }

        #endregion



        #region métodos públicos síncronos

        /// <summary>
        /// obtém  uma atividade pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Atividade GetById(int id)
        {
            return _db.Atividades.Find(id);
        }

        /// <summary>
        /// obtém todas as atividades
        /// </summary>
        /// <returns></returns>
        public virtual List<Atividade> GetAll()
        {
            return _db.Atividades.AsNoTracking().ToList();
        }

        /// <summary>
        /// salva uma atividade, se for correto do ponto de vista do negócio fazer isso
        /// não salva a atividade caso quem esteja editando seja um usuário e o período seja fechado
        /// </summary>
        /// <param name="att">Atividade - atividade a ser salva</param>
        /// <param name="podeSalvarReferenciaFechada">bool - se true, permite salvar atividades em períodos fechados.</param>
        public virtual void Save(Atividade att, bool podeSalvarReferenciaFechada = false)
        {
            DateTime database = new DateTime(att.Inicio.Year, att.Inicio.Month, 1);
            Referencia refe = att.Usuario.GetReferencia(att.Inicio.Year, att.Inicio.Month);

            if (!refe.Fechado || podeSalvarReferenciaFechada)
            {
                if (att.ProjectNode != null)
                {
                    att.Cliente = att.ProjectNode.GetCliente();
                }

                //todo: colocar verificação de duplicidade por data inicial aqui, pesquisar para não lançar duas vezes

                if (att.Id == 0)
                {
                    _db.Atividades.Add(att);
                }

                refe.SincronizaAtividades(database, new CalendarioServices());

                _db.SaveChanges();
            }
        }

        /// <summary>
        /// salva a atividade passando sempre false, para garantir que essa chamada nunca altere períodos fechados
        /// </summary>
        /// <param name="att"></param>
        public virtual void Save(Atividade att)
        {
            this.Save(att, false);
        }

        /// <summary>
        /// exclui uma atividade se for correto fazer isso do ponte de vista do negócio
        /// </summary>
        /// <param name="atv">Atividade - atividade a ser excluída</param>
        /// <param name="podeSalvarReferenciaFechada">bool - se true, permite excluir atividades de referências fechadas</param>
        public virtual void Delete(Atividade atv, bool podeSalvarReferenciaFechada = false)
        {
            if ((atv != null)  && (_db.Atividades.Any(x => x.Id == atv.Id)))
            {
                Delete(atv.Id, podeSalvarReferenciaFechada);
            }
        }

        /// <summary>
        /// exclui uma atividade, mas sempre passando false, para assegurar que esta chamada nunca exclua atividade de período fechado
        /// </summary>
        /// <param name="atv"></param>
        public virtual void Delete(Atividade atv)
        {
            this.Delete(atv, false);
        }


        /// <summary>
        /// exclui uma atividade por id se for correto fazer isso do ponte de vista do negócio
        /// </summary>
        /// <param name="id">int - id da atividade a ser excluída</param>
        /// <param name="podeSalvarReferenciaFechada">bool - se true, permite excluir atividades de referências fechadas</param>
        public virtual void Delete(int id, bool podeSalvarReferenciaFechada = false)
        {
            Atividade atv = _db.Atividades.Find(id);
            if (atv != null)
            {
                Referencia refe = atv.Usuario.GetReferencia(atv.Inicio.Year, atv.Inicio.Month);
                if (!refe.Fechado || podeSalvarReferenciaFechada)
                {
                    _db.Atividades.Remove(atv);
                    DateTime database = new DateTime(atv.Inicio.Year, atv.Inicio.Month, 1);
                    refe.SincronizaAtividades(database, new CalendarioServices());
                    _db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// exclui uma atividade por id, mas sempre passando false, para assegurar que esta chamada nunca exclua atividade de período fechado
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(int id)
        {
            this.Delete(id, false);
        }

        #endregion





        #region métodos públicos assíncronos

        /// <summary>
        /// obtém  uma atividade pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<Atividade> GetByIdAsync(int id)
        {
            return await _db.Atividades.FindAsync(id);
        }

        /// <summary>
        /// obtém todas as atividades
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<Atividade>> GetAllAsync()
        {
            return await _db.Atividades.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// salva uma atividade, se for correto do ponto de vista do negócio fazer isso
        /// não salva a atividade caso quem esteja editando seja um usuário e o período seja fechado
        /// </summary>
        /// <param name="att">Atividade - atividade a ser salva</param>
        /// <param name="podeSalvarReferenciaFechada">bool - se true, permite salvar atividades em períodos fechados.</param>
        /// <returns></returns>
        public virtual async Task SaveAsync(Atividade att, bool podeSalvarReferenciaFechada = false)
        {
            DateTime database = new DateTime(att.Inicio.Year, att.Inicio.Month, 1);
            Referencia refe = att.Usuario.GetReferencia(att.Inicio.Year, att.Inicio.Month);

            if (!refe.Fechado || podeSalvarReferenciaFechada)
            {
                if(att.ProjectNode != null)
                {
                    att.Cliente = att.ProjectNode.GetCliente();
                }

                //todo: colocar verificação de duplicidade por data inicial aqui, pesquisar para não lançar duas vezes

                if (att.Id == 0)
                {
                    _db.Atividades.Add(att);
                }

                await _db.SaveChangesAsync();

                refe.SincronizaAtividades(database, new CalendarioServices());

                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// salva a atividade passando sempre false, para garantir que essa chamada nunca altere períodos fechados
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        public virtual async Task SaveAsync(Atividade att)
        {
            await this.SaveAsync(att, false);
        }

        /// <summary>
        /// exclui uma atividade se for correto fazer isso do ponte de vista do negócio
        /// </summary>
        /// <param name="atv">Atividade - atividade a ser excluída</param>
        /// <param name="podeSalvarReferenciaFechada">bool - se true, permite excluir atividades de referências fechadas</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Atividade atv, bool podeSalvarReferenciaFechada = false)
        {
            if ((atv != null) && (await _db.Atividades.AnyAsync(x => x.Id == atv.Id)))
            {
                await DeleteAsync(atv.Id, podeSalvarReferenciaFechada);
            }
        }

        /// <summary>
        /// exclui uma atividade, mas sempre passando false, para assegurar que esta chamada nunca exclua atividade de período fechado
        /// </summary>
        /// <param name="atv"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Atividade atv)
        {
            await this.DeleteAsync(atv, false);
        }

        /// <summary>
        /// exclui uma atividade por id se for correto fazer isso do ponte de vista do negócio
        /// </summary>
        /// <param name="id">int - id da atividade a ser excluída</param>
        /// <param name="podeSalvarReferenciaFechada">bool - se true, permite excluir atividades de referências fechadas</param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(int id, bool podeSalvarReferenciaFechada = false)
        {
            Atividade atv = _db.Atividades.Find(id);
            if (atv != null)
            {
                Referencia refe = atv.Usuario.GetReferencia(atv.Inicio.Year, atv.Inicio.Month);
                if (!refe.Fechado || podeSalvarReferenciaFechada)
                {
                    _db.Atividades.Remove(atv);
                    DateTime database = new DateTime(atv.Inicio.Year, atv.Inicio.Month, 1);
                    refe.SincronizaAtividades(database, new CalendarioServices());
                    await _db.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// exclui uma atividade por id, mas sempre passando false, para assegurar que esta chamada nunca exclua atividade de período fechado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(int id)
        {
            await this.DeleteAsync(id, false);
        }



        /// <summary>
        /// obtém todos os dados necessários para a montagem da grid analítica nas páginas home e da adividades/index
        /// </summary>
        /// <param name="usuarioIds">List de Int - ids dos usuários participantes da pesquisa que será exibida na grid da página principal</param>
        /// <param name="inicio">DateTime - data inicial</param>
        /// <param name="fim">DateTime - data final</param>
        /// <returns></returns>
        public virtual async Task<List<AtividadeIndexViewModel.AtividadeGridViewModel>> GetAtividadesAsync(List<int> usuarioIds, DateTime? inicio, DateTime? fim)
        {
            var nodes = this.GetProjectNodes();

            if (usuarioIds == null || usuarioIds.Count == 0)
            {
                usuarioIds = _db.Usuarios.Select(u => u.Id).ToList<int>();
            }

            if(inicio == null && fim == null)
            {
                inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                fim = inicio.Value.AddMonths(1).AddSeconds(-1);
            }
            else if(inicio == null)
            {
                inicio = DateTime.MinValue;
            }
            else if (fim == null)
            {
                fim = DateTime.MaxValue;
            }




            var dados = await (
                            from a in _db.Atividades
                                .AsNoTracking()
                                .Include(a => a.ProjectNode)
                                .Include(a => a.TipoAtividade)
                                .Include(a => a.Usuario)
                                .Include(a => a.Cliente)
                                .Include(a => a.Usuario.Funcionario)


                            where usuarioIds.Contains(a.Usuario.Id)
                                && a.Inicio >= inicio
                                && a.Fim < fim


                            select new
                            {
                                atvd = a
                            }).ToListAsync();

            try
            {

            

            var result = (from d in dados
                         from n in nodes
                         where d.atvd.ProjectNodeId == n.Id
                         select new AtividadeIndexViewModel.AtividadeGridViewModel
                         {
                             Id = d.atvd.Id,
                             Fim = d.atvd.Fim,
                             Inicio = d.atvd.Inicio,
                             Observacao = d.atvd.Observacao,
                             ProjectNodeNome = n != null ? n.ValorPath : d.atvd.ProjectNode.Nome,
                             ReferenciaFechado = d.atvd.Usuario.Referencias.Where(r => r.Mes == d.atvd.Inicio.Month && r.Ano == d.atvd.Inicio.Year && r.Fechado).Any(),
                             TipoAtividadeNome = d.atvd.TipoAtividade.Nome,
                             UsuarioFuncionarioNome = d.atvd.Usuario.Funcionario != null ? d.atvd.Usuario.Funcionario.Nome : d.atvd.Usuario.Login,
                             UsuarioId = d.atvd.Usuario.Id,
                             UsuarioLogin = d.atvd.Usuario.Login
                         }).ToList<AtividadeIndexViewModel.AtividadeGridViewModel>();

                return result;
            }
            catch (Exception ex)
            {

                throw;
                
            }

            
        }



        /// <summary>
        /// obtém todos os dados necessários para a montagem da grid consolidada por dia nas páginas home e da adividades/index
        /// </summary>
        /// <param name="usuarioIds">List de Int - ids dos usuários participantes da pesquisa que será exibida na grid da página principal</param>
        /// <param name="inicio">DateTime - data inicial</param>
        /// <param name="fim">DateTime - data final</param>
        /// <returns></returns>
        public virtual async Task<List<AtividadeIndexViewModel.ConsolidadoDiarioGridViewModel>> GetAtividadesConsolidadoDiarioAsync(List<int> usuarioIds, DateTime? inicio, DateTime? fim)
        {

            if (usuarioIds == null || usuarioIds.Count == 0)
            {
                usuarioIds = _db.Usuarios.Select(u => u.Id).ToList<int>();
            }

            if (inicio == null && fim == null)
            {
                inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                fim = inicio.Value.AddMonths(1).AddSeconds(-1);
            }
            else if (inicio == null)
            {
                inicio = DateTime.MinValue;
            }
            else if (fim == null)
            {
                fim = DateTime.MaxValue;
            }

            var dados = (
                            from a in _db.Atividades
                                .AsNoTracking()
                                .Include(a => a.Usuario)
                                .Include(a => a.Cliente)
                            join c in _db.ProjectNodes on a.ClienteId equals c.Id into ac
                            from c in ac.DefaultIfEmpty()
                            from r in _db.Referencias
                            
                            where usuarioIds.Contains( a.Usuario.Id ) 
                                && a.Inicio >= inicio
                                && a.Fim < fim
                                && a.Usuario.Id == r.Usuario.Id
                                && a.Inicio.Month == r.Mes
                                && a.Inicio.Year == r.Ano
                                && (a.Cliente == null || a.Cliente.Id == c.Id)
                            group new {Atividade = a, Referencia = r, Cliente = c } 
                            by new 
                            {
                                Data = DbFunctions.TruncateTime(a.Inicio),
                                Usuario = a.Usuario,
                                Fechado = r.Fechado
                            }
                            into g
                            select new AtividadeIndexViewModel.ConsolidadoDiarioGridViewModel
                            {
                                Data = g.Key.Data??DateTime.MinValue,
                                ClienteNome = g.FirstOrDefault().Cliente!=null? g.FirstOrDefault().Cliente.Nome:"",
                                UsuarioId = g.Key.Usuario.Id,
                                UsuarioLogin = g.Key.Usuario.Login,
                                UsuarioFuncionarioNome = g.Key.Usuario.Funcionario!=null? g.Key.Usuario.Funcionario.Nome: g.Key.Usuario.Login,
                                Segundos =  g.Sum(t => DbFunctions.DiffSeconds(t.Atividade.Inicio, t.Atividade.Fim)) ?? 0
                            }
                         );

            return await dados.ToListAsync<AtividadeIndexViewModel.ConsolidadoDiarioGridViewModel>();
        }


        /// <summary>
        /// obtém os dados de atividades consolidados por cliente no período especificado
        /// </summary>
        /// <param name="usuarioIds">List de Int - ids dos usuários participantes da pesquisa que será exibida na grid da página principal</param>
        /// <param name="inicio">DateTime - data inicial</param>
        /// <param name="fim">DateTime - data final</param>
        /// <returns></returns>
        public virtual async Task<List<AtividadeIndexViewModel.ConsolidadoDiarioGridViewModel>> GetAtividadesConsolidadoDiarioPorClienteAsync(List<int> usuarioIds, DateTime? inicio, DateTime? fim)
        {
            if (usuarioIds == null || usuarioIds.Count == 0)
            {
                usuarioIds = _db.Usuarios.Select(u => u.Id).ToList<int>();
            }


            if (inicio == null && fim == null)
            {
                inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                fim = inicio.Value.AddMonths(1).AddSeconds(-1);
            }
            else if (inicio == null)
            {
                inicio = DateTime.MinValue;
            }
            else if (fim == null)
            {
                fim = DateTime.MaxValue;
            }

            var dados = (
                            from a in _db.Atividades
                                .AsNoTracking()
                                .Include(a => a.Usuario)
                                .Include(a => a.Cliente)
                            from r in _db.Referencias
                            where usuarioIds.Contains(a.Usuario.Id)
                                && a.Inicio >= inicio
                                && a.Fim < fim
                                && a.Usuario.Id == r.Usuario.Id
                                && a.Inicio.Month == r.Mes
                                && a.Inicio.Year == r.Ano
                            group new { Atividade = a, Referencia = r }
                            by new
                            {
                                Data = DbFunctions.TruncateTime(a.Inicio),
                                Usuario = a.Usuario,
                                Fechado = r.Fechado,
                                Cliente = a.Cliente
                            }
                            into g
                            select new AtividadeIndexViewModel.ConsolidadoDiarioGridViewModel
                            {
                                Data = g.Key.Data ?? DateTime.MinValue,
                                ClienteNome = g.Key.Cliente != null ? g.Key.Cliente.Nome : "",
                                UsuarioId = g.Key.Usuario.Id,
                                UsuarioLogin = g.Key.Usuario.Login,
                                UsuarioFuncionarioNome = g.Key.Usuario.Funcionario != null ? g.Key.Usuario.Funcionario.Nome : g.Key.Usuario.Login,
                                Segundos = g.Sum(t => DbFunctions.DiffSeconds(t.Atividade.Inicio, t.Atividade.Fim)) ?? 0
                            }
                         );

            return await dados.ToListAsync<AtividadeIndexViewModel.ConsolidadoDiarioGridViewModel>();
        }



        #endregion









        #region métodos privados
        /// <summary>
        /// Carrega ou recarrega todos os projectnodes em uma estrutura TFWHierarquicalList e traz o elemento correspondente
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TFWHierarchicalParameter GetProjectNode(int id)
        {
            if (AreNewProjectNodes())
            {
                _projectNodes.Clear();
                foreach (ProjectNode pn in _db.ProjectNodes)
                {
                    _projectNodes.Add(pn.Id, pn.Pai_Id, pn.Nome);
                }
            }

            return _projectNodes.Where(p => p.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Carrega ou recarrega todos os projectnodes em uma estrutura TFWHierarquicalList
        /// </summary>
        /// <returns></returns>
        public virtual TFWHierarchicalList GetProjectNodes()
        {
            if (AreNewProjectNodes())
            {
                _projectNodes.Clear();
                foreach (ProjectNode pn in _db.ProjectNodes.AsNoTracking())
                {
                    _projectNodes.Add(pn.Id, pn.Pai_Id, pn.Nome);
                }
            }

            return _projectNodes;
        }

        /// <summary>
        /// verifica se a estrutura está nula ou vazia
        /// </summary>
        /// <returns></returns>
        public virtual bool AreNewProjectNodes()
        {

            return ((_projectNodes.Count == 0)|| (_projectNodes.Count < _db.ProjectNodes.AsNoTracking().Count()));

        }

        #endregion


    }
}