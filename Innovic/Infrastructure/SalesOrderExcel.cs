using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Innovic.Infrastructure
{
    public static class SalesOrderExcel
    {
        public static List<string> Sheets
        {
            get
            {
                return Enum.GetNames(typeof(Sheet)).ToList();
            }
        }

        public static List<string> HeaderDataColumns
        {
            get
            {
                return Enum.GetNames(typeof(HeaderDataColumn)).ToList();
            }
        }

        public static List<string> LineItemsColumns
        {
            get
            {
                return Enum.GetNames(typeof(LineItemsColumn)).ToList();
            }
        }

        public static List<string> HeaderDataNameCells
        {
            get
            {
                return Enum.GetNames(typeof(HeaderDataNameCell)).ToList();
            }
        }

        public static string HeaderDataSheetName
        {
            get
            {
                return Sheet.HeaderData.ToString();
            }
        }

        public static string LineItemsSheetName
        {
            get
            {
                return Sheet.LineItems.ToString();
            }
        }

        public enum HeaderDataColumn
        {
            Name,
            Value
        }

        public enum LineItemsColumn
        {
            ItemNumber,
            MaterialNumber,
            Description,
            Quantity,
            UnitPrice,
            DeliveryDate,
            WBSElement
        }

        public enum Sheet
        {
            HeaderData,
            LineItems
        }

        public enum HeaderDataNameCell
        {
            Customer,
            ExpirationDate,
            OrderDate,
            CustomerReference,
            PaymentTerms,
            Remarks
        }
    }
}