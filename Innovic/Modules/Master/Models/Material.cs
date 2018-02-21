using Innovic.Helpers;
using Red.Wine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Master.Models
{
    public class Material : BaseModel
    {
        public string Number { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (AppConstants.MaterialAbbr + AppConstants.KeySeparator + KeyId.ToString(AppConstants.FixedDigits));
            }
        }
    }
}