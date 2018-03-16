using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.ProcessFlows
{
    public enum GoodsIssueItemFlow
    {
            Insert,
            Update,
            ImportExcel,
            AddRemainingQuantity,
            PendingSalesOrderValue,
            ChangeStatusTo,
            CalCulateCost
    }
}