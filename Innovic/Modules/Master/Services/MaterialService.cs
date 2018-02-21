using Innovic.Models;
using Innovic.Models.Master;
using Innovic.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Modules.Master.Services
{
    public class MaterialService
    {
        BaseService<Material> _service = new BaseService<Material>();

        public Material Find(string number)
        {
            return _service.Find(m => m.Number == number);
        }

        public Material QuickCreate(string number, string description)
        {
            var material = new Material
            {
                Number = number,
                Description = description
            };

            _service.QuickCreateAndSave(material);

            return material;
        }
    }
}