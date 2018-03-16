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
        public static PurchaseOrderItem Process(this PurchaseOrderItem purchaseOrderItem, PurchaseOrderItemFlow flow, PurchaseOrderItemStatus status = PurchaseOrderItemStatus.Closed)
        {
            switch (flow)
            {
                case PurchaseOrderItemFlow.Insert:
                    purchaseOrderItem.Cost = purchaseOrderItem.Quantity * purchaseOrderItem.UnitPrice;
                    break;
                case PurchaseOrderItemFlow.Update:
                    break;
                case PurchaseOrderItemFlow.ChangeStatusTo:
                    purchaseOrderItem.Status = status;
                    break;
                case PurchaseOrderItemFlow.UpdatePRIStatus:
                    foreach (var purchaseRequestItem in purchaseOrderItem.PurchaseRequestItems)
                    {
                        if (purchaseOrderItem.Quantity == purchaseRequestItem.Quantity)
                        {
                            PurchaseRequestItemService.Process(purchaseRequestItem, PurchaseRequestItemFlow.ChangeStatusTo);
                            //purchaseRequestItems.ChangeStatusTo(PurchaseRequestItemStatus.Closed);
                        }
                        else
                        {
                            PurchaseRequestItemService.Process(purchaseRequestItem, PurchaseRequestItemFlow.ChangeStatusTo);
                            //purchaseRequestItems.ChangeStatusTo(PurchaseRequestItemStatus.Open);
                        }
                        purchaseRequestItem.PurchaseRequest.Process(PurchaseRequestFlow.ChangeStatusTo);
                        //purchaseRequestItems.PurchaseRequest.ChangeStatusTo(PurchaseRequestStatus.Closed);
                    }
                    break;
            }

            return purchaseOrderItem;
        }

        internal static int GetRemainingReceiveQuantity(this PurchaseOrderItem purchaseOrderItem)
        {
            int remainingQuantity = purchaseOrderItem.Quantity;

            foreach (var gri in purchaseOrderItem.GoodsReceiptItems)
            {
                remainingQuantity -= gri.Quantity;
            }

            return remainingQuantity;
        }
    }
}