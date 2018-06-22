namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class referenciasegundos : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.Referencia", "PrevistoSegundos", c => c.Int(nullable: false));
            AddColumn("TPA.Referencia", "RealizadoSegundos", c => c.Int(nullable: false));
            AddColumn("TPA.Referencia", "SaldoSegundos", c => c.Int(nullable: false));
            DropColumn("TPA.Referencia", "Previsto");
            DropColumn("TPA.Referencia", "Realizado");
            DropColumn("TPA.Referencia", "Saldo");
        }
        
        public override void Down()
        {
            AddColumn("TPA.Referencia", "Saldo", c => c.Time(nullable: false, precision: 7));
            AddColumn("TPA.Referencia", "Realizado", c => c.Time(nullable: false, precision: 7));
            AddColumn("TPA.Referencia", "Previsto", c => c.Time(nullable: false, precision: 7));
            DropColumn("TPA.Referencia", "SaldoSegundos");
            DropColumn("TPA.Referencia", "RealizadoSegundos");
            DropColumn("TPA.Referencia", "PrevistoSegundos");
        }
    }
}
