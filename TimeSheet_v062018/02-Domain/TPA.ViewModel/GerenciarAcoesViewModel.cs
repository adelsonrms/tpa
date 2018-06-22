using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel
{
    /// <summary>
    /// viewmodel para a action de gerenciar ações
    /// </summary>
    public class GerenciarAcoesViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// id do perfil que está sendo editado
        /// </summary>
        public virtual int IdPerfil { get; set; }

        /// <summary>
        /// cache do nome do perfil para exibição na view
        /// </summary>
        public virtual string NomePerfil { get; set; }


        /// <summary>
        /// lista de controllers
        /// </summary>
        public virtual List<PerfilGerenciarControllerViewModel> Controllers { get; set; }

        #endregion


    }
}
