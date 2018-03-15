using System;

namespace Innovic.Modules.Purchase.Options
{
    public class PurchaseOrderItemUpdateOptions
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public int Quantity { get; set; }

        public double Rate { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}