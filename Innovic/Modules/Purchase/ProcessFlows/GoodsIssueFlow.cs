﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.ProcessFlows
{
    public enum GoodsIssueFlow
    {
        Insert,
        Update,
        ImportExcel,
        AddRemainingQuantity,
        PendingSalesOrderValue,
        SetDefaultStatus,
        PopulateItemsFromPurchaseOrder,
        CalculateTotalValue
    }
}