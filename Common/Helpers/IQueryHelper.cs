using System.Linq.Expressions;
namespace Common.Helpers;
public static class IQueryHelper
{
    public static IQueryable<T> SortOrderBy<T>(this IQueryable<T> source, string? columnName, bool isAscending = true)
    {
        if (String.IsNullOrEmpty(columnName))
        {
            return source;
        }

        ParameterExpression parameter = Expression.Parameter(source.ElementType, "");
        MemberExpression property = Expression.Property(parameter, columnName);
        LambdaExpression lambda = Expression.Lambda(property, parameter);
        string methodName = isAscending ? "OrderBy" : "OrderByDescending";
        Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                              new Type[] { source.ElementType, property.Type },
                              source.Expression, Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(methodCallExpression);
    }

}
