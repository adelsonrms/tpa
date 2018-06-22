using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Microsoft.Extensions.Localization;

namespace TPA.Domain.DomainModel
{

    /// <summary>
    /// define um feriado 
    /// usado nos serviços de calendário para especificar se um dia é util ou não
    /// </summary>
    public class Feriado : TPAEntity
    {

        #region propriedades públicas 

        /// <summary>
        /// id do feriado
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; set; }

        /// <summary>
        /// data do feriado
        /// </summary>
        public virtual DateTime Data { get; set; }

        /// <summary>
        /// cidade do feriado, quando não for feriado nacional
        /// </summary>
        public virtual string Cidade { get; set; }

        /// <summary>
        /// descrição do feriado
        /// </summary>
        public virtual string Descricao { get; set; }


        #endregion 


    }
}