using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TFW.Domain;
using TFW.WebForms.Extensions;

namespace TPA.Presentation.Util
{

    #region classe TreeViewComboBuilder

    /// <summary>
    /// classe responsável por montar treeviews hierárquicos, boaseados em bootstrap e em uma estilização especial no site.css
    /// </summary>
    public class TreeViewComboBuilder
    {

        #region campos privados

        /// <summary>
        /// lista de options
        /// </summary>
        private List<string> _items = new List<string>();
        /// <summary>
        /// nome do controle para o servidor
        /// </summary>
        private string _name;
        /// <summary>
        /// id do controle para o javascript
        /// </summary>
        private string _id;
        /// <summary>
        /// classe css para o controle
        /// </summary>
        private string _classe;

        #endregion



        #region contructors

        /// <summary>
        /// cria um combo com aspecto hierárquico
        /// </summary>
        /// <param name="name">name do controle para o server</param>
        /// <param name="id">id do controle para o javascript</param>
        /// <param name="classe">classe do controle</param>
        public TreeViewComboBuilder(string name, string id, string classe)
        {
            this._name = name;
            this._id = id;
            this._classe = classe;
        }
        #endregion



        #region métodos privados

        /// <summary>
        /// obtém a casca pré formatada do select
        /// </summary>
        /// <returns>string - casca do select</returns>
        private string GetCasca()
        {
            return "<select name=\"{0}\"  id=\"{1}\" class=\"{2}\" {4} >\r\n{3}\r\n</select>";
        }

        /// <summary>
        /// obtém o miolo do select, os options
        /// </summary>
        /// <returns>string - miolo do select</returns>
        private string GetMiolo()
        {
            return string.Join("\r\n", _items);
        }

        /// <summary>
        /// método privado recursivo que constroi e estiliza os itens de um DropDownList ou ListBox hierárquico
        /// </summary>
        /// <param name="lista">HierarchicalList a ter seus itens publicados</param>
        /// <param name="useValorpath">bool - define se usará um tooltip com o caminho completo ou não</param>
        /// <param name="formato">FormatacaoDeArvore - define se permite ou não seleção de itens com nodes filhos</param>
        /// <param name="nivel">int - nivel corrente na hierarquia</param>
        /// <param name="primeiroVazio">bool - define se o primeiro item é vazio ou não. É passado true somente na primeira iteração recursiva.</param>
        private  void _scanTrees( TFWHierarchicalList lista, string valorAtual, bool useValorpath, TFWFormatacaoDeArvore formato, int nivel = 0, bool primeiroVazio = true)
        {
            if (primeiroVazio && (nivel == 0))
            {
                _items.Add("<option value=\"\"></option>");
            }

            foreach (var c in lista)
            {
                string itemStr = "<option {3} {4} {5} data-toggle=\"tooltip\" title=\"{2}\" value=\"{0}\">{1}</option>";

                string[] parts = c.ValorPath.Split('/').ToArray();
                string caminho = String.Join("/", parts);

                string valor = c.Id.ToString("0");
                string texto = (useValorpath && !string.IsNullOrWhiteSpace(caminho)) ? caminho : c.Valor;

                var path = c.ValorPath;
                string title = path;
                string classe = "";
                string disabl = "";
                string selected = c.Id.ToString("0") == valorAtual?"selected":"";

                //deve repetir essa verificação antes da chamada recursiva e antes de adicionar o item
                if (c.HasChildren)
                {
                    switch (formato)
                    {
                        case TFWFormatacaoDeArvore.SugerirApenasFolhas:
                            classe = string.Format ("class=\"{0}\"", "OptionNivel" + nivel.ToString("0") + " disabled");
                            break;
                        case TFWFormatacaoDeArvore.PermitirApenasFolhas:
                            classe = string.Format("class=\"{0}\"", "OptionNivel" + nivel.ToString("0") + " disabled");
                            disabl = "disabled=\"disabled\"";
                            break;
                        default:
                            classe = string.Format("class=\"{0}\"", "OptionNivel" + nivel.ToString("0"));
                            break;
                    }
                }
                else
                {
                    classe = string.Format("class=\"{0}\"", "OptionNivel" + nivel.ToString("0"));
                }

                if (!c.Ativo)
                {
                    disabl = "disabled=\"disabled\"";
                }


                _items.Add(string.Format(itemStr, valor, texto, title, classe, disabl, selected));

                if (c.HasChildren)
                {
                    _scanTrees( (TFWHierarchicalList)c.GetChildren(), valorAtual, useValorpath, formato, nivel + 1, false);
                }
            }

        }

        /// <summary>
        /// obtém as raízes de uma HierarchicalList
        /// </summary>
        /// <param name="lista">HierarchicalList - lista hierárquica a se obterem as raizes</param>
        /// <returns>HierarchicalList - lista apenas com as raízes</returns>
        private  TFWHierarchicalList _getRoots(TFWHierarchicalList lista)
        {
            //return lista.GetRoot();
            var orfaos = lista.Where(x => !lista.Select(i => i.Id).Contains(x.IdPai ?? 0)).ToList();
            return new TFWHierarchicalList(orfaos);
        }


        #endregion



        #region métodos públicos

        /// <summary>
        /// percorre uma lista hierárquica em usa profundidade para adicionar os itend em um ListBox
        /// </summary>
        /// <param name="lista">HierarchicalList - lisa hierárquica a ser exibida em um combo</param>
        /// <param name="useValorpath">bool - define se deve ser criado um hint/tooltip com o caminho completo do item</param>
        /// <param name="formato">define se é permitido ou não nodes com filhos, e se eles são selecionáveis ou não</param>
        public  void HierarchicalDataBind(TFWHierarchicalList lista, string valorAtual, bool useValorpath = false, TFWFormatacaoDeArvore formato = TFWFormatacaoDeArvore.PermitirNosComFilhos)
        {
            _scanTrees( _getRoots(lista), valorAtual, useValorpath, formato, 0, true);
        }

        /// <summary>
        /// Junta e formata todas as strings e atributos pra formar um objeto html select completo
        /// </summary>
        /// <param name="htmlAttributes">Objeto - lista de atributos html para adicionar ao DOM</param>
        /// <returns>string - O HTML do select</returns>
        public string GetDropDown(object htmlAttributes = null)
        {
            string attributes = "";
            if (htmlAttributes != null)
            {
                var RouteData = new  RouteValueDictionary(htmlAttributes);

                attributes = RouteData.Select(s => string.Format("{0}=\"{1}\"", s.Key.Replace("_", "-"), s.Value))
                    .Aggregate((current, next) => string.Format("{0} {1}", current, next));
            }

            return string.Format(GetCasca(), _name, _id, _classe, GetMiolo(), attributes);
        }


        #endregion
    }

    #endregion



    #region classe SelectExtensions

    /// <summary>
    /// classes de extensão / HTML helpers para a criação de dropdown hierárquico no razor
    /// HTML Helper para facilitar o uso de TreeViewComboBuilder no Razor
    /// </summary>
    public static class SelectExtensions
    {

        #region métodos públicos estáticos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper">HtmlHelper - helper para chamar esse método a partir de @HTML no razor</param>
        /// <param name="name">string - name do controle/input no formulário</param>
        /// <param name="id">string - id do controle no formulário</param>
        /// <param name="classe">string - classe OptionNivelX para identificar a profundidade da classe</param>
        /// <param name="lista">TFWHierarchicalList - lista hierárquica com os dados estruturados</param>
        /// <param name="valorAtual">string - valor atual selecionado</param>
        /// <param name="useValorpath">bool - ture para mostrar o path inteiro, false para mostrar só o valor corrente</param>
        /// <param name="formato">TFWFormatacaoDeArvore - define se deve permitir ou restringir a seleção de opções raiz, galhos ou pais de grupos</param>
        /// <param name="htmlAttributes">Objeto - lista de atributos para serem adicionados como atributos html no código gerado</param>
        /// <returns>MvcHtmlString - string razor com o HTML para gerar um dropdown hierárquico</returns>
        public static MvcHtmlString DropDownHierarquico(
            this HtmlHelper helper, 
            string name, 
            string id, 
            string classe, 
            TFWHierarchicalList lista, 
            string valorAtual, 
            bool useValorpath = false, 
            TFWFormatacaoDeArvore formato = TFWFormatacaoDeArvore.PermitirNosComFilhos,
            object htmlAttributes = null)
        {
            TreeViewComboBuilder trv = new TreeViewComboBuilder(name, id, classe);
            trv.HierarchicalDataBind(lista, valorAtual, useValorpath, formato);
            return new MvcHtmlString( trv.GetDropDown(htmlAttributes));
        }


        #endregion
    }

    #endregion


}