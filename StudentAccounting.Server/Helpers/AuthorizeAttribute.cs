using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StudentAccounting.Server.Constants;
using StudentAccounting.Server.Domain.Entities;
using System;
using System.Linq;

namespace StudentAccounting.Server.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] roles;
        public AuthorizeAttribute(params string[] roles)
        {
            this.roles = roles ?? new string[] { };
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (DbUser)context.HttpContext.Items[Names.USER];
            var userRole = context.HttpContext.Items[Names.ROLE]?.ToString();

            
            if (user == null || (roles.Any() && !roles.Contains(userRole)))
            {
                Authorized.UserId = null;
                context.Result =
                    new JsonResult(new { message = "Unauthorized" })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
            }
            else
            {
                Authorized.UserId = user.Id;
            }
        }
    }
}
