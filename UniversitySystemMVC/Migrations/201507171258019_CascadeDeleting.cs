namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeleting : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TeacherCoursesSubjects", "Teacher_Id", "dbo.Teachers");
            DropForeignKey("dbo.TeacherCoursesSubjects", "CoursesSubjects_Id", "dbo.CoursesSubjects");
            AddForeignKey("dbo.TeacherCoursesSubjects", "Teacher_Id", "dbo.Teachers", "Id");
            AddForeignKey("dbo.TeacherCoursesSubjects", "CoursesSubjects_Id", "dbo.CoursesSubjects", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeacherCoursesSubjects", "CoursesSubjects_Id", "dbo.CoursesSubjects");
            DropForeignKey("dbo.TeacherCoursesSubjects", "Teacher_Id", "dbo.Teachers");
            AddForeignKey("dbo.TeacherCoursesSubjects", "CoursesSubjects_Id", "dbo.CoursesSubjects", "Id", cascadeDelete: true);
            AddForeignKey("dbo.TeacherCoursesSubjects", "Teacher_Id", "dbo.Teachers", "Id", cascadeDelete: true);
        }
    }
}
