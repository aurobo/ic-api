using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class GoodsReceiptItem : BaseModel
    {
        public string GoodsReceiptId { get; set; }

        public string Text { get; set; }

        public int Quantity { get; set; }

        public string Notes { get; set; }

        public double Cost { get; set; }

        public string MaterialId { get; set; }

        public double UnitPrice { get; set; }

        public string PurchaseOrderItemId { get; set; }

        public string GoodsIssueItemId { get; set; }

        public virtual Material Material { get; set; }

        public virtual GoodsIssueItem GoodsIssueItem { get; set; }

        public virtual GoodsReceipt GoodsReceipt { get; set; }

        public virtual PurchaseOrderItem PurchaseOrderItem { get; set; }
    }
}