using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TFW.Domain;
using TPA.Application;
using TPA.Domain.DomainModel;
using TPA.Presentation.App_Start;
using TPA.Services.Seguranca;
using TPA.ViewModel;

namespace TPA.Presentation.Controllers
{

    /// <summary>
    /// controlador de usuários
    /// </summary>
    [TPAAuthorize]
    [TPADescricaoAcaoController("Usuários", "Permitir acesso ao controlador de usuários / meus dados")]
    public class UsuariosController : TPAController
    {

        #region constantes

        /// <summary>
        /// senha padrão quando se reseta uma enha
        /// </summary>
        public const string SENHA_RESETADA_PADRAO = "Tecnun@123";

        #endregion



        #region UserManager

        private ApplicationUserManager _userManager;

        /// <summary>
        /// gerenciador de usuários
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion



        #region métodos públicos (actions)

        /// <summary>
        /// get - lista de usuários
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar Usuários", "Permitir usuário liste os usuários do sistema")]
        public async Task<ActionResult> Index()
        {
            return View(await db.Usuarios.ToListAsync());
        }



        /// <summary>
        /// get - criar usuários
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Criar Usuários", "Permitir usuário crie usuários")]
        public ActionResult Create()
        {
            return View();
        }




        /// <summary>
        /// post - criar usuários
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Criar Usuários", "Permitir usuário crie usuários")]
        public async Task<ActionResult> Create([Bind(Include = "Id, Login, EnviarAlertaLancamento")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Login = usuario.Login.Trim();
                db.Usuarios.Add(usuario);

                try
                {
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }

            }

            return View(usuario);
        }




        /// <summary>
        /// get - alterar usuários
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Editar Usuários", "Permitir usuário altere usuários")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para editar", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Usuario usuario = await db.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return HttpNotFound();
                }
                return View(Mapper.Map<Usuario, UsuarioViewModel>(usuario));
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }





        /// <summary>
        /// post - editar usuários
        /// </summary>
        /// <param name="usuVm"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Editar Usuários", "Permitir usuário altere usuários")]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Login, Nome, EmailProfissional, Celular, Ativo, EnviarAlertaLancamento")] UsuarioViewModel usuVm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    usuVm.Login = usuVm.Login.Trim();
                    UsuarioApplication usuApp = new UsuarioApplication(db);
                    Usuario ent = usuApp.GetById(usuVm.Id);

                    if (ent == null)
                    {
                        return HttpNotFound();
                    }


                    if (string.IsNullOrWhiteSpace(usuVm.Login))
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Usuário com login não definido.", TempData, ModelState, "Login");
                        return View(usuVm);
                    }

                    if (await usuApp.VerificaExistenciaEmailAsync(usuVm.Login, usuVm.Id))
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Esse login já está em uso.", TempData, ModelState, "Login");
                        return View(usuVm);
                    }

                    if(usuVm.EmailProfissional == usuVm.Login)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Este e-mail não pode ser igual ao seu login.", TempData, ModelState, "EmailProfissional");
                        return View(usuVm);
                    }

                    if (!string.IsNullOrWhiteSpace(usuVm.EmailProfissional))
                    {
                        if (await usuApp.VerificaExistenciaEmailAsync(usuVm.EmailProfissional, usuVm.Id))
                        {
                            MensagemParaUsuarioViewModel.MensagemErro("Esse e-mail profissional já está em uso.", TempData, ModelState, "EmailProfissional");
                            return View(usuVm);
                        }
                    }


                    var teste = Mapper.Map<UsuarioViewModel, Usuario>(usuVm, ent);

                    await usuApp.SaveAsync(ent);

                    MensagemParaUsuarioViewModel.MensagemSucesso("Usuário salvo com sucesso.", TempData);
                    return View(usuVm);
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }

            }
            return View(usuVm);
        }




        /// <summary>
        /// get - excluir usuários
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Excluir Usuários", "Permitir usuário excluir usuários")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Não foi especificado um item para excluir", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Usuario usuario = await db.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O item não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }
                return View(usuario);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }





        /// <summary>
        /// post - excluir usuários
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Confirmar Exclusão de Usuários", "Permitir usuário excluir usuários")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Usuario usuario = new Usuario { Id = id};

            try
            {
                
                db.Entry(usuario).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído. " + err.Message, TempData, ModelState);
            }

            return View(usuario);
        }



        /// <summary>
        /// get - adicionar perfil a um usuário
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Adicionar Perfil ao Usuários", "Permitir usuário atribuir perfis a outros usuários")]
        public async Task<ActionResult> AdicionarRole(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Escolha um usuário para adicionar perfis", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Usuario usuario = await db.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O usuário não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }

                UsuarioRoleViewModel urvm = new UsuarioRoleViewModel
                {
                    Usuario = usuario,
                    IdUsuario = usuario.Id
                };
                CarregaPerfis(usuario);
                return View(urvm);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }



        /// <summary>
        /// post - adicionar perfil a um usuário
        /// </summary>
        /// <param name="urvm"></param>
        /// <returns></returns>
        [HttpPost, ActionName("AdicionarRole")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Adicionar Perfil ao Usuários", "Permitir usuário atribuir perfis a outros usuários")]
        public async Task<ActionResult> AdicionarRole(UsuarioRoleViewModel urvm)
        {
            if(urvm == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("O usuário e o perfil não podem ser nulos", TempData, ModelState);
                return RedirectToAction("index");
            }

            if (ModelState.IsValid)
            {


                try
                {

                    var usuario = db.Usuarios.Find(urvm.IdUsuario);
                    var perfil = db.Perfis.Find(urvm.IdPerfil);

                    if (usuario == null)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Usuário não encontrado", TempData, ModelState);
                        return RedirectToAction("index");
                    }

                    if (perfil == null)
                    {
                        MensagemParaUsuarioViewModel.MensagemErro("Perfil não encontrado", TempData, ModelState);
                        return RedirectToAction("index");
                    }

                    urvm.Usuario = usuario;
                    urvm.Perfil = perfil;
                    urvm.IdPerfil = perfil.Id;
                    bool jaPossui = usuario.Perfis.Contains(perfil);
                    if (!jaPossui)
                    {
                        usuario.Perfis.Add(perfil);
                    }

                    await db.SaveChangesAsync();

                    if (jaPossui)
                    {
                        MensagemParaUsuarioViewModel.MensagemInfo("Esse usuário já tinha esse perfil", TempData);
                    }
                    else
                    {
                        MensagemParaUsuarioViewModel.MensagemSucesso("Perfil Adicionado", TempData);
                    }

                    CarregaPerfis(usuario);

                    return  View(urvm);
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }

            }
            
            return View(urvm);
        }




        /// <summary>
        /// post - remover perfil de um usuário
        /// </summary>
        /// <param name="IdUsuarioExclusao"></param>
        /// <param name="IdPerfilExclusao"></param>
        /// <returns></returns>
        [HttpPost, ActionName("RemoverPerfil")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Remover Perfil do Usuários", "Permitir usuário remover perfis de outros usuários")]
        public async Task<ActionResult> RemoverPerfil(int IdUsuarioExclusao, int IdPerfilExclusao)
        {

            try
            {
                Usuario usuario = db.Usuarios.Find(IdUsuarioExclusao);
                if (usuario != null)
                {
                    Perfil perfil = usuario.Perfis.Where(x => x.Id == IdPerfilExclusao).FirstOrDefault();
                    if(perfil != null)
                    {
                        usuario.Perfis.Remove(perfil);
                        await db.SaveChangesAsync();
                    }
                }

                return RedirectToAction("AdicionarRole", new { Id = IdUsuarioExclusao });
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído. " + err.Message, TempData, ModelState);
            }

            return RedirectToAction("AdicionarRole", new { Id = IdUsuarioExclusao });
        }











        /// <summary>
        /// get - atribuir estruturas de projeto ao usuário
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [TPADescricaoAcaoController("Adicionar ProjectNode", "Permitir que o usuário atribua estruturas de projetos a outros usuários")]
        public async Task<ActionResult> AdicionarNode(int? id)
        {
            if (id == null)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Escolha um usuário para adicionar estruturas de projeto", TempData, ModelState);
                return RedirectToAction("index");
            }

            try
            {
                Usuario usuario = await db.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("O usuário não foi encontrado", TempData, ModelState);
                    return RedirectToAction("index");
                }

                UsuarioNodeViewModel urvm = new UsuarioNodeViewModel
                {
                    Usuario = usuario,
                    IdUsuario = usuario.Id
                };
                CarregaNodes();
                return View(urvm);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
                return RedirectToAction("index");
            }
        }




        /// <summary>
        /// post - get - atribuir estruturas de projeto ao usuário
        /// </summary>
        /// <param name="urvm"></param>
        /// <returns></returns>
        [HttpPost, ActionName("AdicionarNode")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Adicionar ProjectNode", "Permitir que o usuário atribua estruturas de projetos a outros usuários")]
        public async Task<ActionResult> AdicionarNode(UsuarioNodeViewModel urvm)
        {
            CarregaNodes();

            if ((urvm == null) || (urvm.IdUsuario == 0))
            {
                MensagemParaUsuarioViewModel.MensagemErro("Usuário não encontrado", TempData, ModelState);
                return RedirectToAction("index");
            }


            if ((urvm == null) || (urvm.IdNode == 0) || (urvm.IdUsuario == 0))
            {
                MensagemParaUsuarioViewModel.MensagemErro("O Nó de Projeto e o usuário devem ser informados.", TempData, ModelState);                
                return View(urvm);
            }



            if (ModelState.IsValid)
            {
                try
                {
                    UsuarioApplication usu = new UsuarioApplication(this.db);
                    await usu.AdicionarNodeAsync(urvm);
                    MensagemParaUsuarioViewModel.MensagemSucesso("Projeto adicionado", TempData);

                    return View(urvm);
                }
                catch(UsuarioApplicationException uaex)
                {
                    MensagemParaUsuarioViewModel.MensagemErro(uaex.Message, TempData, ModelState);
                }
                catch (DbUpdateConcurrencyException duce)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
                }
                catch (Exception err)
                {
                    MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser salvo. " + err.Message, TempData, ModelState);
                }

            }

            return View(urvm);
        }



        /// <summary>
        /// post - remover estruturas de projeto de usuários
        /// </summary>
        /// <param name="IdUsuarioExclusao">int - id do usuário</param>
        /// <param name="IdNodeExclusao">int - id do node</param>
        /// <param name="RemoverRecursivo">bool - true se for para removar recursivamente</param>
        /// <returns></returns>
        [HttpPost, ActionName("RemoverNode")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Remover ProjectNode", "Permitir que o usuário remova estruturas de projetos de outros usuários")]
        public async Task<ActionResult> RemoverNode(int IdUsuarioExclusao, int IdNodeExclusao, bool RemoverRecursivo)
        {

            try
            {
                UsuarioApplication usu = new UsuarioApplication(this.db);
                await usu.RemoverNodeAsync(IdUsuarioExclusao, IdNodeExclusao, RemoverRecursivo);
                MensagemParaUsuarioViewModel.MensagemSucesso("Projeto removido", TempData);

                return RedirectToAction("AdicionarNode", new { Id = IdUsuarioExclusao });
            }
            catch (UsuarioApplicationException uaex)
            {
                MensagemParaUsuarioViewModel.MensagemErro(uaex.Message, TempData, ModelState);
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído. " + err.Message, TempData, ModelState);
            }

            return RedirectToAction("AdicionarNode", new { Id = IdUsuarioExclusao });
        }


        /// <summary>
        /// post - remover estruturas de projeto de usuários
        /// </summary>
        /// <param name="IdUsuarioExclusao">int - id do usuário</param>
        /// <param name="IdNodeExclusao">int - id do node</param>
        /// <param name="RemoverRecursivo">bool - true se for para removar recursivamente</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Remover ProjectNode", "Permitir que o usuário remova estruturas de projetos de outros usuários")]
        public async Task<ActionResult> RemoverTodosOsNodes(int IdUsuarioExclusaoTodos)
        {

            try
            {
                UsuarioApplication usuapp = new UsuarioApplication(this.db);
                Usuario usu = usuapp.GetById(IdUsuarioExclusaoTodos);
                usu.NosDoUsuario.Clear();

                await db.SaveChangesAsync();
                MensagemParaUsuarioViewModel.MensagemSucesso("Projetos removidos", TempData);

                return RedirectToAction("AdicionarNode", new { Id = IdUsuarioExclusaoTodos });
            }
            catch (UsuarioApplicationException uaex)
            {
                MensagemParaUsuarioViewModel.MensagemErro(uaex.Message, TempData, ModelState);
            }
            catch (DbUpdateConcurrencyException duce)
            {
                MensagemParaUsuarioViewModel.MensagemErro(" Talvez esse registro tenha sido excluído por outra pessoa. " + duce.Message, TempData, ModelState);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro("Esse registro não pôde ser excluído. " + err.Message, TempData, ModelState);
            }

            return RedirectToAction("AdicionarNode", new { Id = IdUsuarioExclusaoTodos });
        }








        /// <summary>
        /// post - resetar as senhas de todos os usuários
        /// </summary>
        /// <returns></returns>
        [HttpPost, ActionName("ResetarTodasSenhas")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Resetar Todas as Senhas", "Permitir que o usuário resete todas as senhas de todos os usuários")]
        public async Task<ActionResult> ResetarTodasSenhas()
        {
            var todos = await UserManager.Users.ToListAsync(); 
            string erros = "";
            bool deuErro = false;

            foreach(var u in todos)
            {
                try
                {
                    if (await UserManager.HasPasswordAsync(u.Id))
                    {
                        var reset = await UserManager.RemovePasswordAsync(u.Id);
                        if (!reset.Succeeded)
                        {
                            deuErro = true;
                            erros += string.Join("<br/>\r\n", reset.Errors.ToArray());
                        }
                    }

                    var result = await UserManager.AddPasswordAsync(u.Id, SENHA_RESETADA_PADRAO);

                    if (result.Succeeded)
                    {
                        var usr = await UserManager.FindByIdAsync(u.Id);
                        if (usr == null)
                        {
                            deuErro = true;
                            erros += "<br/>\r\nUsuário não encontrado depois de definir senha: " + u.Email + "<br/>\r\n";
                        }

                    }
                    else
                    {
                        deuErro = true;
                        erros += string.Join("<br/>\r\n", result.Errors.ToArray());
                    }
                }
                catch(Exception err)
                {
                    deuErro = true;
                    erros += err.Message +  "<br/>\r\n";
                }
            }


            if(deuErro)
            {
                MensagemParaUsuarioViewModel.MensagemErro(erros, TempData, ModelState);
            }
            else
            {
                MensagemParaUsuarioViewModel.MensagemSucesso("Todas as senhas resetadas para " + SENHA_RESETADA_PADRAO, TempData);
            }


            return  RedirectToAction("Index");
        }




        /// <summary>
        /// post - resetar a senha de um usuário
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("ResetarSenha")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Resetar Senhas", "Permitir que o usuário resete a senha de outro usuário")]
        public async Task<ActionResult> ResetarSenha(int id)
        {

            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "O Id deve ser informado");
            }

            Usuario usu = await db.Usuarios.FindAsync(id);            
            if (usu == null)
            {
                return HttpNotFound("Usuário não encontrado mas tabelas internas.");
            }

            ApplicationUser usuario = await UserManager.FindByEmailAsync(usu.Login);
            if (usuario == null)
            {
                return HttpNotFound("Usuário não encontrado em AspNet Identity.");
            }

            string erros = "";
            bool deuErro = false;

            try
            {
                if (await UserManager.HasPasswordAsync(usuario.Id))
                {
                    var reset = await UserManager.RemovePasswordAsync(usuario.Id);
                    if (!reset.Succeeded)
                    {
                        deuErro = true;
                        erros += string.Join("<br/>\r\n", reset.Errors.ToArray());
                    }
                }

                var result = await UserManager.AddPasswordAsync(usuario.Id, SENHA_RESETADA_PADRAO);

                if (result.Succeeded)
                {
                    var usr = await UserManager.FindByIdAsync(usuario.Id);
                    if (usr == null)
                    {
                        deuErro = true;
                        erros += "<br/>\r\nUsuário não encontrado depois de definir senha: " + usuario.Email + "<br/>\r\n";
                    }

                }
                else
                {
                    deuErro = true;
                    erros += string.Join("<br/>\r\n", result.Errors.ToArray());
                }
            }
            catch (Exception err)
            {
                deuErro = true;
                erros += err.Message + "<br/>\r\n";
            }



            if (deuErro)
            {
                MensagemParaUsuarioViewModel.MensagemErro(erros, TempData, ModelState);
            }
            else
            {
                MensagemParaUsuarioViewModel.MensagemSucesso("Senha resetada para "  + SENHA_RESETADA_PADRAO, TempData);
            }


            return RedirectToAction("Index");

        }
















        /// <summary>
        /// get - listar usuários com atraso no lançamento das atividades
        /// </summary>
        /// <returns></returns>
        [TPADescricaoAcaoController("Listar Atraso no Lançamento", "Permitir que o usuário liste todas as pessoas com atraso no lançamento de atividades")]
        public async Task<ActionResult> AtrasoLancamento()
        {
            var app = new UsuarioApplication(this.db);
            var modelo = await app.GetUsuariosComAtrasoNoEnvioAsync();
            return View(modelo);
        }



        /// <summary>
        /// post - enviar e-mail de alerta de atraso
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        [HttpPost, ActionName("EnviarEmailAlerta")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Enviar e-mail de alerta", "Permitir que o usuário envie e-mails de alerta de atraso")]
        public async Task<ActionResult> EnviarEmailAlerta(int idUsuario)
        {
            try
            {
                UsuarioApplication usuApp = new UsuarioApplication(this.db);
                await usuApp.EnviarAlertaAsync(idUsuario, true, true);
                MensagemParaUsuarioViewModel.MensagemSucesso("Alerta enviado com sucesso.", TempData);
            }
            catch(Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);                
            }

            return RedirectToAction("AtrasoLancamento");
        }



        /// <summary>
        /// post - enviar todos os e-mails de alerta de atraso de uma vez
        /// </summary>
        /// <returns></returns>
        [HttpPost, ActionName("EnviarTodosEmailsDeAlerta")]
        [ValidateAntiForgeryToken]
        [TPADescricaoAcaoController("Enviar Todos e-mails de alerta", "Permitir que o usuário envie todos os e-mails de alerta de atraso de uma vez")]
        public async Task<ActionResult> EnviarTodosEmailsDeAlerta()
        {
            try
            {
                UsuarioApplication usuApp = new UsuarioApplication(this.db);
                await usuApp.EnviarTodosOsAlertasAsync(true, true);
                MensagemParaUsuarioViewModel.MensagemSucesso("Alertas enviados com sucesso.", TempData);
            }
            catch (Exception err)
            {
                MensagemParaUsuarioViewModel.MensagemErro(err.Message, TempData, ModelState);
            }

            return RedirectToAction("AtrasoLancamento");
        }

        #endregion



        #region metodos privados

        private void CarregaPerfis(Usuario usu, int? valor = null)
        {

            var perfis = db.Perfis.ToList().Except(usu.Perfis);
            SelectList PerfisSelectList = new SelectList(perfis, "Id", "Nome", valor);
            ViewBag.Perfis = PerfisSelectList;
        }


        private void CarregaNodes(int? valor = null)
        {
            var nodes = db.ProjectNodes.ToList();
            TFWHierarchicalList lst = new TFWHierarchicalList();

            foreach (var n in nodes)
            {
                lst.Add(n.Id, n.Pai != null ? n.Pai.Id : new Nullable<int>(), n.Nome);
            }

            ViewBag.Nodes = lst;
        }







        #endregion

    }
}
