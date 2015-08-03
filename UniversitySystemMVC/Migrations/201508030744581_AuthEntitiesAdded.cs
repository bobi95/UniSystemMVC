namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuthEntitiesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuthActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AuthControllerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AuthControllers", t => t.AuthControllerId, cascadeDelete: true)
                .Index(t => t.AuthControllerId);
            
            CreateTable(
                "dbo.AuthControllers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AuthActions", "AuthControllerId", "dbo.AuthControllers");
            DropIndex("dbo.AuthActions", new[] { "AuthControllerId" });
            DropTable("dbo.AuthControllers");
            DropTable("dbo.AuthActions");
        }
    }
}
