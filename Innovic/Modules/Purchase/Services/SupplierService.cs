using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Purchase.ProcessFlows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Services
{
    public static class SupplierService
    {
        public static Supplier Process(this Supplier supplier, SupplierFlow flow)
        {
            switch (flow)
            {
                case SupplierFlow.Insert:
                    break;
                case SupplierFlow.Update:
                    break;
            }

            return supplier;
        }
    }
}