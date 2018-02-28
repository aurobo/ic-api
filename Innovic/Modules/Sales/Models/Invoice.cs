using Innovic.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Sales.Models
{
    public class Invoice : BaseModel
    {
        public Invoice()
        {
            InvoiceItems = new List<InvoiceItem>();
        }

        public virtual List<InvoiceItem> InvoiceItems { get; set; }
    }
}