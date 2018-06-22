namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProjectNodeDescricao_Criar_Campo : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.ProjectNode", "Descricao", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("TPA.ProjectNode", "Descricao");
        }
    }
}
