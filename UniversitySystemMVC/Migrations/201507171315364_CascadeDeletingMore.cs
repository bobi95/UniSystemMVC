namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeletingMore : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CoursesSubjects", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CoursesSubjects", "SubjectId", "dbo.Subjects");
            AddForeignKey("dbo.CoursesSubjects", "CourseId", "dbo.Courses", "Id");
            AddForeignKey("dbo.CoursesSubjects", "SubjectId", "dbo.Subjects", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CoursesSubjects", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.CoursesSubjects", "CourseId", "dbo.Courses");
            AddForeignKey("dbo.CoursesSubjects", "SubjectId", "dbo.Subjects", "Id", cascadeDelete: true);
            AddForeignKey("dbo.CoursesSubjects", "CourseId", "dbo.Courses", "Id", cascadeDelete: true);
        }
    }
}
