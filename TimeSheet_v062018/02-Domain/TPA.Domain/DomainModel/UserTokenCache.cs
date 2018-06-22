using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;

namespace TPA.Domain.DomainModel
{

    /// <summary>
    /// usado pelo open id para fazer um cache do token do usuário
    /// </summary>
    public class UserTokenCache
    {

        #region propriedades públicas

        /// <summary>
        /// id do token
        /// </summary>
        [Key]
        public int UserTokenCacheId { get; set; }

        /// <summary>
        /// id unico do usuario
        /// </summary>
        public string webUserUniqueId { get; set; }

        /// <summary>
        /// bytes do token
        /// </summary>
        public byte[] cacheBits { get; set; }

        /// <summary>
        /// data da última escrita
        /// </summary>
        public DateTime LastWrite { get; set; }

        #endregion


    }
}