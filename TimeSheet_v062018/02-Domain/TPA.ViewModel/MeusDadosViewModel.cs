using System.ComponentModel.DataAnnotations;

namespace TPA.ViewModel
{

    /// <summary>
    /// ViewModel para o usuário editar os proprios dados. Não tem as propriedades booleanas Ativo ou EnviaMensagemAlerta
    /// </summary>
    public class MeusDadosViewModel
    {


        #region propriedades públicas

        /// <summary>
        /// id do usuário
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// login do usuário
        /// </summary>
        public virtual string Login { get; set; }

        /// <summary>
        /// nome do usuário
        /// </summary>
        [Required]
        public virtual string Nome {get; set;}


        /// <summary>
        /// celular do usuário
        /// </summary>
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\([0-9]{2}\))\s([0-9]{4})-([0-9]{4,5})$", ErrorMessage = "Telefone Inválido")]
        public virtual string Celular { get; set; }

        /// <summary>
        /// e-mail profissional no ambiente do cliente
        /// </summary>
        [EmailAddress]
        [Display(Name = "E-mail no ambiente do cliente")]
        public virtual string EmailProfissional { get; set; }

        #endregion


    }
}