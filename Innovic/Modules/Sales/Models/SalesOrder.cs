using Innovic.Helpers;
using Red.Wine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Sales.Models
{
    public class SalesOrder : BaseModel
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
                return (AppConstants.SalesOrderAbbr + AppConstants.KeySeparator + KeyId.ToString(AppConstants.FixedDigits));
            }
        }
    }
}