using Innovic.Models;
using Innovic.Models.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Services
{
    public class MaterialService
    {
        private InnovicContext _db = new InnovicContext();

        public Material Find(string number)
        {
            return _db.Materials.Where(m => m.Number == number).SingleOrDefault();
        }

        public Material QuickCreate(string number, string description)
        {
            var material = new Material
            {
                Number = number,
                Description = description
            };

            _db.Materials.Add(material);
            _db.SaveChanges();

            return material;
        }
    }
}