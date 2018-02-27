using Innovic.App;
using Red.Wine;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Sales.Models
{
    public class Customer : BaseModel
    {
        public string Name { get; set; }

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