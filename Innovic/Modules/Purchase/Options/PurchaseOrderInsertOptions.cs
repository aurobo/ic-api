using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;

namespace Innovic.Modules.Purchase.Options
{
    public class PurchaseOrderInsertOptions
    {
        public PurchaseOrderInsertOptions()
        {
            PurchaseRequests = new List<string>();
            PurchaseOrderItems = new List<PurchaseOrderItemInsertOptions>();
        }

        public DateTime Date { get; set; }

        [CopyTo(typeof(Supplier), Red.Wine.Relationship.Dependency, true)]
        public string SupplierId { get; set; }

        [CopyTo(typeof(PurchaseRequest), Red.Wine.Relationship.Dependency, true)]
        public List<string> PurchaseRequests { get; set; }

        [CopyTo(typeof(PurchaseOrderItem), Red.Wine.Relationship.Dependent)]
        public virtual List<PurchaseOrderItemInsertOptions> PurchaseOrderItems { get; set; }
    }
}
