namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class previstocorrenteebancohoras : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.Referencia", "PrevistoCorrenteSegundos", c => c.Int(nullable: false));
            AddColumn("TPA.Referencia", "BancoDeHorasSegundos", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("TPA.Referencia", "BancoDeHorasSegundos");
            DropColumn("TPA.Referencia", "PrevistoCorrenteSegundos");
        }
    }
}
