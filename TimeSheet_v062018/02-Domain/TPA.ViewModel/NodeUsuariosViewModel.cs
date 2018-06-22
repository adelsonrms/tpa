using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace TPA.ViewModel
{

    /// <summary>
    /// Lista de usuários do node, para atribuição de recursos no treeview
    /// </summary>
    public class NodeUsuariosViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// id do node
        /// </summary>
        public virtual int IdNode {get; set;}

        /// <summary>
        /// flag booleana para adicionar recursos recursivamente
        /// </summary>
        public virtual bool AdicionarRecursivo { get; set; }

        /// <summary>
        /// ids dos usuários
        /// </summary>
        public virtual List<int> IdsUsuarios { get; set; }

        /// <summary>
        /// objeto para passar serializado para o treeview
        /// </summary>
        public virtual object Objeto { get; set; }

        #endregion


    }
}
