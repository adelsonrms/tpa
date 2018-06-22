using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TPA.Framework;

namespace TPA.ViewModel
{
    /// <summary>
    /// viewmodel para adição do atestado em anexo
    /// </summary>
    public class AtestadoAnexoViewModel
    {

        #region propriedades públicas

        /// <summary>
        /// id do objeto
        /// </summary>
        public virtual int Id { get; set; }


        /// <summary>
        /// bytes do arquivo em anexo
        /// </summary>
        [Required(ErrorMessage = "Por favor selecione um arquivo.")]
        [FileTypes("jpg,jpeg,png,pdf")]
        [DataType(DataType.Upload)]
        [Display(Name = "Arquivo anexo")]
        public virtual HttpPostedFileBase ArquivoAnexo { get; set; }

        /// <summary>
        /// observação do atestado
        /// </summary>
        public virtual string Observacao { get; set; }


        /// <summary>
        /// data inicial do abono, obrigatório
        /// </summary>
        [Required(ErrorMessage = "Preencha a data inicial")]
        [Display(Name = "Data Inicial")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime DataInicial { get; set; }

        /// <summary>
        /// data final, se for null será igual a inicial
        /// </summary>
        [Display(Name = "Data Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime? DataFinal { get; set; }

        /// <summary>
        /// array com os ids dos usuários
        /// </summary>
        [Required(ErrorMessage = "Preencha o usuário")]
        [Display(Name = "Usuário")]
        public virtual int IdUsuario { get; set; }


        /// <summary>
        /// quantidade de horas
        /// </summary>
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        [Display(Name = "Horas")]
        [Required(ErrorMessage = "Preencha a quantidade de horas abonadas")]
        public virtual TimeSpan? Horas { get; set; }



        /// <summary>
        /// Nome do usuário para o retorno do display template
        /// </summary>
        [Display(Name = "Usuário")]
        public virtual string NomeUsuario { get; set; }

        #endregion


    }
}
