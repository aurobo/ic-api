using Red.Wine.Picker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.App
{
    public static class PickConfigurations
    {
        private static List<Pick> DefaultData
        {
            get
            {
                return new List<Pick>
                {
                    new Pick("LastModifiedByUser", new PickConfig(false, true, new List<Pick>
                    {
                        new Pick("Name")
                    })),
                    new Pick("CreatedByUser", new PickConfig(false, true, new List<Pick>
                    {
                        new Pick("Name")
                    })),
                    new Pick("MetaData", new PickConfig(true, true))
                };
            }
        }

        public static PickConfig Default
        {
            get
            {
                return new PickConfig(true, true, DefaultData);
            }
        }

        public static PickConfig SalesOrders
        {
            get
            {
                return new PickConfig(true, true, new List<Pick>
                {
                    new Pick("Customer", new PickConfig(false, true, new List<Pick>(DefaultData)
                    {
                        new Pick("Name")
                    }))
                });
            }
        }

        public static PickConfig SalesOrder
        {
            get
            {
                return new PickConfig(true, true, new List<Pick>(DefaultData)
                {
                    new Pick("Customer", new PickConfig(false, true, new List<Pick>
                    {
                        new Pick("Name")
                    })),
                    new Pick("SalesOrderItems", new PickConfig(true, true, new List<Pick>(DefaultData)
                    {
                        new Pick("Material", new PickConfig(false, true, new List<Pick>
                        {
                            new Pick("Number")
                        }))
                    })),
                    new Pick("Invoices", new PickConfig(true, true, new List<Pick>(DefaultData)))
                });
            }
        }

        public static PickConfig Invoice
        {
            get
            {
                return new PickConfig(true, true, new List<Pick>(DefaultData)
                {
                    new Pick("SalesOrder", new PickConfig(false, true, new List<Pick>
                    {
                        new Pick("Customer", new PickConfig(false, true, new List<Pick>
                        {
                            new Pick("Name")
                        }))
                    })),
                    new Pick("InvoiceItems", new PickConfig(true, true, new List<Pick>(DefaultData)
                    {
                        new Pick("SalesOrderItem", new PickConfig(true, true, new List<Pick>
                        {
                            new Pick("Material", new PickConfig(false, true, new List<Pick>
                            {
                                new Pick("Number")
                            }))
                        }))
                    }))
                });
            }
        }

        public static PickConfig PurchaseOrders
        {
            get
            {
                return new PickConfig(true, true, new List<Pick>
                {
                    new Pick("Supplier", new PickConfig(false, true, new List<Pick>(DefaultData)
                    {
                        new Pick("Name")
                    }))
                });
            }
        }

        public static PickConfig PurchaseOrder
        {
            get
            {
                return new PickConfig(true, true, new List<Pick>(DefaultData)
                {
                    new Pick("Supplier", new PickConfig(false, true, new List<Pick>
                    {
                        new Pick("Name")
                    })),
                    new Pick("PurchaseOrderItems", new PickConfig(true, true, new List<Pick>(DefaultData)
                    {
                        new Pick("Material", new PickConfig(false, true, new List<Pick>
                        {
                            new Pick("Number")
                        }))
                    }))
                });
            }
        }
        
        public static PickConfig GoodsIssue
        {
            get
            {
                return new PickConfig(true, true, new List<Pick>(DefaultData)
                {
                    new Pick("Customer", new PickConfig(false, true, new List<Pick>
                    {
                        new Pick("Name")
                    })),
                    new Pick("GoodsIssueItem", new PickConfig(true, true, new List<Pick>(DefaultData)
                    {
                        new Pick("Material", new PickConfig(false, true, new List<Pick>
                        {
                            new Pick("Number")
                        }))
                    }))
                });
            }
        }
    }
}