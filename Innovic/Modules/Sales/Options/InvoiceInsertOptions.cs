using Innovic.App;
using Innovic.Modules.Sales.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Sales.Options
{
    public class InvoiceInsertOptions : BaseOptionsModel
    {
        public InvoiceInsertOptions()
        {
            InvoiceItems = new List<InvoiceItemInsertOptions>();
        }

        [CopyTo(typeof(SalesOrder), Red.Wine.Relationship.Dependency, true)]
        public string SalesOrderId { get; set; }
        [CopyTo(typeof(InvoiceItem), Red.Wine.Relationship.Dependent)]
        public List<InvoiceItemInsertOptions> InvoiceItems { get; set; }
    }
}