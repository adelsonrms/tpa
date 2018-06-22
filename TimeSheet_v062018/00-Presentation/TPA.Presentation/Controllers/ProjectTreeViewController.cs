using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TPA.Application;
using TPA.Domain.DomainModel;
using TPA.Infra.Services;
using TPA.Services;
using TPA.Services.Seguranca;
using TPA.ViewModel;
using System.Collections.Generic;
using System.Linq.Dynamic;
using TFW;

namespace TPA.Presentation.Controllers
{


    /// <summary>
    /// controller para a treeview de projectnodes e estruturas de projeto
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Gerenciar ProjectNodes (Treeview)", "Permitir que o usuário gerencie ProjectNodes na visão Treeview")]
    public class ProjectTreeViewController : TPAController
    {

        #region public methods

        /// <summary>
        /// get - treeview de projetos
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Acessar Árvore de Projetos", "Permitir que o usuário Acesse a árvore de projetos")]
        public ActionResult Index()
        {
            ProjectNodeServices srv = new ProjectNodeServices(this.db);
            MvcHtmlString lista = new MvcHtmlString( srv.GetUlTree());
            CarregaNodeLabels();
            return View(model: lista);
        }



        /// <summary>
        /// delete  - excluir nó da treeview
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete, ActionName("Delete")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Excluir da TreeView", "Permitir que o usuário exclua projetos da treeview")]
        public async Task<ActionResult> Delete(int id)
        {
            MensagemParaUsuarioViewModel result = new MensagemParaUsuarioViewModel();
            ProjectNode prj = await db.ProjectNodes.FindAsync(id);

            if (prj != null)
            {

                try
                {
                    if (prj.Filhos != null && prj.Filhos.Count > 0)
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Não se pode excluir um nó de projeto com dependentes. Exclua os itens primeiro.";
                        result.Titulo = "Erro ao excluir nó de projeto";
                    }
                    else if (prj.Atividades != null && prj.Atividades.Count > 0)
                    {
                        result.Sucesso = false;
                        result.Mensagem = "Este nó de projeto não pode ser excluído porque tem atividades lançadas para ele.";
                        result.Titulo = "Erro ao excluir nó de projeto";
                    }
                    else
                    {
                        prj.UsuariosDesteNode.Clear();
                        db.ProjectNodes.Remove(prj);
                        await db.SaveChangesAsync();
                        result.Sucesso = true;
                        result.Mensagem = "Item excluído com sucesso.";
                        result.Titulo = "exclusão de item do projeto";
                    }

                }
                catch (Exception err)
                {
                    LogServices.LogarException(err);
                    result.Sucesso = false;
                    result.Mensagem = err.Message;
                    if(err.InnerException != null)
                    {
                        result.Mensagem += "\r\n" + err.InnerException.Message;
                        if (err.InnerException.InnerException != null)
                        {
                            result.Mensagem += "\r\n" + err.InnerException.InnerException.Message;
                        }
                    }
                    result.Titulo = "Erro ao excluir nó de projeto";
                }
            }
            else
            {
                result.Sucesso = false;
                result.Mensagem = "Não foi selecionado nenhum nó de projeto para exclusão.";
                result.Titulo = "Erro ao excluir nó de projeto";
            }

            return Json(result);
        }



        /// <summary>
        /// post - salvar projeto na treeview
        /// </summary>
        /// <param name="projectNodeVm"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Salvar")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Salvar na TreeView", "Permitir que o usuário salve projetos na treeview")]
        [System.Web.Mvc.ValidateInput(false)]
        public async Task<JsonResult> Salvar([Bind(Include = "Id, Nome, Descricao, HorasEstimadas, IdPai, IdNodeLabel, NomeNodeLabel")] ProjectNodeViewModel projectNodeVm)
        {
            MensagemParaUsuarioViewModel result = new MensagemParaUsuarioViewModel();

            if (ModelState.IsValid)
            {

                try
                {

                    var existente = db.ProjectNodes.Any(x => (x.Pai_Id??0) == projectNodeVm.IdPai && x.Nome == projectNodeVm.Nome && x.Id != projectNodeVm.Id);
                    if(existente)
                    {
                        result.Titulo = "Nome Duplicado.";
                        result.Mensagem = "Já existe um item com esse mesmo nome debaixo deste nó de projeto. \r\n  Itens de projetos com nomes repetidos podem existir dentro de nós de projeto diferentes, mas não debaixo do mesmo nó.";
                        result.Sucesso = false;

                        return Json(result);
                    }

                }
                catch(Exception err)
                {

                    result.Titulo = "Erro.";
                    result.Mensagem = "Não foi possível pesquisar no banco de dados por este item.\r\n " + LogServices.ConcatenaExceptions(err); ;
                    result.Sucesso = false;

                    return Json(result);
                }

                ProjectNode pn = new ProjectNode();

                if( projectNodeVm.Id > 0)
                {
                    pn = db.ProjectNodes.Find(projectNodeVm.Id) ?? new ProjectNode();
                }
                else
                {
                    //só procura o pai para novos
                    if ((projectNodeVm.IdPai ?? 0) > 0)
                    {
                        ProjectNode pai = db.ProjectNodes.Where(x => x.Id == projectNodeVm.IdPai).FirstOrDefault();
                        if (pai != null)
                        {
                            pn.Pai = pai;
                        }
                    }

                    db.ProjectNodes.Add(pn);
                }

                pn.HorasEstimadas = projectNodeVm.HorasEstimadas;
                pn.Nome = projectNodeVm.Nome;
                pn.Descricao = projectNodeVm.Descricao; 

                if ((projectNodeVm.IdNodeLabel ?? 0) > 0)
                {
                    NodeLabel nl = db.NodeLabels.Find(projectNodeVm.IdNodeLabel);
                    if (nl != null)
                    {
                        pn.NodeLabel = nl;
                        
                        projectNodeVm.NomeNodeLabel = nl.Nome;
                    }
                }

                try
                {
                    await db.SaveChangesAsync();

                    result.Titulo = "Salvar nó de projeto.";
                    result.Mensagem = "Salvo com sucesso!";

                    //atualiza o projectNodeVm para os dados voltarem para o node da treeview
                    projectNodeVm.Id = pn.Id;
                    projectNodeVm.IdPai = pn.Pai != null ? pn.Pai.Id : new Nullable<int>();

                    result.Objeto = projectNodeVm;
                    result.Sucesso = true;
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    result.Titulo = "Erro de concorrência ao salvar.";
                    result.Mensagem = "Talvez esse registro tenha sido excluído por outra pessoa. \r\n " + LogServices.ConcatenaExceptions(duce);
                    result.Sucesso = false;


                }
                catch (Exception err)
                {
                    result.Titulo = "Erro ao salvar.";
                    result.Mensagem = "Esse registro não pôde ser salvo.\r\n " + LogServices.ConcatenaExceptions(err); ;
                    result.Sucesso = false;
                }

            }
            else
            {
                result.Titulo = "Estado inválido";
                result.Mensagem = "Favor corrigir erros de preenchimento do formulário";
                result.Sucesso = false;
            }

            return Json(result);
        }



        /// <summary>
        /// post - mover nó de uma pasta para outra
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idNovoPai"></param>
        /// <param name="posicao"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Mover")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Mover nós da TreeView", "Permitir que o usuário mova nós para outras pastas na treeview")]
        public async Task<JsonResult> Mover(int id, int idNovoPai, int posicao)
        {
            if ((id == 0) || (id == idNovoPai))
            {
                return Json(new 
                {
                    Sucesso = false,
                    Mensagem = "O objeto não pode ser movido."
                });
            }

            try
            {
                var node = await db.ProjectNodes.FindAsync(id);
                var novoPai = await db.ProjectNodes.FindAsync(idNovoPai);
                node.Pai = novoPai;

                await db.SaveChangesAsync();

                return Json(new
                {
                    Sucesso = true,
                    Mensagem = "Movido com sucesso."
                });
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                return Json(new
                {
                    Sucesso = false,
                    Mensagem = err.Message
                });
            }
        }



        /// <summary>
        /// post - adicionar pessoas ao projeto
        /// </summary>
        /// <param name="nuvm"></param>
        /// <returns></returns>
        [HttpPost, ActionName("AdicionarRecursos")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Adicionar Recursos", "Permitir que o usuário aloque recursos humanos a um projeto")]
        public async Task<JsonResult> AdicionarRecursos(NodeUsuariosViewModel nuvm)
        {
            UsuarioApplication usuApp = new UsuarioApplication(this.db);

            try
            {

                await usuApp.RemoverUsuariosForaDaLista(nuvm.IdNode, nuvm.IdsUsuarios, nuvm.AdicionarRecursivo);

                if (nuvm.IdsUsuarios != null && nuvm.IdsUsuarios.Count > 0)
                {
                    foreach (int usu in nuvm.IdsUsuarios)
                    {
                        await usuApp.AdicionarNodeAsync(new UsuarioNodeViewModel
                        {
                            AdicionarRecursivo = nuvm.AdicionarRecursivo,
                            IdNode = nuvm.IdNode,
                            IdUsuario = usu
                        });
                    }

                    return Json(new
                    {
                        Sucesso = true,
                        Mensagem = "Recursos adicionados com sucesso."
                    });
                }
                else
                {
                    return Json(new
                    {
                        Sucesso = true,
                        Mensagem = "Recursos removidos com sucesso."
                    });
                }
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                return Json(new
                {
                    Sucesso = false,
                    Mensagem = err.Message
                });
            }

        }



        /// <summary>
        /// get - obter os recursos humanos de uma projeto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Obter Recursos", "Permitir que o usuário obtenha/liste recursos humanos de um projeto")]
        public async Task<JsonResult> ObterRecursos(int id)
        {

            try
            {
                ProjectNode noh = await db.ProjectNodes
                    .Include(x => x.UsuariosDesteNode)
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync();

                return Json(new
                {
                    Sucesso = true,
                    Mensagem = "",
                    Selecionados = noh.UsuariosDesteNode.Select(x => x.Id).ToList<int>().ToArray()
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                return Json(new
                {
                    Sucesso = false,
                    Mensagem = err.Message,
                    Selecionados = new int[] { }
                }, JsonRequestBehavior.AllowGet);
            }

        }



        /// <summary>
        /// get - buscas na treeview
        /// </summary>
        /// <param name="columns">Vetor de Dicionarios de String/String - metadados das colunas que participam da busca</param>
        /// <param name="order">Vetor de Dicionarios de String/String - dados de ordenação</param>
        /// <param name="search">Dicionarios de String/String - dados sobre a busca</param>
        /// <param name="start">int - de onde começa</param>
        /// <param name="length">int - tamanho da busca / até onde vai</param>
        /// <param name="draw">string - mandar o mesmo que a DataTables enviou</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [TPADescricaoAcaoController("Buscas na Treeview", "Permitir que o usuário faça uma busca avançada nos projetos")]
        public async Task<JsonResult> Buscar(Dictionary<string, string>[] columns, Dictionary<string, string>[] order, Dictionary<string, string> search, string start, string length, string draw)
        {
            string procura = search["value"];
            string ordem = order[0]["column"];
            string direcao = order[0]["dir"];

            string coluna = columns[TFWConvert.ToInteger(ordem)]["name"];

            if (string.IsNullOrWhiteSpace(coluna))
                coluna = "Id";
            string clausulaOrderBy = coluna + " " + direcao;

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var queryWhereClause = db.ProjectNodes
                .Include(x => x.NodeLabel)
                .Where(x => x.Nome.ToLower().Contains(procura.ToLower()) || x.Id.ToString() == procura.ToLower());

            var queryOrderByClause = await queryWhereClause
                .AsNoTracking()
                .Select(x => new
                {
                    Id = x.Id,
                    Label = x.NodeLabel.Nome,
                    Nome = x.Nome,
                    Pin = x.Id
                })
                .OrderBy(clausulaOrderBy)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var dados =  queryOrderByClause
                .Select(x => new string[]
                {
                    x.Id.ToString(),
                    x.Label,
                    x.Nome,
                    x.Pin.ToString()
                })
                .ToList();

            var resultados = new
            {
                draw = draw,
                recordsTotal = db.ProjectNodes.Count().ToString(),
                recordsFiltered = queryWhereClause.Count().ToString(),
                data = dados,
            };

            return Json( resultados, JsonRequestBehavior.AllowGet);
        }


        #endregion



        #region private methods

        private void CarregaNodeLabels(int? valor = null)
        {
            var lbls = db.NodeLabels.ToList();
            SelectList lblsSelectList = new SelectList(lbls, "Id", "Nome", valor);
            ViewBag.Labels = lblsSelectList;
        }

        #endregion

    }
}