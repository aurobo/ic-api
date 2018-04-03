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

        [Column(TypeName = "datetime2")]
        public DateTime RequiredByDate { get; set; }
        public string GoodsIssueId { get; set; }
        public int Quantity { get; set; }
        public string MaterialId { get; set; }

        public virtual GoodsIssue GoodsIssue { get; set; }
        public virtual Material Material { get; set; }
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