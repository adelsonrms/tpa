using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TPA.Domain.DomainModel;
using TPA.ViewModel.Buscas;

namespace TPA.ViewModel
{
    /// <summary>
    /// ViewModel para o index do controller de fechamento
    /// </summary>
    public class FechamentoIndexViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// lista de usuários para a página index e busca dos fechamentos (popular combo)
        /// </summary>
        [Display(Name = "Funcionários")]
        public virtual List<Usuario> Usuarios { get; set; }

        /// <summary>
        /// FechamentoBuscaViewModel encapsula os parâmetros de busca
        /// </summary>
        [Display(Name = "Busca")]
        public virtual FechamentoBuscaViewModel Busca {get; set;}

        /// <summary>
        /// mêses de referência buscados e mostrados na view
        /// </summary>
        [Display(Name = "Referências")]
        public virtual List<Referencia> Referencias { get; set; }

        /// <summary>
        /// Ação, que pode ser definida por um botão submit na view, que pode ser por exemplo Fechar, Recalcular, Pesquisar
        /// </summary>
        [Display(Name = "Ação")]
        public virtual string Acao { get; set; }

        #endregion


        #region constructors
        /// <summary>
        /// constructor padrão
        /// </summary>
        public FechamentoIndexViewModel()
        {
            Usuarios = new List<Usuario>();
            Busca = new FechamentoBuscaViewModel();
            Referencias = new List<Referencia>();
        }

        #endregion

    }
}