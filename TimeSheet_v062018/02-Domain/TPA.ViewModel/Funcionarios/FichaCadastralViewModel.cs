using System.ComponentModel.DataAnnotations;

namespace TPA.ViewModel
{
    /// <summary>
    /// View Model para a ficha cadastral de um funcionario
    /// </summary>
    public class FichaCadastralViewModel
    {
        [Key]
        public virtual int Id { get; set; }
        [Required]
        public virtual string Nome { get; set; }
        public virtual string Matricula { get; set; }
        public virtual string CPF { get; set; }
        public virtual string PIS { get; set; }
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\([0-9]{2}\))\s([0-9]{4})-([0-9]{4,5})$", ErrorMessage = "Telefone Inválido")]
        public virtual string Telefone { get; set; }
        [Phone]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\([0-9]{2}\))\s([0-9]{4})-([0-9]{4,5})$", ErrorMessage = "Telefone Inválido")]
        public virtual string Celular { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail de cadastro do Office 365")]
        public virtual string EmailProfissional { get; set; }
        [EmailAddress(ErrorMessage = "Email invalido")]
        [Display(Name = "E-mail do Funcionario")]
        public virtual string EmailPessoal { get; set; }
        [Display(Name = "Endereço do Funcionario")]
        public virtual string Endereco { get; set; }

        public virtual string CEP { get; set; }
        public virtual string Bairro { get; set; }
        public virtual string Cidade { get; set; }
        public virtual string Estado { get; set; }



        public virtual string DataNascimento { get; set; }
        public virtual int SexoID { get; set; }
        


    }
}
