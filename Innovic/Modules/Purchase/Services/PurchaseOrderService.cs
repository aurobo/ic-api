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
                    foreach (var purchaseOrderItem in purchaseOrder.PurchaseOrderItems)
                    {
                        purchaseOrderItem.Process(PurchaseOrderItemFlow.Insert);
                    }
                    break;
                case PurchaseOrderFlow.Update:
                    break;
                case PurchaseOrderFlow.PopulateItemsFromPurchaseRequests:

                    AppMapper mapper = new AppMapper();
                    bool mergeDuplicates = false;

                    var openPurchaseRequests = purchaseOrder.PurchaseRequests.Where(pr => pr.Status == PurchaseRequestStatus.Open);

                    foreach (var purchaseRequest in openPurchaseRequests)
                    {
                        var openPurchaseRequestItems = purchaseRequest.PurchaseRequestItems.Where(pri => pri.Status == PurchaseRequestItemStatus.Open);

                        foreach (var item in openPurchaseRequestItems)
                        {
                            if (mergeDuplicates)
                            {
                                var existingItem = purchaseOrder.PurchaseOrderItems.Where(i => i.MaterialId == item.MaterialId).FirstOrDefault();

                                if (existingItem != null)
                                {
                                    existingItem.Quantity += item.Quantity;
                                    existingItem.PurchaseRequestItems.Add(item);
                                }
                                else
                                {
                                    var newItem = mapper.Convert<PurchaseRequestItem, PurchaseOrderItem>(item);
                                    newItem.PurchaseRequestItems.Add(item);
                                    purchaseOrder.PurchaseOrderItems.Add(newItem);
                                }
                            }
                            else
                            {
                                var poItem = mapper.Convert<PurchaseRequestItem, PurchaseOrderItem>(item);
                                poItem.Quantity = item.GetRemainingQuantity();
                                poItem.DeliveryDate = item.ExpectedDate;
                                poItem.PurchaseRequestItems.Add(item);
                                purchaseOrder.PurchaseOrderItems.Add(poItem);
                            }
                        }
                    }

                    break;
                case PurchaseOrderFlow.ChangeStatusTo:
                    var isOpenstatus = purchaseOrder.PurchaseOrderItems.Any(g => g.Status == PurchaseOrderItemStatus.Open);
                    purchaseOrder.Status = isOpenstatus ? PurchaseOrderStatus.Open : PurchaseOrderStatus.Closed;
                    break;
            }

            return purchaseOrder;
        }


    }
}