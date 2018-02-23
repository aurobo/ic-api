using Innovic.App;
using Red.Wine;
using System.ComponentModel.DataAnnotations.Schema;

namespace Innovic.Modules.Master.Models
{
    public class Material : WineModel
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } // Shift to WineModel

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