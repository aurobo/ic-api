using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Innovic.Infrastructure
{
    public static class ExcelManagerExtensions
    {
        public static List<string> GetCellsForColumn(this DataTable table, int index, bool hasHeaderRow)
        {
            List<string> cells = new List<string>();

            for(int i = 0; i < table.Rows.Count; i++)
            {
                if(!hasHeaderRow && i == 0)
                {
                    continue;
                }

                cells.Add(table.Rows[i][index].ToString());
            }

            return cells;
        }

        public static List<string> GetCellsForColumn(this DataTable table, string name)
        {
            List<string> cells = new List<string>();

            foreach(DataRow row in table.Rows)
            {
                cells.Add(row[name].ToString());
            }

            return cells;
        }
    }
}