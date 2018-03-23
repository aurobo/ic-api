using Innovic.App;
using Innovic.Modules.Purchase.Models;
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
        }

        public string Number { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public virtual List<PurchaseRequestItem> PurchaseRequestItems { get; set; }

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