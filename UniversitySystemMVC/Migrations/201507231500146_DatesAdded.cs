namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatesAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "DateModified", c => c.DateTime(nullable: false));
            AddColumn("dbo.Comments", "DateCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.Comments", "DateModified", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "DateModified");
            DropColumn("dbo.Comments", "DateCreated");
            DropColumn("dbo.Articles", "DateModified");
        }
    }
}
