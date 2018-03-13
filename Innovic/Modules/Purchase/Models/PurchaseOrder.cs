using Innovic.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static Innovic.Modules.Purchase.Models.PurchaseOrderStatus;

namespace Innovic.Modules.Purchase.Models
{
    public class PurchaseOrder : BaseModel
    {
        public PurchaseOrder()
        {
            PurchaseOrderItems = new List<PurchaseOrderItem>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public PurchaseOrderType Type { get; set; }

        public PurchaseOrderReference Reference { get; set; }

        public string SupplierId { get; set; }

        public string TermsAndConditions { get; set; }

        public string PurchaseRequestId { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public virtual PurchaseRequest PurchaseRequest { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.PurchaseOrderAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}