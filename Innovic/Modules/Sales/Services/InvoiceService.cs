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
        public static Invoice Process(this Invoice invoice, InvoiceFlow flow)
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
    }
}