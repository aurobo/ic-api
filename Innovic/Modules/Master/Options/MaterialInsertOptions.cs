using Innovic.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Master.Options
{
    public class MaterialInsertOptions : BaseOptionsModel
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}