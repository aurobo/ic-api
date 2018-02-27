using Innovic.Modules.Accounts.Models;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Sales.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Red.Wine;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Innovic.App
{
    public class InnovicContext : IdentityDbContext<User>
    {
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

        public InnovicContext()
            : base("dbConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Entity<User>()
                .ToTable("Users");

            modelBuilder.Entity<IdentityRole>()
                .ToTable("Roles");

            modelBuilder.Entity<IdentityUserClaim>()
                .ToTable("UserClaims");

            modelBuilder.Entity<IdentityUserLogin>()
                .ToTable("UserLogins");

            modelBuilder.Entity<IdentityUserRole>()
                .ToTable("UserRoles");
        }

        public InnovicContext Create()
        {
            return new InnovicContext();
        }
    }
}