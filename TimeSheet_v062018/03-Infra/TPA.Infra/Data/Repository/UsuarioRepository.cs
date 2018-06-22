using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPA.Domain.DomainModel;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Core.Objects;
using TPA.ViewModel;

namespace TPA.Infra.Data.Repository
{
    /// <summary>
    /// classe repositório para Usuario
    /// </summary>
    public class UsuarioRepository : ITPARepository<Usuario>, ITPARepositoryAsync<Usuario>
    {


        #region propriedades estáticas

        /// <summary>
        /// contexto para as queries
        /// </summary>
        private TPAContext _db;

        #endregion


        #region constructors

        /// <summary>
        /// cosntructor principal
        /// </summary>
        /// <param name="context">TPAContext - contexto do EF para as queries</param>
        public UsuarioRepository(TPAContext context)
        {
            this._db = context;
        }

        #endregion


        #region métodos CRUD públicos síncronos

        /// <summary>
        /// obtém um usuário pelo seu Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Usuario GetById(int id)
        {
            return _db.Usuarios.Find(id);
        }

        /// <summary>
        /// obtém todos os usuários, sem tracking
        /// </summary>
        /// <returns></returns>
        public virtual List<Usuario> GetAll()
        {
            return _db.Usuarios
                .Include(x => x.Funcionario)
                .AsNoTracking()
                .ToList();
        }

        /// <summary>
        /// salva um usuário
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Save(Usuario ent)
        {
            if ((ent.Id == 0) || (!_db.Usuarios.Any(x => x.Id == ent.Id)))
            {
                _db.Usuarios.Add(ent);
            }
            else
            {
                _db.Entry(ent).State = EntityState.Modified;
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// exclui um usuário
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Delete(Usuario ent)
        {
            if ((ent != null) && (_db.Usuarios.Any(x => x.Id == ent.Id)))
            {
                var deletando = _db.Usuarios.Find(ent.Id);
                if (deletando != null)
                {
                    _db.Usuarios.Remove(deletando);
                    _db.SaveChanges();
                }
            }
        }

        
        /// <summary>
        /// obtém os últimos lançamentos de cada usuário e o número de dias sem lançar
        /// </summary>
        /// <returns></returns>
        public virtual List<UsuarioAlertaLancamentoViewModel> GetUltimosLancamentos()
        {
            List<UsuarioAlertaLancamentoViewModel> result = new List<UsuarioAlertaLancamentoViewModel>();

            DateTime hoje = DateTime.Today;

            var query = (
                                from a in _db.Atividades
                                    .Include(atv => atv.Usuario)
                                    .AsNoTracking()
                                where a.Usuario.Ativo == true
                                group a by a.Usuario into g
                                select new UsuarioAlertaLancamentoViewModel
                                {
                                    Usuario = g.Key,
                                    UltimoLancamento = g.Max(t => DbFunctions.TruncateTime(t.Fim)),
                                    DiasSemLancar = DbFunctions.DiffDays(g.Max(t => DbFunctions.TruncateTime(t.Fim)), hoje)
                                }

                        );
            result.AddRange(query.ToList());
            return result;
        }


        /// <summary>
        /// obter o último lançamento de um usuário com a quantidade de dias do último lançamento 
        /// </summary>
        /// <returns></returns>
        public virtual UsuarioAlertaLancamentoViewModel GetUltimosLancamentosById(int id)
        {
            DateTime hoje = DateTime.Today;

            var query = (
                                from a in _db.Atividades
                                    .Include(atv => atv.Usuario)
                                    .AsNoTracking()
                                where a.Usuario.Id == id
                                && a.Usuario.Ativo == true
                                group a by a.Usuario into g
                                select new UsuarioAlertaLancamentoViewModel
                                {
                                    Usuario = g.Key,
                                    UltimoLancamento = g.Max(t => DbFunctions.TruncateTime(t.Fim)),
                                    DiasSemLancar = DbFunctions.DiffDays(g.Max(t => DbFunctions.TruncateTime(t.Fim)), hoje)
                                }


                        );

            return  query.SingleOrDefault();
        }


        /// <summary>
        /// verifica se um e-mail já existe antes de cadastrar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual bool VerificaExistenciaEmail(string email, int? id = null)
        {
            var query = from u in _db.Usuarios
                        .Include(u => u.Funcionario)
                        where (u.Id != id && ((u.Login == email) || (u.Funcionario != null && u.Funcionario.EmailProfissional == email) || (u.Funcionario != null && u.Funcionario.EmailPessoal == email)))
                        select u;

            return query.Any();
        }

        #endregion


        #region métodos CRUD públicos assíncronos

        /// <summary>
        /// obtém um usuário pelo seu id de forma assíncrona
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<Usuario> GetByIdAsync(int id)
        {
            return await _db.Usuarios.FindAsync(id);
        }


        /// <summary>
        /// obtém todos os usuários sem tracking de forma assíncrona
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<Usuario>> GetAllAsync()
        {
            return await _db.Usuarios
                .Include(x => x.Funcionario)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// salva um usuário de forma assíncrona
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task SaveAsync(Usuario ent)
        {
            if ((ent.Id == 0) || (! await _db.Usuarios.AnyAsync(x => x.Id == ent.Id)))
            {
                _db.Usuarios.Add(ent);
            }
            else
            {
                _db.Entry(ent).State = EntityState.Modified;
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// excluir um usuário de forma assíncrona
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Usuario ent)
        {
            if ((ent != null) && (await _db.Usuarios.AnyAsync(x => x.Id == ent.Id)))
            {
                var deletando = await _db.Usuarios.FindAsync(ent.Id);
                if (deletando != null)
                {
                    _db.Usuarios.Remove(deletando);
                    await _db.SaveChangesAsync();
                }
            }
        }


        /// <summary>
        /// obter o último lançamento de cada usuário com a quantidade de dias do último lançamento de forma assíncrona
        /// </summary>
        /// <returns></returns>
        public virtual async Task< List<UsuarioAlertaLancamentoViewModel>> GetUltimosLancamentosAsync()
        {
            List<UsuarioAlertaLancamentoViewModel> result = new List<UsuarioAlertaLancamentoViewModel>();

            DateTime hoje = DateTime.Today;

            var query = (
                                from a in _db.Atividades
                                    .Include(atv => atv.Usuario)
                                    .AsNoTracking()
                                where a.Usuario.Ativo == true
                                group a by a.Usuario into g
                                select new UsuarioAlertaLancamentoViewModel
                                {
                                    Usuario = g.Key,
                                    UltimoLancamento = g.Max(t => DbFunctions.TruncateTime( t.Fim)),
                                    DiasSemLancar = DbFunctions.DiffDays(g.Max(t => DbFunctions.TruncateTime(t.Fim)), hoje)
                                }
                                

                        );
            result.AddRange(await query.ToListAsync());
            return  result;
        }



        /// <summary>
        /// obter o último lançamento de um usuário com a quantidade de dias do último lançamento de forma assíncrona
        /// </summary>
        /// <returns></returns>
        public virtual async Task<UsuarioAlertaLancamentoViewModel> GetUltimosLancamentosByIdAsync(int id)
        {
            DateTime hoje = DateTime.Today;

            var query = (
                                from a in _db.Atividades
                                    .Include(atv => atv.Usuario)
                                    .AsNoTracking()
                                where a.Usuario.Id == id
                                && a.Usuario.Ativo == true
                                group a by a.Usuario into g
                                select new UsuarioAlertaLancamentoViewModel
                                {
                                    Usuario = g.Key,
                                    UltimoLancamento = g.Max(t => DbFunctions.TruncateTime(t.Fim)),
                                    DiasSemLancar = DbFunctions.DiffDays(g.Max(t => DbFunctions.TruncateTime(t.Fim)), hoje)
                                }


                        );

            return await query.SingleOrDefaultAsync();
        }



        /// <summary>
        /// verifica se um e-mail já existe antes de cadastrar
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task< bool> VerificaExistenciaEmailAsync(string email, int? id = null)
        {
            var query = from u in _db.Usuarios
                        .Include(u => u.Funcionario)
                        where (u.Id != id && ((u.Login == email) || (u.Funcionario != null && u.Funcionario.EmailProfissional == email) || (u.Funcionario != null && u.Funcionario.EmailPessoal == email)))
                        select u;

            return await query.AnyAsync();
        }

        #endregion
    }
}
