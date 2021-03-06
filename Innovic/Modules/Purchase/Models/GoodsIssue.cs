﻿using Innovic.App;
using Innovic.Models;
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
            GoodsReceipts = new List<GoodsReceipt>();
        }
        
        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }
        public string Remarks { get; set; }

        public virtual List<GoodsIssueItem> GoodsIssueItems { get; set; }
        public virtual List<GoodsReceipt> GoodsReceipts { get; set; }

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