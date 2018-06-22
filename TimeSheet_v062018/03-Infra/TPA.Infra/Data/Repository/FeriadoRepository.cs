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
    public class FeriadoRepository : ITPARepository<Feriado>, ITPARepositoryAsync<Feriado>
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
        public FeriadoRepository(TPAContext context)
        {
            this._db = context;
        }

        #endregion


        #region métodos CRUD públicos síncronos

        /// <summary>
        /// obtém um feriado por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Feriado GetById(int id)
        {
            return _db.Feriados.Find(id);
        }

        /// <summary>
        /// obtém todos os feriados
        /// </summary>
        /// <returns></returns>
        public virtual List<Feriado> GetAll()
        {
            return _db.Feriados.AsNoTracking().ToList();
        }

        /// <summary>
        /// salva um feriado
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Save(Feriado ent)
        {
            if ((ent.Id == 0) || (!_db.Feriados.Any(x => x.Id == ent.Id)))
            {
                _db.Feriados.Add(ent);
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// exclui um feriado
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Delete(Feriado ent)
        {
            if ((ent != null) && (_db.Feriados.Any(x => x.Id == ent.Id)))
            {
                var deletando = _db.Feriados.Find(ent.Id);
                if (deletando != null)
                {
                    _db.Feriados.Remove(deletando);
                    _db.SaveChanges();
                }
            }
        }
                
        /// <summary>
        /// obtém um feriado pela data, traz null se não for feriado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual List<Feriado> GetByData(DateTime data)
        {
            DateTime apenasData = data.Date;
            return this._db.Feriados.Where(f => f.Data == apenasData).ToList();
        }


        #endregion


        #region métodos CRUD públicos assíncronos

        /// <summary>
        /// obtém um feriado por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<Feriado> GetByIdAsync(int id)
        {
            return await _db.Feriados.FindAsync(id);
        }

        /// <summary>
        /// obtém todos os feriados
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<Feriado>> GetAllAsync()
        {
            return await _db.Feriados.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// salva um feriado
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task SaveAsync(Feriado ent)
        {
            if ((ent.Id == 0) || (! await _db.Feriados.AnyAsync(x => x.Id == ent.Id)))
            {
                _db.Feriados.Add(ent);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// exclui um feriado
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(Feriado ent)
        {
            if ((ent != null) && (await _db.Feriados.AnyAsync(x => x.Id == ent.Id)))
            {
                var deletando = await _db.Feriados.FindAsync(ent.Id);
                if (deletando != null)
                {
                    _db.Feriados.Remove(deletando);
                    await _db.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// obtém um feriado pela data, traz null se não for feriado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async  Task<List<Feriado>> GetByDataAsync(DateTime data)
        {
            DateTime apenasData = data.Date;
            return await this._db.Feriados.Where(f => f.Data == apenasData).ToListAsync();
        }

        #endregion

    }
}
