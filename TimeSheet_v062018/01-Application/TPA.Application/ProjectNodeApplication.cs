using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFW.Domain;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;
using TPA.Infra.Data.Repository;

namespace TPA.Application
{
    /// <summary>
    /// classe de controle / regras de negócio / application para lidar com ProjectNodes, consultar, listar e montar hierarquias
    /// </summary>
    public class ProjectNodeApplication
    {
        #region fileds privados

        /// <summary>
        /// contexto para as queries
        /// </summary>
        private TPAContext _db;

        /// <summary>
        /// repositório dos projectnodes
        /// </summary>
        private ProjectNodeRepository _rep;

        /// <summary>
        /// cache dos projectnodes em forma TFWHierarchicalList
        /// </summary>
        private TFWHierarchicalList _listNodes;

        #endregion


        #region constructors

        /// <summary>
        /// cosntructor principal
        /// </summary>
        /// <param name="context">TPAContext - contexto do EF para as queries</param>
        public ProjectNodeApplication(TPAContext context)
        {
            this._db = context;
            _rep = new ProjectNodeRepository(this._db);
        }

        #endregion



        #region métodos CRUD públicos síncronos

        /// <summary>
        /// obtém o projectnode pelo id
        /// </summary>
        /// <param name="id">int - id do ProjectNode</param>
        /// <returns>ProjectNode encontrado</returns>
        public virtual ProjectNode GetById(int id)
        {
            return _rep.GetById(id);
        }

        /// <summary>
        /// obtém todos os nós de projeto
        /// </summary>
        /// <returns>List de ProjectNode</returns>
        public virtual List<ProjectNode> GetAll()
        {
            return _rep.GetAll();
        }

        /// <summary>
        /// salva um ProjectNode
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Save(ProjectNode ent)
        {
            _rep.Save(ent);
        }

        /// <summary>
        /// exclui um ProjectNode
        /// </summary>
        /// <param name="ent"></param>
        public virtual void Delete(ProjectNode ent)
        {
            _rep.Delete(ent);
        }

        /// <summary>
        /// obtém os ProjectNodes organizados na forma de TFWHierarchicalList, com lazy load
        /// </summary>
        /// <returns>TFWHierarchicalList - estrutura hierárquica com todos os ProjectNodes</returns>
        public virtual TFWHierarchicalList GetNodeTree()
        {
            if (_listNodes == null)
                _listNodes = new TFWHierarchicalList();

            if(_listNodes.Count == 0)
            {
                foreach(ProjectNode n in _rep.GetAll())
                {
                    _listNodes.Add(n.Id, n.Pai_Id, n.Nome);
                }
            }

            TFWHierarchicalList result = new TFWHierarchicalList();
            foreach(TFWHierarchicalParameter p in _listNodes)
            {
                result.Add(p.Id, p.IdPai, p.Valor);
            }

            return result;
        }

        #endregion


        #region métodos CRUD públicos assíncronos

        /// <summary>
        /// obtém o projectnode pelo id
        /// </summary>
        /// <param name="id">int - id do ProjectNode</param>
        /// <returns>ProjectNode encontrado</returns>
        public virtual async Task<ProjectNode> GetByIdAsync(int id)
        {
            return await _rep.GetByIdAsync(id);
        }

        /// <summary>
        /// obtém todos os nós de projeto
        /// </summary>
        /// <returns>List de ProjectNode</returns>
        public virtual async Task<List<ProjectNode>> GetAllAsync()
        {
            return await _rep.GetAllAsync();
        }

        /// <summary>
        /// salva um ProjectNode
        /// </summary>
        /// <param name="ent"></param>
        public virtual async Task SaveAsync(ProjectNode ent)
        {
            await _rep.SaveAsync(ent);
        }

        /// <summary>
        /// exclui um ProjectNode
        /// </summary>
        /// <param name="ent"></param>
        public virtual async Task DeleteAsync(ProjectNode ent)
        {
            await _rep.DeleteAsync(ent);
        }


        /// <summary>
        /// obtém os ProjectNodes organizados na forma de TFWHierarchicalList, com lazy load
        /// </summary>
        /// <returns>TFWHierarchicalList - estrutura hierárquica com todos os ProjectNodes</returns>
        public virtual async Task<TFWHierarchicalList> GetNodeTreeAsync()
        {
            if (_listNodes == null)
                _listNodes = new TFWHierarchicalList();

            if (_listNodes.Count == 0)
            {
                foreach (ProjectNode n in await _rep.GetAllAsync())
                {
                    _listNodes.Add(n.Id, n.Pai_Id, n.Nome);
                }
            }

            TFWHierarchicalList result = new TFWHierarchicalList();
            foreach (TFWHierarchicalParameter p in _listNodes)
            {
                result.Add(p.Id, p.IdPai, p.Valor);
            }

            return result;
        }



        #endregion

    }
}
