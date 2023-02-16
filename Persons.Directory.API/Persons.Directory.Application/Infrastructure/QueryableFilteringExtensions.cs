using Application.Infrastructure.Enums;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Infrastructure;

public static class QueryableFilteringExtensions
{
    public static IQueryable<TSource> PageAndSort<TSource, TProp>(this IQueryable<TSource> source, IPagedQuery request, Dictionary<string, Expression<Func<TSource, TProp>>> columnsMap)
    {
        if (request == null)
        {
            return source;
        }

        // Default values
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 25;
        var sortBy = request.SortBy ?? "Id";
        var isAsc = request.SortOrder == SortOrder.Asc;

        var expression = columnsMap[sortBy];
        var unaryExpression = expression.Body as UnaryExpression;

        if (unaryExpression == null)
        {
            return isAsc ? source.OrderBy(expression) : source.OrderByDescending(expression);
        }

        var propertyExpression = (MemberExpression)unaryExpression.Operand;
        var parameters = expression.Parameters;

        if (propertyExpression.Type == typeof(DateTime))
        {
            var newExpression = Expression.Lambda<Func<TSource, DateTime>>(propertyExpression, parameters);
            return isAsc ? source.OrderBy(newExpression) : source.OrderByDescending(newExpression);
        }

        if (propertyExpression.Type == typeof(DateTime?))
        {
            var newExpression = Expression.Lambda<Func<TSource, DateTime?>>(propertyExpression, parameters);
            return isAsc ? source.OrderBy(newExpression) : source.OrderByDescending(newExpression);
        }

        if (propertyExpression.Type == typeof(int))
        {
            var newExpression = Expression.Lambda<Func<TSource, int>>(propertyExpression, parameters);
            return isAsc ? source.OrderBy(newExpression) : source.OrderByDescending(newExpression);
        }

        if (propertyExpression.Type == typeof(decimal))
        {
            var newExpression = Expression.Lambda<Func<TSource, decimal>>(propertyExpression, parameters);
            return isAsc ? source.OrderBy(newExpression) : source.OrderByDescending(newExpression);
        }

        if (propertyExpression.Type == typeof(decimal?))
        {
            var newExpression = Expression.Lambda<Func<TSource, decimal?>>(propertyExpression, parameters);
            return isAsc ? source.OrderBy(newExpression) : source.OrderByDescending(newExpression);
        }

        if (propertyExpression.Type == typeof(double))
        {
            var newExpression = Expression.Lambda<Func<TSource, double>>(propertyExpression, parameters);
            return isAsc ? source.OrderBy(newExpression) : source.OrderByDescending(newExpression);
        }

        if (propertyExpression.Type == typeof(double?))
        {
            var newExpression = Expression.Lambda<Func<TSource, double?>>(propertyExpression, parameters);
            return isAsc ? source.OrderBy(newExpression) : source.OrderByDescending(newExpression);
        }

        if (propertyExpression.Type.IsEnum)
        {
            return isAsc ? source.OrderBy(expression) : source.OrderByDescending(expression);
        }

        return source
            .Skip(pageSize * (page - 1)).Take(pageSize);
    }

    public static IQueryable<TSource> SortAndPage<TSource>(this IQueryable<TSource> source, IPagedQuery request, string sortThenBy = "")
        where TSource : class
    {
        return Page(source.Sort(request, sortThenBy), request);
    }

    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, IPagedQuery request)
            where TSource : class
    {
        if (request == null || !request.Page.HasValue || !request.PageSize.HasValue)
        {
            return source;
        }

        return source.Skip(request.PageSize.Value * (request.Page.Value - 1)).Take(request.PageSize.Value);
    }

    public static IQueryable<TSource> Sort<TSource>(this IQueryable<TSource> source, IPagedQuery request, string sortThenBy = "")
        where TSource : class
    {
        if (request != null && request.SortOrder.HasValue)
        {
            return source.Sort(request.SortBy, request.SortOrder.Value, sortThenBy);
        }

        return source;
    }

    public static IQueryable<TSource> Sort<TSource>(this IQueryable<TSource> source, string sortBy, SortOrder sortOrder, string sortThenBy = "")
        where TSource : class
    {
        string sort;

        if (!string.IsNullOrWhiteSpace(sort = string.Format("{0} {1}", sortBy, sortOrder)))
        {
            var orderedSoruce = source.OrderBy(sort);

            if (!string.IsNullOrEmpty(sortThenBy))
            {
                string sortThenByQuery = string.Format("{0} {1}", sortThenBy, sortOrder);

                return orderedSoruce.ThenBy(sortThenByQuery);
            }

            return orderedSoruce;
        }

        return source;
    }

    public static IQueryable<TSource> SortQuery<TSource>(this IQueryable<TSource> source, string sortBy, SortOrder sortOrder)
        where TSource : class
    {
        if (!string.IsNullOrEmpty(sortBy))
        {
            string sortOrderMethod = sortOrder == SortOrder.Desc ? "OrderByDescending" : "OrderBy";

            return source.ApplyOrder(sortBy, sortOrderMethod);
        }

        return source;
    }

    public static IOrderedQueryable<TSource> ApplyOrder<TSource>(this IQueryable<TSource> source,
     string property,
     string methodName)
    {
        string[] props = property.Split('.');
        Type type = typeof(TSource);
        ParameterExpression arg = Expression.Parameter(type, "x");
        Expression expr = arg;
        foreach (string prop in props)
        {
            // use reflection (not ComponentModel) to mirror LINQ
            PropertyInfo pi = type.GetProperty(prop);
            expr = Expression.Property(expr, pi);
            type = pi.PropertyType;
        }

        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(TSource), type);
        LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

        object result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TSource), type)
                .Invoke(null, new object[] { source, lambda });

        return (IOrderedQueryable<TSource>)result;
    }

    public static IQueryable<TSource> And<TSource, TFilter>(this IQueryable<TSource> source, TFilter? filter, Expression<Func<TSource, bool>> predicate)
        where TFilter : struct
    {
        if (predicate == null)
        {
            throw new ArgumentNullException("predicate");
        }

        if (filter.HasValue)
        {
            return source.And(predicate);
        }

        return source;
    }

    public static IQueryable<TSource> And<TSource>(this IQueryable<TSource> source, string filter, Expression<Func<TSource, bool>> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException("predicate");
        }

        if (!string.IsNullOrWhiteSpace(filter))
        {
            return source.And(predicate);
        }

        return source;
    }

    public static IQueryable<TSource> And<TSource>(this IQueryable<TSource> source, bool filter, Expression<Func<TSource, bool>> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException("predicate");
        }

        if (filter)
        {
            return source.And(predicate);
        }

        return source;
    }

    public static IQueryable<TSource> And<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException("predicate");
        }

        return source.Where(predicate);
    }

    public static IQueryable<TSource> ContainsIgnoreCase<TSource>(this IQueryable<TSource> source, string candidate, params string[] columnNames)
    {
        Expression<Func<TSource, bool>> predicate = null;

        if (!string.IsNullOrEmpty(candidate))
        {
            var parameterExp = Expression.Parameter(typeof(TSource), "x");

            var lowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var value = Expression.Constant(candidate.ToLower(), typeof(string));

            var containsMethodExpressions = new List<MethodCallExpression>();

            foreach (var columnName in columnNames)
            {
                Expression propInfo = null;

                foreach (var item in columnName.Split('.'))
                {
                    propInfo = propInfo == null
                        ? Expression.Property(parameterExp, item)
                        : Expression.Property(propInfo, item);
                }

                var lowerMethodExp = Expression.Call(propInfo, lowerMethod);
                var containsMethodExp = Expression.Call(lowerMethodExp, containsMethod, value);

                containsMethodExpressions.Add(containsMethodExp);
            }

            Expression conditionExpression = null;

            foreach (var methodCallExpression in containsMethodExpressions)
            {
                if (conditionExpression == null)
                {
                    conditionExpression = methodCallExpression;
                }
                else
                {
                    conditionExpression = Expression.OrElse(methodCallExpression, conditionExpression);
                }
            }

            predicate = Expression.Lambda<Func<TSource, bool>>(conditionExpression, parameterExp);
        }

        return predicate == null ? source : And(source, predicate);
    }
}
