using Egoi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TPA.Services.LocaSMS;

namespace TPA.Services
{

    #region classe SMSServices

    /// <summary>
    /// serviço para envio de sms
    /// 
    /// enviar pelo modem
    /// http://www.andrealveslima.com.br/blog/index.php/2017/10/18/enviando-sms-com-c-e-vb-net/
    /// 
    /// enviar pelo locasms
    /// http://locasms.com.br/
    /// 
    /// enviar pelo textbelt
    /// https://textbelt.com/
    /// 
    /// 
    /// 
    /// 
    /// egoy
    /// sucesso: 
    /// {"ID":"354249", "CAMPAIGN":"064072d0dd5f6571f47ecebcae5d1f5d", "TO":"5511979924544"}
    /// {"ID":"354253", "CAMPAIGN":"87b3f5b4c077a3f064f45a37ee4a28e3", "TO":"5511979924544"}
    /// 
    /// erro:
    /// {"ERROR":"NO_CELLPHONE"}
    /// 
    /// 
    /// 
    /// 
    /// textbelt
    /// sucesso:
    /// {"success":true,"textId":"32181514302469353","quotaRemaining":27}
    /// 
    /// erro:
    /// {"success":false,"error":"Invalid phone number or bad request. If your phone number contains a +, please check that you are URL encoding it properly."}
    /// 
    /// 
    /// locasms
    /// sucesso:
    /// 122301672:SUCESSO
    /// 
    /// erro:
    ///
    /// 
    /// </summary>
    public static class SMSServices
    {


        #region métodos estáticos públicos

        /// <summary>
        /// obtem o servidor de serviço de sms LOCASMS, quando aplicável
        /// aplica-se a LocaSMS
        /// </summary>
        /// <returns></returns>
        public static string SMS_LOCASMS_SERVIDOR()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_LOCASMS_SERVIDOR"].ToString();
        }

        /// <summary>
        /// obtém o usuário do serviço de sms LOCASMS, quando aplicável
        /// aplica-se a LocaSMS
        /// </summary>
        /// <returns></returns>
        public static string SMS_LOCASMS_USUARIO()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_LOCASMS_USUARIO"].ToString();
        }

        /// <summary>
        /// obtem a senha do serviço de sms LOCASMS, quando aplicável
        /// aplica-se a LocaSMS
        /// </summary>
        /// <returns></returns>
        public static string SMS_LOCASMS_SENHA()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_LOCASMS_SENHA"].ToString();
        }

        /// <summary>
        /// obtem a api key do serviço de sms TEXTBELT, quando aplicável
        /// aplica-se ao textbelt
        /// </summary>
        /// <returns></returns>
        public static string SMS_TEXTBELT_APIKEY()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_TEXTBELT_APIKEY"].ToString();
        }


        /// <summary>
        /// obtem a api key do serviço de sms EGOY, quando aplicável
        /// aplica-se ao EGOY
        /// </summary>
        /// <returns></returns>
        public static string SMS_EGOY_APIKEY()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_EGOY_APIKEY"].ToString();
        }


        /// <summary>
        /// contém o id da lista de envio para a campanha de sms do egoy
        /// </summary>
        /// <returns></returns>
        public static string SMS_EGOY_LISTID()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_EGOY_LISTID"].ToString();
        }


        /// <summary>
        /// contém o hash do remetente pré cadastrado no sistema do egoy
        /// </summary>
        /// <returns></returns>
        public static string SMS_EGOY_FROMID()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_EGOY_FROMID"].ToString();
        }


        /// <summary>
        /// define qual lib usaremos para envio de sms: LOCASMS, TEXTBELT, EGOY
        /// </summary>
        /// <returns></returns>
        public static string SMS_LIB()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMS_LIB"].ToString();
        }


        /// <summary>
        /// lê a configuração, define qual classe que implementa IMensagemPorSMS vai criar, configura a classe e envia
        /// </summary>
        /// <param name="telefoneDestinatario">string - telefone do destinatário</param>
        /// <param name="nome">string - nome</param>
        /// <param name="assunto">string - assunto</param>
        /// <param name="corpo">string - corpo</param>
        /// <returns>string - cada api retorna uma estrutura diferente. O retorno deve ser avaliado para se saber se é sucesso ou não</returns>
        public static string Enviar(string telefoneDestinatario, string nome, string assunto, string corpo)
        {
            IMensagemPorSMS msg = null;
            switch(SMS_LIB().ToUpper())
            {
                case "TEXTBELT":
                    msg = new TextBeltSMSClient();
                    break;
                case "LOCASMS":
                    msg = new LocaSMSClient();
                    break;
                case "EGOY":
                    msg = new EGoySMSClient();
                    break;
                default:
                    msg = new EGoySMSClient();
                    break;
            }


            

            msg.TelefoneDestinatario = telefoneDestinatario;
            msg.NomeDestinatario = nome;
            msg.Assunto = assunto;
            msg.Corpo = corpo;

            return msg.Enviar();

        }


        #endregion

    }

    #endregion


    #region interface IMensagemPorSMS

    /// <summary>
    /// interface IMensagemPorSMS, a ser implementada por todas as classes que desejam enviar sms
    /// </summary>
    public interface IMensagemPorSMS
    {

        #region propriedades

        /// <summary>
        /// nome do destinatário
        /// </summary>
        string NomeDestinatario { get; set; }

        /// <summary>
        /// telefone do destinatário
        /// </summary>
        string TelefoneDestinatario { get; set; }

        /// <summary>
        /// assunto
        /// </summary>
        string Assunto { get; set; }

        /// <summary>
        /// corpo
        /// </summary>
        string Corpo { get; set; }

        /// <summary>
        /// servidor
        /// </summary>
        string Servidor { get; set; }

        /// <summary>
        /// usuário
        /// </summary>
        string Usuario { get; set; }

        /// <summary>
        /// senha
        /// </summary>
        string Senha { get; set; }

        /// <summary>
        /// api key
        /// </summary>
        string ApiKey { get; set; }

        #endregion



        #region métodos

        /// <summary>
        /// envia e retorna o retorno do servidor
        /// </summary>
        /// <returns>string - retorno da api</returns>
        string Enviar();

        #endregion

    }

    #endregion





    #region classe LocaSMSClient

    /// <summary>
    /// classe que usa o serviço LocaSMS para mandar a mensagem
    /// </summary>
    public class LocaSMSClient : IMensagemPorSMS
    {


        #region construtores

        /// <summary>
        /// construtor e inicializador da configuração do LocaSMS
        /// </summary>
        public LocaSMSClient()
        {
            this.Usuario = SMSServices.SMS_LOCASMS_USUARIO();
            this.Senha = SMSServices.SMS_LOCASMS_SENHA();
            this.Servidor = SMSServices.SMS_LOCASMS_SERVIDOR();
        }

        #endregion

        #region propriedades públicas


        /// <summary>
        /// nome do destinatário
        /// </summary>
        public virtual string NomeDestinatario { get; set; }

        /// <summary>
        /// telefone do destinatário
        /// </summary>
        public virtual string TelefoneDestinatario { get; set; }

        /// <summary>
        /// assunto
        /// </summary>
        public virtual string Assunto { get; set; }

        /// <summary>
        /// corpo
        /// </summary>
        public virtual string Corpo { get; set; }

        /// <summary>
        /// servidor
        /// </summary>
        public virtual string Servidor { get; set; }

        /// <summary>
        /// usuário
        /// </summary>
        public virtual string Usuario { get; set; }

        /// <summary>
        /// senha
        /// </summary>
        public virtual string Senha { get; set; }

        /// <summary>
        /// api key
        /// </summary>
        public virtual string ApiKey { get; set; }

        #endregion



        #region métodos públicos

        /// <summary>
        /// envia a mensagem
        /// </summary>
        /// <returns>string retorno da api</returns>
        public virtual string Enviar()
        {


            string result = "";

            if (string.IsNullOrWhiteSpace(this.TelefoneDestinatario))
                return "Telefone em branco";

            if (string.IsNullOrWhiteSpace(this.Corpo))
                return "Mensagem em branco";

            try
            {

                EndpointAddress endereco = new EndpointAddress(this.Servidor);
                BasicHttpBinding binding = new BasicHttpBinding();                
                ServiceSMSSoapClient srv = new ServiceSMSSoapClient(binding, endereco);

                List<Destination> list = new List<Destination>();

                list.Add(new Destination()
                {
                    Name = NomeDestinatario,
                    Number = TelefoneDestinatario
                        .Replace(" ","")
                        .Replace("-", "")
                        .Replace("(", "")
                        .Replace(")", "")
                });

                rSMS sms = new rSMS();

                sms.Destinations = list.ToArray();
                sms.Flash = false;
                sms.Msg = Corpo;
                sms.Preso = false;
                sms.Subject = Assunto;
                sms.WarningBeginningTransmission = false;


                result = srv.SendSMS(Usuario, Senha, sms);

            }
            catch (Exception err)
            {
                result = err.ToString();
            }

            return result;
        }

        #endregion

    }

    #endregion



    #region classe TextBeltSMSClient

    /// <summary>
    /// classe que usa o serviço textbelt para mandar a mensagem
    /// </summary>
    public class TextBeltSMSClient : IMensagemPorSMS
    {

        #region construtores

        /// <summary>
        /// construtor e inicializador da configuração do LocaSMS
        /// </summary>
        public TextBeltSMSClient()
        {
            this.ApiKey =   SMSServices.SMS_TEXTBELT_APIKEY();
        }

        #endregion


        #region propriedades públicas


        /// <summary>
        /// nome do destinatário
        /// </summary>
        public virtual string NomeDestinatario { get; set; }

        /// <summary>
        /// telefone do destinatário
        /// </summary>
        public virtual string TelefoneDestinatario { get; set; }

        /// <summary>
        /// assunto
        /// </summary>
        public virtual string Assunto { get; set; }

        /// <summary>
        /// corpo
        /// </summary>
        public virtual string Corpo { get; set; }

        /// <summary>
        /// servidor
        /// </summary>
        public virtual string Servidor { get; set; }

        /// <summary>
        /// usuário
        /// </summary>
        public virtual string Usuario { get; set; }

        /// <summary>
        /// senha
        /// </summary>
        public virtual string Senha { get; set; }

        /// <summary>
        /// api key
        /// </summary>
        public virtual string ApiKey { get; set; }

        #endregion




        #region métodos públicos 

        /// <summary>
        /// envia a mensagem
        /// </summary>
        /// <returns>string retorno da api</returns>
        public virtual string Enviar()
        {
            string result = "";
            string telefone = "+55" + TelefoneDestinatario
                        .Replace(" ", "")
                        .Replace("-", "")
                        .Replace("(", "")
                        .Replace(")", "");

            using (WebClient client = new WebClient())
            {
                byte[] response =  client.UploadValues(new Uri( "http://textbelt.com/text"), new NameValueCollection()
                {
                    { "phone",  telefone},
                    { "message", Corpo},
                    { "key", ApiKey},   
                });

                result = System.Text.Encoding.UTF8.GetString(response);
            }

            return result;
        }

        #endregion 
    }


    #endregion




    #region classe LocaSMSClient

    /// <summary>
    /// classe que usa o serviço EGoy para mandar a mensagem
    /// </summary>
    public class EGoySMSClient : IMensagemPorSMS
    {

        #region construtores

        /// <summary>
        /// construtor e inicializador da configuração do LocaSMS
        /// </summary>
        public EGoySMSClient()
        {
            this.ApiKey = SMSServices.SMS_EGOY_APIKEY();
            this._fromId = SMSServices.SMS_EGOY_FROMID();
            this._listId = SMSServices.SMS_EGOY_LISTID();
        }

        #endregion



        #region campos privados

        private string _fromId;

        private string _listId;

        #endregion



        #region propriedades públicas


        /// <summary>
        /// nome do destinatário
        /// </summary>
        public virtual string NomeDestinatario { get; set; }

        /// <summary>
        /// telefone do destinatário
        /// </summary>
        public virtual string TelefoneDestinatario { get; set; }

        /// <summary>
        /// assunto
        /// </summary>
        public virtual string Assunto { get; set; }

        /// <summary>
        /// corpo
        /// </summary>
        public virtual string Corpo { get; set; }

        /// <summary>
        /// servidor
        /// </summary>
        public virtual string Servidor { get; set; }

        /// <summary>
        /// usuário
        /// </summary>
        public virtual string Usuario { get; set; }

        /// <summary>
        /// senha
        /// </summary>
        public virtual string Senha { get; set; }

        /// <summary>
        /// api key
        /// </summary>
        public virtual string ApiKey { get; set; }

        #endregion



        #region métodos públicos

        /// <summary>
        /// envia a mensagem
        /// </summary>
        /// <returns>string retorno da api</returns>
        public virtual string Enviar()
        {


            string result = "";

            if (string.IsNullOrWhiteSpace(this.TelefoneDestinatario))
                return "Telefone em branco";

            if (string.IsNullOrWhiteSpace(this.Corpo))
                return "Mensagem em branco";

            Protocol p = Protocol.Default;
            try
            {

                string numero = "55-" + TelefoneDestinatario
                        .Replace(" ", "")
                        .Replace("-", "")
                        .Replace("(", "")
                        .Replace(")", "");

                EgoiApi api = EgoiApiFactory.getApi(p);
                EgoiMap arguments = new EgoiMap();
                arguments.Add("apikey", ApiKey);

                arguments.Add("subject", Assunto);
                arguments.Add("cellphone", numero);
                arguments.Add("message", Corpo);
                arguments.Add("listID", this._listId);
                arguments.Add("fromID", this._fromId);

                EgoiMap resultadoEgoy = api.sendSMS(arguments);
                result += resultadoEgoy.ToString();

            }
            catch (Exception err)
            {
                result = err.ToString();
            }

            return result;
        }

        #endregion

    }

    #endregion

}
