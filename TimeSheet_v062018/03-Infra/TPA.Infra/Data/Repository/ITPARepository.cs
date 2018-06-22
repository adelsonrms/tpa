using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TPA.Domain.DomainModel;

namespace TPA.Infra.Data.Repository
{
    /// <summary>
    /// Interfce de repositório genérico
    /// </summary>
    public interface ITPARepository<T> where T : TPAEntity
    {
        /// <summary>
        /// obtém um elemento pelo seu id
        /// </summary>
        /// <param name="id">int - o id da entidade </param>
        /// <returns>T - objeto encontrado</returns>
        T GetById(int id);

        /// <summary>
        /// obtém todos os objetos da base
        /// </summary>
        /// <returns>List de T</returns>
        List<T> GetAll();

        /// <summary>
        /// salva ou atualiza um objeto na base
        /// </summary>
        /// <param name="ent">T - objeto a ser salvo</param>
        void Save(T ent);


        /// <summary>
        /// exclui um objeto da base
        /// </summary>
        /// <param name="ent">T - objeto a ser excluído</param>
        void Delete(T ent); 
        
    }
}