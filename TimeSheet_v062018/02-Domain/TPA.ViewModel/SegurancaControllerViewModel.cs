using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel
{
    /// <summary>
    /// viewmodel para as páginas de construção de perfis,
    /// atribuição e revogação de processos.
    /// 
    /// ela junta o nome e namespace da classe/método obtido via reflection
    /// com o conseguido via atributo ou outros mecanismos
    /// 
    /// </summary>
    public class SegurancaControllerViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// nome da classe 
        /// </summary>
        public virtual string NomeClasse { get; set; }

        /// <summary>
        /// nome completo da classe do .net incluindo assembly e namespace
        /// </summary>
        public virtual string NomeClasseCompleto { get; set; }

        /// <summary>
        /// nome amigável da classe/método 
        /// </summary>
        public virtual string Nome { get; set; }

        /// <summary>
        /// descrição amigável da classe/método 
        /// </summary>
        public virtual string Descricao { get; set; }

        #endregion


    }
}
