namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tirarcascadedeletes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("TPA.PerfilAcao", "Perfil_Id", "TPA.Perfil");
            DropForeignKey("TPA.PerfilAcao", "Acao_Id", "TPA.Acao");
            DropForeignKey("TPA.UsuarioPerfil", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.UsuarioPerfil", "Perfil_Id", "TPA.Perfil");
            DropForeignKey("TPA.Atividade", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.ProjectNodeUsuario", "ProjectNode_Id", "TPA.ProjectNode");
            DropForeignKey("TPA.ProjectNodeUsuario", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.Atividade", "ProjectNode_Id", "TPA.ProjectNode");
            DropForeignKey("TPA.Atividade", "TipoAtividade_Id", "TPA.TipoAtividade");
            AddForeignKey("TPA.PerfilAcao", "Perfil_Id", "TPA.Perfil", "Id");
            AddForeignKey("TPA.PerfilAcao", "Acao_Id", "TPA.Acao", "Id");
            AddForeignKey("TPA.UsuarioPerfil", "Usuario_Id", "TPA.Usuario", "Id");
            AddForeignKey("TPA.UsuarioPerfil", "Perfil_Id", "TPA.Perfil", "Id");
            AddForeignKey("TPA.Atividade", "Usuario_Id", "TPA.Usuario", "Id");
            AddForeignKey("TPA.ProjectNodeUsuario", "ProjectNode_Id", "TPA.ProjectNode", "Id");
            AddForeignKey("TPA.ProjectNodeUsuario", "Usuario_Id", "TPA.Usuario", "Id");
            AddForeignKey("TPA.Atividade", "ProjectNode_Id", "TPA.ProjectNode", "Id");
            AddForeignKey("TPA.Atividade", "TipoAtividade_Id", "TPA.TipoAtividade", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TPA.Atividade", "TipoAtividade_Id", "TPA.TipoAtividade");
            DropForeignKey("TPA.Atividade", "ProjectNode_Id", "TPA.ProjectNode");
            DropForeignKey("TPA.ProjectNodeUsuario", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.ProjectNodeUsuario", "ProjectNode_Id", "TPA.ProjectNode");
            DropForeignKey("TPA.Atividade", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.UsuarioPerfil", "Perfil_Id", "TPA.Perfil");
            DropForeignKey("TPA.UsuarioPerfil", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.PerfilAcao", "Acao_Id", "TPA.Acao");
            DropForeignKey("TPA.PerfilAcao", "Perfil_Id", "TPA.Perfil");
            AddForeignKey("TPA.Atividade", "TipoAtividade_Id", "TPA.TipoAtividade", "Id", cascadeDelete: true);
            AddForeignKey("TPA.Atividade", "ProjectNode_Id", "TPA.ProjectNode", "Id", cascadeDelete: true);
            AddForeignKey("TPA.ProjectNodeUsuario", "Usuario_Id", "TPA.Usuario", "Id", cascadeDelete: true);
            AddForeignKey("TPA.ProjectNodeUsuario", "ProjectNode_Id", "TPA.ProjectNode", "Id", cascadeDelete: true);
            AddForeignKey("TPA.Atividade", "Usuario_Id", "TPA.Usuario", "Id", cascadeDelete: true);
            AddForeignKey("TPA.UsuarioPerfil", "Perfil_Id", "TPA.Perfil", "Id", cascadeDelete: true);
            AddForeignKey("TPA.UsuarioPerfil", "Usuario_Id", "TPA.Usuario", "Id", cascadeDelete: true);
            AddForeignKey("TPA.PerfilAcao", "Acao_Id", "TPA.Acao", "Id", cascadeDelete: true);
            AddForeignKey("TPA.PerfilAcao", "Perfil_Id", "TPA.Perfil", "Id", cascadeDelete: true);
        }
    }
}
