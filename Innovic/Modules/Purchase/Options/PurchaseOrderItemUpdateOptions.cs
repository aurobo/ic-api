using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;

namespace Innovic.Modules.Purchase.Options
{
    public class PurchaseOrderItemUpdateOptions
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public DateTime DeliveryDate { get; set; }

        [CopyTo(typeof(PurchaseOrder), Red.Wine.Relationship.Dependency, true)]
        public string PurchaseOrderId { get; set; }
    }
}