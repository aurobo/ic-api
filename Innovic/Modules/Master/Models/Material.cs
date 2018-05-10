using Innovic.App;
using Innovic.Modules.Purchase.Models;
using Innovic.Modules.Sales.Models;
using Red.Wine;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Master.Models
{
    public class Material : BaseModel
    {
        public Material()
        {
            PurchaseRequestItems = new List<PurchaseRequestItem>();
            PurchaseOrderItems = new List<PurchaseOrderItem>();
            GoodsIssueItems = new List<GoodsIssueItem>();
            GoodsReceiptItems = new List<GoodsReceiptItem>();
            InvoiceItems = new List<InvoiceItem>();
            SalesOrderItems = new List<SalesOrderItem>();
        }

        public string Number { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public virtual List<PurchaseRequestItem> PurchaseRequestItems { get; set; }
        public virtual List<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public virtual List<GoodsIssueItem> GoodsIssueItems { get; set; }
        public virtual List<GoodsReceiptItem> GoodsReceiptItems { get; set; }
        public virtual List<InvoiceItem> InvoiceItems { get; set; }
        public virtual List<SalesOrderItem> SalesOrderItems { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.MaterialAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}