using Innovic.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Purchase.Models
{
    public class PurchaseRequest : BaseModel
    {
        public PurchaseRequest()
        {
            PurchaseRequestItems = new List<PurchaseRequestItem>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public PurchaseRequestStatus Status { get; set; }

        public virtual List<PurchaseRequestItem> PurchaseRequestItems { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants. PurchaseRequestAbbr+ Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }

    }
}