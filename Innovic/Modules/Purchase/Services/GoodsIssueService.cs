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
        public static GoodsIssue AddMetaData(this GoodsIssue goodsIssue, GoodsIssueFlow flow)
        {
            switch (flow)
            {
                case GoodsIssueFlow.Insert:
                    break;
                case GoodsIssueFlow.Update:
                    break;
                case GoodsIssueFlow.AddRemainingQuantity:
                    foreach (var item in goodsIssue.GoodsIssueItems)
                    {
                        item.MetaData.Add("RemainingQuantity", item.Quantity - item.GoodsReceiptItems.Sum(x => x.Quantity));
                    }
                    break;
                case GoodsIssueFlow.TotalRemainingQuantity:
                    int totalRemainingQuantity = goodsIssue.GoodsIssueItems.Sum(x => x.Quantity - x.GoodsReceiptItems.Sum(p => p.Quantity));

                    bool canCreateGoodsReceipt = totalRemainingQuantity > 0;

                    goodsIssue.MetaData.Add("TotalRemainingQuantity", totalRemainingQuantity);
                    goodsIssue.MetaData.Add("CanCreateGoodsReceipt", canCreateGoodsReceipt);

                    break;
            }

            return goodsIssue;
        }

        internal static bool IsInsertable(this GoodsIssue goodsIssue)
        {
            bool isInsertionAllowed = false;

            isInsertionAllowed = goodsIssue.GoodsIssueItems.Count > 0 && goodsIssue.GoodsIssueItems.TrueForAll(gii => gii.Material.Quantity >= gii.Quantity);

            return isInsertionAllowed;
        }

        internal static void SubtractMaterialQuantity(this GoodsIssue goodsIssue)
        {
            goodsIssue.GoodsIssueItems.ForEach(gii => gii.Material.Quantity -= gii.Quantity);
        }
    }
}