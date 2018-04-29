using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Sales.Services
{
    public static class InvoiceService
    {
        internal static Invoice AddMetaData(this Invoice invoice, InvoiceFlow flow)
        {
            switch (flow)
            {
                case InvoiceFlow.Update:
                    // Do what needs to be done
                    break;
                case InvoiceFlow.Insert:
                    // Do what needs to be done
                    break;
            }

            return invoice;
        }

        internal static bool IsInsertable(this Invoice invoice)
        {
            bool hasItems = invoice.InvoiceItems.Count > 0;
            double pendingSalesOrderValue = invoice.SalesOrder.GetPendingSalesOrderValue();
            var hasSufficientQuantity = invoice.InvoiceItems.TrueForAll(ii => ii.Material.Quantity >= ii.Quantity);

            if (hasItems && pendingSalesOrderValue > 0 && hasSufficientQuantity)
            {
                return true;
            }

            return false;
        }

        internal static void SubtractMaterialQuantity(this Invoice invoice)
        {
            invoice.InvoiceItems.ForEach(ii => ii.Material.Quantity -= ii.Quantity);
        }
    }
}