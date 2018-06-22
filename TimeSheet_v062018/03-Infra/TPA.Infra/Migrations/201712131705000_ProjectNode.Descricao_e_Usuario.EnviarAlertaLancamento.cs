namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectNodeDescricao_e_UsuarioEnviarAlertaLancamento : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.Usuario", "EnviarAlertaLancamento", c => c.Boolean(nullable: false));
            DropColumn("TPA.Usuario", "EnviarEmailAlertaLancamento");
            Sql("update TPA.Usuario set EnviarAlertaLancamento = 1");
        }
        
        public override void Down()
        {
            AddColumn("TPA.Usuario", "EnviarEmailAlertaLancamento", c => c.Boolean(nullable: false));
            DropColumn("TPA.Usuario", "EnviarAlertaLancamento");
        }
    }
}
