using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.Wine
{
    public class BaseModel
    {
        public string LastModifiedBy { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime LastModifiedOn { get; set; }
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public long KeyId { get; set; }
    }
}
