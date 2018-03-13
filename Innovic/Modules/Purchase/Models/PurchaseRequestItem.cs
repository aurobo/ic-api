using Innovic.App;
using Innovic.Modules.Master.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class PurchaseRequestItem : BaseModel
    {
        public PurchaseRequestItem()
        {

        }

        public string PurchaseRequestId { get; set; }
        public string MaterialId { get; set; }
        public int LineNumber { get; set; }
        public int Quantity { get; set; }
        public string Make { get; set; }
        public string Reason { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime ExpectedDate { get; set; }

        public PurchaseRequestItemStatus Status { get; set; }

        public virtual Material Material { get; set; }
        public virtual PurchaseRequest PurchaseRequest { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.PurchaseRequestItemAbbr+ Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }

    }
}