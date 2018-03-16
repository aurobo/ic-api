using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public enum GoodsReceiptStatus
    {
        Open = 1,
        Closed = 2
    }

    public enum GoodsReceiptReference
    {
        GoodsIssue = 1,
        PurchaseOrder = 2
    }
}