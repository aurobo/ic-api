using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class GoodsReceiptService
    {
        public static GoodsReceipt Process(this GoodsReceipt goodsReceipt, GoodsReceiptFlow flow, bool mergeDuplicates = true)
        {
            switch (flow)
            {
                case GoodsReceiptFlow.Insert:
                    break;
                case GoodsReceiptFlow.Update:
                    break;
                case GoodsReceiptFlow.PopulateFromPurchaseOrder:

                    AppMapper mapper = new AppMapper();

                    var openPurchaseOrders = goodsReceipt.PurchaseOrders.Where(po => po.Status == PurchaseOrderStatus.Open);

                    foreach (var purchaseOrder in openPurchaseOrders)
                    {
                        var openPurchaseOrderItems = purchaseOrder.PurchaseOrderItems.Where(poi => poi.Status == PurchaseOrderItemStatus.Open);

                        foreach (var purchaseOrderItem in openPurchaseOrderItems)
                        {
                            if (mergeDuplicates)
                            {
                                var existingItem = goodsReceipt.GoodsReceiptItems.Where(g => g.MaterialId == purchaseOrderItem.MaterialId).FirstOrDefault();

                                if (existingItem != null)
                                {
                                    existingItem.Quantity += purchaseOrderItem.Quantity;
                                    existingItem.PurchaseOrderItems.Add(purchaseOrderItem);
                                }
                                else
                                {
                                    var newItem = mapper.Convert<PurchaseOrderItem, GoodsReceiptItem>(purchaseOrderItem);
                                    newItem.PurchaseOrderItems.Add(purchaseOrderItem);
                                    goodsReceipt.GoodsReceiptItems.Add(newItem);
                                }
                            }
                            else
                            {
                                var grItem = mapper.Convert<PurchaseOrderItem, GoodsReceiptItem>(purchaseOrderItem);
                                grItem.Quantity = purchaseOrderItem.GetRemainingReceiveQuantity();
                                grItem.PurchaseOrderItems.Add(purchaseOrderItem);
                                goodsReceipt.GoodsReceiptItems.Add(grItem);
                            }
                        }
                    }

                    break;
                case GoodsReceiptFlow.SetDefaultStatus:
                    foreach (var item in goodsReceipt.GoodsReceiptItems)
                    {
                        item.Process(GoodsReceiptItemFlow.SetDefaultStatus);
                    }

                    goodsReceipt.Status = GoodsReceiptStatus.Open;
                    break;
            }

            return goodsReceipt;
        }
    }
}