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
            PurchaseRequestItems = new HashSet<PurchaseRequestItem>();
        }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public PurchaseRequestStatus Status { get; set; }

        public virtual HashSet<PurchaseRequestItem> PurchaseRequestItems { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.PurchaseRequestCode + Constants.CodeSeparator + KeyId.ToString(Constants.FixDidit6));
            }
        }

    }
}