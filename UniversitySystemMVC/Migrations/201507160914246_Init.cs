namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Administrators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Hash = c.String(),
                        Salt = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Code = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CoursesSubjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        SubjectId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Grades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.Int(nullable: false),
                        SubjectId = c.Int(nullable: false),
                        GradeValue = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Students", t => t.StudentId, cascadeDelete: true)
                .ForeignKey("dbo.Subjects", t => t.SubjectId, cascadeDelete: true)
                .Index(t => t.StudentId)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FacultyNumber = c.String(),
                        CourseId = c.Int(),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Hash = c.String(),
                        Salt = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TitleId = c.Int(nullable: false),
                        Username = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        Hash = c.String(),
                        Salt = c.String(),
                        IsConfirmed = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Titles", t => t.TitleId, cascadeDelete: true)
                .Index(t => t.TitleId);
            
            CreateTable(
                "dbo.Titles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeacherCoursesSubjects",
                c => new
                    {
                        Teacher_Id = c.Int(nullable: false),
                        CoursesSubjects_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Teacher_Id, t.CoursesSubjects_Id })
                .ForeignKey("dbo.Teachers", t => t.Teacher_Id, cascadeDelete: true)
                .ForeignKey("dbo.CoursesSubjects", t => t.CoursesSubjects_Id, cascadeDelete: true)
                .Index(t => t.Teacher_Id)
                .Index(t => t.CoursesSubjects_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Teachers", "TitleId", "dbo.Titles");
            DropForeignKey("dbo.TeacherCoursesSubjects", "CoursesSubjects_Id", "dbo.CoursesSubjects");
            DropForeignKey("dbo.TeacherCoursesSubjects", "Teacher_Id", "dbo.Teachers");
            DropForeignKey("dbo.Grades", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.Grades", "StudentId", "dbo.Students");
            DropForeignKey("dbo.Students", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.CoursesSubjects", "SubjectId", "dbo.Subjects");
            DropForeignKey("dbo.CoursesSubjects", "CourseId", "dbo.Courses");
            DropIndex("dbo.TeacherCoursesSubjects", new[] { "CoursesSubjects_Id" });
            DropIndex("dbo.TeacherCoursesSubjects", new[] { "Teacher_Id" });
            DropIndex("dbo.Teachers", new[] { "TitleId" });
            DropIndex("dbo.Students", new[] { "CourseId" });
            DropIndex("dbo.Grades", new[] { "SubjectId" });
            DropIndex("dbo.Grades", new[] { "StudentId" });
            DropIndex("dbo.CoursesSubjects", new[] { "SubjectId" });
            DropIndex("dbo.CoursesSubjects", new[] { "CourseId" });
            DropTable("dbo.TeacherCoursesSubjects");
            DropTable("dbo.Titles");
            DropTable("dbo.Teachers");
            DropTable("dbo.Students");
            DropTable("dbo.Grades");
            DropTable("dbo.Subjects");
            DropTable("dbo.CoursesSubjects");
            DropTable("dbo.Courses");
            DropTable("dbo.Administrators");
        }
    }
}
