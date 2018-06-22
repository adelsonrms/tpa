namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NosPorUsuario : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TPA.ProjectNodeUsuario",
                c => new
                    {
                        ProjectNode_Id = c.Int(nullable: false),
                        Usuario_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectNode_Id, t.Usuario_Id })
                .ForeignKey("TPA.ProjectNode", t => t.ProjectNode_Id, cascadeDelete: true)
                .ForeignKey("TPA.Usuario", t => t.Usuario_Id, cascadeDelete: true)
                .Index(t => t.ProjectNode_Id)
                .Index(t => t.Usuario_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("TPA.ProjectNodeUsuario", "Usuario_Id", "TPA.Usuario");
            DropForeignKey("TPA.ProjectNodeUsuario", "ProjectNode_Id", "TPA.ProjectNode");
            DropIndex("TPA.ProjectNodeUsuario", new[] { "Usuario_Id" });
            DropIndex("TPA.ProjectNodeUsuario", new[] { "ProjectNode_Id" });
            DropTable("TPA.ProjectNodeUsuario");
        }
    }
}
