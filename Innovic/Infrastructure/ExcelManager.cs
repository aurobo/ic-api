using ExcelDataReader;
using Innovic.App;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Innovic.Modules.Purchase.Models;
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

                    // Sheet Validation
                    errors.AddRange(ValidateSheets(result.Tables, new List<string> { SalesOrderExcel.HeaderDataSheet, SalesOrderExcel.LineItemsSheet }));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Column Validation
                    errors.AddRange(ValidateColumns(SalesOrderExcel.HeaderDataColumns, result.Tables[SalesOrderExcel.HeaderDataSheet]));
                    errors.AddRange(ValidateColumns(SalesOrderExcel.LineItemsColumns, result.Tables[SalesOrderExcel.LineItemsSheet]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }
                    
                    // Cell Validation
                    var cells = result.Tables[SalesOrderExcel.HeaderDataSheet].GetCellsForColumn(0, false);

                    errors.AddRange(ValidateCells(cells, result.Tables[SalesOrderExcel.HeaderDataSheet]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }
                }
            }
            
            // Date Validation
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DateTime expirationDate = new DateTime();
                    DateTime orderDate = new DateTime();

                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    foreach (DataRow row in result.Tables[SalesOrderExcel.HeaderDataSheet].Rows)
                    {
                        string key = row["Name"].ToString();
                        object value = row["Value"];

                        switch (key)
                        {
                            case "ExpirationDate":
                                if (!IsDateValid(value.ToString(), out expirationDate))
                                {
                                    errors.Add("ExpirationDate in sheet " + SalesOrderExcel.HeaderDataSheet + " is not in valid format.");
                                }
                                break;
                            case "OrderDate":
                                if (!IsDateValid(value.ToString(), out orderDate))
                                {
                                    errors.Add("OrderDate in sheet " + SalesOrderExcel.HeaderDataSheet + " is not in valid format.");
                                }
                                break;
                        }
                    }

                    if (expirationDate != DateTime.MinValue && orderDate != DateTime.MinValue)
                    {
                        if(!CompareDates(expirationDate, orderDate))
                        {
                            errors.Add("ExpirationDate is lesser than OrderDate in sheet " + SalesOrderExcel.HeaderDataSheet);
                        }
                    }

                    // Because header row has index = 1 in excel sheet
                    int index = 2; // In excel sheet

                    foreach (DataRow row in result.Tables[SalesOrderExcel.LineItemsSheet].Rows)
                    {
                        string unitPrice = row["Unit Price"].ToString();
                        errors.AddRange(ValidateValueType(unitPrice, result.Tables[SalesOrderExcel.LineItemsSheet], "Double"));

                        var quantity = row["Quantity"];
                        errors.AddRange(ValidateValueType(quantity.ToString(), result.Tables[SalesOrderExcel.LineItemsSheet], "Integer"));

                        var value = row["Delivery Date"];
                        DateTime deliveryDate = new DateTime();
                        if (!IsDateValid(value.ToString(), out deliveryDate))
                        {
                            errors.Add("Delivery Date at row " + index + " is invalid");
                        }
                        index++;
                    }
                }
            }

            return errors;
        }


        public bool CompareDates(DateTime greaterDate, DateTime lesserDate)
        {
            if (greaterDate < lesserDate)
            {
                return false;
            }

            return true;
        }

        public List<string> ValidateColumns(List<string> staticColumns, DataTable table)
        {
            List<string> errors = new List<string>();
            foreach (var column in staticColumns)
            {
                if (!table.Rows[0].ItemArray.Contains(column))
                {
                    errors.Add("Column " + column + " is not present in sheet " + table.TableName);
                }

            }
            return errors;
        }

        public List<string> ValidateValueType(string value, DataTable table, string type)
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


        public bool IsDateValid(string date, out DateTime resultDate)
        {
            try
            {
                resultDate = Convert.ToDateTime(date);
                return true;
            }
            catch (FormatException e)
            {
                resultDate = DateTime.MinValue;
                return false;
            }
        }

        public List<string> ValidateCells(List<string> cells, DataTable sheet)
        {
            List<string> errors = new List<string>();

            SalesOrderExcel.HeaderDataNameRows.ForEach(delegate (string cell)
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

        #region ---- Purchase Request -----


        //..... Add Purchase Request...

        public PurchaseRequest ToPurchaseRequest(string filePath)
        {
            var purchaseRequest = new PurchaseRequest();

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
                            case "Date":
                                DateTime date;
                                DateTime.TryParse(value.ToString(), out date);
                                purchaseRequest.Date = date;
                                break;
                        }
                    }

                    foreach (DataRow row in result.Tables["Line Items"].Rows)
                    {
                        var materialNumber = row["Material Number"].ToString();
                        var number = row["Item Number"].ToString();
                        var quantity = Convert.ToInt32(row["Quantity"]);
                        var expectedDate = DateTime.Parse(row["Expected Date"].ToString());
                        

                        Material material = _context.Materials.Local.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                        if (material == null)
                        {
                            material = _context.Materials.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                            if (material == null)
                            {
                                material = _materialRepository.CreateNewWineModel(new MaterialInsertOptions { Number = materialNumber });
                            }
                        }

                        var purchaseRequestItem = new PurchaseRequestItem
                        {
                            MaterialId = material.Id,
                            Number = number,
                            Quantity = quantity,
                            ExpectedDate = expectedDate
                        };

                        purchaseRequest.PurchaseRequestItems.Add(purchaseRequestItem);
                    }
                }
            }
            _context.PurchaseRequests.Add(purchaseRequest);
            return purchaseRequest;
        }


        public List<string> ValidateForPurchaseRequest(string filePath)
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

                    // Sheet Validation
                    errors.AddRange(ValidateSheets(result.Tables, new List<string> { SalesOrderExcel.HeaderDataSheet, SalesOrderExcel.LineItemsSheet }));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Column Validation
                    errors.AddRange(ValidateColumns(SalesOrderExcel.HeaderDataColumns, result.Tables[SalesOrderExcel.HeaderDataSheet]));
                    errors.AddRange(ValidateColumns(SalesOrderExcel.LineItemsColumns, result.Tables[SalesOrderExcel.LineItemsSheet]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Cell Validation
                    var cells = result.Tables[SalesOrderExcel.HeaderDataSheet].GetCellsForColumn(0, false);

                    errors.AddRange(ValidateCellsForPurchaseRequest(cells, result.Tables[SalesOrderExcel.HeaderDataSheet]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }
                }
            }

            // Date Validation
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    DateTime date = new DateTime();
                    
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    foreach (DataRow row in result.Tables[SalesOrderExcel.HeaderDataSheet].Rows)
                    {
                        string key = row["Name"].ToString();
                        object value = row["Value"];

                        switch (key)
                        {
                            case "Date":
                                if (!IsDateValid(value.ToString(), out date))
                                {
                                    errors.Add("Date in sheet " + SalesOrderExcel.HeaderDataSheet + " is not in valid format.");
                                }
                                break;
                        }
                    }
                    
                    // Because header row has index = 1 in excel sheet
                    int index = 2; // In excel sheet

                    foreach (DataRow row in result.Tables[SalesOrderExcel.LineItemsSheet].Rows)
                    {
                        var quantity = row["Quantity"];
                        errors.AddRange(ValidateValueType(quantity.ToString(), result.Tables[SalesOrderExcel.LineItemsSheet], "Integer"));

                        var lineNumber = row["Line Number"];
                        errors.AddRange(ValidateValueType(lineNumber.ToString(), result.Tables[SalesOrderExcel.LineItemsSheet], "Integer"));
                        
                        var value = row["Expected Date"];
                        DateTime expectedDate = new DateTime();

                        if (!IsDateValid(value.ToString(), out expectedDate))
                        {
                            errors.Add("Expected Date at row " + index + " is invalid");
                        }
                        index++;
                    }
                }
            }

            return errors;
        }



        //.... Validate Cells for purchase request ...
        public List<string> ValidateCellsForPurchaseRequest(List<string> cells, DataTable sheet)
        {
            List<string> errors = new List<string>();

            SalesOrderExcel.HeaderDataNameRows.ForEach(delegate (string cell)
            {
                if (!cells.Contains(cell))
                {
                    errors.Add("Cell " + cell + " is not present in sheet " + SalesOrderExcel.HeaderDataSheet);
                }
            });

            return errors;
        }

        #endregion ---Purchase Request End ----

    }
}