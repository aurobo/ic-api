using Innovic.Modules.Master.Models;

namespace Innovic.Modules.Master.Services
{
    public static class MaterialService
    {
        public static bool IsDeletable(this Material material)
        {
            return !(material.GoodsIssueItems.Count > 0
                || material.GoodsReceiptItems.Count > 0
                || material.PurchaseOrderItems.Count > 0
                || material.PurchaseRequestItems.Count > 0
                || material.SalesOrderItems.Count > 0);
        }
    }
}