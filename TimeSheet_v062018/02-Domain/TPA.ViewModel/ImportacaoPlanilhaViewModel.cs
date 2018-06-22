using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TPA.ViewModel
{


    #region classe ItemImportacaoPlanilha

    /// <summary>
    /// DTO para formatar um item da importação via planilha
    /// </summary>
    public class ItemImportacaoPlanilha
    {

        #region propriedades públicas 

        /// <summary>
        /// id do usuário, para múltiplos usuários por importação
        /// </summary>
        [Display(Name = "Usuário")]
        public virtual int IdUsuario { get; set; }

        /// <summary>
        /// data da atividade
        /// </summary>
        [Display(Name = "Data")]
        public virtual DateTime Data { get; set; }

        /// <summary>
        /// id do projeto
        /// </summary>
        [Display(Name = "Projeto")]
        public virtual int IdProjeto { get; set; }

        /// <summary>
        /// id do tipo da atividade
        /// </summary>
        [Display(Name = "Tipo de Atividade")]
        public virtual int IdTipoAtividade { get; set; }

        /// <summary>
        /// descrição
        /// </summary>
        [Display(Name = "Descrição")]
        public virtual string Descricao { get; set; }

        /// <summary>
        /// entrada da manhã
        /// </summary>
        [Display(Name = "Entrada Manhã")]
        public virtual TimeSpan? EntradaManha { get; set; }

        /// <summary>
        /// saída da manhã
        /// </summary>
        [Display(Name = "Saida Manhã")]
        public virtual TimeSpan? SaidaManha { get; set; }

        /// <summary>
        /// entrada da tarde
        /// </summary>
        [Display(Name = "Entrada Tarde")]
        public virtual TimeSpan? EntradaTarde { get; set; }

        /// <summary>
        /// saída da tarde
        /// </summary>
        [Display(Name = "Saida Tarde")]
        public virtual TimeSpan? SaidaTarde { get; set; }

        #endregion 

    }

    #endregion




    #region classe ImportacaoPlanilhaAtividadesViewModel

    /// <summary>
    /// ViewModel para a importação da planilha, com uma coleção de ItemImportacaoPlanilha e para múltiplos usuários
    /// </summary>
    public class ImportacaoPlanilhaAtividadesViewModel
    {

        #region constructors

        /// <summary>
        /// constructor padrão
        /// inicializa a lista de itens
        /// </summary>
        public ImportacaoPlanilhaAtividadesViewModel()
        {

            Itens = new List<ItemImportacaoPlanilha>();

        }

        #endregion


        #region propriedades públicas 

        /// <summary>
        /// lista de itens ItemImportacaoPlanilha de atividade
        /// </summary>
        public virtual List<ItemImportacaoPlanilha> Itens { get; set; }

        #endregion

    }

    #endregion




    #region classe ImportacaoPlanilhaAtividadesUsuarioViewModel

    /// <summary>
    /// classe para importação de múltiplas atividades para usuários únicos
    /// </summary>
    public class ImportacaoPlanilhaAtividadesUsuarioViewModel : ImportacaoPlanilhaAtividadesViewModel
    {

        #region propriedades públicas 

        /// <summary>
        /// id do usuário, para uma importação de todas as atividades para um usuário único
        /// aqui há um bad smell de herança negada, já que negamos a propriedade homônima dos
        /// itens ItemImportacaoPlanilha da lista
        /// </summary>
        [Display(Name = "Usuário")]
        public virtual int IdUsuario { get; set; }

        #endregion

    }

    #endregion


}