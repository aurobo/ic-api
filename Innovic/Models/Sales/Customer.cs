using Innovic.Helpers;
using Red.Wine;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Models.Sales
{
    public class Customer : BaseModel
    {
        public string Name { get; set; }

        [NotMapped]
        public string Key
        {
            get
            {
                return (AppConstants.CustomerAbbr + AppConstants.KeySeparator + KeyId.ToString(AppConstants.FixedDigits));
            }
        }
    }
}