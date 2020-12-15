using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static ConsoleApp1.FilterUtility;
using static ConsoleApp1.SortingUtility;

namespace ConsoleApp1
{
    public static class SqlServerHelper
    {
        public static string ToPagedQuery(this string query, int offset = 0, int next = 20)
        {

            if(query.Contains("ROW FETCH NEXT"))
            {
                throw new Exception("Pagination can't applied twice");
            }

            return new StringBuilder(query)
                .Append($"OFFSET {offset} ROWS FETCH NEXT {next} ROWS ONLY")
                .Append("\n")
                .ToString();
        }

        public static string ToSortedResult<T>(this string query ,SortingParams sortingParams)
        {
            if (query.Contains("ROW FETCH NEXT") )
            {
                throw new Exception("Pagination can't applied before sorting");
            }

            if (query.Contains("ORDER BY"))
            {
                throw new NotSupportedException("Multiple Sorting Not Supported");
            }

            if (sortingParams == null || string.IsNullOrWhiteSpace(sortingParams.ColumnName))
            {
                return query; 
            }

            PropertyInfo sortColumn = typeof(T).GetProperty(sortingParams.ColumnName);

            if(sortColumn == null)
            {
                throw new Exception($"{sortingParams} not exist in {typeof(T).GetType().Name}");
            }



            return new StringBuilder(query)
                .Append($"ORDER BY {sortingParams.ColumnName} ")
                .Append($"{sortingParams.GetSqlOrderName()}")
                .Append("\n")
                .ToString();
        }

        public static string FilterData<T>(this string query, List<FilterParam> filterParams)
        {
            if (query.Contains("ROW FETCH NEXT"))
            {
                throw new Exception("Pagination can't applied before sorting");
            }

            if (query.Contains("WHERE"))
            {
                throw new Exception("Where CAN'T DUPLICATED");
            }



            if (query.Contains("ORDER BY"))
            {
                throw new Exception("WHERE CLAUSE IS APPLIED BEFORE ORDER CLAUSE ONLY");
            }

            int outValue;
            DateTime dateValue;
            Boolean boolValue;

            StringBuilder filterQueryBuilder = new StringBuilder(query)
                .Append(" WHERE ")
                ;



            if (filterParams != null && filterParams.Count > 0)
            {


                for (int i = 0; i < filterParams.Count; i++)
                {
                    List<string> filters = new List<string>();

                    var filterParam = filterParams[i];
                    PropertyInfo filterColumn = typeof(T).GetProperty(filterParam.ColumnName);

                    if (filterColumn != null && filterParam.Filters != null && filterParam.Filters.Count > 0)
                    {

                        for (int j = 0; j < filterParam.Filters.Count; j++)
                        {

                            if (j == 0)
                            {

                            }
                            var item = filterParam.Filters[j];
                            switch (item.FilterOption)
                            {
                                #region [StringDataType]  

                                case FilterOptions.StartsWith:
                                    filters.Add($"{filterParam.ColumnName} like '{item.FilterValue}%' ");
                                    break;
                                case FilterOptions.EndsWith:
                                    filters.Add($"{filterParam.ColumnName} like '%{item.FilterValue}' ");
                                    break;
                                case FilterOptions.Contains:
                                    filters.Add($"{filterParam.ColumnName} like '%{item.FilterValue}%' ");
                                    break;
                                case FilterOptions.IsNull:
                                    filters.Add($"{filterParam.ColumnName} IS NULL ");
                                    break;
                                case FilterOptions.IsNotNull:
                                    filters.Add($"{filterParam.ColumnName} IS NOT NULL ");
                                    break;
                                #endregion

                                #region [Custom]  

                                case FilterOptions.GreaterThan:
                                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(item.FilterValue, out outValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} > {outValue} ");
                                    }
                                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(item.FilterValue, out dateValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} > {Convert.ToDateTime(item.FilterValue)}");
                                    }
                                    break;

                                case FilterOptions.GreaterThanOrEquals:
                                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(item.FilterValue, out outValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} >= {item.FilterValue} ");
                                    }
                                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(item.FilterValue, out dateValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} >= {Convert.ToDateTime(item.FilterValue)}");
                                    }
                                    break;

                                case FilterOptions.LessThan:
                                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(item.FilterValue, out outValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} < {item.FilterValue} ");
                                    }
                                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(item.FilterValue, out dateValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} < {Convert.ToDateTime(item.FilterValue)}");
                                    }
                                    break;

                                case FilterOptions.LessThanOrEquals:
                                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(item.FilterValue, out outValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} <= {item.FilterValue} ");
                                    }
                                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(item.FilterValue, out dateValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} <= {Convert.ToDateTime(item.FilterValue)}");
                                    }
                                    break;

                                case FilterOptions.Equals:
                                    if (item.FilterValue == string.Empty)
                                    {
                                        filters.Add($"{filterParam.ColumnName} = '' ");
                                    }
                                    else
                                    {
                                        if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(item.FilterValue, out outValue))
                                        {
                                            filters.Add($"{filterParam.ColumnName} = {item.FilterValue} ");
                                        }
                                        else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(item.FilterValue, out dateValue))
                                        {
                                            filters.Add($"{filterParam.ColumnName} = {Convert.ToDateTime(item.FilterValue)} ");
                                        }
                                        else if (filterColumn.PropertyType == typeof(bool) && Boolean.TryParse(item.FilterValue, out boolValue))
                                        {
                                            filters.Add($"{filterParam.ColumnName} = {Convert.ToInt32(boolValue)} ");

                                        }
                                        else
                                        {
                                            filters.Add($"{filterParam.ColumnName} = {item.FilterValue} ");
                                        }
                                    }
                                    break;

                                case FilterOptions.NotEquals:
                                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(item.FilterValue, out outValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} != {item.FilterValue} ");
                                    }
                                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(item.FilterValue, out dateValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} != {Convert.ToDateTime(dateValue)} ");
                                    }
                                    else if (filterColumn.PropertyType == typeof(bool) && Boolean.TryParse(item.FilterValue, out boolValue))
                                    {
                                        filters.Add($"{filterParam.ColumnName} != {Convert.ToInt32(boolValue)} ");

                                    }
                                    else
                                    {
                                        filters.Add($"{filterParam.ColumnName} != {item.FilterValue} ");
                                    }
                                    break;
                                    #endregion
                            }

                        }

                        var queryNotation = filterParam.Notaion == FilterNotaion.AND ? " AND " : " OR ";

                        var columnQuery = string.Join(queryNotation, filters);
                        filterQueryBuilder
                            .Append(" ")
                            .Append(columnQuery)
                            .Append(" ")
                            .Append("\n");
                    }

                }
            }


            return filterQueryBuilder.ToString().EndsWith(" WHERE ") ? query : filterQueryBuilder.ToString();
        }
    }
}
