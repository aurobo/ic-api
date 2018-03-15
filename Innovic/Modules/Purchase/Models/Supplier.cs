using Innovic.App;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Purchase.Models
{
    public class Supplier : BaseModel
    {
        public string Name { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.SupplierAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}