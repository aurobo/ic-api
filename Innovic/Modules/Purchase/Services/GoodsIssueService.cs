using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;

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
            }
            return goodsIssue;
        }
    }
}