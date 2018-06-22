namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atestado : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TPA.AtestadoAnexo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Arquivo = c.Binary(nullable: false),
                        Observacao = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("TPA.Atividade", "AtestadoAnexo_Id", c => c.Int());
            CreateIndex("TPA.Atividade", "AtestadoAnexo_Id");
            AddForeignKey("TPA.Atividade", "AtestadoAnexo_Id", "TPA.AtestadoAnexo", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("TPA.Atividade", "AtestadoAnexo_Id", "TPA.AtestadoAnexo");
            DropIndex("TPA.Atividade", new[] { "AtestadoAnexo_Id" });
            DropColumn("TPA.Atividade", "AtestadoAnexo_Id");
            DropTable("TPA.AtestadoAnexo");
        }
    }
}
