using ExcelDataReader;
using Innovic.Models.Master;
using Innovic.Models.Sales;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Innovic.Services
{
    public class ExcelService
    {
        private CustomerService _customerService = new CustomerService();
        private MaterialService _materialService = new MaterialService();

        public SalesOrder ToSalesOrder(string filePath)
        {
            var salesOrder = new SalesOrder();

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration() {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    foreach (System.Data.DataRow row in result.Tables["Header Data"].Rows)
                    {
                        var key = row["Name"].ToString();
                        var value = row["Value"];

                        switch (key)
                        {
                            case "Customer":
                                var customerName = value.ToString();
                                Customer customer = _customerService.Find(customerName);

                                if (customer == null)
                                {
                                    customer = _customerService.QuickCreate(customerName);
                                }

                                salesOrder.CustomerId = customer.Id;
                                break;

                            case "ExpirationDate":
                                salesOrder.ExpirationDate = DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", null);
                                break;

                            case "OrderDate":
                                salesOrder.OrderDate = DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", null); ;
                                break;

                            case "CustomerReference":
                                salesOrder.CustomerReference = value.ToString();
                                break;

                            case "PaymentTerms":
                                salesOrder.PaymentTerms = value.ToString();
                                break;
                        }
                    }

                    foreach(System.Data.DataRow row in result.Tables["Line Items"].Rows)
                    {
                        var itemNumber = row["Item Number"].ToString();
                        var materialNumber = row["Material Number"].ToString();
                        var description = row["Description"].ToString();
                        var quantity = Convert.ToInt32(row["Quantity"]);
                        var unitPrice = Convert.ToDouble(row["Unit Price"]);

                        Material material = _materialService.Find(materialNumber);

                        if(material == null)
                        {
                            material = _materialService.QuickCreate(materialNumber, description);
                        }

                        var salesOrderItem = new SalesOrderItem
                        {
                            Number = itemNumber,
                            MaterialId = material.Id,
                            Description = string.IsNullOrEmpty(description) ? material.Description : description,
                            Quantity = quantity,
                            UnitPrice = unitPrice,
                            Value = quantity * unitPrice
                        };

                        salesOrder.SalesOrderItems.Add(salesOrderItem);
                    }
                }
            }

            return salesOrder;
        }
    }
}