using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Models
{
    public class InnovicContext : IdentityDbContext<IdentityUser>
    {
        public InnovicContext()
            : base("dbConnection")
        {

        }

        public InnovicContext Create()
        {
            return new InnovicContext();
        }
    }
}