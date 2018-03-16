using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsReceiptItemInsertOptions
    {
        public string GoodsReceiptId { get; set; }

        public string Text { get; set; }

        public int Quantity { get; set; }

        public double Cost { get; set; }

        public string MaterialId { get; set; }

        public double UnitPrice { get; set; }

        [CopyTo(typeof(PurchaseOrderItem), Red.Wine.Relationship.Dependency, true)]
        public string PurchaseOrderItemId { get; set; }
    }
}