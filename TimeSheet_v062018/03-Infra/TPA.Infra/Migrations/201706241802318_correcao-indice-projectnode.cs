namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class correcaoindiceprojectnode : DbMigration
    {
        public override void Up()
        {
            DropIndex("TPA.ProjectNode", "IX_SubNosUnicosPorNo");
            DropIndex("TPA.ProjectNode", new[] { "Pai_Id" });
            CreateIndex("TPA.ProjectNode", new[] { "Nome", "Pai_Id" }, unique: true, name: "UQ_SubNosUnicosPorNo");
            CreateIndex("TPA.ProjectNode", "Pai_Id");
        }
        
        public override void Down()
        {
            DropIndex("TPA.ProjectNode", new[] { "Pai_Id" });
            DropIndex("TPA.ProjectNode", "UQ_SubNosUnicosPorNo");
            CreateIndex("TPA.ProjectNode", "Pai_Id");
            CreateIndex("TPA.ProjectNode", "Nome", unique: true, name: "IX_SubNosUnicosPorNo");
        }
    }
}
