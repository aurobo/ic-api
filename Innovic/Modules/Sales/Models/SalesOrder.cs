using Innovic.App;
using Red.Wine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Sales.Models
{
    public class SalesOrder : WineModel
    {
        public SalesOrder()
        {
            SalesOrderItems = new List<SalesOrderItem>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime ExpirationDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime OrderDate { get; set; }

        public string PaymentTerms { get; set; }

        public string CustomerReference { get; set; }

        public string CustomerId { get; set; }

        public SalesOrderStatus Status { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual List<SalesOrderItem> SalesOrderItems { get; set; }

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