namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class auditoria : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.Acao", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.Acao", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.Acao", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.Acao", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.Perfil", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.Perfil", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.Perfil", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.Perfil", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.Usuario", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.Usuario", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.Usuario", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.Usuario", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.Atividade", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.Atividade", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.Atividade", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.Atividade", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.ProjectNode", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.ProjectNode", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.ProjectNode", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.ProjectNode", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.NodeLabel", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.NodeLabel", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.NodeLabel", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.NodeLabel", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.TipoAtividade", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.TipoAtividade", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.TipoAtividade", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.TipoAtividade", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.Funcionario", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.Funcionario", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.Funcionario", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.Funcionario", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.Referencia", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.Referencia", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.Referencia", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.Referencia", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.AtestadoAnexo", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.AtestadoAnexo", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.AtestadoAnexo", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.AtestadoAnexo", "MomentoEdicao", c => c.DateTime());
            AddColumn("TPA.Feriado", "UsuarioInclusao", c => c.String());
            AddColumn("TPA.Feriado", "MomentoInclusao", c => c.DateTime());
            AddColumn("TPA.Feriado", "UsuarioEdicao", c => c.String());
            AddColumn("TPA.Feriado", "MomentoEdicao", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("TPA.Feriado", "MomentoEdicao");
            DropColumn("TPA.Feriado", "UsuarioEdicao");
            DropColumn("TPA.Feriado", "MomentoInclusao");
            DropColumn("TPA.Feriado", "UsuarioInclusao");
            DropColumn("TPA.AtestadoAnexo", "MomentoEdicao");
            DropColumn("TPA.AtestadoAnexo", "UsuarioEdicao");
            DropColumn("TPA.AtestadoAnexo", "MomentoInclusao");
            DropColumn("TPA.AtestadoAnexo", "UsuarioInclusao");
            DropColumn("TPA.Referencia", "MomentoEdicao");
            DropColumn("TPA.Referencia", "UsuarioEdicao");
            DropColumn("TPA.Referencia", "MomentoInclusao");
            DropColumn("TPA.Referencia", "UsuarioInclusao");
            DropColumn("TPA.Funcionario", "MomentoEdicao");
            DropColumn("TPA.Funcionario", "UsuarioEdicao");
            DropColumn("TPA.Funcionario", "MomentoInclusao");
            DropColumn("TPA.Funcionario", "UsuarioInclusao");
            DropColumn("TPA.TipoAtividade", "MomentoEdicao");
            DropColumn("TPA.TipoAtividade", "UsuarioEdicao");
            DropColumn("TPA.TipoAtividade", "MomentoInclusao");
            DropColumn("TPA.TipoAtividade", "UsuarioInclusao");
            DropColumn("TPA.NodeLabel", "MomentoEdicao");
            DropColumn("TPA.NodeLabel", "UsuarioEdicao");
            DropColumn("TPA.NodeLabel", "MomentoInclusao");
            DropColumn("TPA.NodeLabel", "UsuarioInclusao");
            DropColumn("TPA.ProjectNode", "MomentoEdicao");
            DropColumn("TPA.ProjectNode", "UsuarioEdicao");
            DropColumn("TPA.ProjectNode", "MomentoInclusao");
            DropColumn("TPA.ProjectNode", "UsuarioInclusao");
            DropColumn("TPA.Atividade", "MomentoEdicao");
            DropColumn("TPA.Atividade", "UsuarioEdicao");
            DropColumn("TPA.Atividade", "MomentoInclusao");
            DropColumn("TPA.Atividade", "UsuarioInclusao");
            DropColumn("TPA.Usuario", "MomentoEdicao");
            DropColumn("TPA.Usuario", "UsuarioEdicao");
            DropColumn("TPA.Usuario", "MomentoInclusao");
            DropColumn("TPA.Usuario", "UsuarioInclusao");
            DropColumn("TPA.Perfil", "MomentoEdicao");
            DropColumn("TPA.Perfil", "UsuarioEdicao");
            DropColumn("TPA.Perfil", "MomentoInclusao");
            DropColumn("TPA.Perfil", "UsuarioInclusao");
            DropColumn("TPA.Acao", "MomentoEdicao");
            DropColumn("TPA.Acao", "UsuarioEdicao");
            DropColumn("TPA.Acao", "MomentoInclusao");
            DropColumn("TPA.Acao", "UsuarioInclusao");
        }
    }
}
