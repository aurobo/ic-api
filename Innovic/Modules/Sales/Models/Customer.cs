using Innovic.Helpers;
using Red.Wine;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Sales.Models
{
    public class Customer : WineModel
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