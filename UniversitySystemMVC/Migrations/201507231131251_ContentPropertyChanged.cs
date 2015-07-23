namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContentPropertyChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "Content", c => c.String());
            DropColumn("dbo.Articles", "Contect");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Articles", "Contect", c => c.String());
            DropColumn("dbo.Articles", "Content");
        }
    }
}
