using Innovic.Models.Master;
using Red.Wine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Models.Sales
{
    public class SalesOrderItem : BaseModel
    {
        public string Number { get; set; }

        public string Description { get; set; }

        public double UnitPrice { get; set; }

        public double Value { get; set; }

        public int Quantity { get; set; }

        public string MaterialId { get; set; }

        public string SalesOrderId { get; set; }

        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Material Material { get; set; }
    }
}