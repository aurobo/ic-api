using System;

namespace Red.Wine
{
    public class BaseModel
    {
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }
}
