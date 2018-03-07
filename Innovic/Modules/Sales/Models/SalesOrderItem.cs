using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Sales.Models
{
    public class SalesOrderItem : BaseModel
    {
        public SalesOrderItem()
        {
            InvoiceItems = new List<InvoiceItem>();
        }

        public string Number { get; set; }

        public string Description { get; set; }

        public double UnitPrice { get; set; }

        public double Value { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DeliveryDate { get; set; }

        public string WBSElement { get; set; }

        public string MaterialId { get; set; }

        public string SalesOrderId { get; set; }

        public virtual SalesOrder SalesOrder { get; set; }
        public virtual Material Material { get; set; }
        public virtual List<InvoiceItem> InvoiceItems { get; set; }
    }
}