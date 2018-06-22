using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TFW.Domain;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;
using TPA.Infra.Data.Repository;

namespace TPA.Presentation.Util
{
    /// <summary>
    /// atalhos para obter as coleções mais comuns de itens de combos usando um context independente
    /// </summary>
    public static class Colecoes
    {



        #region métodos públicos estáticos

        /// <summary>
        /// obtém todos os tipos de atividade públicos ou não administrativos em forma de um SelectList 
        /// deixando pré selecionada a opção que vier como parâmetro em caso de edição da form
        /// </summary>
        /// <param name="valor">int? - valor que já estava selecionado ou salvo no Model</param>
        /// <returns>SelectList - lista para usar em combos/dropdowns</returns>
        public static SelectList GetTiposPublicos(int? valor = null)
        {
            using (TPAContext db = new TPAContext())
            {
                TipoAtividadeRepository rep = new TipoAtividadeRepository(db);
                var tipo = rep.GetPublic().OrderBy(x=>x.Nome).ToList();
                SelectList TipoSelectList = new SelectList(tipo, "Id", "Nome", valor);
                return TipoSelectList;
            }
        }


        /// <summary>
        /// Obtém os tipos de atividades administrativos para abono deixando
        /// o parâmetro passado pré selecionado em caso de edição
        /// </summary>
        /// <param name="valor">int? - valor que já estava selecionado ou salvo no Model</param>
        /// <returns>SelectList - lista para usar em combos/dropdowns</returns>
        public static SelectList GetTiposAdmin(int? valor = null)
        {
            using (TPAContext db = new TPAContext())
            {
                TipoAtividadeRepository rep = new TipoAtividadeRepository(db);
                var tipo = rep.GetAdmin().OrderBy(x => x.Nome).ToList();
                SelectList TipoSelectList = new SelectList(tipo, "Id", "Nome", valor);
                return TipoSelectList;
            }
        }


        /// <summary>
        /// obtém todos os tipos de atividade em forma de um SelectList 
        /// deixando pré selecionada a opção que vier como parâmetro em caso de edição da form
        /// </summary>
        /// <param name="valor">int? - valor que já estava selecionado ou salvo no Model</param>
        /// <returns>SelectList - lista para usar em combos/dropdowns</returns>
        public static SelectList GetTodosTipos(int? valor = null)
        {
            using (TPAContext db = new TPAContext())
            {
                TipoAtividadeRepository rep = new TipoAtividadeRepository(db);
                var tipo = rep.GetAll().OrderBy(x => x.Nome).ToList();
                SelectList TipoSelectList = new SelectList(tipo, "Id", "Nome", valor);
                return TipoSelectList;
            }
        }

        /// <summary>
        /// obtém todos os nós de projeto em forma de um TFWHierarchicalList
        /// deixando pré selecionada a opção que vier como parâmetro em caso de edição da form
        /// </summary>
        /// <param name="valor">int? - valor que já estava selecionado ou salvo no Model</param>
        /// <returns>TFWHierarchicalList - lista para usar em um combo com simulação de hierarquia no estilo</returns>
        public static TFWHierarchicalList GetNodes(int? valor = null)
        {
            TPAContext db = new TPAContext();
            TFWHierarchicalList lstUsuario = new TFWHierarchicalList();
            TFWHierarchicalList lst = new TFWHierarchicalList();
            List<int> idsNodesUsuario = new List<int>();

            var usu = HttpContext.Current.User.Identity.Name;
            var usuLogado = db.Usuarios.Where(u => u.Login == usu).FirstOrDefault();
            if (usuLogado != null)
            {
                var nodes = usuLogado.NosDoUsuario.ToList();
                if (nodes != null && nodes.Any())
                {
                    foreach (var n in nodes)
                    {
                        lstUsuario.Add(n.Id, n.Pai != null ? n.Pai.Id : new Nullable<int>(), n.Nome);
                    }
                }
            }

            idsNodesUsuario.AddRange(lstUsuario.Select(s => s.Id).ToList());

            foreach (var node in db.ProjectNodes.ToList())
            {
                if (idsNodesUsuario.Contains(node.Id))
                {
                    lst.Add(node.Id, node.Pai_Id != null ? node.Pai_Id : new Nullable<int>(), node.Nome);
                }
                else
                {
                    lst.Add(node.Id, node.Pai != null ? node.Pai.Id : new Nullable<int>(), node.Nome, false);
                }
            }

            return lst;


        }


        /// <summary>
        /// retorna uma MultiSelectList de usuários com os usuários passados no parâmetro pré selecionados
        /// </summary>
        /// <param name="valores">int[] - valores selecionados</param>
        /// <returns>MultiSelectList - lista múltipla com os usuários</returns>
        public static MultiSelectList GetUsuariosMultiplos(params int[] valores)
        {
            using (TPAContext db = new TPAContext())
            {
                UsuarioRepository rep = new UsuarioRepository(db);
                List<Usuario> usuarios = rep.GetAll();

                MultiSelectList result = new MultiSelectList(usuarios, "Id", "FuncionarioNomeOuLogin", valores);
                return result;
            }
        }



        /// <summary>
        /// retorna uma SelectList de usuários com o usuário passado no parâmetro pré selecionado
        /// </summary>
        /// <param name="valores">int? - valor pré selecionado</param>
        /// <returns>SelectList - lista de usuários</returns>
        public static SelectList GetUsuarios(int? valor = null)
        {
            using (TPAContext db = new TPAContext())
            {
                UsuarioRepository rep = new UsuarioRepository(db);
                List<Usuario> usuarios = rep.GetAll();

                SelectList result = new SelectList(usuarios, "Id", "FuncionarioNomeOuLogin", valor);
                return result;
            }
        }

        #endregion


    }
}