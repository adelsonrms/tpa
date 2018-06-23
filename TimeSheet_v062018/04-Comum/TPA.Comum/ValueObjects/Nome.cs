namespace ERP.Shared.ValueObjects
{
    public class Nome
    {
        public string PrimeiroNome { get; set; }
        public string SobreNome { get; set; }
        public string NomeCompleto
        { 
               get {
                   return string.Format("{0} {1}", PrimeiroNome, SobreNome);
               }
        }
        public override string ToString()
        {
            return NomeCompleto;
        }

        public Nome()
        {

        }

        public Nome(string nome)
        {
            this.PrimeiroNome = nome;
        }
        public Nome(string nome, string sobrenome)
        {
            this.PrimeiroNome = nome;
            this.SobreNome = sobrenome;
        }
    }
}
