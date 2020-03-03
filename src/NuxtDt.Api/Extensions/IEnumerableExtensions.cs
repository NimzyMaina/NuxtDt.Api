using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DataTables.AspNet.Core;

namespace NuxtDt.Api.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Compute<T>(this IEnumerable<T> data, IDataTablesRequest request, out int filteredDataCount)
        {
            filteredDataCount = 0;
            if (!data.Any() || request == null)
                return data;

            // Global filtering.
            // Filter is being manually applied due to in-memmory (IEnumerable) data.
            // If you want something rather easier, check IEnumerableExtensions Sample.
            IEnumerable<T> filteredData = Enumerable.Empty<T>();
            
            if (!String.IsNullOrEmpty(request.Search.Value))
            {
                var filteredColumn = request.Columns.Where(c => c.IsSearchable);
                Console.WriteLine($"Searchable count {filteredColumn.Count()}");
                foreach (IColumn sColumn in filteredColumn)
                {
                    var propertyInfo = data.First().GetType().GetProperty(sColumn.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    IEnumerable<T> columnResult = data.PropertyContains(propertyInfo, request.Search.Value);
                    filteredData = filteredData.Concat(columnResult);
                }
                // Pour éviter les doublons
                filteredData = filteredData.Distinct();
            }
            else filteredData = data;

            // Ordering filtred data
            var orderedColumn = request.Columns.Where(c => c.IsSortable == true && c.Sort != null);
            foreach (IColumn sColumn in orderedColumn)
            {
                filteredData = filteredData.OrderBy(sColumn);
            }

            // Paging filtered data.
            // Paging is rather manual due to in-memmory (IEnumerable) data.
            // var dataPage = filteredData.OrderBy(d => d.ID).Skip(request.Start);
            var dataPage = filteredData.Skip(request.Start);
            if (request.Length != -1) dataPage = dataPage.Take(request.Length);

            filteredDataCount = filteredData.Count();
            return dataPage;
        }
        
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, IColumn column)
        {
            if (!entities.Any() || column == null)
                return entities;

            var propertyInfo = entities.First().GetType().GetProperty(column.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (column.Sort.Direction == SortDirection.Ascending)
            {
                return entities.OrderBy(e => propertyInfo.GetValue(e, null));
            }
            return entities.OrderByDescending(e => propertyInfo.GetValue(e, null));
        }
        
        public static IEnumerable<T> PropertyContains<T>(this IEnumerable<T> data, PropertyInfo propertyInfo, string value)
        {
//            ParameterExpression param = Expression.Parameter(typeof(T));
//            MemberExpression m = Expression.MakeMemberAccess(param, propertyInfo);
//            ConstantExpression c = Expression.Constant(value.ToLower(), typeof(string));
//            MethodInfo mi_contains = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
//            MethodInfo mi_tostring = typeof(object).GetMethod("ToString");
//            MethodInfo mi_tolower = typeof(string).GetMethod("ToLower",new Type[] { typeof(string) });
//            Expression.Call(m, mi_tostring);
//            Expression call = Expression.Call(Expression.Call(m, mi_tostring), mi_contains, c);
//
//            Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(call, param);

            var lambda = CreateLike<T>(propertyInfo, value);

            return data.AsQueryable().Where(lambda);
        }
        
        private static Expression<Func<T, bool>> CreateLike<T>(PropertyInfo prop, 
            string value, 
            StringComparison comparison=StringComparison.InvariantCultureIgnoreCase)
        {
            var parameter = Expression.Parameter(typeof(T), "f");
            MethodInfo mi_tostring = typeof(object).GetMethod("ToString");
            var m = Expression.MakeMemberAccess(parameter, prop);

            var indexOf = Expression.Call(Expression.Call(m, mi_tostring), "IndexOf", null, 
                Expression.Constant(value, typeof(string)),
                Expression.Constant(comparison));
            var like=Expression.GreaterThanOrEqual(indexOf, Expression.Constant(0));
            return Expression.Lambda<Func<T, bool>>(like, parameter);
        }

    }
}