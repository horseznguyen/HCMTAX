using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Services.Common.LinqUtils
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var seenKeys = new HashSet<TKey>();

            foreach (var element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<TSource> RemoveWhere<TSource>(this IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            return source.Where(x => !predicate(x));
        }

        /// <summary>
        ///     Continues processing items in a collection until the end condition is true.
        /// </summary>
        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> source, Predicate<T> endCondition)
        {
            return source.TakeWhile(item => !endCondition(item));
        }

        /// <summary>
        ///     Compare Through a predicate every element of a list with the previous one
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <example>
        ///     var items = new List{int} { 1, 5, 7, 3, 10, 9, 6};
        ///     var result = items.WherePrevious((first, second) => second > first); => Result is 5, 7, 10
        /// </example>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<T> WherePrevious<T>(this IEnumerable<T> collection, Func<T, T, bool> predicate)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            using (var e = collection.GetEnumerator())
            {
                e.MoveNext();

                var previous = e.Current;

                while (e.MoveNext())
                {
                    if (predicate(previous, e.Current))
                    {
                        yield return e.Current;
                    }

                    previous = e.Current;
                }
            }
        }

        /// <summary>
        ///     Random items position inside the source
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            var random = new Random();

            return source.OrderBy(x => random.Next());
        }

        /// <summary>
        ///     ToList() with an extra capacity argument. This can boost the speed of creating the list.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="capacity"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static List<TSource> ToList<TSource>(this IEnumerable<TSource> source, int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");
            }

            if (source == null)
            {
                return null;
            }

            var list = new List<TSource>(capacity);

            list.AddRange(source);

            return list;
        }

        #region Dynamic LINQ OrderBy on IEnumerable<T> / IQueryable<T>

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');

            Type type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;

            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);

                expr = Expression.Property(expr, pi ?? throw new InvalidOperationException());

                type = pi.PropertyType;
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);

            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                              && method.IsGenericMethodDefinition
                              && method.GetGenericArguments().Length == 2
                              && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        #endregion Dynamic LINQ OrderBy on IEnumerable<T> / IQueryable<T>

        #region Paging query

        public static IQueryable<T> GetPaged<T>(this IQueryable<T> query, int skipItem, int pageSize)
        {
            if (skipItem < 0 || pageSize < 0 || pageSize == 0) return query;

            return query.Skip(skipItem).Take(pageSize);
        }

        #endregion Paging query
    }
}