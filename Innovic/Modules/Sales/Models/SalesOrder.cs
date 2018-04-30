using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Sales.Models
{
    public class SalesOrder : BaseModel
    {
        public SalesOrder()
        {
            SalesOrderItems = new List<SalesOrderItem>();
            Invoices = new List<Invoice>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime ExpirationDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime OrderDate { get; set; }

        public string PaymentTerms { get; set; }

        public string CustomerReference { get; set; }

        public string CustomerId { get; set; }

        public string Remarks { get; set; }

        public SalesOrderStatus Status { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual List<SalesOrderItem> SalesOrderItems { get; set; }

        public virtual List<Invoice> Invoices { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.SalesOrderAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}