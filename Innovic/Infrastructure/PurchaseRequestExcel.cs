using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Infrastructure
{
    public class PurchaseRequestExcel
    {
        public static List<string> LineItemsColumns = new List<string> { "Material Number", "Line Number", "Quantity", "Make", "Reason", "Expected Date" };
        public static List<string> HeaderDataNameRows = new List<string> { "Date" };
    }
}