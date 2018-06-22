using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace TPA.Services
{

    #region classe MailServices

    /// <summary>
    /// serviços para envio de e-mail
    /// </summary>
    public static class MailServices
    {


        #region métodos estáticos públicos


        /// <summary>
        /// obtem o servidor SMNTP do arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public static string SMTP_SERVIDOR()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMTP_SERVIDOR"].ToString();
        }

        /// <summary>
        /// obtém o usuario SMTP do arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public static string SMTP_USUARIO()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMTP_USUARIO"].ToString();
        }

        /// <summary>
        /// Obtém a senha SMTP do arquivo de configuração
        /// </summary>
        /// <returns></returns>
        public static string SMTP_SENHA()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMTP_SENHA"].ToString();
        }

        /// <summary>
        /// Obtem o nome a ser exibido para o remetente
        /// </summary>
        /// <returns></returns>
        public static string SMTP_DISPLAYNAME()
        {
            return System.Configuration.ConfigurationManager.AppSettings["SMTP_DISPLAYNAME"].ToString();
        }


        /// <summary>
        /// cria um objeto MensagemPorEmail e envia
        /// </summary>
        /// <param name="destinatario">string - e-mail do destinatário (um por vez)</param>
        /// <param name="assunto">string - assunto</param>
        /// <param name="corpo">string - corpo</param>
        public static void Enviar(string destinatario, string assunto, string corpo)
        {
            MensagemPorEmail msg = new MensagemPorEmail();

            msg.Usuario = SMTP_USUARIO();
            msg.Senha = SMTP_SENHA();
            msg.Servidor = SMTP_SERVIDOR();
            msg.DisplayNameRemetente = SMTP_DISPLAYNAME();

            msg.EnderecoRemetente = SMTP_USUARIO();

            msg.EnderecoDestinatario = destinatario;
            msg.Assunto = assunto;
            msg.Corpo = corpo;

            msg.Enviar();

        }

        #endregion

    }


    #endregion



    #region classe MensagemPorEmail

    /// <summary>
    /// classe que implementa o envio de mensagens por e-mail
    /// </summary>
    public class MensagemPorEmail
    {
        #region propriedades públicas

        /// <summary>
        /// endereço do remetente
        /// </summary>
        public virtual string EnderecoRemetente { get; set; }

        /// <summary>
        /// nome de exibição do remetente
        /// </summary>
        public virtual string DisplayNameRemetente { get; set; }

        /// <summary>
        /// endereço do destinatário
        /// </summary>
        public virtual string EnderecoDestinatario {get; set;}

        /// <summary>
        /// assunto
        /// </summary>
        public virtual string Assunto { get; set; }

        /// <summary>
        /// corpo da mensagem
        /// </summary>
        public virtual string Corpo { get; set; }



        
        /// <summary>
        /// servidor smtp
        /// </summary>
        public virtual string Servidor { get; set; }

        /// <summary>
        /// usuario smtp
        /// </summary>
        public virtual string Usuario { get; set; }

        /// <summary>
        /// senha smtp
        /// </summary>
        public virtual string Senha { get; set; }

        #endregion




        #region métodos públicos

        /// <summary>
        /// envia o e-mail
        /// </summary>
        public virtual void Enviar()
        {
            var msg = new MailMessage();

            MailAddress from = new MailAddress(EnderecoRemetente, DisplayNameRemetente);
            MailAddress to = new MailAddress(EnderecoDestinatario);

            msg.From = from;
            msg.To.Add(to);
            msg.Subject = Assunto;


            msg.Body = Corpo;


            SmtpClient smtp = new SmtpClient(Servidor);
            smtp.Port = 587;  
            //smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network; 
            smtp.UseDefaultCredentials = false; 
            smtp.Credentials = new NetworkCredential(Usuario, Senha);  
            smtp.Send(msg);

        }

        #endregion
    }

    #endregion

}
