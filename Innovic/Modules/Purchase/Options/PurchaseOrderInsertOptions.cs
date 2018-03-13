using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class PurchaseOrderInsertOptions
    {
        public PurchaseOrderInsertOptions()
        {
            PurchaseRequests = new List<string>();
            PurchaseOrderItems = new List<PurchaseOrderItemInsertOptions>();
        }

        public string Type { get; set; }

        public string Reference { get; set; }

        public DateTime Date { get; set; }

        [CopyTo(typeof(Supplier), Red.Wine.Relationship.Dependency, true)]
        public string SupplierId { get; set; }

        public string TermsAndConditions { get; set; }

        [CopyTo(typeof(PurchaseOrderItem), Red.Wine.Relationship.Dependent)]
        public List<PurchaseOrderItemInsertOptions> PurchaseOrderItems { get; set; }

        [CopyTo(typeof(PurchaseRequest), Red.Wine.Relationship.Dependency, true)]
        public List<string> PurchaseRequests { get; set; }
    }
}