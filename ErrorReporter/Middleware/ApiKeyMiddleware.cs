using System.Security.Cryptography;
using System.Text;
using ErrorReporter.Data;
using Microsoft.EntityFrameworkCore;

namespace ErrorReporter.Middleware
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeader = "X-Api-Key";
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db, IConfiguration config)
        {
            var path = context.Request.Path.Value ?? "";

            // Allow OpenAPI/Scalar docs through without auth
            if (path.StartsWith("/openapi") || path.StartsWith("/scalar"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Missing X-Api-Key header" });
                return;
            }

            var key = providedKey.ToString();

            // Check if it's the master key
            var masterKey = config["Authentication:MasterApiKey"];
            if (!string.IsNullOrEmpty(masterKey) && key == masterKey)
            {
                context.Items["IsMaster"] = true;
                await _next(context);
                return;
            }

            // Check against client keys in the database
            var keyHash = HashKey(key);
            var client = await db.ApiClients
                .FirstOrDefaultAsync(c => c.KeyHash == keyHash && c.IsActive);

            if (client is null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid or revoked API key" });
                return;
            }

            context.Items["ApiClientId"] = client.Id;
            context.Items["ApiClientName"] = client.Name;
            await _next(context);
        }

        public static string HashKey(string key)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            return Convert.ToHexStringLower(bytes);
        }
    }
}
