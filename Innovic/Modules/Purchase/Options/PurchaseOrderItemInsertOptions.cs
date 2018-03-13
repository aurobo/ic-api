using Innovic.Modules.Master.Models;
using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class PurchaseOrderItemInsertOptions
    {
        [CopyTo(typeof(Material), Red.Wine.Relationship.Dependency, true)]
        public string MaterialId { get; set; }

        public string Text { get; set; }

        public int Quantity { get; set; }

        public double Rate { get; set; }

        public DateTime DeliveryDate { get; set; }

        [CopyTo(typeof(PurchaseOrder), Red.Wine.Relationship.Dependency, true)]
        public string PurchaseOrderId { get; set; }
    }
}