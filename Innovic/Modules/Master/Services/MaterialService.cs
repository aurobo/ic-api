using Innovic.Modules.Master.Models;

namespace Innovic.Modules.Master.Services
{
    public class MaterialService
    {
        MaterialService _service = new MaterialService();

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