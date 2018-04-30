using Innovic.App;

namespace Innovic.Modules.Master.Options
{
    public class MaterialInsertOptions : BaseOptionsModel
    {
        public string Number { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}