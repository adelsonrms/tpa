using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TPA.Framework
{
    /// <summary>
    /// atributo para action de upload de arquivo
    /// dado uma lista de extensões separadas por vírgula, verifica se o arquivo 
    /// sendo enviado pertence a uma dessas extensões.
    /// bloqueia a operação caso contrário
    /// </summary>
    public class FileTypesAttribute : ValidationAttribute
    {

        #region fields privados

        private readonly List<string> _types;

        #endregion



        #region construtores

        /// <summary>
        /// construtor padrão que aceita a string com a lista de extensões separadas por vírgula e converte para um array
        /// </summary>
        /// <param name="types">string - tipos de arquivos separados por vírgula</param>
        public FileTypesAttribute(string types)
        {
            _types = types.Split(',').ToList();
        }

        #endregion


        #region métodos públicos

        /// <summary>
        /// valida o arquivo quanto ao tipo e retorna true pra permitir ou false para bloquear
        /// </summary>
        /// <param name="value">HttpPostedFileBase - o arquivo sendo enviado</param>
        /// <returns>bool - true caso o arquivo se enquadre, false caso contrário</returns>
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            var fileExt = System.IO.Path.GetExtension((value as HttpPostedFileBase).FileName).Substring(1);
            return _types.Contains(fileExt, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// formata a mensagem de saída em caso de erro
        /// </summary>
        /// <param name="name"></param>
        /// <returns>string - a mensagem de erro</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format("Tipo de arquivo inválido. Os tipos suportados são {0}.", String.Join(", ", _types));
        }

        #endregion



    }
}
