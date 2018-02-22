using Red.Wine.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.App
{
    public class BaseOptionsModel
    {
        [IgnoreWhileCopy]
        public Dictionary<string, object> MetaData { get; set; }
    }
}