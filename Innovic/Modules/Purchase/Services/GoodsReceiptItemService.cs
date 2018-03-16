using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class GoodsReceiptItemService
    {
        public static GoodsReceiptItem Process(this GoodsReceiptItem goodsReceiptItem, GoodsReceiptItemFlow flow)
        {
            switch (flow)
            {
                case GoodsReceiptItemFlow.Insert:
                    break;
                case GoodsReceiptItemFlow.Update:
                    break;
            }

            return goodsReceiptItem;
        }
    }
}