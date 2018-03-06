using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.ProcessFlows;
using System.Linq;

namespace Innovic.Modules.Sales.Services
{
    public static class SalesOrderService
    {
        public static SalesOrder Process(this SalesOrder salesOrder, SalesOrderFlow flow)
        {
            switch(flow)
            {
                case SalesOrderFlow.Insert:
                    break;
                case SalesOrderFlow.Update:
                    break;
                case SalesOrderFlow.ImportExcel:
                    break;
                case SalesOrderFlow.AddRemainingQuantity:
                    foreach(var item in salesOrder.SalesOrderItems)
                    {
                        item.MetaData.Add("RemainingQuantity", item.Quantity - item.InvoiceItems.Sum(x => x.Quantity));
                    }
                    break;
            }

            return salesOrder;
        }
    }
}