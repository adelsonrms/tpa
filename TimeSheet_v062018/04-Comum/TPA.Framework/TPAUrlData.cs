using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace TPA.Presentation.Util
{

    /// <summary>
    /// classe para construir uma url com vários parâmetros de querystring baseado na adição de várias parcelas de chave/valor
    /// </summary>
    public class TPAUrlData
    {


        #region campos privados

        private Uri _baseUrl;

        /// <summary>
        /// guarda os valores da querystring e não deixa repetir
        /// </summary>
        private Dictionary<string, string> _valores;

        #endregion



        #region constructors

        /// <summary>
        /// constroi uma url com dados baseada em uma string
        /// </summary>
        /// <param name="urlBase">string - a url base de onde será composta o restante da url</param>
        public TPAUrlData(string urlBase)
        {
            _baseUrl = new Uri(urlBase);
            _valores = new Dictionary<string, string>();
            
            NameValueCollection queryOriginal = HttpUtility.ParseQueryString(_baseUrl.Query);
            foreach (string v in queryOriginal.Keys)
            {
                _valores.Add(v, queryOriginal[v]);
            }
        }



        /// <summary>
        /// constroi uma url com dados baseada em uma uri
        /// </summary>
        /// <param name="urlBase">Uri - a url base de onde será composta o restante da url</param>
        public TPAUrlData(Uri urlBase)
        {
            _baseUrl = urlBase;
            _valores = new Dictionary<string, string>();

            NameValueCollection queryOriginal = HttpUtility.ParseQueryString(_baseUrl.Query);
            foreach (string v in queryOriginal.Keys)
            {
                _valores.Add(v, queryOriginal[v]);
            }
        }

        #endregion



        #region métodos públicos

        /// <summary>
        /// Adiciona um par de chave (string) e valor (string)
        /// podem ser adicionados outros para trabalhar com datas, inteiros etc
        /// </summary>
        /// <param name="chave">string - o identificador do dictionary</param>
        /// <param name="valor">string - o valor como string</param>
        /// <returns></returns>
        public TPAUrlData Add(string chave, string valor)
        {
            //se os dois estiverem preenchidos atribui o valor à chave
            //se apenas a chave estiver preenchida, mantém a chave com o valor em branco
            if (!string.IsNullOrWhiteSpace(chave))
            {

                if (!_valores.ContainsKey(chave))
                {
                    _valores.Add(chave, valor??"");
                }
                else
                {
                    _valores[chave] = valor ?? "";
                }

            }

            //para encadeamento de chamadas
            return this;
        }


        /// <summary>
        /// remove o item cuja chave seja a passada como parâmetro
        /// </summary>
        /// <param name="chave">string - a chave a ser removida</param>
        /// <returns></returns>
        public TPAUrlData Remove(string chave)
        {
            if ((!string.IsNullOrWhiteSpace(chave)) && (_valores.ContainsKey(chave)))
            {
                _valores.Remove(chave);
            }

            //para encadeamento de chamadas
            return this;
        }


        /// <summary>
        /// Adiciona um dictionary inteiro aos parametros
        /// Internamente usa Add para não repetir os valores e atualizar
        /// </summary>
        /// <param name="pars">dicionario de string/string com os parametros a serem adicionados</param>
        /// <returns>Dictionary de string, string</returns>
        public TPAUrlData AddParams(Dictionary<string, string> pars)
        {

            foreach (var d in pars)
            {
                this.Add(d.Key, d.Value);
            }

            return this;
        }


        /// <summary>
        /// Retorna a lista interna de parametros para clonar os parametros em outra Url
        /// </summary>
        /// <returns>Dictionary de string, string</returns>
        public Dictionary<string, string> GetParams()
        {
            Dictionary<string, string> result = new Dictionary<string, string>(_valores);
            return result;
        }

        #endregion



        #region metodos públicos sobrecarregados padrão object


        /// <summary>
        /// retorna esta url com todos os seus dados como a string completa que representa a url
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (new Uri(GetCompleteUrlAsString())).ToString();
        }


        /// <summary>
        /// Verifica se dois objetos são iguais para poder fazer ordenações, igualdade e distinct
        /// </summary>
        /// <param name="obj">objeto a ser comparado</param>
        /// <returns>bool - True se forem iguais</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;


            if (object.ReferenceEquals(this, obj))
                return true;

            return this.ToString().Equals(obj.ToString());
        }


        /// <summary>
        /// Chama o hashcode da sua string
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }


        #endregion



        #region operator overloading

        /// <summary>
        /// usa o método Equals para verificar se dois objetos são iguais e sobrecarregar a ação do operador de igualdade
        /// usa equals para comparar os dois
        /// </summary>
        /// <param name="x">UrlData</param>
        /// <param name="y">UrlData</param>
        /// <returns>bool</returns>
        public static bool operator ==(TPAUrlData x, TPAUrlData y)
        {
            if (((object)x == null) && ((object)y == null))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            return x.Equals(y);
        }

        /// <summary>
        /// usa o método Equals para verificar se dois objetos são iguais e sobrecarregar a ação do operador de igualdade
        /// compara string com UrlData
        /// converte y para string e compara com string x
        /// </summary>
        /// <param name="x">string</param>
        /// <param name="y">UrlData</param>
        /// <returns></returns>
        public static bool operator ==(string x, TPAUrlData y)
        {
            if (((object)x == null) && ((object)y == null))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            return x.Equals(y.ToString());
        }

        /// <summary>
        /// usa o método Equals para verificar se dois objetos são iguais e sobrecarregar a ação do operador de igualdade
        /// compara UrlData com string
        /// converte x para string e compara com string y
        /// </summary>
        /// <param name="x">UrlData</param>
        /// <param name="y">string</param>
        /// <returns></returns>
        public static bool operator ==(TPAUrlData x, string y)
        {
            if (((object)x == null) && ((object)y == null))
            {
                return true;
            }

            if (((object)x == null) || ((object)y == null))
            {
                return false;
            }

            return x.ToString().Equals(y);
        }

        /// <summary>
        /// usa o método Equals para verificar se dois objetos são iguais e sobrecarregar a ação do operador de desigualdade
        /// </summary>
        /// <param name="x">UrlData</param>
        /// <param name="y">UrlData</param>
        /// <returns></returns>
        public static bool operator !=(TPAUrlData x, TPAUrlData y)
        {
            return !(x == y);
        }

        /// <summary>
        /// usa o método Equals para verificar se dois objetos são iguais e sobrecarregar a ação do operador de desigualdade
        /// comparação com string
        /// </summary>
        /// <param name="x">string</param>
        /// <param name="y">UrlData</param>
        /// <returns></returns>
        public static bool operator !=(string x, TPAUrlData y)
        {
            return !(x == y.ToString());
        }

        /// <summary>
        /// usa o método Equals para verificar se dois objetos são iguais e sobrecarregar a ação do operador de desigualdade
        /// comparação com string
        /// </summary>
        /// <param name="x">UrlData</param>
        /// <param name="y">string</param>
        /// <returns></returns>
        public static bool operator !=(TPAUrlData x, string y)
        {
            return !(x.ToString() == y);
        }



        /// <summary>
        /// converte um UrlData para string implicitamente
        /// </summary>
        /// <param name="d">UrlData - lado direito</param>
        /// <returns>string - lado esquerdo</returns>
        public static implicit operator string(TPAUrlData d)  
        {
            return d.ToString(); 
        }


        /// <summary>
        /// converte um UrlData para Uri implicitamente
        /// </summary>
        /// <param name="d">UrlData - lado direito</param>
        /// <returns>Uri - lado esquerdo</returns>
        public static implicit operator Uri(TPAUrlData d)
        {
            return new Uri(d.ToString());
        }



        /// <summary>
        /// converte implicitamente um string para UrlData
        /// </summary>
        /// <param name="s">string - lado direito</param>
        /// <returns>UrlData - lado esquerdo</returns>
        public static implicit operator TPAUrlData(string s)
        {
            return new TPAUrlData(s);
        }

        /// <summary>
        /// converte um Uri para UrlData
        /// </summary>
        /// <param name="u">Uri - Lado Direito</param>
        /// <returns>UrlData - lado esquerdo</returns>
        public static implicit operator TPAUrlData(Uri u)
        {
            return new TPAUrlData(u);
        }

        #endregion



        #region métodos privados

        /// <summary>
        /// retorna a base da url até o root sem a querystring
        /// </summary>
        /// <returns></returns>
        private string GetBaseUrl()
        {
            return _baseUrl.GetLeftPart(UriPartial.Path);
        }

        /// <summary>
        /// retorna a url completa
        /// </summary>
        /// <returns></returns>
        private string GetCompleteUrlAsString()
        {
            string burl = GetBaseUrl();

            int cont = 0;
            foreach (var v in _valores)
            {
                burl += cont == 0 ? "?" : "&";
                burl += v.Key + "=" + v.Value;
                cont++;
            }

            return burl;
        }

        #endregion


    }
}
