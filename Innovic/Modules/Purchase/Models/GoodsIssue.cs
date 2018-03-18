using Innovic.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Purchase.Models
{
    public class GoodsIssue : BaseModel
    {
        public GoodsIssue()
        {
            GoodsIssueItems = new List<GoodsIssueItem>();
            PurchaseOrders = new List<PurchaseOrder>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public virtual List<GoodsIssueItem> GoodsIssueItems { get; set; }
        public virtual List<PurchaseOrder> PurchaseOrders { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.GoodsIssueAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}