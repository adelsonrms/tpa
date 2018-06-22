namespace TPA.Infra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aspnetIdentity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "TPA.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "TPA.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("TPA.AspNetRoles", t => t.RoleId)
                .ForeignKey("TPA.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "TPA.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "TPA.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("TPA.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "TPA.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("TPA.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("TPA.AspNetUserRoles", "UserId", "TPA.AspNetUsers");
            DropForeignKey("TPA.AspNetUserLogins", "UserId", "TPA.AspNetUsers");
            DropForeignKey("TPA.AspNetUserClaims", "UserId", "TPA.AspNetUsers");
            DropForeignKey("TPA.AspNetUserRoles", "RoleId", "TPA.AspNetRoles");
            DropIndex("TPA.AspNetUserLogins", new[] { "UserId" });
            DropIndex("TPA.AspNetUserClaims", new[] { "UserId" });
            DropIndex("TPA.AspNetUsers", "UserNameIndex");
            DropIndex("TPA.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("TPA.AspNetUserRoles", new[] { "UserId" });
            DropIndex("TPA.AspNetRoles", "RoleNameIndex");
            DropTable("TPA.AspNetUserLogins");
            DropTable("TPA.AspNetUserClaims");
            DropTable("TPA.AspNetUsers");
            DropTable("TPA.AspNetUserRoles");
            DropTable("TPA.AspNetRoles");
        }
    }
}
