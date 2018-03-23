using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class PurchaseOrderService
    {
        public static PurchaseOrder Process(this PurchaseOrder purchaseOrder, PurchaseOrderFlow flow)
        {
            switch (flow)
            {
                case PurchaseOrderFlow.CalculateItemCost:
                    break;
                case PurchaseOrderFlow.Update:
                    break;
            }

            return purchaseOrder;
        }

        internal static bool IsInsertionAllowed(this PurchaseOrder purchaseOrder)
        {
            bool isInsertionAllowed = false;

            isInsertionAllowed = purchaseOrder.PurchaseRequests.Count > 0 && purchaseOrder.PurchaseOrderItems.Count > 0;

            return isInsertionAllowed;
        }
    }
}