using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class PurchaseRequestService
    {
        public static PurchaseRequest Process(this PurchaseRequest purchaseRequest, PurchaseRequestFlow flow)
        {
            switch (flow)
            {
                case PurchaseRequestFlow.Insert:
                    break;
                case PurchaseRequestFlow.Update:
                    break;
                case PurchaseRequestFlow.ChangeStatusTo:
                    var isOpenstatus = purchaseRequest.PurchaseRequestItems.Any(g => g.Status == PurchaseRequestItemStatus.Open);
                    purchaseRequest.Status = isOpenstatus ? PurchaseRequestStatus.Open : PurchaseRequestStatus.Closed;
                    break;
            }

            return purchaseRequest;
        }
    }
}