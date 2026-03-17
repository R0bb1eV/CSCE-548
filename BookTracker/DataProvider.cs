using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookTracker
{
    public class DataProvider
    {
        private readonly HttpClient _httpClient;
        private readonly string _restBaseUrl;

        public DataProvider(string supabaseUrl, string anonKey)
        {
            if (string.IsNullOrWhiteSpace(supabaseUrl))
            {
                throw new ArgumentException("Supabase URL is required.", nameof(supabaseUrl));
            }

            if (string.IsNullOrWhiteSpace(anonKey))
            {
                throw new ArgumentException("Supabase anon key is required.", nameof(anonKey));
            }

            _restBaseUrl = $"{supabaseUrl.TrimEnd('/')}/rest/v1";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", anonKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", anonKey);
        }

        // ==================== AUTHOR CRUD ====================
        public void CreateAuthor(Author author)
        {
            var payload = new Dictionary<string, object?>
            {
                ["first_name"] = author.FirstName,
                ["middle_name"] = author.MiddleName,
                ["last_name"] = author.LastName,
                ["birth_year"] = author.BirthYear
            };

            var json = Send(HttpMethod.Post, "author", null, payload, preferReturn: true);
            var rows = ParseArray(json);
            if (rows.Count > 0)
            {
                author.AuthorId = GetInt(rows[0], "author_id");
            }
        }

        public List<Author> ReadAllAuthors()
        {
            var json = Send(HttpMethod.Get, "author", "select=*");
            var rows = ParseArray(json);
            var list = new List<Author>(rows.Count);
            foreach (var row in rows)
            {
                list.Add(MapAuthor(row));
            }
            return list;
        }

        public Author ReadAuthorById(int id)
        {
            var json = Send(HttpMethod.Get, "author", $"author_id=eq.{id}&select=*");
            var rows = ParseArray(json);
            return rows.Count > 0 ? MapAuthor(rows[0]) : null;
        }

        public void UpdateAuthor(Author author)
        {
            var payload = new Dictionary<string, object?>
            {
                ["first_name"] = author.FirstName,
                ["middle_name"] = author.MiddleName,
                ["last_name"] = author.LastName,
                ["birth_year"] = author.BirthYear
            };

            Send(HttpMethod.Patch, "author", $"author_id=eq.{author.AuthorId}", payload);
        }

        public void DeleteAuthor(int id)
        {
            Send(HttpMethod.Delete, "author", $"author_id=eq.{id}");
        }

        // ==================== BOOK CRUD ====================
        public void CreateBook(Book book)
        {
            var payload = new Dictionary<string, object?>
            {
                ["title"] = book.Title,
                ["page_count"] = book.PageCount,
                ["genre"] = book.Genre,
                ["publishing_house"] = book.PublishingHouse,
                ["year_of_release"] = book.YearOfRelease,
                ["isbn"] = book.ISBN,
                ["author_id"] = book.AuthorId
            };

            var json = Send(HttpMethod.Post, "book", null, payload, preferReturn: true);
            var rows = ParseArray(json);
            if (rows.Count > 0)
            {
                book.ID = GetInt(rows[0], "id");
            }
        }

        public List<Book> ReadAllBooks()
        {
            var json = Send(HttpMethod.Get, "book", "select=*");
            var rows = ParseArray(json);
            var list = new List<Book>(rows.Count);
            foreach (var row in rows)
            {
                list.Add(MapBook(row));
            }
            return list;
        }

        public Book ReadBookById(int id)
        {
            var json = Send(HttpMethod.Get, "book", $"id=eq.{id}&select=*");
            var rows = ParseArray(json);
            return rows.Count > 0 ? MapBook(rows[0]) : null;
        }

        public void UpdateBook(Book book)
        {
            var payload = new Dictionary<string, object?>
            {
                ["title"] = book.Title,
                ["page_count"] = book.PageCount,
                ["genre"] = book.Genre,
                ["publishing_house"] = book.PublishingHouse,
                ["year_of_release"] = book.YearOfRelease,
                ["isbn"] = book.ISBN,
                ["author_id"] = book.AuthorId
            };

            Send(HttpMethod.Patch, "book", $"id=eq.{book.ID}", payload);
        }

        public void DeleteBook(int id)
        {
            Send(HttpMethod.Delete, "book", $"id=eq.{id}");
        }

        // ==================== USER CRUD ====================
        public void CreateUser(User user)
        {
            var payload = new Dictionary<string, object?>
            {
                ["username"] = user.Username,
                ["email"] = user.Email,
                ["dob"] = user.DOB,
                ["account_creation_date"] = user.AccountCreationDate == default ? DateTime.UtcNow : user.AccountCreationDate
            };

            var json = Send(HttpMethod.Post, "user", null, payload, preferReturn: true);
            var rows = ParseArray(json);
            if (rows.Count > 0)
            {
                user.UserId = GetInt(rows[0], "user_id");
            }
        }

        public List<User> ReadAllUsers()
        {
            var json = Send(HttpMethod.Get, "user", "select=*");
            var rows = ParseArray(json);
            var list = new List<User>(rows.Count);
            foreach (var row in rows)
            {
                list.Add(MapUser(row));
            }
            return list;
        }

        public User ReadUserById(int id)
        {
            var json = Send(HttpMethod.Get, "user", $"user_id=eq.{id}&select=*");
            var rows = ParseArray(json);
            return rows.Count > 0 ? MapUser(rows[0]) : null;
        }

        public void UpdateUser(User user)
        {
            var payload = new Dictionary<string, object?>
            {
                ["username"] = user.Username,
                ["email"] = user.Email,
                ["dob"] = user.DOB,
                ["account_creation_date"] = user.AccountCreationDate
            };

            Send(HttpMethod.Patch, "user", $"user_id=eq.{user.UserId}", payload);
        }

        public void DeleteUser(int id)
        {
            Send(HttpMethod.Delete, "user", $"user_id=eq.{id}");
        }

        // ==================== ACTIVITY CRUD ====================
        public void CreateActivity(Activity activity)
        {
            var payload = new Dictionary<string, object?>
            {
                ["user_id"] = activity.UserId,
                ["book_id"] = activity.BookId,
                ["book_status"] = activity.BookStatus,
                ["progress_completed"] = activity.ProgressCompleted,
                ["start_date"] = activity.StartDate,
                ["end_date"] = activity.EndDate
            };

            var json = Send(HttpMethod.Post, "activity", null, payload, preferReturn: true);
            var rows = ParseArray(json);
            if (rows.Count > 0)
            {
                activity.ActivityId = GetInt(rows[0], "activity_id");
            }
        }

        public List<Activity> ReadAllActivities()
        {
            var json = Send(HttpMethod.Get, "activity", "select=*");
            var rows = ParseArray(json);
            var list = new List<Activity>(rows.Count);
            foreach (var row in rows)
            {
                list.Add(MapActivity(row));
            }
            return list;
        }

        public Activity ReadActivityById(int id)
        {
            var json = Send(HttpMethod.Get, "activity", $"activity_id=eq.{id}&select=*");
            var rows = ParseArray(json);
            return rows.Count > 0 ? MapActivity(rows[0]) : null;
        }

        public void UpdateActivity(Activity activity)
        {
            var payload = new Dictionary<string, object?>
            {
                ["user_id"] = activity.UserId,
                ["book_id"] = activity.BookId,
                ["book_status"] = activity.BookStatus,
                ["progress_completed"] = activity.ProgressCompleted,
                ["start_date"] = activity.StartDate,
                ["end_date"] = activity.EndDate
            };

            Send(HttpMethod.Patch, "activity", $"activity_id=eq.{activity.ActivityId}", payload);
        }

        public void DeleteActivity(int id)
        {
            Send(HttpMethod.Delete, "activity", $"activity_id=eq.{id}");
        }

        // ==================== REST HELPERS ====================
        private string Send(HttpMethod method, string table, string? query, object? body = null, bool preferReturn = false)
        {
            var url = $"{_restBaseUrl}/{table}";
            if (!string.IsNullOrWhiteSpace(query))
            {
                url += "?" + query;
            }

            using var request = new HttpRequestMessage(method, url);
            if (preferReturn)
            {
                request.Headers.TryAddWithoutValidation("Prefer", "return=representation");
            }

            if (body != null)
            {
                var payload = JsonSerializer.Serialize(body);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            }

            using var response = _httpClient.Send(request);
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Supabase request failed ({(int)response.StatusCode}): {content}");
            }

            return content;
        }

        private static List<JsonElement> ParseArray(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<JsonElement>();
            }

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return new List<JsonElement>();
            }

            var list = new List<JsonElement>();
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                list.Add(item.Clone());
            }
            return list;
        }

        private static Author MapAuthor(JsonElement row)
        {
            return new Author
            {
                AuthorId = GetInt(row, "author_id"),
                FirstName = GetString(row, "first_name"),
                MiddleName = GetStringNullable(row, "middle_name"),
                LastName = GetString(row, "last_name"),
                BirthYear = GetInt(row, "birth_year")
            };
        }

        private static Book MapBook(JsonElement row)
        {
            return new Book
            {
                ID = GetInt(row, "id"),
                Title = GetString(row, "title"),
                PageCount = GetInt(row, "page_count"),
                Genre = GetString(row, "genre"),
                PublishingHouse = GetString(row, "publishing_house"),
                YearOfRelease = GetInt(row, "year_of_release"),
                ISBN = GetString(row, "isbn"),
                AuthorId = GetInt(row, "author_id")
            };
        }

        private static User MapUser(JsonElement row)
        {
            return new User
            {
                UserId = GetInt(row, "user_id"),
                Username = GetString(row, "username"),
                Email = GetString(row, "email"),
                DOB = GetDate(row, "dob") ?? DateTime.MinValue,
                AccountCreationDate = GetDate(row, "account_creation_date") ?? DateTime.MinValue
            };
        }

        private static Activity MapActivity(JsonElement row)
        {
            return new Activity
            {
                ActivityId = GetInt(row, "activity_id"),
                UserId = GetInt(row, "user_id"),
                BookId = GetInt(row, "book_id"),
                BookStatus = GetString(row, "book_status"),
                ProgressCompleted = GetInt(row, "progress_completed"),
                StartDate = GetDate(row, "start_date"),
                EndDate = GetDate(row, "end_date")
            };
        }

        private static int GetInt(JsonElement row, string name)
        {
            return row.GetProperty(name).GetInt32();
        }

        private static string GetString(JsonElement row, string name)
        {
            return row.GetProperty(name).GetString() ?? string.Empty;
        }

        private static string? GetStringNullable(JsonElement row, string name)
        {
            return row.TryGetProperty(name, out var value) && value.ValueKind != JsonValueKind.Null
                ? value.GetString()
                : null;
        }

        private static DateTime? GetDate(JsonElement row, string name)
        {
            if (!row.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            var text = value.GetString();
            return string.IsNullOrWhiteSpace(text)
                ? null
                : DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }
}
