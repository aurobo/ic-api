using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsIssueItemUpdateOptions
    {
        public string Id { get; set; }

        [CopyTo(typeof(GoodsIssue),Red.Wine.Relationship.Dependency, true)]
        public int GoodsIssueId { get; set; }

        public string Text { get; set; }
        public string Size { get; set; }
        public string Notes { get; set; }
        public int Quantity { get; set; }
        public double UnitCost { get; set; }
        
    }
}