using Innovic.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Sales.Models
{
    public class InvoiceItem : BaseModel
    {
        public int Quantity { get; set; }
        public string InvoiceId { get; set; }
        public string SalesOrderItemId { get; set; }

        public virtual Invoice Invoice { get; set; }
        public virtual SalesOrderItem SalesOrderItem { get; set; }
    }
}