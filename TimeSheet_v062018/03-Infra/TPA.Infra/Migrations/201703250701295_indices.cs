namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class indices : DbMigration
    {
        public override void Up()
        {
            AlterColumn("TPA.Atividade", "Observacao", c => c.String());
            CreateIndex("TPA.ProjectNode", "Nome", unique: true, name: "IX_SubNosUnicosPorNo");
        }
        
        public override void Down()
        {
            DropIndex("TPA.ProjectNode", "IX_SubNosUnicosPorNo");
            AlterColumn("TPA.Atividade", "Observacao", c => c.String(nullable: false));
        }
    }
}
