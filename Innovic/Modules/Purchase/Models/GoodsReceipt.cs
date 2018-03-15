﻿using Innovic.App;
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
        }
        
        public GoodsReceiptReference Reference { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; }

        public string VendorId { get; set; }

        public string SlipLevelNote { get; set; }

        public GoodsReceiptStatus Status { get; set; }

        public virtual List<GoodsReceiptItem> GoodsReceiptItems { get; set; }

        public virtual List<PurchaseOrder> PurchaseOrders { get; set; }
    
        [ForeignKey("VendorId")]
        public virtual Customer Vendor { get; set; }
    }
}