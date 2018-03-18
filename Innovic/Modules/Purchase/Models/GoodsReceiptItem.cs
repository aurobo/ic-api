using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class GoodsReceiptItem : BaseModel
    {
        public GoodsReceiptItem()
        {
            PurchaseOrderItems = new List<PurchaseOrderItem>();
        }

        public string GoodsReceiptId { get; set; }
        public int Quantity { get; set; }
        public string MaterialId { get; set; }
        public double UnitPrice { get; set; }

        public virtual Material Material { get; set; }
        public virtual GoodsReceipt GoodsReceipt { get; set; }
        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.GoodsReceiptItemAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}