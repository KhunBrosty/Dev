using API_Core.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<MiddlewareClass>();

var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    if (context.Request.Path.StartsWithSegments("/admin"))
//    {
//        context.Response.StatusCode = 403;
//        await context.Response.WriteAsync("Access Denied");
//    }
//    else
//    {
//        await next();
//    }
//});

app.UseCors(configurePolicy: policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
});

// Register Decryption Middleware
app.UseMiddleware<EncryptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();