using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Innovic.Infrastructure
{
    public static class SalesOrderExtension
    {
        public static List<string> GetCellsForColumn(this DataTable table, int index)
        {
            List<string> cells = new List<string>();
            
            for(int i = 0; i < table.Rows.Count; i++)
            {
                if(i > 0)
                {
                    cells.Add(table.Rows[i][index].ToString());
                }
            }

            return cells;
        }

    }
}