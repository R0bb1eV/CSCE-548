using System.Net;
using System.Net.Http.Json;
using BookTracker;

// Console test client for service layer.
// Hosting platform for service: ASP.NET Core Kestrel (local machine).
// 1) Start API first: dotnet run --project .\BookTrackerApi\BookTrackerApi.csproj
// 2) Run this client: dotnet run --project .\BookTrackerConsoleClient\BookTrackerConsoleClient.csproj
// API base URL expected by this client: http://localhost:5080

var apiBaseUrl = "http://localhost:5080";
using var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };

Console.WriteLine("=== Service CRUD Verification Start ===");

try
{
    // Create
    var author = await PostAsync<Author>(httpClient, "api/authors", new Author
    {
        FirstName = "Service",
        MiddleName = "Layer",
        LastName = "Author",
        BirthYear = 1985
    });
    Console.WriteLine($"Author created: {author}");

    // Read
    var fetchedAuthor = await GetByIdAsync<Author>(httpClient, "api/authors", author.AuthorId);
    Console.WriteLine($"Author fetched after create: {fetchedAuthor}");

    // Update
    author.FirstName = "UpdatedService";
    await PutAsync(httpClient, $"api/authors/{author.AuthorId}", author);
    var updatedAuthor = await GetByIdAsync<Author>(httpClient, "api/authors", author.AuthorId);
    Console.WriteLine($"Author fetched after update: {updatedAuthor}");

    var user = await PostAsync<User>(httpClient, "api/users", new User
    {
        Username = "svc_user",
        Email = "svc_user@example.com",
        DOB = new DateTime(1999, 9, 9),
        AccountCreationDate = DateTime.UtcNow
    });
    Console.WriteLine($"User created: {user}");

    var fetchedUser = await GetByIdAsync<User>(httpClient, "api/users", user.UserId);
    Console.WriteLine($"User fetched after create: {fetchedUser}");

    user.Username = "svc_user_updated";
    await PutAsync(httpClient, $"api/users/{user.UserId}", user);
    var updatedUser = await GetByIdAsync<User>(httpClient, "api/users", user.UserId);
    Console.WriteLine($"User fetched after update: {updatedUser}");

    var book = await PostAsync<Book>(httpClient, "api/books", new Book
    {
        Title = "Service Testing Book",
        PageCount = 250,
        Genre = "Technical",
        PublishingHouse = "CSCE Press",
        YearOfRelease = 2026,
        ISBN = "9781111111111",
        AuthorId = author.AuthorId
    });
    Console.WriteLine($"Book created: {book}");

    var fetchedBook = await GetByIdAsync<Book>(httpClient, "api/books", book.ID);
    Console.WriteLine($"Book fetched after create: {fetchedBook}");

    book.Title = "Service Testing Book Updated";
    await PutAsync(httpClient, $"api/books/{book.ID}", book);
    var updatedBook = await GetByIdAsync<Book>(httpClient, "api/books", book.ID);
    Console.WriteLine($"Book fetched after update: {updatedBook}");

    var activity = await PostAsync<Activity>(httpClient, "api/activities", new Activity
    {
        UserId = user.UserId,
        BookId = book.ID,
        BookStatus = "reading",
        ProgressCompleted = 30,
        StartDate = DateTime.UtcNow,
        EndDate = null
    });
    Console.WriteLine($"Activity created: {activity}");

    var fetchedActivity = await GetByIdAsync<Activity>(httpClient, "api/activities", activity.ActivityId);
    Console.WriteLine($"Activity fetched after create: {fetchedActivity}");

    activity.BookStatus = "completed";
    activity.ProgressCompleted = 100;
    activity.EndDate = DateTime.UtcNow;
    await PutAsync(httpClient, $"api/activities/{activity.ActivityId}", activity);
    var updatedActivity = await GetByIdAsync<Activity>(httpClient, "api/activities", activity.ActivityId);
    Console.WriteLine($"Activity fetched after update: {updatedActivity}");

    // Delete + Read check for all entities
    await DeleteAsync(httpClient, $"api/activities/{activity.ActivityId}");
    await ConfirmDeletedAsync(httpClient, "api/activities", activity.ActivityId, "Activity");

    await DeleteAsync(httpClient, $"api/books/{book.ID}");
    await ConfirmDeletedAsync(httpClient, "api/books", book.ID, "Book");

    await DeleteAsync(httpClient, $"api/users/{user.UserId}");
    await ConfirmDeletedAsync(httpClient, "api/users", user.UserId, "User");

    await DeleteAsync(httpClient, $"api/authors/{author.AuthorId}");
    await ConfirmDeletedAsync(httpClient, "api/authors", author.AuthorId, "Author");

    Console.WriteLine("=== Service CRUD Verification Complete ===");
}
catch (Exception ex)
{
    Console.WriteLine("Client run failed.");
    Console.WriteLine(ex.Message);
}

static async Task<T> PostAsync<T>(HttpClient client, string route, object payload)
{
    var response = await client.PostAsJsonAsync(route, payload);
    await EnsureSuccessAsync(response, $"POST {route}");

    var result = await response.Content.ReadFromJsonAsync<T>();
    return result ?? throw new InvalidOperationException($"POST {route} returned empty JSON.");
}

static async Task<T> GetByIdAsync<T>(HttpClient client, string route, int id)
{
    var response = await client.GetAsync($"{route}/{id}");
    await EnsureSuccessAsync(response, $"GET {route}/{id}");

    var result = await response.Content.ReadFromJsonAsync<T>();
    return result ?? throw new InvalidOperationException($"GET {route}/{id} returned empty JSON.");
}

static async Task PutAsync(HttpClient client, string route, object payload)
{
    var response = await client.PutAsJsonAsync(route, payload);
    await EnsureSuccessAsync(response, $"PUT {route}");
}

static async Task DeleteAsync(HttpClient client, string route)
{
    var response = await client.DeleteAsync(route);
    await EnsureSuccessAsync(response, $"DELETE {route}");
}

static async Task ConfirmDeletedAsync(HttpClient client, string route, int id, string entityName)
{
    var response = await client.GetAsync($"{route}/{id}");
    if (response.StatusCode == HttpStatusCode.NotFound)
    {
        Console.WriteLine($"{entityName} delete verified. GET returned 404 as expected.");
        return;
    }

    var body = await response.Content.ReadAsStringAsync();
    throw new InvalidOperationException($"{entityName} delete verification failed. Status={(int)response.StatusCode}, Body={body}");
}

static async Task EnsureSuccessAsync(HttpResponseMessage response, string operation)
{
    if (response.IsSuccessStatusCode)
    {
        return;
    }

    var body = await response.Content.ReadAsStringAsync();
    throw new InvalidOperationException($"{operation} failed. Status={(int)response.StatusCode}, Body={body}");
}
