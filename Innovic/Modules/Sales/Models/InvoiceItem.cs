using Innovic.App;
using Innovic.Modules.Master.Models;
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
        public string MaterialId { get; set; }

        public virtual Material Material { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual SalesOrderItem SalesOrderItem { get; set; }
    }
}