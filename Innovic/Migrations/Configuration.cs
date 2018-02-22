using Innovic.App;
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
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
