using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace StudentAccounting.Server.Helpers
{
    public static class HelpersExtentions
    {
        public static Dictionary<string, string> GetErrorsFromResult(this IdentityResult result)
        {
            var modelErrors = new Dictionary<string, string>();
            foreach (var error in result.Errors)
            {
                modelErrors.Add(error.Code, error.Description);
            }

            return modelErrors;
        }

        public static int GetDaysToStudy(this DateTime studyDate)
        {         
            var today = DateTime.UtcNow;
            var daysToStudyCount = (studyDate - today).Days + 1;
            return (daysToStudyCount >= 0) ? daysToStudyCount : 0;
        }

        public static IQueryable<TEntity> OrderByDynamic<TEntity>
            (this IQueryable<TEntity> sourse, string orderByProperty, string sortOrder)
        {
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            if (property == null)
            {
                return sourse;
            }

            string orderCommand;
            switch (sortOrder)
            {
                case "ascend": orderCommand = "OrderBy"; break;
                case "descend": orderCommand = "OrderByDescending"; break;
                default: return sourse;
            }

            var parameter = Expression.Parameter(type, "user");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var lambda = Expression.Lambda(propertyAccess, parameter);
 
            var types = new Type[] { type, property.PropertyType };
            var resultExpression = Expression.Call(
                                                type: typeof(Queryable),
                                                methodName: orderCommand,
                                                typeArguments: types,
                                                sourse.Expression,
                                                Expression.Quote(lambda));

            return sourse.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
