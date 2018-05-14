using Innovic.App;
using Innovic.Models;
using Innovic.Modules.Sales.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class GoodsReceipt : BaseModel
    {
        public GoodsReceipt()
        {
            GoodsReceiptItems = new List<GoodsReceiptItem>();
            PurchaseOrders = new List<PurchaseOrder>();
            GoodsIssues = new List<GoodsIssue>();
            Links = new List<Link>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }
        public string Remarks { get; set; }

        public virtual List<GoodsReceiptItem> GoodsReceiptItems { get; set; }
        public virtual List<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual List<GoodsIssue> GoodsIssues { get; set; }
        public virtual List<Link> Links { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.GoodsReceiptAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}