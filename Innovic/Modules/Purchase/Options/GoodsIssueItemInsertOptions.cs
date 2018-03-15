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

        public string Id { get; set; }

        public string Text { get; set; }

        public string Size { get; set; }

        public string Notes { get; set; }

        public int Quantity { get; set; }

        
        [CopyTo(typeof(Material), Red.Wine.Relationship.Dependent, true)]
        public int MaterialId { get; set; }

        [CopyTo(typeof(GoodsIssue), Red.Wine.Relationship.Dependent, true)]
        public int GoodsIssueId { get; set; }
        
        [CopyTo(typeof(PurchaseOrderItem), Red.Wine.Relationship.Dependency, true)]
        public string PurchaseOrderItemId { get; set; }
        
    }
}