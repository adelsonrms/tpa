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
    /// atributo para limitar o tamanho de um arquivo para upload
    /// </summary>
    public class FileSizeAttribute : ValidationAttribute
    {
        #region fileds privadas

        private readonly int _maxSize;

        #endregion


        #region construtores

        /// <summary>
        /// construtor padrão aceita o tamanho máximo no parâmetro
        /// </summary>
        /// <param name="maxSize">int - tamanho máximo permitido</param>
        public FileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        #endregion


        #region métodos públicos

        /// <summary>
        /// retorna true se o objeto postado for um HttpPostedFileBase != de null e o tamanho for menor ou igual _maxSize
        /// false caso contrário
        /// </summary>
        /// <param name="value">HttpPostedFileBase - arquivo para upload</param>
        /// <returns>bool - avalia se o resultado é válido ou não baseado no tamanho do arquivo</returns>
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            return (value as HttpPostedFileBase).ContentLength <= _maxSize;
        }

        /// <summary>
        /// mostra a mensagem de erro caso seja impossível fazer o upload
        /// </summary>
        /// <param name="name"></param>
        /// <returns>sting - a mensagem de erro</returns>
        public override string FormatErrorMessage(string name)
        {
            return string.Format("O tamanho máximo do arquivo não pode exceder {0} bytes", _maxSize);
        }


        #endregion

    }
}
