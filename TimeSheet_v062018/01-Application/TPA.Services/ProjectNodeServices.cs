using System;
using System.Collections.Generic;
using System.Linq;
using TPA.Domain.DomainModel;
using TFW.Domain;
using TPA.Infra.Data;
using System.Web;

namespace TPA.Services
{

    /// <summary>
    /// classes para consultar / formatar project nodes para apresentar na treeview
    /// </summary>
    public  class ProjectNodeServices
    {

        #region propriedades privadas

        /// <summary>
        /// contexto da aplicação
        /// </summary>
        private TPAContext _db;

        #endregion



        #region construtores

        /// <summary>
        /// construtor padrao passa o contexto
        /// </summary>
        /// <param name="db"></param>
        public ProjectNodeServices(TPAContext db)
        {
            this._db = db;
        }

        #endregion



        #region métodos públicos

        /// <summary>
        /// obtém um ProjectNode pelo id
        /// todo: usar o repositório na implementação desse método
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProjectNode GetById(int id)
        {
            var nl = this._db.ProjectNodes.Find(id);
            return nl;
        }

        /// <summary>
        /// obtém uma estrutura TFWHierarchicalList com os projectnodes montados
        /// </summary>
        /// <returns>TFWHierarchicalList - lista com todos os projectnodes em forma de elementos hierárquicos</returns>
        public TFWHierarchicalList GetTree()
        {
            TFWHierarchicalList result = new TFWHierarchicalList();
            foreach(var n in this._db.ProjectNodes.AsNoTracking().OrderBy(x => x.Nome))
            {
                result.Add(n.Id, n.Pai != null ? n.Pai.Id : new Nullable<int>(), n.NodeLabel.Nome + ": " + n.Nome);
            }

            return result;
        }

        /// <summary>
        /// obtém os nós formatados em html para exibição em treeview
        /// </summary>
        /// <returns>string - html com os nodes</returns>
        public  string GetUlTree()
        {
            var nodes = this._db.ProjectNodes.AsNoTracking().Where(x => x.Pai == null).OrderBy(x => x.Nome).ToList();         
            return GetList(nodes);
        }

        #endregion



        #region métodos privados

        /// <summary>
        /// obtém a lista de nodes em formato de itens de menu html
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static string GetList(IEnumerable<ProjectNode> nodes)
        {
            string casca = "<ul>{0}</ul>";

            string miolo = "";
            foreach(var n in nodes)
            {
                if(n.Filhos != null && n.Filhos.Count > 0)
                {
                    miolo += string.Format("<li  data-nodelabelnome='{0}'  data-nodenome='{1}' data-nodelabelid='{2}' data-idnode='{3}' id='li{3}'  data-nodehoras='{4}' data-idpai='{5}' data-nodedescricao='{7}'  data-jstree='{{\"icon\":\"jstree-icon jstree-folder\"}}'>  <strong>{0}</strong>:{1} {6}  </li>",
                        HttpUtility.HtmlEncode(n.NodeLabel.Nome),
                        HttpUtility.HtmlEncode(n.Nome),
                        n.NodeLabel.Id,
                        n.Id,
                        n.HorasEstimadas, 
                        n.Pai!=null?n.Pai.Id.ToString():"",
                        GetList(n.Filhos.OrderBy(x => x.Nome)),
                        HttpUtility.HtmlEncode(n.Descricao));
                }
                else
                {
                    miolo += string.Format("<li  data-nodelabelnome='{0}'  data-nodenome='{1}' data-nodelabelid='{2}' data-idnode='{3}' id='li{3}' data-nodehoras='{4}' data-idpai='{5}' data-nodedescricao='{6}' data-jstree='{{\"icon\":\"jstree-icon jstree-file\"}}'>  <strong>{0}</strong>:{1}  </li>",
                        HttpUtility.HtmlEncode(n.NodeLabel.Nome),
                        HttpUtility.HtmlEncode(n.Nome), 
                        n.NodeLabel.Id, 
                        n.Id,  
                        n.HorasEstimadas,
                        n.Pai != null ? n.Pai.Id.ToString() : "",
                        HttpUtility.HtmlEncode(n.Descricao));
                }
            }

            string result = string.Format(casca, miolo);
            return result;
        }

        #endregion

    }
}