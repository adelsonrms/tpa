using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.ViewModel
{

    /// <summary>
    /// ViewModel container para informações sobre a action
    /// </summary>
    public class PerfilGerenciarActionViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// id desta action/método  se ela já estiver cadastrada no banco de dados
        /// </summary>
        public virtual int Id { get; set; }

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
        /// nome da controller/action
        /// </summary>
        public virtual string NomeAction { get; set; }
        

        /// <summary>
        /// nome amigável da action/método 
        /// </summary>
        public virtual string Nome { get; set; }

        /// <summary>
        /// descrição amigável da action/método 
        /// </summary>
        public virtual string Descricao { get; set; }




        #endregion



        #region métodos públicos overrides de object

        /// <summary>
        /// esta classe precisa de um override de equals para poder usar o distinct
        /// </summary>
        /// <param name="obj">objeto a ser comparado</param>
        /// <returns>bool - True se forem iguais</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (object.ReferenceEquals(this, obj))
                return true;

            if (!(obj is PerfilGerenciarActionViewModel))
                return false;

            return this.NomeAction.Equals((obj as PerfilGerenciarActionViewModel).NomeAction);
        }

        /// <summary>
        /// esta classe precisa de um override de equals para poder usar o distinct
        /// igual o objeto acima, mas tipado, por performance
        /// </summary>
        /// <param name="ent">PerfilGerenciarActionViewModel a ser comparada</param>
        /// <returns>bool - True se forem iguais</returns>
        public virtual bool Equals(PerfilGerenciarActionViewModel ent)
        {
            if (ent == null)
                return false;

            if (object.ReferenceEquals(this, ent))
                return true;

            return this.NomeAction.Equals(ent.NomeAction);
        }


        /// <summary>
        /// Chama o hashcode da propriedade usada como Id para comparações
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.NomeAction.GetHashCode();
        }

        #endregion




    }
}
