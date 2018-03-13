using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class PurchaseOrderItemService
    {
        public static PurchaseOrderItem Process(this PurchaseOrderItem purchaseOrderItem, PurchaseOrderItemFlow flow)
        {
            switch (flow)
            {
                case PurchaseOrderItemFlow.Insert:
                    purchaseOrderItem.Cost = purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    break;
                case PurchaseOrderItemFlow.Update:
                    break;
            }

            return purchaseOrderItem;
        }
    }
}