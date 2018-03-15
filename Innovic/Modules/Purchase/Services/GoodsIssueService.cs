using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class GoodsIssueService
    {
        public static GoodsIssue Process(this GoodsIssue goodsIssue, GoodsIssueFlow flow)
        {
            switch (flow)
            {
                case GoodsIssueFlow.Insert:

                    break;

                case GoodsIssueFlow.Update:

                    break;

                case GoodsIssueFlow.SetDefaultStatus:

                    foreach (var item in goodsIssue.GoodsIssueItems)
                    {
                        GoodsIssueItemService.Process(item, GoodsIssueItemFlow.ChangeStatusTo);
                    }
                    goodsIssue.Status = GoodsIssueStatus.Open;

                    break;


                case GoodsIssueFlow.CalculateTotalValue:
                     

                    foreach (var item in goodsIssue.GoodsIssueItems)
                    {
                        GoodsIssueItemService.Process(item, GoodsIssueItemFlow.CalCulateCost);
                        goodsIssue.TotalValue += item.Cost;
                    }
                    break;
                    
                case GoodsIssueFlow.PopulateItemsFromPurchaseOrder:

                    bool mergeDuplicates = false;

                    AppMapper mapper = new AppMapper();

                    var openPurchaseOrders = goodsIssue.PurchaseOrders.Where(po => po.Status == PurchaseOrderStatus.Open);

                    foreach (var purchaseOrder in openPurchaseOrders)
                    {
                        var openPurchaseOrderItems = purchaseOrder.PurchaseOrderItems.Where(poi => poi.Status == PurchaseOrderItemStatus.Open);

                        foreach (var purchaseOrderItem in openPurchaseOrderItems)
                        {
                            if (mergeDuplicates)
                            {
                                var existingItem  =  goodsIssue.GoodsIssueItems.Where(g => g.MaterialId == purchaseOrderItem.MaterialId).FirstOrDefault();

                                if (existingItem != null)
                                {
                                    existingItem.Quantity += purchaseOrderItem.Quantity;
                                    existingItem.PurchaseOrderItems.Add(purchaseOrderItem);
                                }
                                else
                                {
                                    var newItem = mapper.Convert<PurchaseOrderItem, GoodsIssueItem>(purchaseOrderItem);
                                    newItem.PurchaseOrderItems.Add(purchaseOrderItem);
                                   goodsIssue.GoodsIssueItems.Add(newItem);
                                }
                            }
                            else
                            {
                                var giItem = mapper.Convert<PurchaseOrderItem, GoodsIssueItem>(purchaseOrderItem);
                                giItem.Quantity = PurchaseOrderItemService.GetRemainingQuantity(purchaseOrderItem);
                                giItem.PurchaseOrderItems.Add(purchaseOrderItem);
                                goodsIssue.GoodsIssueItems.Add(giItem);
                            }

                           PurchaseOrderItemService.Process(purchaseOrderItem,PurchaseOrderItemFlow.ChangeStatusTo);
                        }
                        PurchaseOrderService.Process(purchaseOrder, PurchaseOrderFlow.ChangeStatusTo,true);
                    }
                    
                    break;
            }
            return goodsIssue;
        }
    }
}