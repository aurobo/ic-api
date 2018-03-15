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
        public GoodsReceiptItem()
        {
            PurchaseOrderItems = new List<PurchaseOrderItem>();
        }

        public string GoodsReceiptId { get; set; }

        public string Text { get; set; }

        public int Quantity { get; set; }

        public string Note { get; set; }

        public double Cost { get; set; }

        public string MaterialId { get; set; }

        public double UnitCost { get; set; }

        public GoodsReceiptItemStatus Status { get; set; }

        public virtual Material Material { get; set; }

        public virtual GoodsReceipt GoodsReceipt { get; set; }

        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }
}