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
                    foreach (var item in goodsIssue.GoodsIssueItems)
                    {
                        item.Process(GoodsIssueItemFlow.CalCulateCost);
                    }
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
            }
            return goodsIssue;
        }
    }
}