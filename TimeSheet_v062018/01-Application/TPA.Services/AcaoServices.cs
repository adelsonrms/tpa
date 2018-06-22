using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;
using TPA.Infra.Data.Repository;
using TPA.Infra.Services;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Services
{
    /// <summary>
    /// classe que controla cadastros de tipos de permissões / ações que os usuários podem tomar
    /// cadastra também os nomes dos controller/actions para um controle de segurança mais preciso
    /// </summary>
    public class AcaoServices
    {


        #region métodos públicos estáticos

        /// <summary>
        /// método estático independente com context próprio para ser executado em jobs 
        /// </summary>
        public static void AtualizaAcoesCopiandoDoAssembly()
        {
            try
            {
                Type tipo = Type.GetType("TPA.Presentation.Controllers.TPAController, TPA.Presentation");
                using (TPAContext db = new TPAContext())
                {
                    AcaoServices g = new AcaoServices(db);
                    g.ImportarDoAssembly(tipo);
                    g.AtualizaAdmin();
                }
            }
            catch(Exception err)
            {
                LogServices.LogarException(err);
            }
        }

        #endregion


        #region campos privados

        /// <summary>
        /// contexto da aplicação
        /// </summary>
        private TPAContext _db;


        #endregion


        #region constructor

        /// <summary>
        /// cosntrutor padrão, injeta o contexto
        /// </summary>
        /// <param name="ctx"></param>
        public AcaoServices(TPAContext ctx)
        {
            this._db = ctx;
        }

        #endregion


        #region métodos públicos

        /// <summary>
        /// obtém do assembly os novos controllers/actions para cadastrar na tabela Acao
        /// </summary>
        /// <param name="tipo">Type - tipo de onde vão ser retiradas as actions, geralmente um controller</param>
        /// <returns>int - O número de ações cadastradas</returns>
        public int ImportarDoAssembly(Type tipo, bool atualizarDescricoes = false)
        {
            int result = 0;

            Assembly asm = Assembly.GetAssembly(tipo);

            AcaoRepository rep = new AcaoRepository(this._db);


            ///obtém a lista de actions do assembly
            ///http://stackoverflow.com/questions/21583278/getting-all-controllers-and-actions-names-in-c-sharp
            List<SegurancaActionViewModel> actionList =
                (
                    from t in asm.GetTypes().SelectMany(tp => tp.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public), (parent, child) => new { TipoController = parent, MetodoAction = child })
                    where tipo.IsAssignableFrom(t.TipoController) 
                    orderby t.MetodoAction.Name
                    select new SegurancaActionViewModel
                    {
                        NomeAction = t.TipoController.Name + "/" + t.MetodoAction.Name,
                        Nome = ((t.MetodoAction.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute(t.MetodoAction.Name, "")).Nome,
                        Descricao = ((t.MetodoAction.GetCustomAttribute(typeof(TPADescricaoAcaoControllerAttribute), false) as TPADescricaoAcaoControllerAttribute) ?? new TPADescricaoAcaoControllerAttribute("", t.TipoController.Name + "/" + t.MetodoAction.Name)).Descricao,

                    }
                ).ToList<SegurancaActionViewModel>();



            foreach (SegurancaActionViewModel a in actionList)
            {
                Acao acao = rep.GetByName(a.NomeAction);
                if (acao == null)
                {
                    acao = new Acao();
                    acao.Nome = a.NomeAction;
                    acao.NomeAmigavel = a.Nome;
                    acao.DescricaoAmigavel = a.Descricao;
                    rep.Save(acao);
                }
                else if (atualizarDescricoes)
                {
                    acao.NomeAmigavel = a.Nome;
                    acao.DescricaoAmigavel = a.Descricao;
                    rep.Save(acao);
                }


                result++;
            }


            //obtém a lista de acoes do SegurancaResources
            ResourceSet resourceSet = SegurancaResources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            List<string> listaResources = new List<string>();

            foreach (DictionaryEntry entry in resourceSet)
            {
                listaResources.Add(entry.Value.ToString());
            }

            var ResourcesParaCadastrar = listaResources.Where(x => !this._db.Acoes.Select(a => a.Nome).Any(c => c == x)).ToList<string>();

            foreach (string s in ResourcesParaCadastrar)
            {
                this._db.Acoes.Add(new Acao
                {
                    Nome = s
                });

                result++;
            }


            this._db.SaveChanges();

            return result;
        }


        /// <summary>
        /// Sincroniza o admin lançando para ele as novas ações encontradas
        /// </summary>
        public void AtualizaAdmin()
        {
            var admin = this._db.Perfis.Where(p => p.Nome == "Admin").First();

            if (admin.Acoes == null)
                admin.Acoes = new List<Acao>();

            admin.Acoes.Clear();
            foreach (Acao a in this._db.Acoes)
            {
                admin.Acoes.Add(a);
            }

            this._db.SaveChanges();
        }

        #endregion


    }
}