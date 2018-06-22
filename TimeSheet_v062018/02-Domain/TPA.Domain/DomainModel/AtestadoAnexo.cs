using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPA.Domain.DomainModel
{
    /// <summary>
    /// Container para o arquivo anexo e lista de dias para o qual o atestado é válido
    /// </summary>
    public class AtestadoAnexo : TPAEntity
    {
        #region propriedades públicas

        /// <summary>
        /// id do objeto
        /// </summary>
        [Key]
        public override int Id { get; set; }

        /// <summary>
        /// lista de atividades lançadas automaticamente na abrangência desse atestado
        /// </summary>
        [Required]
        [Display(Name = "Atividades / Abrangência")]
        public virtual List<Atividade> Atividades {get; set;}

        /// <summary>
        /// bytes do arquivo em anexo
        /// </summary>
        [Required]
        [Display(Name = "Arquivo anexo")]
        public virtual byte[] Arquivo { get; set; }

        /// <summary>
        /// observação do atestado
        /// </summary>
        public virtual string Observacao { get; set; }

        /// <summary>
        /// nome do arquivo no momento do upload
        /// </summary>
        public virtual string NomeArquivoOriginal { get; set; }

        #endregion


        #region constructors

        /// <summary>
        /// constructor padrão, inicializa Arquivo para ele não ser null
        /// </summary>
        public AtestadoAnexo()
        {
            Arquivo = new byte[0];
        }

        #endregion
    }
}
