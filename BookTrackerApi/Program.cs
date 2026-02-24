using BookTracker;
using BookTracker.Business.Implementations;
using BookTracker.Business.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Hosting notes:
// Platform: ASP.NET Core Web API hosted by Kestrel on local machine.
// Run command: dotnet run --project .\BookTrackerApi\BookTrackerApi.csproj
// Service URL: http://localhost:5080
builder.WebHost.UseUrls("http://localhost:5080");

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("BookTrackerDb")
    ?? throw new InvalidOperationException("Missing connection string: ConnectionStrings:BookTrackerDb");

builder.Services.AddScoped(_ => new DataProvider(connectionString));
builder.Services.AddScoped<IAuthorBusiness, AuthorBusiness>();
builder.Services.AddScoped<IBookBusiness, BookBusiness>();
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IActivityBusiness, ActivityBusiness>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();
