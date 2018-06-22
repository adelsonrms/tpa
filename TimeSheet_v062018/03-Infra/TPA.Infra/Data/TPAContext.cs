using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.History;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TPA.Domain.DomainModel;
using TPA.Infra.Services;

namespace TPA.Infra.Data
{

    /// <summary>
    /// context do EF, com todos os sets e com o microsoft identity também
    /// </summary>
    public class TPAContext : IdentityDbContext<ApplicationUser>, IDisposable
    {

        #region métodos estáticos públicos / factory methods

        /// <summary>
        /// factory method que usa o constructor padrão para criar uma instância
        /// </summary>
        /// <returns></returns>
        public static TPAContext Create()
        {
            return new TPAContext();
        }



        /// <summary>
        /// traz um context especial sem lazy load, proxys ou validations para , em conjunto com noTracking, melhorar a performance das consultas
        /// </summary>
        /// <returns></returns>
        public static TPAContext CreateDetachedContext()
        {
            TPAContext db = new TPAContext();
            db.Configuration.AutoDetectChangesEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.ValidateOnSaveEnabled = false;
            return db;
        }



        #endregion


        #region constgructors

        /// <summary>
        /// constructor padrão que pega a connection string dinamicamente de um serviço que a cria e configura um initializer com migrations
        /// </summary>
        public TPAContext() : base(ConfigServices.GetConnectionString())
        {
            Database.SetInitializer<TPAContext>(new MigrateDatabaseToLatestVersion<TPAContext, TPA.Infra.Migrations.Configuration>());
            //Database.SetInitializer<TPAContext>(null);
        }


        #endregion



        #region propriedades públicas


        /// <summary>
        /// dbset de ações da aplicação / permissões
        /// </summary>
        public DbSet<Acao> Acoes { get; set; }

        /// <summary>
        /// dbset de atividadeds dos funcionários
        /// </summary>
        public DbSet<Atividade> Atividades { get; set; }

        /// <summary>
        /// dbset de  atestados médicos
        /// </summary>
        public DbSet<AtestadoAnexo> Atestados { get; set; }

        /// <summary>
        /// dbset de NodeLabels (labels para os tipos de nós de projeto)
        /// </summary>
        public DbSet<NodeLabel> NodeLabels { get; set; }

        /// <summary>
        /// dbset de  perfis/roles de usuários
        /// </summary>
        public DbSet<Perfil> Perfis { get; set; }

        /// <summary>
        /// dbset de  de nodos das estruturas de projetos
        /// </summary>
        public DbSet<ProjectNode> ProjectNodes { get; set; }

        /// <summary>
        /// dbset de tipos de atividades
        /// </summary>
        public DbSet<TipoAtividade> TiposAtividade { get; set; }

        /// <summary>
        /// dbset de  de usuários
        /// </summary>
        public DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// dbset de  feriados
        /// </summary>
        public DbSet<Feriado> Feriados { get; set; }

        /// <summary>
        /// dbset de funcionários (dados complementares do usuário, 1x1)
        /// </summary>
        public DbSet<Funcionario> Funcionarios { get; set; }

        /// <summary>
        /// dbset de período/mês de referência
        /// </summary>
        public DbSet<Referencia> Referencias { get; set; }

        /// <summary>
        /// dbset de usertokencache
        /// </summary>
        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }

        #endregion



        #region métodos protegidos sobrecarregados de DBContext

        /// <summary>
        /// dita o schema e as regras para a criação do model
        /// remove as convenções de cascade e de pluralização
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //para criar a tabela em um schema
            modelBuilder.HasDefaultSchema("TPA");


            //remove a pluralização (nomes das tabelas no singular)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //remove o manytomany cascade (para apagar um membro, tem que apagar todas as dependencias e ele mesmo ser removido de todas as listas)
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //remove convenção onetomany (só apaga um item se apagar todos os dependentes)
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);

            /*
                código com mapeamentos de contexto que colocam prefixos nas tabelas. 
                Vantagens: não necessitam de schemas, 
                desvantagens: mapeamento nxn manual, sem separação do migrations, 
                risco de excluir o banco ou ter que fazer as migrações manualmente. 
                Esses riscos não existem com um banco de dados dedicado.
            */
            ////para criar a tabela com prefixos personalizados
            ////http://stackoverflow.com/questions/12633015/how-to-add-table-prefix-in-entity-framework-code-first-globally/12967044#12967044
            //modelBuilder.Types().Configure(entity => entity.ToTable("TPA_" + entity.ClrType.Name));
            ////http://stackoverflow.com/questions/9564819/how-to-specify-relationship-name-in-code-first-many-to-many-relationship
            //modelBuilder.Entity<Usuario>()
            //  .HasMany(a => a.Perfis)
            //  .WithMany(b => b.Usuarios)
            //  .Map(mc =>
            //  {
            //      mc.ToTable("TPA_UsuarioPerfil"/*, "TPA"*/); //é possível alterar o schema também
            //      //talvez não seja necessário alterar o nome do campo
            //      //mc.MapLeftKey("Usuario_Id");
            //      //mc.MapRightKey("Perfil_Id");
            //  });

            //modelBuilder.Entity<Perfil>()
            //  .HasMany(a => a.Acoes)
            //  .WithMany(b => b.Perfis)
            //  .Map(mc =>
            //  {
            //      mc.ToTable("TPA_PerfilAcao"/*, "TPA"*/); //é possível alterar o schema também
            //      //talvez não seja necessário alterar o nome do campo
            //      //mc.MapLeftKey("Usuario_Id");
            //      //mc.MapRightKey("Perfil_Id");
            //  });
        }


        /// <summary>
        /// intercepta todos os eventos de salvar.
        /// validações genéricas devem ser todos aqui
        /// logs genéricos devem ser todos aqui
        /// restrições de segurança genéricos (baseados em database)  devem ser todos aqui
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            try
            {
                AddTimestamps();
                return base.SaveChanges();
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                throw;
            }
        }


        /// <summary>
        /// intercepta todos os eventos de salvar assíncronos.
        /// validações genéricas devem ser todos aqui
        /// logs genéricos devem ser todos aqui
        /// restrições de segurança genéricos (baseados em database)  devem ser todos aqui
        /// </summary>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync()
        {
            try
            {
                AddTimestamps();
                return await base.SaveChangesAsync();
            }
            catch (Exception err)
            {
                LogServices.LogarException(err);
                throw;
            }
        }

        /// <summary>
        /// dispose padrão
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


        #endregion




        #region métodos privados

        /// <summary>
        /// adiciona os campos de log de auditoria no registro sendo salvo
        /// conforme entrada de blog https://benjii.me/2014/03/track-created-and-modified-fields-automatically-with-entity-framework-code-first/
        /// </summary>
        private void AddTimestamps()
        {

            try
            {

                var entries = ChangeTracker.Entries().Where(x => x.Entity is TPAEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

                var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name)
                    ? HttpContext.Current.User.Identity.Name
                    : "Anonymous";

                foreach (var entry in entries)
                {
                    if (entry.State == EntityState.Added)
                    {
                        ((TPAEntity)entry.Entity).MomentoInclusao = DateTime.Now;
                        ((TPAEntity)entry.Entity).UsuarioInclusao = currentUsername;
                    }

                    ((TPAEntity)entry.Entity).MomentoEdicao = DateTime.Now;
                    ((TPAEntity)entry.Entity).UsuarioEdicao = currentUsername;
                }

            }
            catch (Exception auditEx)
            {
                LogServices.LogarException(auditEx);
            }

        }

        #endregion

    }
}