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
        private InnovicContext _db = new InnovicContext();

        public bool Exists(string name)
        {
            return _db.Customers.Count(c => c.Name == name) > 0;
        }

        public Customer QuickCreate(string name)
        {
            var customer = new Customer { Name = name };

            _db.Customers.Add(customer);
            _db.SaveChanges();

            return customer;
        }

        public Customer Find(string name)
        {
            return _db.Customers.Where(c => c.Name == name).SingleOrDefault();
        }
    }
}