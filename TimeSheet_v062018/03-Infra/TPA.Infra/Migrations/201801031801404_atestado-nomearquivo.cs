namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class atestadonomearquivo : DbMigration
    {
        public override void Up()
        {
            AddColumn("TPA.AtestadoAnexo", "NomeArquivoOriginal", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("TPA.AtestadoAnexo", "NomeArquivoOriginal");
        }
    }
}
