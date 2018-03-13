using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public enum PurchaseOrderType
    {
        Standard = 1,
        SubContract = 2,
        Service = 3
    }

    public enum PurchaseOrderReference
    {
        None = 1,
        PurchaseRequest = 2
    }

    public enum PurchaseOrderStatus
    {
        Open = 1,
        Closed = 2
    }
}