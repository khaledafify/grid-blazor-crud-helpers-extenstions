using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class SortingUtility
    {
        
        public enum SortOrders
        {
            Asc = 1,
            Desc = 2
        }

        public class SortingParams
        {
            public SortOrders SortOrder { get; set; } = SortOrders.Asc;
            public string ColumnName { get; set; }

            public string GetSqlOrderName()
            {
                return this.SortOrder == SortOrders.Asc ? "ASC" : "DESC";
            }

        }



    }

}
