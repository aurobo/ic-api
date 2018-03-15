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
        }
        
        public string GoodsIssueId { get; set; }
        public GoodsIssueItemStatus Status { get; set; }
        public string Text { get; set; }
        public string Size { get; set; }
        public string Notes { get; set; }
        public int Quantity { get; set; }
        public string MaterialId { get; set; }
        public double Cost { get; set; }
        public double UnitCost { get; set; }

        
        public virtual GoodsIssue GoodsIssue { get; set; }
        public virtual Material Material { get; set; }

        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }


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