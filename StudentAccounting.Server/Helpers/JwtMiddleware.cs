using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudentAccounting.Server.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using StudentAccounting.Server.Constants;


namespace StudentAccounting.Server.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly JwtBearerTokenSettings jwtSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtBearerTokenSettings> jwtOptions)
        {
            this.next = next;
            jwtSettings = jwtOptions.Value;
        }
        public async Task Invoke(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers[Names.AUTHORIZATION].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AddPayloadToContext(context, userService, token);
            }

            await next(context);
        }

        private void AddPayloadToContext(HttpContext context, IUserService userService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtSettings.GetSymmetricSecurityKey(),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                
                var userId = jwtToken.Claims.First(claim => claim.Type == Names.ID).Value;
                var userRole = jwtToken.Claims.First(claim => claim.Type == Names.ROLE).Value;

                context.Items[Names.USER] = userService.GetUserById(userId);
                context.Items[Names.ROLE] = userRole;
            }
            catch
            {
            }
        }
    }
}
