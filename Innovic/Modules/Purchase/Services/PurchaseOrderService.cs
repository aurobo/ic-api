using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class PurchaseOrderService
    {
        public static PurchaseOrder Process(this PurchaseOrder purchaseOrder, PurchaseOrderFlow flow)
        {
            switch (flow)
            {
                case PurchaseOrderFlow.CalculateItemCost:
                    break;
                case PurchaseOrderFlow.Update:
                    break;
                case PurchaseOrderFlow.AddRemainingQuantity:
                    foreach (var item in purchaseOrder.PurchaseOrderItems)
                    {
                        item.MetaData.Add("RemainingQuantity", item.Quantity - item.GoodsReceiptItems.Sum(p => p.Quantity));
                    }
                    break;
                case PurchaseOrderFlow.TotalRemainingQuantity:
                    int totalRemainingQuantity = purchaseOrder.PurchaseOrderItems.Sum(s => s.Quantity - s.GoodsReceiptItems.Sum(p => p.Quantity));

                    bool canCreateGoodsReceipt = totalRemainingQuantity > 0;

                    purchaseOrder.MetaData.Add("TotalRemainingQuantity", totalRemainingQuantity);
                    purchaseOrder.MetaData.Add("CanCreateGoodsReceipt", canCreateGoodsReceipt);

                    break;
            }

            return purchaseOrder;
        }

        internal static bool IsInsertionAllowed(this PurchaseOrder purchaseOrder)
        {
            bool isInsertionAllowed = false;

            isInsertionAllowed = purchaseOrder.PurchaseRequests.Count > 0 && purchaseOrder.PurchaseOrderItems.Count > 0;

            return isInsertionAllowed;
        }
    }
}