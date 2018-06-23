using System;
namespace TPA.Comum.Extensoes
{
    public static class StringExtension
    {
        /// <summary>
        /// Força a conversão de uma expressao String para Int.
        /// </summary>
        /// <param name="String">A expressão numerica a ser convertida</param>
        /// <returns>Retorna o valor convertido em Int. Se ocorrer erro, retorna 0</returns>
        public static int ToInt(this string String){try { return Convert.ToInt32(String);} catch { return 0;}}
    }
}
