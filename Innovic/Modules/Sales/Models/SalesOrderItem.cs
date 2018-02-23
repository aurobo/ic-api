using Innovic.Modules.Master.Models;
using Red.Wine;

namespace Innovic.Modules.Sales.Models
{
    public class SalesOrderItem : WineModel
    {
        public string Number { get; set; }

        public string Description { get; set; }

        public double UnitPrice { get; set; }

        public double Value { get; set; }

        public int Quantity { get; set; }

        public string MaterialId { get; set; }

        public string SalesOrderId { get; set; }

        public bool IsDeleted { get; set; } // Shift to WineModel

        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Material Material { get; set; }
    }
}