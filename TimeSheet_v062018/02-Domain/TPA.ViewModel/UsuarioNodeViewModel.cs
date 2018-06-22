using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TFW.Domain;
using TPA.Domain.DomainModel;

namespace TPA.ViewModel
{

    /// <summary>
    /// classe que associa um usuário a um ProjectNode
    /// Essa classe não é uma regra de negócio em si, mas serve para facilitar a escolha do usuário
    /// Embora ele poderia estar fazendo outras atividades, aparecerá nos combos apenas as atividades 
    /// que se espera que ele estivesse fazendo
    /// </summary>
    public class UsuarioNodeViewModel
    {

        #region classe UserNodesViewModel
        /// <summary>
        /// classe ViewModel para compor uma lista dos nodes que o usuário já possui
        /// </summary>
        public class UserNodesViewModel
        {
            /// <summary>
            /// id do node
            /// </summary>
            public virtual int Id { get; set; }

            /// <summary>
            /// nome do node
            /// </summary>
            public virtual string Nome { get; set; }

            /// <summary>
            /// path do node
            /// </summary>
            public virtual string Path { get; set; }
        }

        #endregion



        #region propriedades públicas

        /// <summary>
        /// objeto usuário (para as visualizações dos retornos)
        /// </summary>
        public virtual Usuario Usuario { get; set; }

        /// <summary>
        /// objeto node  (para as visualizações dos retornos)
        /// </summary>
        public virtual ProjectNode Node { get; set; }

        /// <summary>
        /// flag, se for true, os nodes são adicionados/removidos recursivamente,
        /// caso contrário apenas o node associado é adicionado/removido
        /// </summary>
        public virtual bool AdicionarRecursivo { get; set; }


        /// <summary>
        /// id do usuário
        /// </summary>
        [Required]
        [Display(Name = "Usuário")]
        public virtual int IdUsuario { get; set; }

        /// <summary>
        /// id do node
        /// </summary>
        [Required]
        [Display(Name = "Project Node")]
        public virtual int IdNode { get; set; }

        #endregion



        #region métodos públicos

        /// <summary>
        /// obtém uma lista de UserNodesViewModel que são todos os nodes desse usuário, para as construções dos combos
        /// </summary>
        /// <returns>list de UserNodesViewModel</returns>
        public virtual List<UserNodesViewModel> GetNosDoUsuario()
        {
            List<UserNodesViewModel> result = new List<UserNodesViewModel>();

            if(this.Usuario != null && this.Usuario.NosDoUsuario != null && this.Usuario.NosDoUsuario.Any())
            {
                foreach(var n in this.Usuario.NosDoUsuario)
                {
                    result.Add(new UserNodesViewModel
                    {
                        Id = n.Id,
                        Nome = n.Nome,
                        Path = n.NomePath
                    });
                }
            }

            return result;
        }

        #endregion

    }
}