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
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
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

app.UseCors("WebClientCors");
app.MapControllers().RequireCors("WebClientCors");
app.MapGet("/", () => Results.Ok("BookTracker API is running."));
app.MapGet("/api/ping", () => Results.Ok(new { status = "ok", supabaseConfigured = hasSupabaseConfig }));
app.Run();
