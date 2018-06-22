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
    public class PerfilGerenciarControllerViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// id do perfil que estamos gerenciando
        /// </summary>
        public virtual int IdPerfil { get; set; }

        /// <summary>
        /// cache do nome do perfil para exibição na view
        /// </summary>
        public virtual string NomePerfil { get; set; }

        /// <summary>
        /// indica se o perfil de IdPerfil tem acesso ou não a essa ação
        /// </summary>
        public virtual bool TemAcesso { get; set; }


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
