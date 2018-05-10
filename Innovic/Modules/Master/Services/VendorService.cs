using Innovic.Modules.Master.Models;

namespace Innovic.Modules.Master.Services
{
    public static class VendorService
    {
        public static bool IsDeletable(this Vendor vendor)
        {
            return false;
        }
    }
}