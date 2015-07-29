namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateCreatedToLikeAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Likes", "DateCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Likes", "DateCreated");
        }
    }
}
