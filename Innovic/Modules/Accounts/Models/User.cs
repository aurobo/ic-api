using Innovic.App;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Accounts.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}