﻿using ExcelDataReader;
using Innovic.App;
using Innovic.Models;
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
        private readonly string _userId;
        private readonly BaseRepository<Material> _materialRepository;
        private readonly BaseRepository<Customer> _customerRepository;

        public ExcelManager(InnovicContext context, string userId)
        {
            _context = context;
            _userId = userId;
            _materialRepository = new BaseRepository<Material>(context, userId);
            _customerRepository = new BaseRepository<Customer>(context, userId);
        }

        public void ToVendors(string filePath)
        {
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

                    foreach (DataRow row in result.Tables["Vendors"].Rows)
                    {
                        string name = row["name"].ToString();

                        Vendor vendor = _context.Vendors.Local.Where(v => v.Name.Equals(name)).SingleOrDefault();

                        if (vendor == null)
                        {
                            vendor = _context.Vendors.Where(v => v.Name.Equals(name)).SingleOrDefault();

                            if (vendor == null)
                            {
                                BaseRepository<Vendor> vendorRepository = new BaseRepository<Vendor>(_context, _userId);
                                vendor = vendorRepository.CreateNewWineModel(new VendorInsertOptions { Name = name });
                            }
                        }
                    }
                }
            }
        }

        public void ToMaterials(string filePath)
        {
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

                    foreach (DataRow row in result.Tables["Materials"].Rows)
                    {
                        string materialNumber = row["MaterialNumber"].ToString();
                        string description = row["Description"].ToString();
                        int quantity = Convert.ToInt32(row["Quantity"]);


                        // The following snippet is repeating throughout this class, create a separate method for it.
                        Material material = _context.Materials.Local.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                        if (material == null)
                        {
                            material = _context.Materials.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                            if (material == null)
                            {
                                material = _materialRepository.CreateNewWineModel(new MaterialInsertOptions { Number = materialNumber, Description = description, Quantity = quantity });
                            }
                        }
                    }
                }
            }
        }

        internal List<string> ValidateForGoodsReceipt(string filePath)
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
                    errors.AddRange(ValidateSheets(result.Tables, new List<string> { GoodsReceiptExcel.HeaderDataSheetName, GoodsReceiptExcel.LineItemsSheetName }));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Column Validation
                    errors.AddRange(ValidateColumns(GoodsReceiptExcel.HeaderDataColumns, result.Tables[GoodsReceiptExcel.HeaderDataSheetName]));
                    errors.AddRange(ValidateColumns(GoodsReceiptExcel.LineItemsColumns, result.Tables[GoodsReceiptExcel.LineItemsSheetName]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Cell Validation
                    var cells = result.Tables[GoodsReceiptExcel.HeaderDataSheetName].GetCellsForColumn(0, false);

                    errors.AddRange(ValidateCells(cells, result.Tables[GoodsReceiptExcel.HeaderDataSheetName], GoodsReceiptExcel.HeaderDataNameCells));

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

                    foreach (DataRow row in result.Tables[GoodsReceiptExcel.HeaderDataSheetName].Rows)
                    {
                        string key = row[GoodsReceiptExcel.HeaderDataColumn.Name.ToString()].ToString();
                        object value = row[GoodsReceiptExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out GoodsReceiptExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case GoodsReceiptExcel.HeaderDataNameCell.Date:
                                if (!IsDateValid(value.ToString(), out date))
                                {
                                    errors.Add(key + " in sheet " + GoodsReceiptExcel.HeaderDataSheetName + " is not in valid format.");
                                }
                                break;
                        }
                    }

                    // Because header row has index = 1 in excel sheet
                    int index = 2; // In excel sheet

                    foreach (DataRow row in result.Tables[GoodsReceiptExcel.LineItemsSheetName].Rows)
                    {
                        // Quantity
                        var quantity = row[GoodsReceiptExcel.LineItemsColumn.Quantity.ToString()];
                        errors.AddRange(ValidateValueType(quantity.ToString(), result.Tables[GoodsReceiptExcel.LineItemsSheetName], "Integer"));

                        // Date
                        var value = row[GoodsReceiptExcel.LineItemsColumn.Date.ToString()];
                        DateTime requiredByDate = new DateTime();

                        if (!IsDateValid(value.ToString(), out requiredByDate))
                        {
                            errors.Add(GoodsReceiptExcel.LineItemsColumn.Date.ToString() + " at row " + index + " is invalid");
                        }

                        index++;
                    }

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    //No need to validate the materials. If the specified materials not exist, we should generate it.
                }
            }

            return errors;
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

                    foreach (DataRow row in result.Tables[SalesOrderExcel.HeaderDataSheetName].Rows)
                    {
                        var key = row[SalesOrderExcel.HeaderDataColumn.Name.ToString()].ToString();
                        var value = row[SalesOrderExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out SalesOrderExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case SalesOrderExcel.HeaderDataNameCell.Customer:
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

                            case SalesOrderExcel.HeaderDataNameCell.ExpirationDate:
                                DateTime expirationDate;
                                DateTime.TryParse(value.ToString(), out expirationDate);
                                salesOrder.ExpirationDate = expirationDate;
                                break;

                            case SalesOrderExcel.HeaderDataNameCell.OrderDate:
                                DateTime orderDate;
                                DateTime.TryParse(value.ToString(), out orderDate);
                                salesOrder.OrderDate = orderDate;
                                break;

                            case SalesOrderExcel.HeaderDataNameCell.CustomerReference:
                                salesOrder.CustomerReference = value.ToString();
                                break;

                            case SalesOrderExcel.HeaderDataNameCell.PaymentTerms:
                                salesOrder.PaymentTerms = value.ToString();
                                break;

                            case SalesOrderExcel.HeaderDataNameCell.Remarks:
                                salesOrder.Remarks = value.ToString();
                                break;
                        }
                    }

                    foreach (DataRow row in result.Tables[SalesOrderExcel.LineItemsSheetName].Rows)
                    {
                        var itemNumber = row[SalesOrderExcel.LineItemsColumn.ItemNumber.ToString()].ToString();
                        var materialNumber = row[SalesOrderExcel.LineItemsColumn.MaterialNumber.ToString()].ToString();
                        var description = row[SalesOrderExcel.LineItemsColumn.Description.ToString()].ToString();
                        var quantity = Convert.ToInt32(row[SalesOrderExcel.LineItemsColumn.Quantity.ToString()]);
                        var unitPrice = Convert.ToDouble(row[SalesOrderExcel.LineItemsColumn.UnitPrice.ToString()]);
                        var deliveryDate = DateTime.Parse(row[SalesOrderExcel.LineItemsColumn.DeliveryDate.ToString()].ToString());
                        var wbsElement = row[SalesOrderExcel.LineItemsColumn.WBSElement.ToString()].ToString();

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

        public GoodsIssue ToGoodsIssue(string filePath)
        {
            var goodsIssue = new GoodsIssue();

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

                    foreach (DataRow row in result.Tables[GoodsIssueExcel.HeaderDataSheetName].Rows)
                    {
                        var key = row[GoodsIssueExcel.HeaderDataColumn.Name.ToString()].ToString();
                        var value = row[GoodsIssueExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out GoodsIssueExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case GoodsIssueExcel.HeaderDataNameCell.Date:
                                DateTime date;
                                DateTime.TryParse(value.ToString(), out date);
                                goodsIssue.Date = date;
                                break;
                            case GoodsIssueExcel.HeaderDataNameCell.Remarks:
                                goodsIssue.Remarks = value.ToString();
                                break;
                            case GoodsIssueExcel.HeaderDataNameCell.PurchaseOrderReferences:
                                var purchaseOrderKeys = value.ToString().Split(',');
                                var purchaseOrders = _context.PurchaseOrders.ToList();

                                foreach (var po in purchaseOrders)
                                {
                                    if (purchaseOrderKeys.Contains(po.Key))
                                    {
                                        goodsIssue.Links.Add(new Link
                                        {
                                            PurchaseOrder = po,
                                            GoodsIssue = goodsIssue,
                                        });
                                    }
                                }
                                break;
                        }
                    }

                    foreach (DataRow row in result.Tables[GoodsIssueExcel.LineItemsSheetName].Rows)
                    {
                        var materialNumber = row[GoodsIssueExcel.LineItemsColumn.MaterialNumber.ToString()].ToString();
                        var quantity = Convert.ToInt32(row[GoodsIssueExcel.LineItemsColumn.Quantity.ToString()]);
                        var requiredByDate = DateTime.Parse(row[GoodsIssueExcel.LineItemsColumn.RequiredByDate.ToString()].ToString());
                        var description = row[GoodsIssueExcel.LineItemsColumn.Description.ToString()].ToString();

                        Material material = _context.Materials.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                        var goodsIssueItem = new GoodsIssueItem
                        {
                            MaterialId = material.Id,
                            Quantity = quantity,
                            Date = requiredByDate
                        };

                        goodsIssue.GoodsIssueItems.Add(goodsIssueItem);
                    }
                }
            }
            _context.GoodsIssues.Add(goodsIssue);
            return goodsIssue;
        }

        public GoodsReceipt ToGoodsReceipt(string filePath)
        {
            var goodsReceipt = new GoodsReceipt();

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

                    foreach (DataRow row in result.Tables[GoodsReceiptExcel.HeaderDataSheetName].Rows)
                    {
                        var key = row[GoodsReceiptExcel.HeaderDataColumn.Name.ToString()].ToString();
                        var value = row[GoodsReceiptExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out GoodsReceiptExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case GoodsReceiptExcel.HeaderDataNameCell.Date:
                                DateTime date;
                                DateTime.TryParse(value.ToString(), out date);
                                goodsReceipt.Date = date;
                                break;
                            case GoodsReceiptExcel.HeaderDataNameCell.Remarks:
                                goodsReceipt.Remarks = value.ToString();
                                break;
                            case GoodsReceiptExcel.HeaderDataNameCell.SalesOrderReferences:
                                var salesOrderKeys = value.ToString().Split(',');
                                var salesOrders = _context.SalesOrders.ToList();

                                foreach (var so in salesOrders)
                                {
                                    if (salesOrderKeys.Contains(so.Key))
                                    {
                                        goodsReceipt.Links.Add(new Link
                                        {
                                            SalesOrder = so,
                                            GoodsReceipt = goodsReceipt
                                        });
                                    }
                                }
                                break;
                        }
                    }

                    foreach (DataRow row in result.Tables[GoodsReceiptExcel.LineItemsSheetName].Rows)
                    {
                        var materialNumber = row[GoodsReceiptExcel.LineItemsColumn.MaterialNumber.ToString()].ToString();
                        var quantity = Convert.ToInt32(row[GoodsReceiptExcel.LineItemsColumn.Quantity.ToString()]);

                        //Need to confirm with Ronak
                        var date = DateTime.Parse(row[GoodsReceiptExcel.LineItemsColumn.Date.ToString()].ToString());
                        var description = row[GoodsReceiptExcel.LineItemsColumn.Description.ToString()].ToString();

                        Material material = _context.Materials.Local.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                        if (material == null)
                        {

                            material = _context.Materials.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();


                            if (material == null)
                            {
                                material = _materialRepository.CreateNewWineModel(new MaterialInsertOptions { Number = materialNumber, Description = description });
                            }
                        }


                        var goodsReceiptItem = new GoodsReceiptItem
                        {
                            MaterialId = material.Id,
                            Quantity = quantity,
                            Date = date,

                            //Had to do it, so that it would be referenced correrctly we add quantity to the material in GoodsReceiptService.AddMaterialQuantity
                            // otherwise the goodsReceiptItem.Material would be NULL
                            Material = material,
                        };

                        goodsReceipt.GoodsReceiptItems.Add(goodsReceiptItem);
                    }
                }
            }
            _context.GoodsReceipts.Add(goodsReceipt);
            return goodsReceipt;
        }

        public List<string> ValidateForGoodsIssue(string filePath)
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
                    errors.AddRange(ValidateSheets(result.Tables, new List<string> { GoodsIssueExcel.HeaderDataSheetName, GoodsIssueExcel.LineItemsSheetName }));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Column Validation
                    errors.AddRange(ValidateColumns(GoodsIssueExcel.HeaderDataColumns, result.Tables[GoodsIssueExcel.HeaderDataSheetName]));
                    errors.AddRange(ValidateColumns(GoodsIssueExcel.LineItemsColumns, result.Tables[GoodsIssueExcel.LineItemsSheetName]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Cell Validation
                    var cells = result.Tables[GoodsIssueExcel.HeaderDataSheetName].GetCellsForColumn(0, false);

                    errors.AddRange(ValidateCells(cells, result.Tables[GoodsIssueExcel.HeaderDataSheetName], GoodsIssueExcel.HeaderDataNameCells));

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

                    foreach (DataRow row in result.Tables[GoodsIssueExcel.HeaderDataSheetName].Rows)
                    {
                        string key = row[GoodsIssueExcel.HeaderDataColumn.Name.ToString()].ToString();
                        object value = row[GoodsIssueExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out GoodsIssueExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case GoodsIssueExcel.HeaderDataNameCell.Date:
                                if (!IsDateValid(value.ToString(), out date))
                                {
                                    errors.Add(key + " in sheet " + GoodsIssueExcel.HeaderDataSheetName + " is not in valid format.");
                                }
                                break;
                        }
                    }

                    // Because header row has index = 1 in excel sheet
                    int index = 2; // In excel sheet

                    foreach (DataRow row in result.Tables[GoodsIssueExcel.LineItemsSheetName].Rows)
                    {
                        // Quantity
                        var quantity = row[GoodsIssueExcel.LineItemsColumn.Quantity.ToString()];
                        errors.AddRange(ValidateValueType(quantity.ToString(), result.Tables[GoodsIssueExcel.LineItemsSheetName], "Integer"));

                        // Date
                        var value = row[GoodsIssueExcel.LineItemsColumn.RequiredByDate.ToString()];
                        DateTime requiredByDate = new DateTime();

                        if (!IsDateValid(value.ToString(), out requiredByDate))
                        {
                            errors.Add(GoodsIssueExcel.LineItemsColumn.RequiredByDate.ToString() + " at row " + index + " is invalid");
                        }

                        index++;
                    }

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    foreach (DataRow row in result.Tables[GoodsIssueExcel.LineItemsSheetName].Rows)
                    {
                        // Material
                        var materialNumber = row[GoodsIssueExcel.LineItemsColumn.MaterialNumber.ToString()].ToString();
                        var quantity = Convert.ToInt32(row[GoodsIssueExcel.LineItemsColumn.Quantity.ToString()]);

                        var material = _context.Materials.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                        if (material == null)
                        {
                            errors.Add(materialNumber + " does not exist in Master.");
                        }
                        else if (material.Quantity < quantity)
                        {
                            errors.Add(materialNumber + " has " + material.Quantity + " quantity available. The issued quantity amount is " + quantity);
                        }
                    }
                }
            }

            return errors;
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
                    errors.AddRange(ValidateSheets(result.Tables, SalesOrderExcel.Sheets));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Column Validation
                    errors.AddRange(ValidateColumns(SalesOrderExcel.HeaderDataColumns, result.Tables[SalesOrderExcel.HeaderDataSheetName]));
                    errors.AddRange(ValidateColumns(SalesOrderExcel.LineItemsColumns, result.Tables[SalesOrderExcel.LineItemsSheetName]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Cell Validation
                    var cells = result.Tables[SalesOrderExcel.HeaderDataSheetName].GetCellsForColumn(0, false);

                    errors.AddRange(ValidateCells(cells, result.Tables[SalesOrderExcel.HeaderDataSheetName], SalesOrderExcel.HeaderDataNameCells));

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

                    foreach (DataRow row in result.Tables[SalesOrderExcel.HeaderDataSheetName].Rows)
                    {
                        string key = row[SalesOrderExcel.HeaderDataColumn.Name.ToString()].ToString();
                        object value = row[SalesOrderExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out SalesOrderExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case SalesOrderExcel.HeaderDataNameCell.ExpirationDate:
                                if (!IsDateValid(value.ToString(), out expirationDate))
                                {
                                    errors.Add(key + " in sheet " + SalesOrderExcel.HeaderDataSheetName + " is not in valid format.");
                                }
                                break;
                            case SalesOrderExcel.HeaderDataNameCell.OrderDate:
                                if (!IsDateValid(value.ToString(), out orderDate))
                                {
                                    errors.Add(key + " in sheet " + SalesOrderExcel.HeaderDataSheetName + " is not in valid format.");
                                }
                                break;
                        }
                    }

                    if (expirationDate != DateTime.MinValue && orderDate != DateTime.MinValue)
                    {
                        if (!CompareDates(expirationDate, orderDate))
                        {
                            errors.Add("ExpirationDate is lesser than OrderDate in sheet " + SalesOrderExcel.HeaderDataSheetName);
                        }
                    }

                    // Because header row has index = 1 in excel sheet
                    int index = 2; // In excel sheet

                    foreach (DataRow row in result.Tables[SalesOrderExcel.LineItemsSheetName].Rows)
                    {
                        string unitPrice = row[SalesOrderExcel.LineItemsColumn.UnitPrice.ToString()].ToString();
                        errors.AddRange(ValidateValueType(unitPrice, result.Tables[SalesOrderExcel.LineItemsSheetName], "Double"));

                        var quantity = row[SalesOrderExcel.LineItemsColumn.Quantity.ToString()];
                        errors.AddRange(ValidateValueType(quantity.ToString(), result.Tables[SalesOrderExcel.LineItemsSheetName], "Integer"));

                        var value = row[SalesOrderExcel.LineItemsColumn.DeliveryDate.ToString()];
                        DateTime deliveryDate = new DateTime();
                        if (!IsDateValid(value.ToString(), out deliveryDate))
                        {
                            errors.Add(SalesOrderExcel.LineItemsColumn.DeliveryDate.ToString() + " at row " + index + " is invalid.");
                        }
                        index++;
                    }
                }
            }

            return errors;
        }

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

                    foreach (DataRow row in result.Tables[PurchaseRequestExcel.HeaderDataSheetName].Rows)
                    {
                        var key = row[PurchaseRequestExcel.HeaderDataColumn.Name.ToString()].ToString();
                        var value = row[PurchaseRequestExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out PurchaseRequestExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case PurchaseRequestExcel.HeaderDataNameCell.Date:
                                DateTime date;
                                DateTime.TryParse(value.ToString(), out date);
                                purchaseRequest.Date = date;
                                break;
                            case PurchaseRequestExcel.HeaderDataNameCell.Remarks:
                                purchaseRequest.Remarks = value.ToString();
                                break;
                            case PurchaseRequestExcel.HeaderDataNameCell.SalesOrderReferences:
                                var salesOrderKeys = value.ToString().Split(',');
                                var salesOrders = _context.SalesOrders.ToList();

                                foreach (var so in salesOrders)
                                {
                                    if (salesOrderKeys.Contains(so.Key))
                                    {
                                        purchaseRequest.Links.Add(new Link
                                        {
                                            SalesOrder = so,
                                            PurchaseRequest = purchaseRequest
                                        });
                                    }
                                }
                                break;
                        }
                    }

                    foreach (DataRow row in result.Tables[PurchaseRequestExcel.LineItemsSheetName].Rows)
                    {
                        var materialNumber = row[PurchaseRequestExcel.LineItemsColumn.MaterialNumber.ToString()].ToString();
                        var number = row[PurchaseRequestExcel.LineItemsColumn.ItemNumber.ToString()].ToString();
                        var quantity = Convert.ToInt32(row[PurchaseRequestExcel.LineItemsColumn.Quantity.ToString()]);
                        var requiredByDate = DateTime.Parse(row[PurchaseRequestExcel.LineItemsColumn.RequiredByDate.ToString()].ToString());
                        var description = row[PurchaseRequestExcel.LineItemsColumn.Description.ToString()].ToString();

                        Material material = _context.Materials.Local.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                        if (material == null)
                        {
                            material = _context.Materials.Where(m => m.Number.Equals(materialNumber)).SingleOrDefault();

                            if (material == null)
                            {
                                material = _materialRepository.CreateNewWineModel(new MaterialInsertOptions { Number = materialNumber, Description = description });
                            }
                        }

                        var purchaseRequestItem = new PurchaseRequestItem
                        {
                            MaterialId = material.Id,
                            Number = number,
                            Quantity = quantity,
                            Date = requiredByDate
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
                    errors.AddRange(ValidateSheets(result.Tables, new List<string> { PurchaseRequestExcel.HeaderDataSheetName, PurchaseRequestExcel.LineItemsSheetName }));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Column Validation
                    errors.AddRange(ValidateColumns(PurchaseRequestExcel.HeaderDataColumns, result.Tables[PurchaseRequestExcel.HeaderDataSheetName]));
                    errors.AddRange(ValidateColumns(PurchaseRequestExcel.LineItemsColumns, result.Tables[PurchaseRequestExcel.LineItemsSheetName]));

                    if (errors.Count > 0)
                    {
                        return errors;
                    }

                    // Cell Validation
                    var cells = result.Tables[PurchaseRequestExcel.HeaderDataSheetName].GetCellsForColumn(0, false);

                    errors.AddRange(ValidateCells(cells, result.Tables[PurchaseRequestExcel.HeaderDataSheetName], PurchaseRequestExcel.HeaderDataNameCells));

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

                    foreach (DataRow row in result.Tables[PurchaseRequestExcel.HeaderDataSheetName].Rows)
                    {
                        string key = row[PurchaseRequestExcel.HeaderDataColumn.Name.ToString()].ToString();
                        object value = row[PurchaseRequestExcel.HeaderDataColumn.Value.ToString()];

                        Enum.TryParse(key, out PurchaseRequestExcel.HeaderDataNameCell cellName);

                        switch (cellName)
                        {
                            case PurchaseRequestExcel.HeaderDataNameCell.Date:
                                if (!IsDateValid(value.ToString(), out date))
                                {
                                    errors.Add(key + " in sheet " + PurchaseRequestExcel.HeaderDataSheetName + " is not in valid format.");
                                }
                                break;
                        }
                    }

                    // Because header row has index = 1 in excel sheet
                    int index = 2; // In excel sheet

                    foreach (DataRow row in result.Tables[PurchaseRequestExcel.LineItemsSheetName].Rows)
                    {
                        var quantity = row[PurchaseRequestExcel.LineItemsColumn.Quantity.ToString()];
                        errors.AddRange(ValidateValueType(quantity.ToString(), result.Tables[PurchaseRequestExcel.LineItemsSheetName], "Integer"));

                        var lineNumber = row[PurchaseRequestExcel.LineItemsColumn.ItemNumber.ToString()];
                        errors.AddRange(ValidateValueType(lineNumber.ToString(), result.Tables[PurchaseRequestExcel.LineItemsSheetName], "Integer"));

                        var value = row[PurchaseRequestExcel.LineItemsColumn.RequiredByDate.ToString()];
                        DateTime requiredByDate = new DateTime();

                        if (!IsDateValid(value.ToString(), out requiredByDate))
                        {
                            errors.Add(PurchaseRequestExcel.LineItemsColumn.RequiredByDate.ToString() + " at row " + index + " is invalid");
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
            catch (FormatException)
            {
                resultDate = DateTime.MinValue;
                return false;
            }
        }

        public List<string> ValidateCells(List<string> cells, DataTable sheet, List<string> cellNames)
        {
            List<string> errors = new List<string>();

            cellNames.ForEach(delegate (string cell)
            {
                if (!cells.Contains(cell))
                {
                    errors.Add("Cell " + cell + " is not present in sheet " + sheet.TableName);
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