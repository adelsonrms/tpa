using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TPA.Domain.DomainModel;

namespace TPA.Infra.Data.Repository
{
    public interface ITPARepositoryAsync<T> where T : TPAEntity
    {

        /// <summary>
        /// obtém um elemento pelo seu id
        /// Assíncrono
        /// </summary>
        /// <param name="id">int - o id da entidade </param>
        /// <returns>Task de T - o bojeto retornado do banco, assíncrono</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// obtém todos os objetos da base
        /// Assíncrono
        /// </summary>
        /// <returns>Task de List de T - lista com todos os objetos retornados do banco</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// salva ou atualiza um objeto na base
        /// Assíncrono
        /// </summary>
        /// <param name="ent">T - objeto a ser salvo</param>
        /// <returns>Task</returns>
        Task SaveAsync(T ent);


        /// <summary>
        /// exclui um objeto da base
        /// Assíncrono
        /// </summary>
        /// <param name="ent">T - objeto a ser excluído</param>
        /// <returns>Task</returns>
        Task DeleteAsync(T ent);
    }
}