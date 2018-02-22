using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.ProcessFlows;

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
            }

            return salesOrder;
        }
    }
}