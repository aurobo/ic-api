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
            PurchaseOrderItems = new List<PurchaseOrderItemUpdateOptions>();
        }

        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string TermsAndConditions { get; set; }

        [CopyTo(typeof(PurchaseOrderItem), Red.Wine.Relationship.Dependent)]
        public List<PurchaseOrderItemUpdateOptions> PurchaseOrderItems { get; set; }
    }
}