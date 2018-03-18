using Innovic.Modules.Purchase.Models;
using Red.Wine;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsReceiptItemInsertOptions
    {
        public GoodsReceiptItemInsertOptions()
        {
            PurchaseOrderItems = new List<string>();
        }

        public string GoodsReceiptId { get; set; }
        public int Quantity { get; set; }
        public string MaterialId { get; set; }
        public double UnitPrice { get; set; }

        [CopyTo(typeof(PurchaseOrderItem), Relationship.Dependency, true)]
        public List<string> PurchaseOrderItems { get; set; }
    }
}