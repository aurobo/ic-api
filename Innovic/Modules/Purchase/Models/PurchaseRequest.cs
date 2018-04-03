using Innovic.App;
using Innovic.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class PurchaseRequest : BaseModel
    {
        public PurchaseRequest()
        {
            PurchaseRequestItems = new List<PurchaseRequestItem>();
            PurchaseOrders = new List<PurchaseOrder>();
            Links = new List<Link>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }
        public string Remarks { get; set; }

        public virtual List<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public virtual List<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual List<Link> Links { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.PurchaseRequestAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}