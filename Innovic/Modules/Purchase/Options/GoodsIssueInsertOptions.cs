using Innovic.Modules.Purchase.Models;
using Red.Wine;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsIssueInsertOptions
    {
        public GoodsIssueInsertOptions()
        {
            PurchaseOrders = new List<string>();
            GoodsIssueItems = new List<GoodsIssueItemInsertOptions>();
        }

        public DateTime Date { get; set; }

        [CopyTo(typeof(PurchaseOrder), Relationship.Dependency, true)]
        public List<string> PurchaseOrders { get; set; }

        [CopyTo(typeof(GoodsIssueItem), Relationship.Dependent)]
        public List<GoodsIssueItemInsertOptions> GoodsIssueItems { get; set; }
    }
}