﻿using Innovic.Modules.Sales.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Sales.Options
{
    public class InvoiceItemInsertOptions
    {
        [CopyTo(typeof(SalesOrderItem), Red.Wine.Relationship.Dependency, true)]
        public string SalesOrderItemId { get; set; }
        public int Quantity { get; set; }
    }
}