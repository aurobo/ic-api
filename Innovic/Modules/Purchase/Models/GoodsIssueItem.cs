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
        public string GoodsIssueId { get; set; }
        public int Quantity { get; set; }
        public string MaterialId { get; set; }
        public double UnitPrice { get; set; }

        public virtual GoodsIssue GoodsIssue { get; set; }
        public virtual Material Material { get; set; }

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