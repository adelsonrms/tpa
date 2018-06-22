namespace TPA.Infra.Migrations
{
    using Domain.DomainModel;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<TPA.Infra.Data.TPAContext>
    {

        #region contructors

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "TPA.Data.TPAContext";
        }


        #endregion


        #region protected seed

        protected override void Seed(TPA.Infra.Data.TPAContext context)
        {
            //AddInitialData(context);

        }

        #endregion





        #region private methods

        private void AddInitialData(Data.TPAContext context)
        {
            TryAddNodeLabels(context);
            TryAddTipoAtividade(context);
            TryAddPerfisUsuarios(context);
            TryImportarAcoesDoAssemblyAtualizandoPerfilAdmin(context);
            TryAddEstruturaDeProjetos(context);
            //TryAddAtividadesTeste(context);
        }

        #endregion



        #region metodos privados que tentam inserir dados de teste e interceptam ou ignoram a exception

        private void TryAddAtividadesTeste(Data.TPAContext context)
        {
            try
            {
                AddAtividadesTeste(context);
            }
            catch
            {

            }
        }

        private void TryAddEstruturaDeProjetos(Data.TPAContext context)
        {
            try
            {
                AddEstruturaDeProjetos(context);
            }
            catch
            {

            }
        }

        private void TryImportarAcoesDoAssemblyAtualizandoPerfilAdmin(Data.TPAContext context)
        {
            try
            {
                ImportarAcoesDoAssemblyAtualizandoPerfilAdmin(context);
            }
            catch
            {

            }
        }

        private void TryAddPerfisUsuarios(Data.TPAContext context)
        {
            try
            {
                AddPerfisUsuarios(context);
            }
            catch
            {

            }
        }

        private void TryAddTipoAtividade(Data.TPAContext context)
        {
            try
            {
                AddTipoAtividade(context);
            }
            catch
            {

            }
        }

        private void TryAddNodeLabels(Data.TPAContext context)
        {
            try
            {
                AddNodeLabels(context);
            }
            catch
            {

            }
        }

        #endregion



        #region métodos privados de dados default e/ou teste

        private void ImportarAcoesDoAssemblyAtualizandoPerfilAdmin(Data.TPAContext context)
        {
            //todo: colocar aqui rotina para, sem criar vinculo com application, semear os nomes de controllers/actions usando interfaces e um container de DI usado como service locator
            //AcaoServices g = new AcaoServices(context);
            //g.ImportarDoAssembly();
            //g.AtualizaAdmin();

            //context.SaveChanges();
        }

        private void AddAtividadesTeste(Data.TPAContext context)
        {
            var projetoTpa = context.ProjectNodes.Where(p => p.Nome == "TPA Web").FirstOrDefault();
            var atvs = new List<Atividade>
            {

                new Atividade
                {
                    Inicio = DateTime.Today.AddHours(8),
                    Fim = DateTime.Today.AddHours(10),
                    Observacao = "Teste de atividade 1 - " + DateTime.Today.ToString("dd/MM/yyyy"),
                    ProjectNode = projetoTpa,
                    Cliente = projetoTpa.GetCliente(),
                    TipoAtividade = context.TiposAtividade.First(),
                    Usuario = context.Usuarios.Where(u => u.Login == "vitor@tecnun.com.br").FirstOrDefault()

                },


                new Atividade
                {
                    Inicio = DateTime.Today.AddHours(10),
                    Fim = DateTime.Today.AddHours(12),
                    Observacao = "Teste de atividade 2 - " + DateTime.Today.ToString("dd/MM/yyyy"),
                    ProjectNode = projetoTpa,
                    Cliente = projetoTpa.GetCliente(),
                    TipoAtividade = context.TiposAtividade.First(),
                    Usuario = context.Usuarios.Where(u => u.Login == "vitor@tecnun.com.br").FirstOrDefault()

                },

                new Atividade
                {
                    Inicio = DateTime.Today.AddHours(13),
                    Fim = DateTime.Today.AddHours(15),
                    Observacao = "Teste de atividade 3 - " + DateTime.Today.ToString("dd/MM/yyyy"),
                    ProjectNode = projetoTpa,
                    Cliente = projetoTpa.GetCliente(),
                    TipoAtividade = context.TiposAtividade.First(),
                    Usuario = context.Usuarios.Where(u => u.Login == "vitor@tecnun.com.br").FirstOrDefault()

                },

                new Atividade
                {
                    Inicio = DateTime.Today.AddHours(15),
                    Fim = DateTime.Today.AddHours(17),
                    Observacao = "Teste de atividade 4 - " + DateTime.Today.ToString("dd/MM/yyyy"),
                    ProjectNode = projetoTpa,
                    Cliente = projetoTpa.GetCliente(),
                    TipoAtividade = context.TiposAtividade.First(),
                    Usuario = context.Usuarios.Where(u => u.Login == "vitor@tecnun.com.br").FirstOrDefault()

                },
            };

            context.Atividades

                .AddOrUpdate(x => x.Inicio, atvs.ToArray());

            context.SaveChanges();
        }

        private ProjectNode AddEstruturaDeProjetos(Data.TPAContext context)
        {
            var bradesco = new ProjectNode
            {
                Nome = "Bradesco",
                HorasEstimadas = 0,
                NodeLabel = context.NodeLabels.Where(l => l.Nome == "Cliente").First(),
                Filhos = new List<ProjectNode>
                {
                    new ProjectNode
                    {
                        Nome = "Private",
                        HorasEstimadas = 0,
                        NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Area").First(),
                        Filhos = new List<ProjectNode>
                        {
                            new ProjectNode
                            {
                                Nome = "Projeto 1",
                                HorasEstimadas = 0,
                                NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Projeto").First(),
                                Filhos = new List<ProjectNode>
                                {
                                    new ProjectNode
                                    {
                                        Nome = "Entregavel 1",
                                        HorasEstimadas = 0,
                                        NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Entregaveis").First(),
                                        Filhos = new  List<ProjectNode>
                                        {
                                            new ProjectNode { Nome = "Etapa 1",HorasEstimadas = 0, NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Etapas").First()}
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

            };

            var bbi = new ProjectNode
            {
                Nome = "BBI",
                HorasEstimadas = 0,
                NodeLabel = context.NodeLabels.Where(l => l.Nome == "Cliente").First(),

                Filhos = new List<ProjectNode>
                {
                    new ProjectNode { Nome = "SSD", HorasEstimadas = 0, NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Area").First()}
                }
            };

            var tecnun = new ProjectNode
            {
                Nome = "Tecnun",
                HorasEstimadas = 0,
                NodeLabel = context.NodeLabels.Where(l => l.Nome == "Cliente").First(),

                Filhos = new List<ProjectNode>
                {
                    new ProjectNode
                    {
                        Nome = "RH",
                        HorasEstimadas = 0,
                        NodeLabel = context.NodeLabels.Where(l => l.Nome == "Area").First(),
                        Filhos = new List<ProjectNode>
                        {
                            new ProjectNode
                            {
                                Nome = "Tecnun Projetos e Apontamentos (TPA)",
                                HorasEstimadas = 0,
                                NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Projeto").First(),
                                Filhos = new List<ProjectNode>
                                {
                                    new ProjectNode
                                    {
                                        Nome = "TPA Web",
                                        HorasEstimadas = 0,
                                        NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Entregaveis").First(),
                                        Filhos = new  List<ProjectNode>
                                        {
                                            new ProjectNode { Nome = "Fase 1",HorasEstimadas = 0, NodeLabel = context.NodeLabels.Where(l=>l.Nome=="Etapas").First() }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            };

            var administrativo = new ProjectNode
            {
                Nome = ApplicationResources.NODELABEL_ADMINISTRATIVO_NOME,
                HorasEstimadas = 0,
                NodeLabel = context.NodeLabels.Where(l => l.Nome == ApplicationResources.NODELABEL_ADMINISTRATIVO_NOME).First()
            };

            context.ProjectNodes.AddOrUpdate(x => x.Nome, administrativo, bradesco, bbi, tecnun );

            context.SaveChanges();
            return tecnun;
        }

        private void AddPerfisUsuarios(Data.TPAContext context)
        {
            context.Perfis.AddOrUpdate(x => x.Nome,

                new Perfil
                {
                    Nome = "Admin",
                    Usuarios = new List<Usuario>
                    {
                        new Usuario {Login="vitor@tecnun.com.br", NosDoUsuario = context.ProjectNodes.ToList() },
                        new Usuario {Login="adelson@tecnun.com.br", NosDoUsuario = context.ProjectNodes.ToList() },
                        new Usuario {Login="henrique@tecnun.com.br", NosDoUsuario = context.ProjectNodes.ToList() },
                        new Usuario {Login="ortega@tecnun.com.br", NosDoUsuario = context.ProjectNodes.ToList() },
                        new Usuario {Login="jefferson@tecnun.com.br", NosDoUsuario = context.ProjectNodes.ToList() }
                    }
                },


                new Perfil
                {
                    Nome = "Programador",
                    Usuarios = new List<Usuario>
                    {
                        new Usuario {Login="usuario@tecnun.com.br", NosDoUsuario = context.ProjectNodes.ToList() }
                    }
                }
            );


            context.SaveChanges();
        }

        private void AddTipoAtividade(Data.TPAContext context)
        {
            context.TiposAtividade.AddOrUpdate(x => x.Nome,

                new TipoAtividade { Nome = ApplicationResources.TIPOATIVIDADE_ATESTADO_NOME },
                new TipoAtividade { Nome = ApplicationResources.TIPOATIVIDADE_FALTAJUSTIFICADA_NOME },
                new TipoAtividade { Nome = ApplicationResources.TIPOATIVIDADE_FERIAS_NOME },
                new TipoAtividade { Nome = ApplicationResources.TIPOATIVIDADE_FOLGAREMUNERADA_NOME },
                new TipoAtividade { Nome = "Análise" },
                new TipoAtividade { Nome = "Codificação" },
                new TipoAtividade { Nome = "Teste" },
                new TipoAtividade { Nome = "Implantação" },
                new TipoAtividade { Nome = "Suporte" },
                new TipoAtividade { Nome = "Treinamento" },
                new TipoAtividade { Nome = "Refactoring" },
                new TipoAtividade { Nome = "Reunião" }
            );


            context.SaveChanges();
        }

        private void AddNodeLabels(Data.TPAContext context)
        {
            context.NodeLabels.AddOrUpdate(l => l.Nome,

                new NodeLabel { Nome = ApplicationResources.NODELABEL_ADMINISTRATIVO_NOME },
                new NodeLabel { Nome = "Cliente" },
                new NodeLabel { Nome = "Area" },
                new NodeLabel { Nome = "Projeto" },
                new NodeLabel { Nome = "Entregaveis" },
                new NodeLabel { Nome = "Etapas" }

            );

            context.SaveChanges();
        }

        #endregion
    }
}
