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
                case PurchaseOrderItemFlow.CalculateCost:
                    purchaseOrderItem.Cost = purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    break;
                case PurchaseOrderItemFlow.Update:
                    break;
                case PurchaseOrderItemFlow.ChangeStatusTo:
                    purchaseOrderItem.Status = PurchaseOrderItemStatus.Closed;
                    break;
            }

            return purchaseOrderItem;

        }
        public static int GetRemainingQuantity(this PurchaseOrderItem purchaseOrderItem)
        {
            int remainingQuantity = purchaseOrderItem.Quantity;

            foreach (var gii in purchaseOrderItem.GoodsIssueItems)
            {
                remainingQuantity -= gii.Quantity;
            }

            return remainingQuantity;
        }
    }
}