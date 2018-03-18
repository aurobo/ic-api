﻿using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.ProcessFlows;
using System.Linq;

namespace Innovic.Modules.Sales.Services
{
    public static class SalesOrderService
    {
        internal static SalesOrder Process(this SalesOrder salesOrder, SalesOrderFlow flow)
        {
            switch (flow)
            {
                case SalesOrderFlow.Insert:
                    break;
                case SalesOrderFlow.Update:
                    break;
                case SalesOrderFlow.ImportExcel:
                    break;
                case SalesOrderFlow.PendingSalesOrderValue:
                    salesOrder.MetaData.Add("PendingSalesOrderValue", salesOrder.GetPendingSalesOrderValue());
                    break;
                case SalesOrderFlow.AddRemainingQuantity:
                    foreach (var item in salesOrder.SalesOrderItems)
                    {
                        item.MetaData.Add("RemainingQuantity", item.Quantity - item.InvoiceItems.Sum(x => x.Quantity));
                    }
                    break;
            }

            return salesOrder;
        }

        internal static double GetPendingSalesOrderValue(this SalesOrder salesOrder)
        {
            double invoicedValue = salesOrder.Invoices.SelectMany(s => s.InvoiceItems).Sum(i => i.Quantity * i.SalesOrderItem.UnitPrice);

            return salesOrder.SalesOrderItems.Sum(s => s.Value) - invoicedValue;
        }
    }
}