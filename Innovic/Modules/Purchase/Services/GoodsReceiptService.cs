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
        public static GoodsReceipt AddMetaData(this GoodsReceipt goodsReceipt, GoodsReceiptFlow flow)
        {
            switch (flow)
            {
                case GoodsReceiptFlow.Insert:
                    break;
                case GoodsReceiptFlow.Update:
                    break;
            }

            return goodsReceipt;
        }

        internal static bool IsInsertable(this GoodsReceipt goodsReceipt)
        {
            bool isInsertionAllowed = false;

            isInsertionAllowed = (goodsReceipt.PurchaseOrders.Count > 0 || goodsReceipt.GoodsIssues.Count > 0) && goodsReceipt.GoodsReceiptItems.Count > 0;

            return isInsertionAllowed;
        }

        internal static void AddMaterialQuantity(this GoodsReceipt goodsReceipt)
        {
            goodsReceipt.GoodsReceiptItems.ForEach(gri => gri.Material.Quantity += gri.Quantity);
        }
    }
}