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
        }
    }
}
