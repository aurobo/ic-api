using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Sales.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsReceiptUpdateOptions
    {
        public GoodsReceiptUpdateOptions()
        {
            GoodsReceiptItems = new List<GoodsReceiptItemUpdateOptions>();
        }

        public string Id { get; set; }

        public DateTime Date { get; set; }

        public string SlipLevelNote { get; set; }

        [CopyTo(typeof(GoodsReceiptItem), Red.Wine.Relationship.Dependent)]
        public virtual List<GoodsReceiptItemUpdateOptions> GoodsReceiptItems { get; set; }
    }
}