using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Sales.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsIssueInsertOptions : BaseOptionsModel
    {
        public GoodsIssueInsertOptions()
        {
            PurchaseOrders = new List<string>();
            GoodsIssueItems = new List<GoodsIssueItemInsertOptions>();
        }

        public DateTime Date { get; set; }
        
        [CopyTo(typeof(PurchaseOrder), Red.Wine.Relationship.Dependency, true)]
        public List<string> PurchaseOrders { get; set; }


        [CopyTo(typeof(GoodsIssueItem), Red.Wine.Relationship.Dependent)]
        public List<GoodsIssueItemInsertOptions> GoodsIssueItems { get; set; }
    }
}