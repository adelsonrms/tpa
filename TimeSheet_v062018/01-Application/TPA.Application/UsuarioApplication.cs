using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;
using TPA.Infra.Data.Repository;
using TPA.Infra.Services;
using TPA.Services;
using TPA.ViewModel;

namespace TPA.Application
{
    /// <summary>
    /// exceptions especiais com descrição amigável que podem acontecer em UsuarioApplication
    /// </summary>
    public class UsuarioApplicationException : Exception
    {
        public UsuarioApplicationException()
        {
        }

        public UsuarioApplicationException(string message)
        : base(message)
        {
        }

        public UsuarioApplicationException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }

    /// <summary>
    /// classe de controle/regras de negócio para lidar com usuários
    /// </summary>
    public class UsuarioApplication
    {

        #region métodos estáticos públicos

        /// <summary>
        /// dispara alertas de atraso nas atividades diariamente
        /// </summary>
        public static void EnviaAlertasAgendados()
        {
            try
            {
                if (!DevServices.IsDevEnv())
                {
                    using (var db = new TPAContext())
                    {
                        UsuarioApplication usuApp = new UsuarioApplication(db);
                        usuApp.EnviarTodosOsAlertas(true, true);

                    }
                }
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
            }
        }

        /// <summary>
        /// testa os logs dos serviços diários
        /// </summary>
        public static void LogServicoDiario()
        {
            LogServices.Logar("Serviço diário");
        }

        #endregion

        #region private fields
        /// <summary>
        /// a instância do DBContext para esse application
        /// </summary>
        private TPAContext _db;
        #endregion


        #region contructors 

        /// <summary>
        /// constructor que obtém como parâmetros um context e um repository 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rep"></param>
        public UsuarioApplication(TPAContext db)
        {
            this._db = db;
        }



        #endregion



        #region métodos públicos

        #region métodos assíncronos

        /// <summary>
        /// retorna um usuário pelo seu id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<Usuario> GetByIdAsync(int id)
        {
            UsuarioRepository rep = new UsuarioRepository(this._db);
            return await rep.GetByIdAsync(id);
        }

        /// <summary>
        /// retorna de forma assíncrona a lista com todos os usuários e seu último lançamento
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<UsuarioAlertaLancamentoViewModel>> GetUltimosLancamentosAsync()
        {
            CalendarioServices calendarioSvc = new CalendarioServices();
            var rep = new UsuarioRepository(_db);
            var results = await rep.GetUltimosLancamentosAsync();
            results.ForEach(x => x.DiasUteisSemLancar = x.UltimoLancamento != null ? calendarioSvc.DiasUteisEntreDatas(x.UltimoLancamento.Value, DateTime.Now): 0);
            return results;
        }


        /// <summary>
        /// retorna  de forma assíncrona o usuário e seu último lançamento
        /// </summary>
        /// <returns></returns>
        public virtual async Task<UsuarioAlertaLancamentoViewModel> GetUltimosLancamentosByIdAsync(int id)
        {
            CalendarioServices calendarioSvc = new CalendarioServices();
            var rep = new UsuarioRepository(_db);
            var results = await rep.GetUltimosLancamentosByIdAsync(id);
            results.DiasUteisSemLancar = results.UltimoLancamento != null ? calendarioSvc.DiasUteisEntreDatas(results.UltimoLancamento.Value, DateTime.Now) : 0;
            return results;
        }

        /// <summary>
        /// retorna só os usuários atrasados a mais de um dia útil
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<UsuarioAlertaLancamentoViewModel>> GetUsuariosComAtrasoNoEnvioAsync()
        {
            List<UsuarioAlertaLancamentoViewModel> result = new List<UsuarioAlertaLancamentoViewModel>();
            var ultimosLanctos = await GetUltimosLancamentosAsync();
            var atrasados = ultimosLanctos.Where(x => x.DiasUteisSemLancar > 1 && x.Usuario.EnviarAlertaLancamento == true).OrderByDescending(x => x.DiasUteisSemLancar).ToList();
            if (atrasados.Count > 0)
                result.AddRange(atrasados);
            return result;
        }



        /// <summary>
        /// verifica se um e-mail já existe antes de cadastrar
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> VerificaExistenciaEmailAsync(string email, int? id = null)
        {
            UsuarioRepository rep = new UsuarioRepository(this._db);

            return await rep.VerificaExistenciaEmailAsync(email, id);
        }

        /// <summary>
        /// salva um usuário de forma assíncrona
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual async Task SaveAsync(Usuario ent)
        {
            UsuarioRepository rep = new UsuarioRepository(this._db);
            await rep.SaveAsync(ent);
        }






        /// <summary>
        /// Evia o alerta de lançamento em atraso para um usuário
        /// </summary>
        /// <param name="idUsuario">id do usuário que vai receber o alerta</param>
        /// <param name="email">boolean - se vai receber por e-mail ou não</param>
        /// <param name="sms">boolean - se vai receber por sms ou não</param>
        /// <param name="telegram">boolean - se vai receber por telegram ou não</param>
        /// <returns></returns>
        public virtual async Task EnviarAlertaAsync(int idUsuario, bool email = true, bool sms = false)
        {
            string smsResult = "";

            if (idUsuario == 0)
            {
                throw new Exception("O usuário não foi informado");
            }

            var modelo = await this.GetUltimosLancamentosByIdAsync(idUsuario);

            if (modelo == null)
            {

                throw new Exception("O usuário não foi encontrado");
            }


            try
            {
                if(email)
                    MailServices.Enviar(modelo.Usuario.EmailPreferencial, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", modelo.DiasUteisSemLancar));
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                throw new Exception("Erro ao enviar email: " + err.Message, err);
            }


            try
            {
                if (sms && modelo.Usuario.Funcionario != null && !string.IsNullOrWhiteSpace(modelo.Usuario.Funcionario.Celular))
                {
                    smsResult = SMSServices.Enviar(modelo.Usuario.Funcionario.Celular, modelo.Usuario.FuncionarioNomeOuLogin, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", modelo.DiasUteisSemLancar));
                    LogServices.Logar(smsResult);
                }
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                throw new Exception("Erro ao enviar sms: " + err.Message, err);
            }


        }


        /// <summary>
        /// Evia o alerta de lançamento em atraso para todos os usuários
        /// </summary>
        /// <param name="email">boolean - se vai receber por e-mail ou não</param>
        /// <param name="sms">boolean - se vai receber por sms ou não</param>
        /// <param name="telegram">boolean - se vai receber por telegram ou não</param>
        /// <returns></returns>
        public virtual async Task EnviarTodosOsAlertasAsync(bool email = true, bool sms = false)
        {
            var modelo = await this.GetUsuariosComAtrasoNoEnvioAsync();

            if ((modelo == null) || (modelo.Count == 0))
            {
                throw new Exception("O usuário não foi encontrado");
            }


            bool erro = false;
            string mensagens = "";

            foreach (var x in modelo)
            {

                try
                {
                    if(email)
                        MailServices.Enviar(x.Usuario.EmailPreferencial, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", x.DiasUteisSemLancar));
                }
                catch (Exception err)
                {
                    erro = true;
                    mensagens += "Ocorreu um erro ao enviar o alerta via e-mail para " + x.Usuario.Login + ": " + err.Message + "<br/>\r\n";
                    LogServices.LogarException(err);
                }

                string smsResult = "";
                try
                {
                    if (sms && x.Usuario.Funcionario != null && !string.IsNullOrWhiteSpace(x.Usuario.Funcionario.Celular))
                    {
                        smsResult = SMSServices.Enviar(x.Usuario.Funcionario.Celular, x.Usuario.FuncionarioNomeOuLogin, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", x.DiasUteisSemLancar));
                        LogServices.Logar(smsResult);
                    }
                }
                catch (Exception err)
                {
                    erro = true;
                    mensagens += "Ocorreu um erro ao enviar o alerta via sms para " + x.Usuario.Login + ": " + err.Message + "<br/>\r\n";
                    LogServices.LogarException(err);
                }



            }

            if (erro)
            {
                throw new Exception("Erros ocorridos: " + mensagens);
            }

        }



        /// <summary>
        /// Adiciona um nó ou a hierarquia abaixo dele a um usuário
        /// </summary>
        /// <param name="urvm">UsuarioNodeViewModel - View Model da página AdicionarNode</param>
        /// <returns></returns>
        public virtual async Task AdicionarNodeAsync(UsuarioNodeViewModel urvm)
        {
            Usuario usuario = await this._db.Usuarios.FindAsync(urvm.IdUsuario);
            var node = await this._db.ProjectNodes.FindAsync(urvm.IdNode);

            if (usuario == null)
            {
                throw new UsuarioApplicationException("Usuário não encontrado");
            }

            if (node == null)
            {
                throw new UsuarioApplicationException("Node não encontrado");
            }

            urvm.Usuario = usuario;
            urvm.Node = node;
            urvm.IdNode = node.Id;

            if (urvm.AdicionarRecursivo)
            {
                AdicionaNodesRecursivamente(usuario, node);
            }
            else
            {
                bool jaPossui = usuario.NosDoUsuario.Contains(node);
                if (!jaPossui)
                {
                    usuario.NosDoUsuario.Add(node);
                }
            }

            await this._db.SaveChangesAsync();
        }



        /// <summary>
        /// remover recursivamente todos os nós de projeto de um usuário
        /// </summary>
        /// <param name="IdUsuarioExclusao">int - id do usuário</param>
        /// <param name="IdNodeExclusao">int - id do node a ser removido</param>
        /// <returns></returns>
        public virtual async Task RemoverNodeAsync(int IdUsuarioExclusao, int IdNodeExclusao, bool Recursivo = false)
        {
            if (IdUsuarioExclusao == 0 || IdNodeExclusao == 0)
                throw new UsuarioApplicationException("O Usuário e o Node a ser excluído devem ser informados.");

            Usuario usuario = this._db.Usuarios.Find(IdUsuarioExclusao);
            if (usuario != null)
            {
                ProjectNode node = usuario.NosDoUsuario.Where(x => x.Id == IdNodeExclusao).FirstOrDefault();
                if (node != null)
                {


                    if (Recursivo)
                    {
                        RemoveNodesRecursivamente(usuario, node);
                    }
                    else
                    {
                        bool possui = usuario.NosDoUsuario.Contains(node);
                        if (possui)
                        {
                            usuario.NosDoUsuario.Remove(node);
                        }
                    }

                    await this._db.SaveChangesAsync();
                }
                else
                {
                    throw new UsuarioApplicationException("Node não encontrado.");
                }
            }
            else
            {
                throw new UsuarioApplicationException("Usuário não encontrado.");
            }
        }



        /// <summary>
        /// Remove de um node todos os usuários que não estão na lista passada no segundo parâmetro
        /// </summary>
        /// <param name="idNode">int - id do Node</param>
        /// <param name="idsUsuarios">int[] - array com os usuários que deveriam ficar</param>
        /// <returns></returns>
        public virtual async Task RemoverUsuariosForaDaLista(int idNode, List<int> idsUsuarios, bool recursivo = false)
        {
            if(idNode == 0)
                throw new UsuarioApplicationException("Id do Node não informado.");

            ProjectNode node = await _db.ProjectNodes.FindAsync(idNode);

            if(node == null)
                throw new UsuarioApplicationException("Node não encontrado.");

            List<Usuario> removidos = new List<Usuario>();
            if (idsUsuarios == null || idsUsuarios.Count == 0)
            {
                removidos.AddRange(node.UsuariosDesteNode);
            }
            else
            {
                removidos = node.UsuariosDesteNode.Where(x => !idsUsuarios.Contains(x.Id)).ToList();
            }

            removidos.ForEach(u =>
            {
                if (recursivo)
                    RemoveNodesRecursivamente(u, node);
                else
                    node.UsuariosDesteNode.Remove(u);
            });

            await _db.SaveChangesAsync();
        }

        #endregion




        #region métodos síncronos

        /// <summary>
        /// retorna um usuário pelo seu id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Usuario GetById(int id)
        {
            UsuarioRepository rep = new UsuarioRepository(this._db);
            return  rep.GetById(id);
        }

        /// <summary>
        /// retorna a lista com todos os usuários e seu último lançamento
        /// </summary>
        /// <returns></returns>
        public virtual  List<UsuarioAlertaLancamentoViewModel> GetUltimosLancamentos()
        {
            CalendarioServices calendarioSvc = new CalendarioServices();
            var rep = new UsuarioRepository(_db);
            var results = rep.GetUltimosLancamentos();
            results.ForEach(x => x.DiasUteisSemLancar = x.UltimoLancamento != null ? calendarioSvc.DiasUteisEntreDatas(x.UltimoLancamento.Value, DateTime.Now) : 0);
            return results;
        }


        /// <summary>
        /// retorna o usuário e seu último lançamento
        /// </summary>
        /// <returns></returns>
        public virtual UsuarioAlertaLancamentoViewModel GetUltimosLancamentosById(int id)
        {
            CalendarioServices calendarioSvc = new CalendarioServices();
            var rep = new UsuarioRepository(_db);
            var results = rep.GetUltimosLancamentosById(id);
            results.DiasUteisSemLancar = results.UltimoLancamento != null ? calendarioSvc.DiasUteisEntreDatas(results.UltimoLancamento.Value, DateTime.Now) : 0;
            return results;
        }

        /// <summary>
        /// retorna só os usuários atrasados a mais de um dia útil
        /// </summary>
        /// <returns></returns>
        public virtual List<UsuarioAlertaLancamentoViewModel> GetUsuariosComAtrasoNoEnvio()
        {
            List<UsuarioAlertaLancamentoViewModel> result = new List<UsuarioAlertaLancamentoViewModel>();
            var ultimosLanctos =  GetUltimosLancamentos();
            var atrasados = ultimosLanctos.Where(x => x.DiasUteisSemLancar > 1 && x.Usuario.EnviarAlertaLancamento == true).OrderByDescending(x => x.DiasUteisSemLancar).ToList();
            if (atrasados.Count > 0)
                result.AddRange(atrasados);
            return result;
        }


        /// <summary>
        /// verifica se um e-mail já existe antes de cadastrar
        /// </summary>
        /// <param name="email"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool VerificaExistenciaEmail(string email, int? id = null)
        {
            UsuarioRepository rep = new UsuarioRepository(this._db);

            return  rep.VerificaExistenciaEmail(email, id);
        }


        /// <summary>
        /// salva um usuário
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public virtual void Save(Usuario ent)
        {
            UsuarioRepository rep = new UsuarioRepository(this._db);
            rep.Save(ent);
        }





        /// <summary>
        /// Evia o alerta de lançamento em atraso para um usuário
        /// </summary>
        /// <param name="idUsuario">id do usuário que vai receber o alerta</param>
        /// <param name="email">boolean - se vai receber por e-mail ou não</param>
        /// <param name="sms">boolean - se vai receber por sms ou não</param>
        /// <param name="telegram">boolean - se vai receber por telegram ou não</param>
        /// <returns></returns>
        public virtual void EnviarAlerta(int idUsuario, bool email = true, bool sms = false)
        {
            string smsResult = "";

            if (idUsuario == 0)
            {
                throw new Exception("O usuário não foi informado");
            }

            var modelo =  this.GetUltimosLancamentosById(idUsuario);

            if (modelo == null)
            {

                throw new Exception("O usuário não foi encontrado");
            }


            try
            {
                if (email)
                    MailServices.Enviar(modelo.Usuario.EmailPreferencial, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", modelo.DiasUteisSemLancar));
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                throw new Exception("Erro ao enviar email: " + err.Message, err);
            }


            try
            {
                if (sms && modelo.Usuario.Funcionario != null && !string.IsNullOrWhiteSpace(modelo.Usuario.Funcionario.Celular))
                {
                    smsResult = SMSServices.Enviar(modelo.Usuario.Funcionario.Celular, modelo.Usuario.FuncionarioNomeOuLogin, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", modelo.DiasUteisSemLancar));
                    LogServices.Logar(smsResult);
                }
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                throw new Exception("Erro ao enviar sms: " + err.Message, err);
            }


        }


        /// <summary>
        /// Evia o alerta de lançamento em atraso para todos os usuários
        /// </summary>
        /// <param name="email">boolean - se vai receber por e-mail ou não</param>
        /// <param name="sms">boolean - se vai receber por sms ou não</param>
        /// <param name="telegram">boolean - se vai receber por telegram ou não</param>
        /// <returns></returns>
        public virtual void EnviarTodosOsAlertas(bool email = true, bool sms = false)
        {
            var modelo =  this.GetUsuariosComAtrasoNoEnvio();

            if ((modelo == null) || (modelo.Count == 0))
            {
                throw new Exception("O usuário não foi encontrado");
            }


            bool erro = false;
            string mensagens = "";

            foreach (var x in modelo)
            {

                try
                {
                    if (email)
                        MailServices.Enviar(x.Usuario.EmailPreferencial, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", x.DiasUteisSemLancar));
                }
                catch (Exception err)
                {
                    erro = true;
                    mensagens += "Ocorreu um erro ao enviar o alerta via e-mail para " + x.Usuario.Login + ": " + err.Message + "<br/>\r\n";
                    LogServices.LogarException(err);
                }

                string smsResult = "";
                try
                {
                    if (sms && x.Usuario.Funcionario != null && !string.IsNullOrWhiteSpace(x.Usuario.Funcionario.Celular))
                    {
                        smsResult = SMSServices.Enviar(x.Usuario.Funcionario.Celular, x.Usuario.FuncionarioNomeOuLogin, "Faz tempo que você não lança suas atividades", string.Format("Detectamos que faz {0} dias que você não lança suas atividades. Por favor lance hoje.", x.DiasUteisSemLancar));
                        LogServices.Logar(smsResult);
                    }
                }
                catch (Exception err)
                {
                    erro = true;
                    mensagens += "Ocorreu um erro ao enviar o alerta via sms para " + x.Usuario.Login + ": " + err.Message + "<br/>\r\n";
                    LogServices.LogarException(err);
                }



            }

            if (erro)
            {
                throw new Exception("Erros ocorridos: " + mensagens);
            }

        }


        #endregion


        #endregion



        #region métodos privados

        private void AdicionaNodesRecursivamente(Usuario usuario, ProjectNode node)
        {
            if (node != null)
            {
                bool jaPossui = usuario.NosDoUsuario.Contains(node);
                if (!jaPossui)
                {
                    usuario.NosDoUsuario.Add(node);
                }

                if (node.Filhos != null && node.Filhos.Any())
                {
                    foreach (var f in node.Filhos)
                    {
                        AdicionaNodesRecursivamente(usuario, f);
                    }
                }
            }

        }



        private void RemoveNodesRecursivamente(Usuario usuario, ProjectNode node)
        {
            if (node != null)
            {
                bool possui = usuario.NosDoUsuario.Contains(node);
                if (possui)
                {
                    usuario.NosDoUsuario.Remove(node);
                }

                if (node.Filhos != null && node.Filhos.Any())
                {
                    foreach (var f in node.Filhos)
                    {
                        RemoveNodesRecursivamente(usuario, f);
                    }
                }
            }

        }

        #endregion

    }
}
