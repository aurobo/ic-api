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
        public static GoodsReceipt Process(this GoodsReceipt goodsReceipt, GoodsReceiptFlow flow)
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
    }
}