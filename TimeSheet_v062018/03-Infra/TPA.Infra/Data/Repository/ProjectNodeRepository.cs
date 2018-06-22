using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TPA.Domain.DomainModel;
using System.Data.Entity;

namespace TPA.Infra.Data.Repository
{
    /// <summary>
    /// classe repositório para ProjectNode
    /// </summary>
    public class ProjectNodeRepository : ITPARepository<ProjectNode>, ITPARepositoryAsync<ProjectNode>
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
        public ProjectNodeRepository(TPAContext context)
        {
            this._db = context;
        }

        #endregion


        #region métodos CRUD públicos síncronos

        /// <summary>
        /// obtém um node por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ProjectNode GetById(int id)
        {
            return _db.ProjectNodes.Find(id);
        }

        /// <summary>
        /// obtém todos os nodes
        /// </summary>
        /// <returns></returns>
        public virtual List<ProjectNode> GetAll()
        {
            return _db.ProjectNodes.AsNoTracking().ToList();
        }

        /// <summary>
        /// salva um  node
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Save(ProjectNode ent)
        {
            if ((ent.Id == 0) || (!_db.ProjectNodes.Any(x => x.Id == ent.Id)))
            {
                _db.ProjectNodes.Add(ent);
            }

            _db.SaveChanges();
        }

        /// <summary>
        /// exclui um node
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Delete(ProjectNode ent)
        {
            if ((ent != null) && (_db.ProjectNodes.Any(x => x.Id == ent.Id)))
            {
                var deletando = _db.ProjectNodes.Find(ent.Id);
                if (deletando != null)
                {
                    _db.ProjectNodes.Remove(deletando);
                    _db.SaveChanges();
                }
            }
        }

        #endregion


        #region métodos CRUD públicos assíncronos

        /// <summary>
        /// obtém um node por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ProjectNode> GetByIdAsync(int id)
        {
            return await _db.ProjectNodes.FindAsync(id);
        }

        /// <summary>
        /// obtém todos os nodes
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<ProjectNode>> GetAllAsync()
        {
            return await _db.ProjectNodes.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// salva um node
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task SaveAsync(ProjectNode ent)
        {
            if ((ent.Id == 0) || (! await _db.ProjectNodes.AnyAsync(x => x.Id == ent.Id)))
            {
                _db.ProjectNodes.Add(ent);
            }

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// exclui um node
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task DeleteAsync(ProjectNode ent)
        {
            if ((ent != null) && (await _db.ProjectNodes.AnyAsync(x => x.Id == ent.Id)))
            {
                var deletando = await _db.ProjectNodes.FindAsync(ent.Id);
                if (deletando != null)
                {
                    _db.ProjectNodes.Remove(deletando);
                    await _db.SaveChangesAsync();
                }
            }
        }

        #endregion


    }
}
