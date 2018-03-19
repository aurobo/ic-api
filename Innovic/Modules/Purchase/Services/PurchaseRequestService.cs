using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class PurchaseRequestService
    {
        public static PurchaseRequest Process(this PurchaseRequest purchaseRequest, PurchaseRequestFlow flow)
        {
            switch (flow)
            {
                case PurchaseRequestFlow.Insert:
                    break;
                case PurchaseRequestFlow.Update:
                    break;
                case PurchaseRequestFlow.AddRemainingQuantity:
                    foreach (var item in purchaseRequest.PurchaseRequestItems)
                    {
                        item.MetaData.Add("RemainingQuantity", item.Quantity - item.PurchaseOrderItems.Sum(p => p.Quantity));
                    }
                    break;
                case PurchaseRequestFlow.TotalRemainingQuantity:
                    int totalRemainingQuantity = purchaseRequest.PurchaseRequestItems.Sum(s => s.Quantity - s.PurchaseOrderItems.Sum(p => p.Quantity));

                    bool canCreatePurchaseOrder = (purchaseRequest.PurchaseRequestItems.Sum(s => s.Quantity) - totalRemainingQuantity) > 0;

                    purchaseRequest.MetaData.Add("TotalRemainingQuantity", totalRemainingQuantity);
                    purchaseRequest.MetaData.Add("CanCreatePurchaseOrder", canCreatePurchaseOrder);

                    break;
            }

            return purchaseRequest;
        }


    }
}