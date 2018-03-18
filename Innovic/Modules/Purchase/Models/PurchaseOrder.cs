using Innovic.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Purchase.Models
{
    public class PurchaseOrder : BaseModel
    {
        public PurchaseOrder()
        {
            PurchaseRequests = new List<PurchaseRequest>();
            PurchaseOrderItems = new List<PurchaseOrderItem>();
            GoodsReceipts = new List<GoodsReceipt>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }
        public string SupplierId { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public virtual List<PurchaseRequest> PurchaseRequests { get; set; }
        public virtual List<GoodsReceipt> GoodsReceipts { get; set; }

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