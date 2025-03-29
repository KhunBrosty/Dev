using Microsoft.AspNetCore.Mvc;
using API_Core.Modles;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using API_Core.Middleware;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_Core.Controllers
{
    // below is the route name I pick my route as api/(controller name)
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly string _connectionString, _accessKey, _refrashKey, _issuer, _audience;
        private readonly int _accessKeyExpires, _refrashKeyExpires;

        // Get data from appsettings.json
        public UsersController(ILogger<UsersController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _accessKey = configuration["JWTConfig:AccessSecretkey"]!;
            _refrashKey = configuration["JWTConfig:RefreshSecretkey"]!;
            _issuer = configuration["JWTConfig:Issuer"]!;
            _audience = configuration["JWTConfig:Audience"]!;
            _accessKeyExpires = Convert.ToInt16(configuration["JWTConfig:AccessTokenExpires"]!);
            _refrashKeyExpires = Convert.ToInt16(configuration["JWTConfig:RefreshTokenExpires"]!);
        }

        [HttpGet]
        [ServiceFilter(typeof(MiddlewareClass))]
        public async Task<List<UsesModles>> Get()
        {
            List<UsesModles> result = new List<UsesModles>();
            result.Add(new UsesModles
            {
                Username = "ABC",
                Password = "DEF"
            });
            //using (var con = new OracleConnection(_connectionString))
            //{
            //    await con.OpenAsync();
            //    using (var cmd = con.CreateCommand())
            //    {
            //        cmd.CommandText = "SELECT * FROM USERS";
            //        using (var reader = await cmd.ExecuteReaderAsync())
            //        {
            //            DataTable dt = new DataTable();
            //            dt.Load(reader);
            //            foreach (DataRow row in dt.Rows)
            //            {
            //                result.Add(new UsesModles
            //                {
            //                    Username = row["USERNAME"].ToString(),
            //                    Password = row["PASSWORD"].ToString()
            //                });
            //            }
            //        }
            //    }
            //}

            return result;
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Post([FromBody] UsesModles request)
        {
            //return Ok(new { accessToken = "asdasd", user = "sdfsdf"});
            UsesModles result = new UsesModles();
            //using (var con = new OracleConnection(_connectionString))
            //{
                //await con.OpenAsync();
                //using (var cmd = con.CreateCommand())
                //{
                //    cmd.CommandText = "SELECT * FROM USERS WHERE USERNAME = :username AND PASSWORD = :password";
                //    cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = request.Username;
                //    cmd.Parameters.Add("password", OracleDbType.Varchar2).Value = request.Password;
                //    using (var reader = await cmd.ExecuteReaderAsync())
                //    {
                //        DataTable dt = new DataTable();
                //        dt.Load(reader);

                //        if (dt.Rows.Count == 0)
                //            return BadRequest("Username or Password is incorrect");
                        
                //        var userInfo = dt.Rows[0]["USERNAME"].ToString()!;
                        // Create access and refresh token
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, "asdad")
                        };

                        var accesskey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessKey));
                        var refrashkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refrashKey));

                        var accessToken = new JwtSecurityToken(
                            issuer: _issuer,
                            audience: _audience,
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(_accessKeyExpires),
                            signingCredentials: new SigningCredentials(accesskey, SecurityAlgorithms.HmacSha256)
                        );

                        var refreshToken = new JwtSecurityToken(
                            issuer: _issuer,
                            audience: _audience,
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(_refrashKeyExpires),
                            signingCredentials: new SigningCredentials(refrashkey, SecurityAlgorithms.HmacSha256)
                        );

                        var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
                        var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

                        //// Set Refresh Token as HttpOnly cookie
                        //var cookieOptions = new CookieOptions
                        //{
                        //    HttpOnly = true,
                        //    Secure = true,
                        //    SameSite = SameSiteMode.Strict,
                        //    Expires = DateTime.UtcNow.AddHours(_refrashKeyExpires)
                        //};

                        //Response.Cookies.Append("refreshToken", refreshTokenString, cookieOptions);

                        // Return access token in response body
                        return Ok(new { accessToken = accessTokenString, refreshToken = refreshTokenString });
                //        }
                //    }
            //}
        }

        [HttpPost("/getAuth")]
        public IActionResult getAuth()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "asdad")
            };

            var accesskey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_accessKey));
            var refrashkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refrashKey));

            var accessToken = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_accessKeyExpires),
                signingCredentials: new SigningCredentials(accesskey, SecurityAlgorithms.HmacSha256)
            );

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            return Ok(new { accessToken = accessTokenString });
        }

        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            ///Delete cookie
            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out successfully.");
        }

    }
}
