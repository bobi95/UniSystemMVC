using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using UniversitySystemMVC.Entity;

namespace UniversitySystemMVC.DA
{
    public class AppContext : DbContext
    {
        public AppContext() : base("UniversitySystemDB") { }


        //public DbSet<User> Users { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<CoursesSubjects> CoursesSubjects { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Student>()
                .HasOptional(s => s.Course)
                .WithMany(s => s.Students)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Teacher>()
                .HasOptional(t => t.Title)
                .WithMany(t => t.Teachers)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Students)
                .WithOptional(s => s.Course)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.CoursesSubjects)
                .WithRequired(cs => cs.Course)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Subject>()
                .HasMany(s => s.CoursesSubjects)
                .WithRequired(cs => cs.Subject)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Administrator>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Administrators");
            //});

            //modelBuilder.Entity<Student>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Students");
            //});

            //modelBuilder.Entity<Teacher>().Map(m =>
            //{
            //    m.MapInheritedProperties();
            //    m.ToTable("Teachers");
            //});
        }
    }
}