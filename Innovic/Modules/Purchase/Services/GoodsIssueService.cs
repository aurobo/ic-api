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

            }

            return goodsIssue;
        }

        internal static bool IsInsertionAllowed(this GoodsIssue goodsIssue)
        {
            bool isInsertionAllowed = false;

            isInsertionAllowed = goodsIssue.PurchaseOrders.Count > 0 && goodsIssue.GoodsIssueItems.Count > 0 && goodsIssue.GoodsIssueItems.TrueForAll(gii => gii.Material.Quantity >= gii.Quantity);

            return isInsertionAllowed;
        }

        internal static void SubtractMaterialQuantity(this GoodsIssue goodsIssue)
        {
            goodsIssue.GoodsIssueItems.ForEach(gii => gii.Material.Quantity -= gii.Quantity);
        }
    }
}