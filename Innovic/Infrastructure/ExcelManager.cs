using ExcelDataReader;
using Innovic.App;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Innovic.Infrastructure
{
    public class ExcelManager
    {
        private readonly InnovicContext _context;
        private readonly BaseRepository<Material> _materialRepository;
        private readonly BaseRepository<Customer> _customerRepository;
        private readonly string filePath;

        public ExcelManager(InnovicContext context, string userId)
        {
            _context = context;
            _materialRepository = new BaseRepository<Material>(context, userId);
            _customerRepository = new BaseRepository<Customer>(context, userId);
        }

        public SalesOrder ToSalesOrder(string filePath)
        {
            var salesOrder = new SalesOrder();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    foreach (DataRow row in result.Tables["Header Data"].Rows)
                    {
                        var key = row["Name"].ToString();
                        var value = row["Value"];

                        switch (key)
                        {
                            case "Customer":
                                var customerName = value.ToString();
                                Customer customer = _context.Customers.Local.Where(c => c.Name.Equals(customerName)).SingleOrDefault();

                                if (customer == null)
                                {
                                    customer = _context.Customers.Where(c => c.Name.Equals(customerName)).SingleOrDefault();

                                    if (customer == null)
                                    {
                                        customer = _customerRepository.CreateNewWineModel(new CustomerInsertOptions { Name = customerName });
                                    }
                                }

                                salesOrder.CustomerId = customer.Id;
                                break;

                            case "ExpirationDate":
                                DateTime expirationDate;
                                DateTime.TryParse(value.ToString(), out expirationDate);
                                salesOrder.ExpirationDate = expirationDate;
                                break;

                            case "OrderDate":
                                DateTime orderDate;
                                DateTime.TryParse(value.ToString(), out orderDate);
                                salesOrder.OrderDate = orderDate;
                                break;

                            case "CustomerReference":
                                salesOrder.CustomerReference = value.ToString();
                                break;

                            case "PaymentTerms":
                                salesOrder.PaymentTerms = value.ToString();
                                break;

                            case "Description":
                                salesOrder.Description = value.ToString();
                                break;
                        }
                    }

                    foreach (DataRow row in result.Tables["Line Items"].Rows)
                    {
                        var itemNumber = row["Item Number"].ToString();
                        var materialNumber = row["Material Number"].ToString();
                        var description = row["Description"].ToString();
                        var quantity = Convert.ToInt32(row["Quantity"]);
                        var unitPrice = Convert.ToDouble(row["Unit Price"]);
                        var deliveryDate = DateTime.Parse(row["Delivery Date"].ToString());
                        var wbsElement = row["WBS Element"].ToString();

                        Material material = _context.Materials.Local.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                        if (material == null)
                        {
                            material = _context.Materials.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                            if (material == null)
                            {
                                material = _materialRepository.CreateNewWineModel(new MaterialInsertOptions { Number = materialNumber, Description = description });
                            }
                        }

                        var salesOrderItem = new SalesOrderItem
                        {
                            Number = itemNumber,
                            MaterialId = material.Id,
                            Description = string.IsNullOrEmpty(description) ? material.Description : description,
                            Quantity = quantity,
                            UnitPrice = unitPrice,
                            Value = quantity * unitPrice,
                            DeliveryDate = deliveryDate,
                            WBSElement = wbsElement
                        };

                        salesOrder.SalesOrderItems.Add(salesOrderItem);
                    }
                }
            }
            _context.SalesOrders.Add(salesOrder);
            return salesOrder;
        }

        public List<string> ValidateForSalesOrder(string filePath)
        {
            List<string> errors = new List<string>();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = false
                        }
                    });

                    // Sheets Validation
                    errors.AddRange(ValidateSheets(result.Tables, new List<string> { SalesOrderExcel.HeaderDataSheet, SalesOrderExcel.LineItemsSheet }));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    
                    #region -- Columns Validation --
                    errors.AddRange(validateColumns(SalesOrderExcel.HeaderDataColumns, result.Tables[SalesOrderExcel.HeaderDataSheet]));
                    errors.AddRange(validateColumns(SalesOrderExcel.LineItemsColumns, result.Tables[SalesOrderExcel.LineItemsSheet]));
                    
                    if (errors.Count > 0)
                    {
                        return errors;
                    }
                    
                    #endregion -- Columns Validation --


                    

                    #region -- Column fields Validation

                    List<string> cells = result.Tables[SalesOrderExcel.HeaderDataSheet].GetCellsForColumn(0);

                    errors.AddRange(validateCells(cells, result.Tables[SalesOrderExcel.HeaderDataSheet]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    #endregion


                }
            }
                    #region --- Check date format and Unitprice, Quantity ---
                    using (var streams = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var newreader = ExcelReaderFactory.CreateReader(streams))
                        {
                            var newresult = newreader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true
                                }
                            });

                            foreach (DataRow row in newresult.Tables[SalesOrderExcel.HeaderDataSheet].Rows)
                            {
                                var key = row["Name"].ToString();
                                switch (key)
                                {
                                    case "ExpirationDate":
                                        var value = row["Value"];
                                        errors.AddRange(validateDateFormat(value.ToString(), newresult.Tables[SalesOrderExcel.HeaderDataSheet]));
                                        break;
                                    case "OrderDate":
                                        value = row["Value"];
                                        errors.AddRange(validateDateFormat(value.ToString(), newresult.Tables[SalesOrderExcel.HeaderDataSheet]));
                                        break;
                                }
                            }


                            foreach (System.Data.DataRow row in newresult.Tables[SalesOrderExcel.LineItemsSheet].Rows)
                            {

                                var unitPrice = row["Unit Price"];
                                errors.AddRange(validateValueType(unitPrice.ToString(), newresult.Tables[SalesOrderExcel.LineItemsSheet], "Double"));

                                var quantity = row["Quantity"];
                                errors.AddRange(validateValueType(quantity.ToString(), newresult.Tables[SalesOrderExcel.LineItemsSheet], "Integer"));

                                var value = row["Delivery Date"];
                                errors.AddRange(validateDateFormat(value.ToString(), newresult.Tables[SalesOrderExcel.LineItemsSheet]));
                            }
                        }
                    }
                    
                    #endregion
            return errors;
        }


        public  List<string> validateColumns(List<string> staticColumns, DataTable table)
        {
            List<string> errors = new List<string>();
            foreach(var column in staticColumns)
            {
                if (!table.Rows[0].ItemArray.Contains(column))
                {
                    errors.Add("Column " + column + " is not present in sheet " + table.TableName);
                }

            }
            return errors;
        }

        public List<string> validateValueType(string value, DataTable table, string type)
        {
            List<string> errors = new List<string>();
            switch (type)
            {
                case "Double":
                    double doubleValue;
                    if (!double.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out doubleValue))
                    {
                        errors.Add(value + " is not valid in sheet " + table.TableName);
                    }
                    break;
                    
                case "Integer":
                    int integervalue;
                    if (!int.TryParse(value, out integervalue))
                    {
                        errors.Add(value + " is not valid in sheet " + table.TableName);
                    }
                    break;
            }

            return errors;
        }


        public List<string> validateDateFormat(string date, DataTable table)
        {
            List<string> errors = new List<string>();
            string[] formats = { "dd/MM/yyyy", "dd-MM-yyyy", "d-M-yyyy", "M/d/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };
            DateTime resultDate = new DateTime();

            if (!DateTime.TryParseExact(date, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out resultDate))
            {
                errors.Add(date + " is not valid date in sheet " + table.TableName);
            }

            return errors;
        }
        
        public List<string> validateCells(List<string> cells, DataTable sheet)
        {
            List<string> errors = new List<string>();
            SalesOrderExcel.HeaderDataColumnsFields.ForEach(delegate (string cell)
            {
                if (!cells.Contains(cell))
                {
                    errors.Add("Cell " + cell + " is not present in sheet " + SalesOrderExcel.HeaderDataSheet);
                }
            });
            return errors;
        }



        public List<string> ValidateSheets(DataTableCollection sheets, List<string> sheetNames)
        {
            List<string> errors = new List<string>();

            foreach (var sheetName in sheetNames)
            {
                if (!sheets.Contains(sheetName))
                {
                    errors.Add("Does not contain " + sheetName + " sheet.");
                }
            }

            return errors;
        }
    }
}