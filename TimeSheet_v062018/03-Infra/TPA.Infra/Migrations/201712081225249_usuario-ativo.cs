namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usuarioativo : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.Usuario", "Ativo", c => c.Boolean(nullable: false));
            Sql("update TPA.Usuario set Ativo = 1");
        }
        
        public override void Down()
        {
            DropColumn("TPA.Usuario", "Ativo");
        }
    }
}
