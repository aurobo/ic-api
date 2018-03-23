using Innovic.Modules.Accounts.Models;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Sales.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Red.Wine;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Innovic.App
{
    public class InnovicContext : IdentityDbContext<User>
    {
        private readonly string _userId;

        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
        public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }
        public DbSet<GoodsIssue> GoodsIssues { get; set; }
        public DbSet<GoodsIssueItem> GoodsIssueItems { get; set; }

        public InnovicContext()
            : base("dbConnection")
        {
        }

        public InnovicContext(string userId = null)
            : base("dbConnection")
        {
            _userId = userId;
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

        public override int SaveChanges()
        {
            this.UpdateContextWithDefaultValues(_userId);
            return base.SaveChanges();
        }
    }
}