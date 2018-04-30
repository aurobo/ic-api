using Innovic.App;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Master.Models
{
    public class Vendor : BaseModel
    {
        public string Name { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.VendorAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}