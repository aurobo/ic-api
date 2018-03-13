using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class PurchaseOrderUpdateOptions
    {
        public PurchaseOrderUpdateOptions()
        {
            PurchaseOrderItems = new List<PurchaseOrderItemInsertOptions>();
        }

        public string Id { get; set; }

        public string Type { get; set; }

        public string Reference { get; set; }

        public DateTime Date { get; set; }

        public string TermsAndConditions { get; set; }

        [CopyTo(typeof(PurchaseOrderItem), Red.Wine.Relationship.Dependent)]
        public List<PurchaseOrderItemInsertOptions> PurchaseOrderItems { get; set; }
    }
}