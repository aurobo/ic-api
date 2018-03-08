﻿using ExcelDataReader;
using Innovic.App;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.Options;
using System;
using System.Collections.Generic;
using System.Data;
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

                    if(errors.Count > 0)
                    {
                        return errors;
                    }

                    // Columns Validation

                    #region -- Sheets Validation --
                    if(!result.Tables.Contains(SalesOrderExcel.HeaderDataSheet))
                    {
                        errors.Add("Does not contain " + SalesOrderExcel.HeaderDataSheet + " sheet.");
                    }

                    if (!result.Tables.Contains(SalesOrderExcel.LineItemsSheet))
                    {
                        errors.Add("Does not contain " + SalesOrderExcel.HeaderDataSheet + " sheet.");
                    }
                    #endregion -- Sheets Validation --

                    #region -- Columns Validation --
                    SalesOrderExcel.HeaderDataColumns.ForEach(delegate(string column) {
                        if(!result.Tables[SalesOrderExcel.HeaderDataSheet].Rows[0].ItemArray.Contains(column))
                        {
                            errors.Add("Column " + column + " is not present in sheet " + SalesOrderExcel.HeaderDataSheet);
                        }
                    });

                    SalesOrderExcel.LineItemsColumns.ForEach(delegate (string column) {
                        if (!result.Tables[SalesOrderExcel.LineItemsSheet].Rows[0].ItemArray.Contains(column))
                        {
                            errors.Add("Column " + column + " is not present in sheet " + SalesOrderExcel.LineItemsSheet);
                        }
                    });
                    #endregion -- Columns Validation --
                }
            }

            return errors;
        }

        public List<string> ValidateSheets(DataTableCollection sheets, List<string> sheetNames)
        {
            List<string> errors = new List<string>();

            foreach(var sheetName in sheetNames)
            {
                if (!sheets.Contains(sheetName))
                {
                    errors.Add("Does not contain " + sheetName + " sheet.");
                }
            }

            return errors;
        }

        public List<string> ValidateSheet(DataTableCollection sheets, string sheetName)
        {
            List<string> errors = new List<string>();

            if (!sheets.Contains(sheetName))
            {
                errors.Add("Does not contain " + sheetName + " sheet.");
            }

            return errors;
        }

        public List<string> ValidateColumns(DataTable sheet, List<string> columns)
        {
            List<string> errors = new List<string>();

            foreach (var column in columns)
            {
                if (!sheet.Rows[0].ItemArray.Contains(column))
                {
                    errors.Add("Column " + column + " is not present in sheet " + sheet.TableName);
                }
            }

            return errors;
        }
    }
}