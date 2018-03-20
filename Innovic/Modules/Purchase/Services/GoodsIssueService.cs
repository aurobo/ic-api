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
                case GoodsIssueFlow.AddRemainingQuantity:
                    foreach (var item in goodsIssue.GoodsIssueItems)
                    {
                        item.MetaData.Add("RemainingQuantity", item.Quantity - item.GoodsReceiptItems.Sum(x => x.Quantity));
                    }
                    break;
            }

            return goodsIssue;
        }

        public static List<GoodsIssue> Process(this List<GoodsIssue> goodsIssues, GoodsIssueItemFlow flow)
        {
            switch (flow)
            {
                case GoodsIssueItemFlow.AddRemainingQuantity:
                    foreach (var goodsIssue in goodsIssues)
                    {
                        if(goodsIssue.GoodsIssueItems.Count > 0)
                        {
                            foreach(var goodsIssueItem in goodsIssue.GoodsIssueItems)
                            {
                                goodsIssueItem.MetaData.Add("RemainingQuantity", GetRemainingQuantity(goodsIssueItem));
                            }
                        }
                    }

                    break;

                case GoodsIssueItemFlow.CanCreateGoodsReceipt:
                    foreach (var goodsIssue in goodsIssues)
                    {
                        int TotalQuantity = GetGoodsIssueTotalQuantity(goodsIssue);
                        if(TotalQuantity > 0)
                        {
                            goodsIssue.MetaData.Add("CanCreateGoodsReceipt", true);
                        }
                        else
                        {
                            goodsIssue.MetaData.Add("CanCreateGoodsReceipt", false);
                        }
                    }

                    break;

            }

            return goodsIssues;
        }


        public static int GetRemainingQuantity(GoodsIssueItem item)
        {
            return item.Quantity - item.GoodsReceiptItems.Sum(x => x.Quantity);
        }


        internal static int GetGoodsIssueTotalQuantity(this GoodsIssue goodissue)
        {
            return goodissue.GoodsIssueItems.Sum(i => i.Quantity);
        }
    }
}