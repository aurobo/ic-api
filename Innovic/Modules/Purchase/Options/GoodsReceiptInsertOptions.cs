using Innovic.Modules.Purchase.Models;
using Red.Wine;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsReceiptInsertOptions
    {
        public GoodsReceiptInsertOptions()
        {
            PurchaseOrders = new List<string>();
            GoodsReceiptItems = new List<GoodsReceiptItemInsertOptions>();
        }

        public DateTime ExpectedDate { get; set; }

        [CopyTo(typeof(PurchaseOrder), Relationship.Dependency, true)]
        public List<string> PurchaseOrders { get; set; }

        [CopyTo(typeof(GoodsReceiptItem), Relationship.Dependent)]
        public List<GoodsReceiptItemInsertOptions> GoodsReceiptItems { get; set; }
    }
}