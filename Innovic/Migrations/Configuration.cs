using Innovic.App;
using Innovic.Modules.Accounts.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.Migrations;
using System.Linq;

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
                new User
                {
                    Id = "admin",
                    Name = "Tony Stark",
                    UserName = "admin",
                    PasswordHash = hasher.HashPassword("Admin123!"),
                    SecurityStamp = "admin"
                }
            );
        }
    }
}
