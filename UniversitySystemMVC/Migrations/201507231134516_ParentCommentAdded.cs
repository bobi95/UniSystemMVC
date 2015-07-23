namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParentCommentAdded : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Comments", "CommentId");
            AddForeignKey("dbo.Comments", "CommentId", "dbo.Comments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "CommentId", "dbo.Comments");
            DropIndex("dbo.Comments", new[] { "CommentId" });
        }
    }
}
