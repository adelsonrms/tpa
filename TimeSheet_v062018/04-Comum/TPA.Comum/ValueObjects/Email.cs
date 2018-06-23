namespace ERP.Shared.ValueObjects
{
   public class Email
    {
        public string Endereco { get; set; }
        public string Dominio { get { return Endereco.Split("@".ToCharArray())[1].ToString();}}

        public Email()
        {

        }

        public Email(string endereco)
        {
            this.Endereco = endereco;
        }

        public override string ToString()
        {
            return Endereco;
        }
    }
}
