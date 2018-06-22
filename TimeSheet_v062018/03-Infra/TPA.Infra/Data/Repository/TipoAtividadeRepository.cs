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
    public class TipoAtividadeRepository : ITPARepository<TipoAtividade>, ITPARepositoryAsync<TipoAtividade>
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
        public TipoAtividadeRepository(TPAContext context)
        {
            this._db = context;
        }

        #endregion


        #region métodos CRUD públicos síncronos

        /// <summary>
        /// obtém um tipo de atividade pelo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TipoAtividade GetById(int id)
        {
            return _db.TiposAtividade.Find(id);
        }

        /// <summary>
        /// obtém todos os tipos de atividades
        /// </summary>
        /// <returns></returns>
        public virtual List<TipoAtividade> GetAll()
        {
            return _db.TiposAtividade.AsNoTracking().ToList();
        }

        /// <summary>
        /// salva um tipo de atividade
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Save(TipoAtividade ent)
        {
            if ((ent.Id == 0) || (!_db.TiposAtividade.Any(x => x.Id == ent.Id)))
            {
                _db.TiposAtividade.Add(ent);
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// exclui um tipo de atividade
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Delete(TipoAtividade ent)
        {
            if ((ent != null) && (_db.TiposAtividade.Any(x => x.Id == ent.Id)))
            {
                var deletando = _db.TiposAtividade.Find(ent.Id);
                if (deletando != null)
                {
                    _db.TiposAtividade.Remove(deletando);
                    _db.SaveChanges();
                }
            }
        }
                
        /// <summary>
        /// obtém todos os tipos de atividade públicos
        /// </summary>
        /// <returns></returns>
        public virtual List<TipoAtividade> GetPublic()
        {

            var dados = _db.TiposAtividade.Where(atv => atv.Administrativo != true).ToList();

            return dados;
        }

        /// <summary>
        /// obtém todos os tipos de atividade administrativas
        /// </summary>
        /// <returns></returns>
        public virtual List<TipoAtividade> GetAdmin()
        {

            var dados = _db.TiposAtividade.Where(atv => atv.Administrativo == true).ToList();

            return dados;
        }

        #endregion


        #region métodos CRUD públicos assíncronos

        /// <summary>
        /// obtém um tipo de atividade por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TipoAtividade> GetByIdAsync(int id)
        {
            return await _db.TiposAtividade.FindAsync(id);
        }

        /// <summary>
        /// obtém todos os tipos de atividade
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TipoAtividade>> GetAllAsync()
        {
            return await _db.TiposAtividade.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// salva um tipo de atividade
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task SaveAsync(TipoAtividade ent)
        {
            if ((ent.Id == 0) || (! await _db.TiposAtividade.AnyAsync(x => x.Id == ent.Id)))
            {
                _db.TiposAtividade.Add(ent);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// exclui um tipo de atividade
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(TipoAtividade ent)
        {
            if ((ent != null) && (await _db.TiposAtividade.AnyAsync(x => x.Id == ent.Id)))
            {
                var deletando = await _db.TiposAtividade.FindAsync(ent.Id);
                if (deletando != null)
                {
                    _db.TiposAtividade.Remove(deletando);
                    await _db.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// obtém todos os tipos de atividade públicos
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TipoAtividade>> GetPublicAsync()
        {

            var dados = await _db.TiposAtividade.Where(atv => atv.Administrativo != true).ToListAsync();

            return dados;
        }


        /// <summary>
        /// obtém todos os tipos de atividades administrativos
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TipoAtividade>> GetAdminAsync()
        {

            var dados = await _db.TiposAtividade.Where(atv => atv.Administrativo == true).ToListAsync();

            return dados;
        }


        #endregion

    }
}
