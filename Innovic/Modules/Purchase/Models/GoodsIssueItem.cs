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
            GoodsReceiptItems = new List<GoodsReceiptItem>();
        }
        
        public string GoodsIssueId { get; set; }
        public string Text { get; set; }
        public string Size { get; set; }
        public string Notes { get; set; }
        public int Quantity { get; set; }
        public string MaterialId { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public string PurchaseOrderItemId { get; set; }

        public virtual GoodsIssue GoodsIssue { get; set; }
        public virtual Material Material { get; set; }
        public virtual PurchaseOrderItem PurchaseOrderItem { get; set; }

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