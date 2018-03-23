using Innovic.Modules.Master.Models;
using Innovic.Modules.Purchase.Models;
using Red.Wine;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsIssueItemInsertOptions
    {
        public GoodsIssueItemInsertOptions()
        {
            PurchaseOrderItems = new List<string>();
        }

        public string GoodsIssueId { get; set; }
        public int Quantity { get; set; }
        [CopyTo(typeof(Material), Relationship.Dependency, true)]
        public string MaterialId { get; set; }
        public DateTime Date { get; set; }

        [CopyTo(typeof(PurchaseOrderItem), Relationship.Dependency, true)]
        public List<string> PurchaseOrderItems { get; set; }
    }
}