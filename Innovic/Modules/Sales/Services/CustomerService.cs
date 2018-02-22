using System;
using System.Linq;
using Innovic.App;
using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.ProcessFlows;

namespace Innovic.Modules.Sales.Services
{
    public static class CustomerService
    {
        public static Customer Process(this Customer customer, CustomerFlow flow)
        {
            switch(flow)
            {
                case CustomerFlow.Update:
                    // Do what needs to be done
                    break;
                case CustomerFlow.Insert:
                    // Do what needs to be done
                    break;
            }

            return customer;
        }
    }
}