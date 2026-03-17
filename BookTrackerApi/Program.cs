using BookTracker;
using BookTracker.Business.Implementations;
using BookTracker.Business.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Hosting notes:
// Platform: ASP.NET Core Web API hosted by Kestrel on local machine.
// Run command: dotnet run --project .\BookTrackerApi\BookTrackerApi.csproj
// Service URL: http://localhost:5080 (local default)
//
// Render binds to the PORT environment variable via ASPNETCORE_URLS in the Dockerfile.
// We should not hard-code a URL here.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("WebClientCors", policy =>
    {
        var allowedOriginsRaw = builder.Configuration["ALLOWED_ORIGINS"];
        var allowedOrigins = string.IsNullOrWhiteSpace(allowedOriginsRaw)
            ? Array.Empty<string>()
            : allowedOriginsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (allowedOrigins.Length == 0)
        {
            // Fallback for local development and default Vercel previews.
            policy.SetIsOriginAllowed(origin =>
                    origin.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase) ||
                    origin.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase) ||
                    origin.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase))
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    });
});

var supabaseUrl = builder.Configuration["Supabase:Url"] ?? builder.Configuration["SUPABASE_URL"];
var supabaseAnonKey = builder.Configuration["Supabase:AnonKey"] ?? builder.Configuration["SUPABASE_ANON_KEY"];

var hasSupabaseConfig = !string.IsNullOrWhiteSpace(supabaseUrl) && !string.IsNullOrWhiteSpace(supabaseAnonKey);
if (!hasSupabaseConfig)
{
    Console.WriteLine("Supabase config missing. Check SUPABASE_URL and SUPABASE_ANON_KEY environment variables.");
    builder.Services.AddScoped<DataProvider>(_ => throw new InvalidOperationException(
        "Missing Supabase settings: SUPABASE_URL and SUPABASE_ANON_KEY."));
}
else
{
    builder.Services.AddScoped(_ => new DataProvider(supabaseUrl!, supabaseAnonKey!));
}
builder.Services.AddScoped<IAuthorBusiness, AuthorBusiness>();
builder.Services.AddScoped<IBookBusiness, BookBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IActivityBusiness, ActivityBusiness>();

var app = builder.Build();

// OpenAPI is disabled for now to keep the .NET 8 build minimal on Render.

bool IsAllowedOrigin(string origin, string[] allowedOrigins)
{
    if (allowedOrigins.Length == 0)
    {
        return origin.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase) ||
               origin.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase) ||
               origin.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase);
    }

    return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
}

app.UseRouting();
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers.Origin.ToString();
    var allowedOriginsRaw = context.RequestServices.GetRequiredService<IConfiguration>()["ALLOWED_ORIGINS"];
    var allowedOrigins = string.IsNullOrWhiteSpace(allowedOriginsRaw)
        ? Array.Empty<string>()
        : allowedOriginsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (!string.IsNullOrWhiteSpace(origin) && IsAllowedOrigin(origin, allowedOrigins))
    {
        context.Response.Headers["Access-Control-Allow-Origin"] = origin;
        context.Response.Headers["Vary"] = "Origin";
        context.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,PUT,DELETE,OPTIONS";
        context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
    }

    if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = StatusCodes.Status204NoContent;
        return;
    }

    await next();
});
app.UseCors("WebClientCors");
app.MapControllers();
app.MapGet("/", () => Results.Ok("BookTracker API is running."));
app.MapGet("/api/ping", () => Results.Ok(new { status = "ok", supabaseConfigured = hasSupabaseConfig }));
app.Run();
