using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsIssueUpdateOptions
    {
        public GoodsIssueUpdateOptions()
        {
            GoodsIssueItems = new List<GoodsIssueItemUpdateOptions>();
        }

        public string Id { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public string SlipLevelNote { get; set; }

        [CopyTo(typeof(GoodsIssueItem), Red.Wine.Relationship.Dependent)]
        public virtual List<GoodsIssueItemUpdateOptions> GoodsIssueItems { get; set; }
        
    }
}