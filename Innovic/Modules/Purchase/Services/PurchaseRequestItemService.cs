using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class PurchaseRequestItemService
    {
        public static PurchaseRequestItem Process(this PurchaseRequestItem purchaseRequestItem, PurchaseRequestItemFlow flow)
        {
            switch (flow)
            {
                case PurchaseRequestItemFlow.Insert:
                    break;
                case PurchaseRequestItemFlow.Update:
                    break;
            }

            return purchaseRequestItem;
        }
    }
}