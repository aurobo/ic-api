using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Sales.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsReceiptInsertOptions
    {
        public GoodsReceiptInsertOptions()
        {
            PurchaseOrders = new List<string>();
            GoodsReceiptItems = new List<GoodsReceiptItemInsertOptions>();
        }

        public string Reference { get; set; }

        public DateTime Date { get; set; }

        [CopyTo(typeof(Customer), Red.Wine.Relationship.Dependency, true)]
        public string VendorId { get; set; }

        [CopyTo(typeof(PurchaseOrder), Red.Wine.Relationship.Dependency, true)]
        public List<string> PurchaseOrders { get; set; }

        [CopyTo(typeof(GoodsReceiptItem), Red.Wine.Relationship.Dependent)]
        public virtual List<GoodsReceiptItemInsertOptions> GoodsReceiptItems { get; set; }
    }
}