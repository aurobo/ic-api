using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class GoodsIssueItem : BaseModel
    {
        public GoodsIssueItem()
        {
            PurchaseOrderItems = new List<PurchaseOrderItem>();
            GoodsReceiptItems = new List<GoodsReceiptItem>();
        }

        public string GoodsIssueId { get; set; }
        public int Quantity { get; set; }
        public string MaterialId { get; set; }
        public double UnitPrice { get; set; }

        
        public virtual GoodsIssue GoodsIssue { get; set; }
        public virtual Material Material { get; set; }
        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public virtual List<GoodsReceiptItem> GoodsReceiptItems { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.GoodsIssueItemAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }

    }
}