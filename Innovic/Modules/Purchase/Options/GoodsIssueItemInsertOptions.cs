using Innovic.Modules.Master.Models;
using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsIssueItemInsertOptions
    {
        
        public string Text { get; set; }
        
        public int Quantity { get; set; }

        public double Cost { get; set; }

        [CopyTo(typeof(Material), Red.Wine.Relationship.Dependency, true)]
        public string MaterialId { get; set; }

        [CopyTo(typeof(GoodsIssue), Red.Wine.Relationship.Dependency, true)]
        public string GoodsIssueId { get; set; }
        
        [CopyTo(typeof(PurchaseOrderItem), Red.Wine.Relationship.Dependency, true)]
        public string PurchaseOrderItemId { get; set; }
        
    }
}