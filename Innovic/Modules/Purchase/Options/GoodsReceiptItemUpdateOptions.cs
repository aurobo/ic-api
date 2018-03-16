using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class GoodsReceiptItemUpdateOptions
    {
        public string Text { get; set; }

        public int Quantity { get; set; }

        public string Note { get; set; }

        public double Cost { get; set; }

        public double UnitCost { get; set; }
    }
}