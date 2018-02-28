﻿using Red.Wine.Picker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovic.App
{
    public static class PickConfigurations
    {
        public static List<Pick> UserData
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
                    }))
                };
            }
        }

        public static PickConfig Default
        {
            get
            {
                return new PickConfig(true, true, UserData);
            }
        }
        public static PickConfig SalesOrders
        {
            get
            {
                return new PickConfig(true, true, new List<Pick>
                {
                    new Pick("Customer", new PickConfig(false, true, new List<Pick>(UserData)
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
                return new PickConfig(true, true, new List<Pick>(UserData)
                {
                    new Pick("Customer", new PickConfig(false, true, new List<Pick>
                    {
                        new Pick("Name")
                    })),
                    new Pick("SalesOrderItems", new PickConfig(true, true, new List<Pick>
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