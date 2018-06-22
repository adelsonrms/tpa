using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Globalization;
using TPA.Domain.DomainModel;
using TPA.ViewModel;

namespace TPA.Presentation.App_Start
{


    public static class AutoMapBindings
    {
        private static bool _inicializado = false;


        static AutoMapBindings()
        {
            AutoMapBindings.Config();
        }


        public static void Config()
        {

            if (!AutoMapBindings._inicializado)
            {

                AutoMapper.Mapper.Initialize(cfg =>
                {


                    cfg.CreateMap<Atividade, AtividadeViewModel>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Observacao, opt => opt.MapFrom(src => src.Observacao))
                        .ForMember(dest => dest.IdUsuario, opt => opt.MapFrom(src => src.Usuario != null ? src.Usuario.Id : 0))
                        .ForMember(dest => dest.IdTipoAtividade, opt => opt.MapFrom(src => src.TipoAtividade != null ? src.TipoAtividade.Id : 0))
                        .ForMember(dest => dest.IdProjectNode, opt => opt.MapFrom(src => src.ProjectNode != null ? src.ProjectNode.Id : 0))
                        .ForMember(dest => dest.Inicio, opt => opt.MapFrom(src => new TimeSpan(src.Inicio.Hour, src.Inicio.Minute, 0)))
                        .ForMember(dest => dest.Fim, opt => opt.MapFrom(src => new TimeSpan(src.Fim.Hour, src.Fim.Minute, 0)))
                        .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Inicio.Date));








                    cfg.CreateMap<Usuario, UsuarioViewModel>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                        .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo))
                        .ForMember(dest => dest.EnviarAlertaLancamento, opt => opt.MapFrom(src => src.EnviarAlertaLancamento))
                        .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Funcionario != null ? src.Funcionario.Nome : ""))
                        .ForMember(dest => dest.Celular, opt => opt.MapFrom(src => src.Funcionario != null ? src.Funcionario.Celular : ""))
                        .ForMember(dest => dest.EmailProfissional, opt => opt.MapFrom(src => src.Funcionario != null ? src.Funcionario.EmailProfissional : ""));


                    cfg.CreateMap<UsuarioViewModel, Usuario>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                        .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => src.Ativo))
                        .ForMember(dest => dest.EnviarAlertaLancamento, opt => opt.MapFrom(src => src.EnviarAlertaLancamento))
                        .ForPath(dest => dest.Funcionario.Id, opt => opt.MapFrom(src => src.Id))
                        .ForPath(dest => dest.Funcionario.Nome, opt => opt.MapFrom(src => src.Nome))
                        .ForPath(dest => dest.Funcionario.Celular, opt => opt.MapFrom(src => src.Celular))
                        .ForPath(dest => dest.Funcionario.EmailProfissional, opt => opt.MapFrom(src => src.EmailProfissional));








                    cfg.CreateMap<Usuario, MeusDadosViewModel>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                        .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Funcionario != null ? src.Funcionario.Nome : ""))
                        .ForMember(dest => dest.Celular, opt => opt.MapFrom(src => src.Funcionario != null ? src.Funcionario.Celular : ""))
                        .ForMember(dest => dest.EmailProfissional, opt => opt.MapFrom(src => src.Funcionario != null ? src.Funcionario.EmailProfissional : ""));


                    cfg.CreateMap<MeusDadosViewModel, Usuario>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                        .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                        .ForPath(dest => dest.Funcionario.Id, opt => opt.MapFrom(src => src.Id))
                        .ForPath(dest => dest.Funcionario.Nome, opt => opt.MapFrom(src => src.Nome))
                        .ForPath(dest => dest.Funcionario.Celular, opt => opt.MapFrom(src => src.Celular))
                        .ForPath(dest => dest.Funcionario.EmailProfissional, opt => opt.MapFrom(src => src.EmailProfissional));

                });



                AutoMapBindings._inicializado = true;

            }
        }
    }
}