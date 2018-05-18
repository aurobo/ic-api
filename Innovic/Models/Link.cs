using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Sales.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Models
{
    public class Link : BaseModel
    {
        public string PurchaseRequestId { get; set; }
        public string GoodsIssueId { get; set; }
        public string SalesOrderId { get; set; }
        public string PurchaseOrderId { get; set; }

        public virtual PurchaseRequest PurchaseRequest { get; set; }
        public virtual GoodsIssue GoodsIssue { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
    }
}