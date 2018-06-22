using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Security;

namespace TPA.Infra.Services
{


    /// <summary>
    /// classe de serviços de log, separação e composição de relatórios de erros baseados na exception e em sua stack
    /// </summary>
    public class LogServices
    {


        #region métodos estáticos públicos para tratamento de erro

        /// <summary>
        /// pega uma exception e varre sua inner exception para montar uma lista até que não haja mais inner exception
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static List<Exception> GetExceptionList(Exception err)
        {
            List<Exception> result = new List<Exception>();
            var tmp = err;


            while (tmp != null)
            {

                result.Add(tmp);
                tmp = tmp.InnerException;

            }


            return result;
        }


        /// <summary>
        /// pega todas as informações da lista de exceptions e transforma em uma lista de strings
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static List<string> GetExceptionMessages(List<Exception> errors)
        {
            var result = new List<string>();

            foreach (Exception err in errors)
            {
                if (err != null)
                {
                    result.Add("Mensagem:".PadRight(80, '-'));
                    result.Add(err.Message);
                    result.Add("Source:".PadRight(80, '-'));
                    result.Add(err.Source);
                    result.Add("StackTrace:".PadRight(80, '-'));
                    result.Add(err.StackTrace);

                    if (err is DbEntityValidationException)
                    {
                        result.AddRange(ExtraiErrosDbEntityValidation(err as DbEntityValidationException));
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// pega todos os erros de validação de entidades e transforma-os em uma lista de strings
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static List<string> ExtraiErrosDbEntityValidation(DbEntityValidationException err)
        {
            var result = new List<string>();

            if ((err is DbEntityValidationException) && (err != null))
            {
                foreach (var eve in err.EntityValidationErrors)
                {
                    string entity = eve.Entry.Entity.GetType().Name;
                    string state = eve.Entry.State.ToString();
                    string mensagem = string.Format("-Entidade do tipo \"{0}\" no estado \"{1}\" tem os seguintes erros de validação:", entity, state);

                    result.Add(mensagem);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        string prop = ve.PropertyName;
                        string erro = ve.ErrorMessage;
                        string valor = "";
                        try
                        {
                            valor = eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName).ToString();
                        }
                        catch
                        {
                        }
                        string detalhe = string.Format("--Property: \"{0}\", Valor: \"{1}\", Erro: \"{2}\"", prop, valor, erro);

                        result.Add(detalhe);
                    }
                }
            }


            return result;

        }


        /// <summary>
        /// loga uma exception
        /// </summary>
        /// <param name="err"></param>
        /// <param name="rethrow"></param>
        public static void LogarException(Exception err)
        {
            string mensagens = "";
            try
            {
                mensagens = ConcatenaExceptions(err);
            }
            catch(Exception err2)
            {
                mensagens = "Não foi possível concatenar todas as mensagens de erro\r\n" + err.ToString() + "\r\n" + err2.ToString();

                System.Diagnostics.Debug.WriteLine(mensagens);
                System.Diagnostics.Trace.WriteLine(mensagens);
                Console.WriteLine(mensagens);
            }

            try
            {


                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string pathAssembly = Uri.UnescapeDataString(uri.Path);
                string diretorioAssembly = Path.GetDirectoryName(pathAssembly);
                string diretorioAplicacao = Directory.GetParent(diretorioAssembly).FullName;
                string caminhoArquivoLog = Path.Combine(diretorioAplicacao, "App_Data\\ExceptionLog.txt");


                if (HttpContext.Current != null)
                {
                    caminhoArquivoLog = HttpContext.Current.Server.MapPath("~/App_Data/ExceptionLog.txt");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(mensagens);
                    System.Diagnostics.Trace.WriteLine(mensagens);
                    Console.WriteLine(mensagens);
                }

                if (!Directory.Exists(Path.GetDirectoryName(caminhoArquivoLog)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivoLog));
                }

                using (StreamWriter sw = new StreamWriter(caminhoArquivoLog, true))
                {
                    sw.WriteLine(string.Format("Data: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(80, '-'));
                    sw.WriteLine(ConcatenaExceptions(err));
                    sw.WriteLine("=".PadRight(80, '='));
                    sw.Flush();
                    sw.Close();
                }
            }
            catch(Exception err2)
            {
                mensagens = "Não foi possível logar o erro.\r\n" + err2.ToString() + "\r\n" + mensagens;
                System.Diagnostics.Debug.WriteLine(mensagens);
                System.Diagnostics.Trace.WriteLine(mensagens);
                Console.WriteLine(mensagens);
            }
        }


        /// <summary>
        /// loga um texto / evento qualquer
        /// </summary>
        /// <param name="texto">string - texto a logar</param>
        public static void Logar(string texto)
        {
            try
            {

                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string pathAssembly = Uri.UnescapeDataString(uri.Path);
                string diretorioAssembly = Path.GetDirectoryName(pathAssembly);
                string diretorioAplicacao = Directory.GetParent(diretorioAssembly).FullName;
                string caminhoArquivoLog = Path.Combine(diretorioAplicacao, "App_Data\\Log.txt");


                if (HttpContext.Current != null)
                {
                    caminhoArquivoLog = HttpContext.Current.Server.MapPath("~/App_Data/Log.txt");
                }


                if (!Directory.Exists(Path.GetDirectoryName(caminhoArquivoLog)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivoLog));
                }

                using (StreamWriter sw = new StreamWriter(caminhoArquivoLog, true))
                {
                    sw.WriteLine(string.Format("Data: {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).PadRight(80, '-'));
                    sw.WriteLine(texto);
                    sw.WriteLine("=".PadRight(80, '='));
                    sw.Flush();
                    sw.Close();
                }


            }
            catch (Exception err2)
            {
                LogServices.LogarException(err2);
            }
        }

        /// <summary>
        /// concatena todas as exceptions em uma string com \r\n separando
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static string ConcatenaExceptions(Exception err)
        {
            var mensagens = GetExceptionMessages(GetExceptionList(err));
            return string.Join("\r\n", mensagens.ToArray());
        }

        /// <summary>
        /// para mensagens de erros de validação na própria tela, pega só os erros de validação
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static string ConcatenaErrosDbEntityValidation(DbEntityValidationException err)
        {
            var mensagens = ExtraiErrosDbEntityValidation(err);
            return string.Join("\r\n", mensagens.ToArray());
            
        }






       
        #endregion



    }

}