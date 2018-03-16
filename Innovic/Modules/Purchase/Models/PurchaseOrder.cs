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
            PurchaseRequests = new List<PurchaseRequest>();
            PurchaseOrderItems = new List<PurchaseOrderItem>();
            GoodsReceipts = new List<GoodsReceipt>();
            GoodsIssues = new List<GoodsIssue>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public PurchaseOrderType Type { get; set; }

        public PurchaseOrderReference Reference { get; set; }

        public string SupplierId { get; set; }

        public string TermsAndConditions { get; set; }  

        public PurchaseOrderStatus Status { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public virtual List<PurchaseRequest> PurchaseRequests { get; set; }

        public virtual List<GoodsReceipt> GoodsReceipts { get; set; }

        public virtual List<GoodsIssue> GoodsIssues { get; set; }

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