﻿using Innovic.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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

        public string SalesOrderId { get; set; }

        public virtual SalesOrder SalesOrder { get; set; }
        public virtual List<InvoiceItem> InvoiceItems { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.InvoiceAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}