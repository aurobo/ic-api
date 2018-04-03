using Innovic.App;
using Innovic.Modules.Purchase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Models
{
    public class Link : BaseModel
    {
        public Link()
        {
            PurchaseRequests = new List<PurchaseRequest>();
            GoodsIssues = new List<GoodsIssue>();
        }

        public string ReferenceId { get; set; }
        public string ReferenceName { get; set; }
        public string Type { get; set; }

        public virtual List<PurchaseRequest> PurchaseRequests { get; set; }
        public virtual List<GoodsIssue> GoodsIssues { get; set; }
    }
}