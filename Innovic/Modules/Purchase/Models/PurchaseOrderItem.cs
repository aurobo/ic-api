using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Purchase.Models
{
    public class PurchaseOrderItem : BaseModel
    {
        public PurchaseOrderItem()
        {
            PurchaseRequestItems = new List<PurchaseRequestItem>();
            GoodsReceiptItems = new List<GoodsReceiptItem>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime ExpectedDate { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string PurchaseOrderId { get; set; }
        public string MaterialId { get; set; }

        public virtual Material Material { get; set; }
        public virtual PurchaseOrder PurchaseOrder { get; set; }
        public virtual List<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public virtual List<GoodsReceiptItem> GoodsReceiptItems { get; set; }

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