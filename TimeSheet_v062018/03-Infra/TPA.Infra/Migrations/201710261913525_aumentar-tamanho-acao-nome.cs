namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aumentartamanhoacaonome : DbMigration
    {
        public override void Up()
        {
            DropIndex("TPA.Acao", new[] { "Nome" });
            AlterColumn("TPA.Acao", "Nome", c => c.String(nullable: false, maxLength: 250));
            CreateIndex("TPA.Acao", "Nome", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("TPA.Acao", new[] { "Nome" });
            AlterColumn("TPA.Acao", "Nome", c => c.String(nullable: false, maxLength: 50));
            CreateIndex("TPA.Acao", "Nome", unique: true);
        }
    }
}
