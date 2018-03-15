using Innovic.App;
using Innovic.Modules.Sales.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class GoodsIssue : BaseModel
    {

        public GoodsIssue()
        {
            GoodsIssueItems = new List<GoodsIssueItem>();
            PurchaseOrders = new List<PurchaseOrder>();
        }
        
        public GoodsIssueReference Reference { get; set; }
        public GoodsIssueStatus Status { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public string VendorId { get; set; }
        public string SlipLevelNote { get; set; }
        public double TotalValue { get; set; }


        public virtual List<GoodsIssueItem> GoodsIssueItems { get; set; }
        public virtual List<PurchaseOrder> PurchaseOrders { get; set; }

        [ForeignKey("VendorId")]
        public virtual Customer Vendor { get; set; }

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