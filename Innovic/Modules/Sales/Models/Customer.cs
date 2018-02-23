using Innovic.App;
using Red.Wine;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Sales.Models
{
    public class Customer : WineModel
    {
        public string Name { get; set; }
        public bool IsDeleted { get; set; } // Shift to WineModel

        [NotMapped]
        public string Key
        {
            get
            {
                return (Constants.CustomerAbbr + Constants.KeySeparator + KeyId.ToString(Constants.FixedDigits));
            }
        }
    }
}