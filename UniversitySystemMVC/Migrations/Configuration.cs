namespace UniversitySystemMVC.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using UniversitySystemMVC.Entity;
    using UniversitySystemMVC.Hasher;

    internal sealed class Configuration : DbMigrationsConfiguration<UniversitySystemMVC.DA.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UniversitySystemMVC.DA.AppContext context)
        {
            var passPhrase = PasswordHasher.Hash("pass");
            Administrator a = new Administrator
            {
                Id = 1,
                Username = "admin",
                FirstName = "Admin",
                LastName = "Adminov",
                Email = "admin@domain.com",
                Hash = passPhrase.Hash,
                Salt = passPhrase.Salt,
                IsConfirmed = true,
                IsActive = true
            };

            context.Administrators.AddOrUpdate(a);

            context.Courses.AddOrUpdate(
                new Course
                {
                    Id = 1,
                    Name = "Software Technologies and Design",
                    Code = 68
                });
            context.Courses.AddOrUpdate(
                new Course
                {
                    Id = 2,
                    Name = "Bussiness Information Technologies",
                    Code = 67
                });

            context.Students.AddOrUpdate(
                new Student
                {
                    Id = 1,
                    Username = "student",
                    FirstName = "Student",
                    LastName = "Student",
                    Email = "student@domain.com",
                    FacultyNumber = "1401681001",
                    CourseId = 1,
                    Hash = passPhrase.Hash,
                    Salt = passPhrase.Salt,
                    IsActive = true
                });
        }
    }
}
