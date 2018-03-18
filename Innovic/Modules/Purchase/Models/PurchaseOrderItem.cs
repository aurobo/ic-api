using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class PurchaseOrderItem : BaseModel
    {
        public PurchaseOrderItem()
        {
            GoodsReceiptItems = new List<GoodsReceiptItem>();
            GoodsIssueItems = new List<GoodsIssueItem>();
        }

        public string MaterialId { get; set; }

        public string Text { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime RequestByDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime DeliveryDate { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public double Cost { get; set; }

        public string PurchaseOrderId { get; set; }

        public string PurchaseRequestItemId { get; set; }

        public virtual Material Material { get; set; }

        public virtual PurchaseOrder PurchaseOrder { get; set; }

        public virtual PurchaseRequestItem PurchaseRequestItem { get; set; }
        public virtual List<GoodsReceiptItem> GoodsReceiptItems { get; set; }
        public virtual List<GoodsIssueItem> GoodsIssueItems { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.PurchaseOrderItemAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}