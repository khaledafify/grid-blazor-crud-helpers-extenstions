using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class FilterUtility
    {

        /// <summary>  
        /// Enums for filter options  
        /// same sequence UI is following  
        /// </summary>  
        public enum FilterOptions
        {
            Equals = 1,
            Contains = 2,
            StartsWith = 3,
            EndsWith = 4,
            GreaterThan = 5,
            LessThan = 6,
            GreaterThanOrEquals = 7,
            LessThanOrEquals = 8,
            Condition = 9,
            NotEquals = 10,
            IsNull = 11,
            IsNotNull = 12
        }

        /// <summary>  
        /// Filter parameters Model Class  
        /// </summary>  
        public class FilterParam
        {
            public string ColumnName { get; set; } = string.Empty;

            public FilterNotaion Notaion { get; set; } = FilterNotaion.NOT_SET;

            public List<FilterValues> Filters { get; set; }

            public FilterParam(string columnName, List<FilterValues> filters)
            {
                ColumnName = columnName;
                Filters = filters;

                if (filters.Count > 1)
                {


                    var conditionFilter = filters.Where(x => x.FilterOption == FilterOptions.Condition)
                        .FirstOrDefault();

                    if(conditionFilter == null)
                    {
                        throw new Exception("Condition FilterValue must be exist");
                    }

                    filters.Remove(conditionFilter);

                    Notaion = (FilterNotaion)Convert.ToInt32(conditionFilter.FilterValue);
                }
            }
        }

        public class FilterValues
        {
            public string FilterValue { get; set; } = string.Empty;
            public FilterOptions FilterOption { get; set; } = FilterOptions.Contains;

            public FilterValues (string filterOption, string filterValue)
            {
                FilterOption = (FilterOptions)Convert.ToInt32(filterOption);
                FilterValue = filterValue;
            }

        }

        public enum FilterNotaion
        {
            AND = 1,
            OR = 2 , 
            NOT_SET = 3
        }







    }

}
