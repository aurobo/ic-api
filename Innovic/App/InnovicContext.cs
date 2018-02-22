using Innovic.Modules.Master.Models;
using Innovic.Modules.Sales.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Red.Wine;
using System;
using System.Data.Entity;
using System.Linq;

namespace Innovic.App
{
    public class InnovicContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

        public InnovicContext()
            : base("dbConnection")
        {

        }

        public InnovicContext Create()
        {
            return new InnovicContext();
        }       
    }
}