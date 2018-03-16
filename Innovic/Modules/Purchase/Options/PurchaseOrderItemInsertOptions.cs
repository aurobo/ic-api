using Innovic.Modules.Master.Models;
using Innovic.Modules.Purchase.Models;
using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Options
{
    public class PurchaseOrderItemInsertOptions
    {
        [CopyTo(typeof(Material), Red.Wine.Relationship.Dependency, true)]
        public string MaterialId { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        [CopyTo(typeof(PurchaseRequestItem), Red.Wine.Relationship.Dependency, true)]
        public string PurchaseRequestItemId { get; set; }
    }
}