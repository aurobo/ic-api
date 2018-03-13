using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.Infrastructure
{
    public static class ExcelConstants
    {
        public static string HeaderDataSheet { get; } = "Header Data";
        public static string LineItemsSheet { get; } = "Line Items";
        public static List<string> HeaderDataColumns = new List<string> { "Name", "Value" };
        public static List<string> LineItemsColumns = new List<string> { "Item Number", "Material Number", "Description", "Quantity", "Unit Price", "Delivery Date", "WBS Element" };
        public static List<string> HeaderDataNameRows = new List<string> { "Customer","ExpirationDate","OrderDate","CustomerReference","PaymentTerms","Description" };
        
        public static List<string> LineItemsColumnsForPurchaseRequest = new List<string> { "Material Number", "Line Number", "Quantity", "Make", "Reason", "Expected Date" };
        public static List<string> HeaderDataNameRowsForPurchaseRequest = new List<string> { "Date" };
    }
}