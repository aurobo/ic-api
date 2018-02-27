using Innovic.App;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.Migrations;

namespace Innovic.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<InnovicContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(InnovicContext context)
        {
            PasswordHasher hasher = new PasswordHasher();

            context.Users.AddOrUpdate(
                x => x.UserName,
                new IdentityUser
                {
                    UserName = "admin",
                    PasswordHash = hasher.HashPassword("Admin123!"),
                    SecurityStamp = new Guid().ToString()
                }
            );
        }
    }
}
