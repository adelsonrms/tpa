using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPA.Domain.DomainModel;
using System.Data.Entity;

namespace TPA.Infra.Data.Repository
{
    /// <summary>
    /// classe repositório para TipoAtividade
    /// </summary>
    public class AcaoRepository : ITPARepository<Acao>, ITPARepositoryAsync<Acao>
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
        public AcaoRepository(TPAContext context)
        {
            this._db = context;
        }

        #endregion


        #region métodos CRUD públicos síncronos

        /// <summary>
        /// traz uma ação de usuário/ação do sistema/permissão pelo id
        /// </summary>
        /// <param name="id">int - id da ação</param>
        /// <returns></returns>
        public virtual Acao GetById(int id)
        {
            return _db.Acoes.Find(id);
        }

        /// <summary>
        /// obtém a lista de todas as ações do sistema
        /// </summary>
        /// <returns></returns>
        public virtual List<Acao> GetAll()
        {
            return _db.Acoes.AsNoTracking().ToList();
        }

        /// <summary>
        /// salva uma ação do sistema
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Save(Acao ent)
        {
            if ((ent.Id == 0) || (!_db.Acoes.Any(x => x.Id == ent.Id)))
            {
                _db.Acoes.Add(ent);
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// exclui uma ação do sistema
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Delete(Acao ent)
        {
            if ((ent != null) && (_db.Acoes.Any(x => x.Id == ent.Id)))
            {
                var deletando = _db.Acoes.Find(ent.Id);
                if (deletando != null)
                {
                    _db.Acoes.Remove(deletando);
                    _db.SaveChanges();
                }
            }
        }

        
        /// <summary>
        /// obtém uma ação do sistema única pelo nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public virtual Acao GetByName(string nome)
        {
            return _db.Acoes.Where(a => a.Nome == nome).SingleOrDefault();
        }


        /// <summary>
        /// verifica se uma ação existe pelo seu nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public virtual bool Exists(string nome)
        {
            return _db.Acoes.Where(a => a.Nome == nome).Any();
        }


        #endregion


        #region métodos CRUD públicos assíncronos

        /// <summary>
        /// traz uma ação de usuário/ação do sistema/permissão pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<Acao> GetByIdAsync(int id)
        {
            return await _db.Acoes.FindAsync(id);
        }

        /// <summary>
        /// obtém a lista de todas as ações do sistema
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<Acao>> GetAllAsync()
        {
            return await _db.Acoes.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// salva uma ação do sistema
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task SaveAsync(Acao ent)
        {
            if ((ent.Id == 0) || (! await _db.Acoes.AnyAsync(x => x.Id == ent.Id)))
            {
                _db.Acoes.Add(ent);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// exclui uma ação do sistema
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Acao ent)
        {
            if ((ent != null) && (await _db.Acoes.AnyAsync(x => x.Id == ent.Id)))
            {
                var deletando = await _db.Acoes.FindAsync(ent.Id);
                if (deletando != null)
                {
                    _db.Acoes.Remove(deletando);
                    await _db.SaveChangesAsync();
                }
            }
        }


        /// <summary>
        /// obtém uma ação do sistema única pelo nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public virtual async Task<Acao> GetByNameAsync(string nome)
        {
            return await _db.Acoes.Where(a => a.Nome == nome).SingleOrDefaultAsync();
        }

        /// <summary>
        /// verifica se uma ação existe pelo seu nome
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(string nome)
        {
            return await _db.Acoes.Where(a => a.Nome == nome).AnyAsync();
        }


        #endregion


    }
}
