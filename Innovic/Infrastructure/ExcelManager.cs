using ExcelDataReader;
using Innovic.App;
using Innovic.Modules.Master.Models;
using Innovic.Modules.Master.Options;
using Innovic.Modules.Sales.Models;
using Innovic.Modules.Sales.Options;
using System;
using System.IO;

namespace Innovic.Infrastructure
{
    public class ExcelManager
    {
        private readonly GenericRepository<Material> _materialRepository;
        private readonly GenericRepository<Customer> _customerRepository;

        public ExcelManager(InnovicContext context, string userId)
        {
            _materialRepository = new GenericRepository<Material>(context, userId);
            _customerRepository = new GenericRepository<Customer>(context, userId);
        }

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
                                Customer customer = _customerRepository.Find(c => c.Name.Equals(customerName));

                                if (customer == null)
                                {
                                    customer = _customerRepository.CreateNewWineModel(new CustomerInsertOptions { Name = customerName });
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

                        Material material = _materialRepository.Find(m => m.Number.Equals(materialNumber));

                        if(material == null)
                        {
                            material = _materialRepository.CreateNewWineModel(new MaterialInsertOptions { Number = materialNumber, Description = description });
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