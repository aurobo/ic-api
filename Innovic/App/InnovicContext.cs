using Innovic.Models.Master;
using Innovic.Models.Sales;
using Microsoft.AspNet.Identity.EntityFramework;
using Red.Wine;
using System;
using System.Data.Entity;
using System.Linq;

namespace Innovic.App
{
    public class InnovicContext : IdentityDbContext<IdentityUser>
    {
        private readonly string _userId = "Jarvis";

        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }

        public InnovicContext()
            : base("dbConnection")
        {

        }

        public InnovicContext(string userId)
        {
            _userId = userId;
        }

        public InnovicContext Create()
        {
            return new InnovicContext();
        }

        public override int SaveChanges()
        {
            SetDefaultValues();
            return base.SaveChanges();
        }

        private void SetDefaultValues()
        {
            var entries = ChangeTracker.Entries<BaseModel>().Where(x => x.State == EntityState.Modified || x.State == EntityState.Added);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.SetWhenModifying(_userId, DateTime.Now);
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.Entity.SetWhenInserting(
                        Guid.NewGuid().ToString(), 
                        _userId,
                        DateTime.Now,
                        true,
                        GetIncrementedKeyId(entry.Entity));
                }
            }
        }

        private long GetIncrementedKeyId(BaseModel entity)
        {
            var dbSet = Set(entity.GetType());
            var entityList = Enumerable.Cast<BaseModel>(dbSet).ToList();
            long currentCount = 0;

            if(entityList.Count > 0)
            {
                var lastInsertedEntity = entityList
                    .OrderByDescending(t => t.KeyId)
                    .First();

                currentCount = lastInsertedEntity.KeyId;
            }

            return ++currentCount;
        }
    }
}