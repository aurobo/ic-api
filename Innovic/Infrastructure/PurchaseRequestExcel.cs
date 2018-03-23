using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Infrastructure
{
    public class PurchaseRequestExcel
    {
        public static string HeaderDataSheet { get; } = "Header Data";
        public static string LineItemsSheet { get; } = "Line Items";
        public static List<string> HeaderDataColumns = new List<string> { "Name", "Value" };
        public static List<string> LineItemsColumns = new List<string> { "Material Number", "Item Number", "Quantity", "Date", "Description" };
        public static List<string> HeaderDataNameRows = new List<string> { "Date" };
    }
}