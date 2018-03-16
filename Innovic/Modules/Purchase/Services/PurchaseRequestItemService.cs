using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class PurchaseRequestItemService
    {
        public static PurchaseRequestItem Process(this PurchaseRequestItem purchaseRequestItem, PurchaseRequestItemFlow flow)
        {
            switch (flow)
            {
                case PurchaseRequestItemFlow.Insert:
                    break;
                case PurchaseRequestItemFlow.Update:
                    break;
                case PurchaseRequestItemFlow.ChangeStatusTo:
                    purchaseRequestItem.Status = PurchaseRequestItemStatus.Closed;
                    break;
            }

            return purchaseRequestItem;
        }

        public static int GetRemainingQuantity(this PurchaseRequestItem purchaseRequestItem)
        {
            int remainingQuantity = purchaseRequestItem.Quantity;

            foreach (var poi in purchaseRequestItem.PurchaseOrderItems)
            {
                remainingQuantity -= poi.Quantity;
            }

            return remainingQuantity;
        }
    }
}