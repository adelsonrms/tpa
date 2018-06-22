using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.Services.Seguranca
{

    /// <summary>
    /// fornece um nome e descrição amigáveis para as ações, para serem importadas do assembly diretamente para o banco de dados e para 
    /// construção de perfis mais facilmente, pois o administrador tem uma idéia mais clara do que a ação faz
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TPADescricaoAcaoControllerAttribute : Attribute
    {

        #region propriedades públicas

        /// <summary>
        /// nome amigável do controller ou da ação
        /// </summary>
        public virtual string Nome { get; set; }

        /// <summary>
        /// descrição amigável do controller ou da ação
        /// </summary>
        public virtual string Descricao { get; set; }

        #endregion


        #region constructors

        /// <summary>
        /// construtor padrão
        /// </summary>
        /// <param name="nome">string - nome amigável</param>
        /// <param name="descr">string - descrição amigável</param>
        public TPADescricaoAcaoControllerAttribute(string nome, string descr)
        {
            this.Nome = nome;
            this.Descricao = descr;
        }

        #endregion
    }



}
