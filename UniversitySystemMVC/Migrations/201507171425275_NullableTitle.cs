namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullableTitle : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Teachers", "TitleId", "dbo.Titles");
            DropIndex("dbo.Teachers", new[] { "TitleId" });
            AlterColumn("dbo.Teachers", "TitleId", c => c.Int());
            CreateIndex("dbo.Teachers", "TitleId");
            AddForeignKey("dbo.Teachers", "TitleId", "dbo.Titles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teachers", "TitleId", "dbo.Titles");
            DropIndex("dbo.Teachers", new[] { "TitleId" });
            AlterColumn("dbo.Teachers", "TitleId", c => c.Int(nullable: false));
            CreateIndex("dbo.Teachers", "TitleId");
            AddForeignKey("dbo.Teachers", "TitleId", "dbo.Titles", "Id", cascadeDelete: true);
        }
    }
}
