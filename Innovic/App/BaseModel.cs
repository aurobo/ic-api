﻿using Innovic.Modules.Accounts.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Red.Wine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.App
{
    public class BaseModel : WineModel
    {
        [ForeignKey("CreatedByUser")]
        public override string CreatedBy { get; protected set; }

        [ForeignKey("LastModifiedByUser")]
        public override string LastModifiedBy { get; protected set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User LastModifiedByUser { get; set; }
    }
}