using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ConsoleApp1.FilterUtility;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            var filterQuery = "OrderID__1__10249,OrderID__1__10248,OrderID__9__2";

            var sortingQuery = new SortingUtility.SortingParams();
            sortingQuery.SortOrder = SortingUtility.SortOrders.Asc;
            sortingQuery.ColumnName = "OrderID";

            var result = BuildFilterParams(filterQuery);

            var sqlQuery = @"SELECT * FROM ORDERS "
                .FilterData<Order>(result)
                .ToSortedResult<Order>(sortingQuery)
                .ToPagedQuery(10,20)
                .ToString();



            Console.WriteLine(sqlQuery);

            var x =  0;
        }

        public static List<FilterParam> BuildFilterParams(string filterQuery)
        {
            #region Initial Mapping
            //split filterQuery string into list contains all filter queries
            List<string> filterQueryList = filterQuery.Split(',').ToList<string>();


            // filters contain every unique column with all of it's filters             
            Dictionary<string, List<FilterValues>> filtersDict = new Dictionary<string, List<FilterValues>>();
            #endregion


            #region Extract Applied Filters
            // iterate over filterQueries to assign every applied filter to it's unique column
            foreach (var item in filterQueryList)
            {

                /**
                 * split filter parts
                 * [0] ClassName.PropertyName {Customer.CompanyName} 
                 * [1] FilterOption Enum ... {Equals,StartsWith} 
                 * [2] Filter Value ...{2,3} 
                **/
                var singleQueryPartsList = item.Split("__").ToList<string>();


                /*[0] get the last part of propertyName ClassName.PropertyName 
                 *it will result PropertyName
                 * example Customer.CompanyName will result CompanyName
                 */
                var colName = singleQueryPartsList[0].Split(".").Last();


                /**
                 * check if the columnName isn't exist in our dictionary 
                 * initial filtersList to prevent null exceptions
                 */
                if (!filtersDict.ContainsKey(colName))
                {
                    filtersDict.Add(colName, new List<FilterValues>());
                }

                //update dictionary and append new filter 
                filtersDict[colName] = filtersDict[colName].Concat(new[] { new FilterValues(singleQueryPartsList[1], singleQueryPartsList[2]) }).ToList();
            }

            #endregion


            #region Build Filter Params
            var filterParams = new List<FilterParam>();
            foreach (var item in filtersDict.Keys)
            {
                filterParams.Add(new FilterParam(item, filtersDict[item]));
            } 
            #endregion

            return filterParams;
        }

    }


}
