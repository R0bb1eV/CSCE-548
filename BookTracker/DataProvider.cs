using System;
using System.Collections.Generic;
using Npgsql;

namespace BookTracker
{
    public class DataProvider
    {
        private string _connectionString;

        public DataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==================== AUTHOR CRUD ====================
        public void CreateAuthor(Author author)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"INSERT INTO author (first_name, middle_name, last_name, birth_year)
                             VALUES (@FirstName, @MiddleName, @LastName, @BirthYear)
                             RETURNING author_id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FirstName", author.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", author.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", author.LastName);
            cmd.Parameters.AddWithValue("@BirthYear", author.BirthYear);
            author.AuthorId = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Author> ReadAllAuthors()
        {
            var list = new List<Author>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM author";
            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Author
                {
                    AuthorId = GetInt32(reader, "author_id"),
                    FirstName = GetString(reader, "first_name"),
                    MiddleName = IsNull(reader, "middle_name") ? null : GetString(reader, "middle_name"),
                    LastName = GetString(reader, "last_name"),
                    BirthYear = GetInt32(reader, "birth_year")
                });
            }
            return list;
        }

        public Author ReadAuthorById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM author WHERE author_id = @Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Author
                {
                    AuthorId = GetInt32(reader, "author_id"),
                    FirstName = GetString(reader, "first_name"),
                    MiddleName = IsNull(reader, "middle_name") ? null : GetString(reader, "middle_name"),
                    LastName = GetString(reader, "last_name"),
                    BirthYear = GetInt32(reader, "birth_year")
                };
            }
            return null;
        }

        public void UpdateAuthor(Author author)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"UPDATE author SET first_name=@FirstName, middle_name=@MiddleName, last_name=@LastName, birth_year=@BirthYear WHERE author_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@FirstName", author.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", author.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", author.LastName);
            cmd.Parameters.AddWithValue("@BirthYear", author.BirthYear);
            cmd.Parameters.AddWithValue("@Id", author.AuthorId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteAuthor(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "DELETE FROM author WHERE author_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        // ==================== BOOK CRUD ====================
        public void CreateBook(Book book)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"INSERT INTO book (title, page_count, genre, publishing_house, year_of_release, isbn, author_id)
                             VALUES (@Title, @PageCount, @Genre, @PublishingHouse, @YearOfRelease, @ISBN, @AuthorId)
                             RETURNING id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@PageCount", book.PageCount);
            cmd.Parameters.AddWithValue("@Genre", book.Genre);
            cmd.Parameters.AddWithValue("@PublishingHouse", book.PublishingHouse);
            cmd.Parameters.AddWithValue("@YearOfRelease", book.YearOfRelease);
            cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
            cmd.Parameters.AddWithValue("@AuthorId", book.AuthorId);
            book.ID = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Book> ReadAllBooks()
        {
            var books = new List<Book>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM book";
            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    ID = GetInt32(reader, "id"),
                    Title = GetString(reader, "title"),
                    PageCount = GetInt32(reader, "page_count"),
                    Genre = GetString(reader, "genre"),
                    PublishingHouse = GetString(reader, "publishing_house"),
                    YearOfRelease = GetInt32(reader, "year_of_release"),
                    ISBN = GetString(reader, "isbn"),
                    AuthorId = GetInt32(reader, "author_id")
                });
            }
            return books;
        }

        public Book ReadBookById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM book WHERE id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Book
                {
                    ID = GetInt32(reader, "id"),
                    Title = GetString(reader, "title"),
                    PageCount = GetInt32(reader, "page_count"),
                    Genre = GetString(reader, "genre"),
                    PublishingHouse = GetString(reader, "publishing_house"),
                    YearOfRelease = GetInt32(reader, "year_of_release"),
                    ISBN = GetString(reader, "isbn"),
                    AuthorId = GetInt32(reader, "author_id")
                };
            }
            return null;
        }

        public void UpdateBook(Book book)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"UPDATE book SET title=@Title, page_count=@PageCount, genre=@Genre, publishing_house=@PublishingHouse, year_of_release=@YearOfRelease, isbn=@ISBN, author_id=@AuthorId WHERE id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", book.Title);
            cmd.Parameters.AddWithValue("@PageCount", book.PageCount);
            cmd.Parameters.AddWithValue("@Genre", book.Genre);
            cmd.Parameters.AddWithValue("@PublishingHouse", book.PublishingHouse);
            cmd.Parameters.AddWithValue("@YearOfRelease", book.YearOfRelease);
            cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
            cmd.Parameters.AddWithValue("@AuthorId", book.AuthorId);
            cmd.Parameters.AddWithValue("@Id", book.ID);
            cmd.ExecuteNonQuery();
        }

        public void DeleteBook(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "DELETE FROM book WHERE id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        // ==================== USER CRUD ====================
        public void CreateUser(User user)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"INSERT INTO ""user"" (username, email, dob, account_creation_date)
                             VALUES (@Username, @Email, @DOB, @AccountCreationDate)
                             RETURNING user_id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@DOB", user.DOB);
            cmd.Parameters.AddWithValue("@AccountCreationDate", user.AccountCreationDate == default ? DateTime.Now : user.AccountCreationDate);
            user.UserId = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<User> ReadAllUsers()
        {
            var list = new List<User>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"SELECT * FROM ""user""";
            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new User
                {
                    UserId = GetInt32(reader, "user_id"),
                    Username = GetString(reader, "username"),
                    Email = GetString(reader, "email"),
                    DOB = GetDateTime(reader, "dob"),
                    AccountCreationDate = GetDateTime(reader, "account_creation_date")
                });
            }
            return list;
        }

        public User ReadUserById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"SELECT * FROM ""user"" WHERE user_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = GetInt32(reader, "user_id"),
                    Username = GetString(reader, "username"),
                    Email = GetString(reader, "email"),
                    DOB = GetDateTime(reader, "dob"),
                    AccountCreationDate = GetDateTime(reader, "account_creation_date")
                };
            }
            return null;
        }

        public void UpdateUser(User user)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"UPDATE ""user"" SET username=@Username, email=@Email, dob=@DOB, account_creation_date=@AccountCreationDate WHERE user_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@DOB", user.DOB);
            cmd.Parameters.AddWithValue("@AccountCreationDate", user.AccountCreationDate);
            cmd.Parameters.AddWithValue("@Id", user.UserId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteUser(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"DELETE FROM ""user"" WHERE user_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        // ==================== ACTIVITY CRUD ====================
        public void CreateActivity(Activity activity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"INSERT INTO activity (user_id, book_id, book_status, progress_completed, start_date, end_date)
                             VALUES (@UserId, @BookId, @BookStatus, @ProgressCompleted, @StartDate, @EndDate)
                             RETURNING activity_id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", activity.UserId);
            cmd.Parameters.AddWithValue("@BookId", activity.BookId);
            cmd.Parameters.AddWithValue("@BookStatus", activity.BookStatus);
            cmd.Parameters.AddWithValue("@ProgressCompleted", activity.ProgressCompleted);
            cmd.Parameters.AddWithValue("@StartDate", (object)activity.StartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", (object)activity.EndDate ?? DBNull.Value);
            activity.ActivityId = Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Activity> ReadAllActivities()
        {
            var list = new List<Activity>();
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM activity";
            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Activity
                {
                    ActivityId = GetInt32(reader, "activity_id"),
                    UserId = GetInt32(reader, "user_id"),
                    BookId = GetInt32(reader, "book_id"),
                    BookStatus = GetString(reader, "book_status"),
                    ProgressCompleted = GetInt32(reader, "progress_completed"),
                    StartDate = IsNull(reader, "start_date") ? null : GetDateTime(reader, "start_date"),
                    EndDate = IsNull(reader, "end_date") ? null : GetDateTime(reader, "end_date")
                });
            }
            return list;
        }

        public Activity ReadActivityById(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM activity WHERE activity_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Activity
                {
                    ActivityId = GetInt32(reader, "activity_id"),
                    UserId = GetInt32(reader, "user_id"),
                    BookId = GetInt32(reader, "book_id"),
                    BookStatus = GetString(reader, "book_status"),
                    ProgressCompleted = GetInt32(reader, "progress_completed"),
                    StartDate = IsNull(reader, "start_date") ? null : GetDateTime(reader, "start_date"),
                    EndDate = IsNull(reader, "end_date") ? null : GetDateTime(reader, "end_date")
                };
            }
            return null;
        }

        public void UpdateActivity(Activity activity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = @"UPDATE activity SET user_id=@UserId, book_id=@BookId, book_status=@BookStatus, progress_completed=@ProgressCompleted, start_date=@StartDate, end_date=@EndDate WHERE activity_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", activity.UserId);
            cmd.Parameters.AddWithValue("@BookId", activity.BookId);
            cmd.Parameters.AddWithValue("@BookStatus", activity.BookStatus);
            cmd.Parameters.AddWithValue("@ProgressCompleted", activity.ProgressCompleted);
            cmd.Parameters.AddWithValue("@StartDate", (object)activity.StartDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@EndDate", (object)activity.EndDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Id", activity.ActivityId);
            cmd.ExecuteNonQuery();
        }

        public void DeleteActivity(int id)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            string query = "DELETE FROM activity WHERE activity_id=@Id";
            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        private static int GetInt32(NpgsqlDataReader reader, string column)
        {
            return reader.GetInt32(reader.GetOrdinal(column));
        }

        private static string GetString(NpgsqlDataReader reader, string column)
        {
            return reader.GetString(reader.GetOrdinal(column));
        }

        private static DateTime GetDateTime(NpgsqlDataReader reader, string column)
        {
            return reader.GetDateTime(reader.GetOrdinal(column));
        }

        private static bool IsNull(NpgsqlDataReader reader, string column)
        {
            return reader.IsDBNull(reader.GetOrdinal(column));
        }
    }
}
