using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TPA.Domain.DomainModel;
using TPA.Infra.Data;
using TPA.ViewModel;

namespace TPA.Services
{
    /// <summary>
    /// classe de serviços voltados ao usuário, como verificar login, etc
    /// </summary>
    public class UserServices
    {
        /// <summary>
        /// dado um login, retorna o usuário
        /// </summary>
        /// <param name="login">string login - o login do usuário, no caso da Tecnun, o e-mail</param>
        /// <returns>UsuarioViewModel - um DTO com login e nome do usuário, para mostrar no topo das páginas</returns>
        public static MeusDadosViewModel GetByLogin(string login)
        {
            MeusDadosViewModel result = new MeusDadosViewModel
            {
                Nome = "Cadastro Incompleto"
            };


            using (TPAContext db = new TPAContext())
            {
                var usu = db.Usuarios.Where(u => u.Login == login).FirstOrDefault();
                if(usu != null)
                {
                    result = Mapper.Map<Usuario, MeusDadosViewModel>(usu);
                }
            }

            return result;
        }

    }
}