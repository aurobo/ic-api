using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class GoodsIssueItemService
    {
        public static GoodsIssueItem Process(this GoodsIssueItem goodsIssueItem, GoodsIssueItemFlow flow)
        {
            switch (flow)
            {
                case GoodsIssueItemFlow.Insert:
                    break;

                case GoodsIssueItemFlow.Update:
                    break;

                case GoodsIssueItemFlow.CalCulateCost:
                   goodsIssueItem.Cost = goodsIssueItem.UnitPrice * goodsIssueItem.Quantity;
                    break;
            }

            return goodsIssueItem;
        }
    }
}