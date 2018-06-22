namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationInicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TPA.Acao",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, unique: true);
            
            CreateTable(
                "TPA.Perfil",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, unique: true);
            
            CreateTable(
                "TPA.Usuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false, maxLength: 800),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Login, unique: true);
            
            CreateTable(
                "TPA.Atividade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Observacao = c.String(nullable: false),
                        Inicio = c.DateTime(nullable: false),
                        Fim = c.DateTime(nullable: false),
                        ProjectNode_Id = c.Int(nullable: false),
                        TipoAtividade_Id = c.Int(nullable: false),
                        Usuario_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TPA.ProjectNode", t => t.ProjectNode_Id, cascadeDelete: true)
                .ForeignKey("TPA.TipoAtividade", t => t.TipoAtividade_Id, cascadeDelete: true)
                .ForeignKey("TPA.Usuario", t => t.Usuario_Id, cascadeDelete: true)
                .Index(t => t.ProjectNode_Id)
                .Index(t => t.TipoAtividade_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "TPA.ProjectNode",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        HorasEstimadas = c.Int(nullable: false),
                        Pai_Id = c.Int(),
                        NodeLabel_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TPA.ProjectNode", t => t.Pai_Id)
                .ForeignKey("TPA.NodeLabel", t => t.NodeLabel_Id)
                .Index(t => t.Pai_Id)
                .Index(t => t.NodeLabel_Id);
            
            CreateTable(
                "TPA.NodeLabel",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, unique: true);
            
            CreateTable(
                "TPA.TipoAtividade",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, unique: true);
            
            CreateTable(
                "TPA.Funcionario",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Nome = c.String(),
                        Matricula = c.String(),
                        CPF = c.String(),
                        PIS = c.String(),
                        Telefone = c.String(),
                        Celular = c.String(),
                        EmailProfissional = c.String(),
                        EmailPessoal = c.String(),
                        Endereco = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TPA.Usuario", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "TPA.Referencia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ano = c.Int(nullable: false),
                        Mes = c.Int(nullable: false),
                        Previsto = c.Time(nullable: false, precision: 7),
                        Realizado = c.Time(nullable: false, precision: 7),
                        Saldo = c.Time(nullable: false, precision: 7),
                        Fechado = c.Boolean(nullable: false),
                        Usuario_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TPA.Usuario", t => t.Usuario_Id)
                .Index(t => t.Usuario_Id);
            
            CreateTable(
                "TPA.Feriado",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Data = c.DateTime(nullable: false),
                        Cidade = c.String(),
                        Descricao = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "TPA.UserTokenCache",
                c => new
                    {
                        UserTokenCacheId = c.Int(nullable: false, identity: true),
                        webUserUniqueId = c.String(),
                        cacheBits = c.Binary(),
                        LastWrite = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserTokenCacheId);
            
            CreateTable(
                "TPA.PerfilAcao",
                c => new
                    {
                        Perfil_Id = c.Int(nullable: false),
                        Acao_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Perfil_Id, t.Acao_Id })
                .ForeignKey("TPA.Perfil", t => t.Perfil_Id, cascadeDelete: true)
                .ForeignKey("TPA.Acao", t => t.Acao_Id, cascadeDelete: true)
                .Index(t => t.Perfil_Id)
                .Index(t => t.Acao_Id);
            
            CreateTable(
                "TPA.UsuarioPerfil",
                c => new
                    {
                        Usuario_Id = c.Int(nullable: false),
                        Perfil_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Usuario_Id, t.Perfil_Id })
                .ForeignKey("TPA.Usuario", t => t.Usuario_Id, cascadeDelete: true)
                .ForeignKey("TPA.Perfil", t => t.Perfil_Id, cascadeDelete: true)
                .Index(t => t.Usuario_Id)
                .Index(t => t.Perfil_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("TPA.Referencia", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.UsuarioPerfil", "Perfil_Id", "TPA.Perfil");
            DropForeignKey("TPA.UsuarioPerfil", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.Funcionario", "Id", "TPA.Usuario");
            DropForeignKey("TPA.Atividade", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.Atividade", "TipoAtividade_Id", "TPA.TipoAtividade");
            DropForeignKey("TPA.Atividade", "ProjectNode_Id", "TPA.ProjectNode");
            DropForeignKey("TPA.ProjectNode", "NodeLabel_Id", "TPA.NodeLabel");
            DropForeignKey("TPA.ProjectNode", "Pai_Id", "TPA.ProjectNode");
            DropForeignKey("TPA.PerfilAcao", "Acao_Id", "TPA.Acao");
            DropForeignKey("TPA.PerfilAcao", "Perfil_Id", "TPA.Perfil");
            DropIndex("TPA.UsuarioPerfil", new[] { "Perfil_Id" });
            DropIndex("TPA.UsuarioPerfil", new[] { "Usuario_Id" });
            DropIndex("TPA.PerfilAcao", new[] { "Acao_Id" });
            DropIndex("TPA.PerfilAcao", new[] { "Perfil_Id" });
            DropIndex("TPA.Referencia", new[] { "Usuario_Id" });
            DropIndex("TPA.Funcionario", new[] { "Id" });
            DropIndex("TPA.TipoAtividade", new[] { "Nome" });
            DropIndex("TPA.NodeLabel", new[] { "Nome" });
            DropIndex("TPA.ProjectNode", new[] { "NodeLabel_Id" });
            DropIndex("TPA.ProjectNode", new[] { "Pai_Id" });
            DropIndex("TPA.Atividade", new[] { "Usuario_Id" });
            DropIndex("TPA.Atividade", new[] { "TipoAtividade_Id" });
            DropIndex("TPA.Atividade", new[] { "ProjectNode_Id" });
            DropIndex("TPA.Usuario", new[] { "Login" });
            DropIndex("TPA.Perfil", new[] { "Nome" });
            DropIndex("TPA.Acao", new[] { "Nome" });
            DropTable("TPA.UsuarioPerfil");
            DropTable("TPA.PerfilAcao");
            DropTable("TPA.UserTokenCache");
            DropTable("TPA.Feriado");
            DropTable("TPA.Referencia");
            DropTable("TPA.Funcionario");
            DropTable("TPA.TipoAtividade");
            DropTable("TPA.NodeLabel");
            DropTable("TPA.ProjectNode");
            DropTable("TPA.Atividade");
            DropTable("TPA.Usuario");
            DropTable("TPA.Perfil");
            DropTable("TPA.Acao");
        }
    }
}
