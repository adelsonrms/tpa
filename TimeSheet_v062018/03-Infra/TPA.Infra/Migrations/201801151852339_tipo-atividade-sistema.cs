namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tipoatividadesistema : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.TipoAtividade", "Administrativo", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("TPA.TipoAtividade", "Administrativo");
        }
    }
}
