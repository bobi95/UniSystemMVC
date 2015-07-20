namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsActiveBase : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Courses", "IsActive");
            DropColumn("dbo.CoursesSubjects", "IsActive");
            DropColumn("dbo.Subjects", "IsActive");
            DropColumn("dbo.Grades", "IsActive");
            DropColumn("dbo.Titles", "IsActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Titles", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Grades", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Subjects", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.CoursesSubjects", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Courses", "IsActive", c => c.Boolean(nullable: false));
        }
    }
}
