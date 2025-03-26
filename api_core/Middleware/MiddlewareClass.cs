using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Core.Middleware
{
    public class MiddlewareClass : IAsyncActionFilter
    {
        private readonly string _accessKey, _refrashKey, _issuer, _audience;
        private readonly int _accessKeyExpires;

        public MiddlewareClass(IConfiguration configuration)
        {
            _accessKey = configuration["JWTConfig:AccessSecretkey"]!;
            _refrashKey = configuration["JWTConfig:RefreshSecretkey"]!;
            _issuer = configuration["JWTConfig:Issuer"]!;
            _audience = configuration["JWTConfig:Audience"]!;
            _accessKeyExpires = Convert.ToInt16(configuration["JWTConfig:AccessTokenExpires"]!);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            accessToken = accessToken?.Substring("Bearer ".Length).Trim();
            var refreshToken = context.HttpContext.Request.Cookies["refreshToken"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var accessTokenParameters = new TokenValidationParameters
            {
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessKey!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
            var refreshTokenParameters = new TokenValidationParameters
            {
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refrashKey!)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
            ClaimsPrincipal principal;
            string? newAccessTokenString = null;

            if (string.IsNullOrEmpty(accessToken) || accessToken == "")
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized: Invalid or missing token");
                return;
            }

            try
            {
                principal = tokenHandler.ValidateToken(accessToken, accessTokenParameters, out SecurityToken validatedToken);
            }
            catch (SecurityTokenExpiredException)
            {
                if (string.IsNullOrEmpty(refreshToken) || refreshToken == "")
                {
                    context.Result = new UnauthorizedObjectResult("Unauthorized: Invalid or missing token");
                    return;
                }

                try
                {
                    principal = tokenHandler.ValidateToken(refreshToken, refreshTokenParameters, out SecurityToken validatedRefreshToken);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, principal.FindFirstValue(ClaimTypes.Name)!)
                    };
                    var accesskey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessKey));
                    var newAccessToken = new JwtSecurityToken(
                        issuer: _issuer,
                        audience: _audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddHours(_accessKeyExpires),
                        signingCredentials: new SigningCredentials(accesskey, SecurityAlgorithms.HmacSha256)
                    );

                    newAccessTokenString = tokenHandler.WriteToken(newAccessToken);
                }
                catch (Exception)
                {
                    context.Result = new UnauthorizedObjectResult("Unauthorized: Both access and refresh tokens are expired or invalid");
                    return;
                }
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedObjectResult("Unauthorized: Invalid or expired token");
                return;
            }

            var resultContext = await next();
            if (newAccessTokenString != null && resultContext.Result is ObjectResult objectResult)
            {
                var originalValue = objectResult.Value;
                objectResult.Value = new { data = originalValue, accessToken = newAccessTokenString };
            }
        }
    }
}
