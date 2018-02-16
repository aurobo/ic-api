using Innovic.Models;
using Innovic.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Services
{
    public class CustomerService
    {
        BaseService<Customer> _service = new BaseService<Customer>();

        public bool Exists(string name)
        {
            return _service.Exists(c => c.Name == name);
        }

        public Customer QuickCreate(string name)
        {
            var customer = new Customer { Name = name };

            _service.QuickCreateAndSave(customer);

            return customer;
        }

        public Customer Find(string name)
        {
            return _service.Find(c => c.Name == name);
        }

        //For now there is no need to create any process method 
        //public Customer Process(Customer customer) { }
    }
}